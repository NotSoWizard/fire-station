using Content.Server.Radio.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Pinpointer;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Scp.Other.Radio;

public sealed class RadioCallButtonSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly SharedTransformSystem _sharedTransformSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RadioCallButtonComponent, InteractHandEvent>(OnButtonPressed);
    }

    private void OnButtonPressed(Entity<RadioCallButtonComponent> ent, ref InteractHandEvent args)
    {
        // 1. Check if the button is on cooldown.
        if (ent.Comp.NextUse != null && _timing.CurTime < ent.Comp.NextUse)
            return;

        var channel = _prototypeManager.Index(ent.Comp.RadioChannel);

        // 2. The button was pressed successfully! Update the NextUse state.
        ent.Comp.NextUse = _timing.CurTime + ent.Comp.Cooldown;

        // 1. Get button's MapCoordinates via TransformSystem
        var coordinates = _sharedTransformSystem.GetMapCoordinates(ent);

        // 2. Initialize closest distance tracking variables
        var closest = 15f;

        var query = EntityQueryEnumerator<NavMapBeaconComponent, TransformComponent>();
        var locationName = Loc.GetString("scp-radio-button-unknown-location");
        if (ent.Comp.RoomName == "UNSET_LOCATION")
        {
            while (query.MoveNext(out var beaconUid, out var beacon, out var beaconXform))
            {
                if(coordinates.MapId != beaconXform.MapID)
                    continue;

                var beaconCoords = _sharedTransformSystem.GetMapCoordinates(beaconUid, beaconXform);
                var distance = (coordinates.Position - beaconCoords.Position).Length();

                if(distance > closest)
                    continue;

                if(distance < closest && beacon.Text != null)
                {
                    closest = distance;
                    locationName = Loc.GetString(beacon.Text);
                }
            }
        }
        else
        {
            locationName = ent.Comp.RoomName;
        }
        // 3. Get the localized message.
        var message = Loc.GetString(ent.Comp.MessageKey, ("location", locationName));

        // 4. Send the radio message.
        _radio.SendRadioMessage(ent.Owner, message, channel, ent.Owner);
    }
}
