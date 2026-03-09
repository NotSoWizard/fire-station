using Content.Shared.Radio;
using Robust.Shared.Prototypes;

namespace Content.Server._Scp.Other.Radio;

/// <summary>
/// Contains configuration and cooldown state for a physical button that broadcasts localized messages to a radio channel. Consumed by RadioCallButtonSystem.
/// </summary>

[RegisterComponent]
public sealed partial class RadioCallButtonComponent : Component
{
    /// <summary>
    /// The message key to send to the radio.
    /// </summary>
    [DataField("messageKey", required: true)]
    public string MessageKey = string.Empty;

    /// <summary>
    /// The channel to send the message to.
    /// </summary>
    [DataField("radioChannel", required: true)]
    public ProtoId<RadioChannelPrototype> RadioChannel = default!;

    /// <summary>
    /// The timer before the button can be used again.
    /// </summary>
    [DataField("cooldown")]
    public TimeSpan Cooldown = TimeSpan.FromSeconds(5);

    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan? NextUse = null;

    /// <summary>
    /// The room name of where the call is coming from.
    /// </summary>
    [DataField("roomName")]
    public string RoomName = "UNSET_LOCATION";
}
