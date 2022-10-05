namespace ShipbreakerCompanion.Client.Models
{
    /// <summary>
    /// Defines the state of game tracking for memory reading purposes.
    /// </summary>
    /// <see cref="ViewModels.CurrentSalvageViewModel"/>
    public enum GameTrackingState
    {
        /// <summary>
        /// Tracking is stopped or has not yet been turned on.
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// Game is attached but the values cannot (yet) be tracked.
        /// </summary>
        Attached = 1,

        /// <summary>
        /// Tracking is fully functional.
        /// </summary>
        Tracking = 2
    }
}
