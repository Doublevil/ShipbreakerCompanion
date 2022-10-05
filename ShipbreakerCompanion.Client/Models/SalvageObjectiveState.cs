namespace ShipbreakerCompanion.Client.Models
{
    /// <summary>
    /// Enumerates the possible states of a salvaging objective (e.g. "Reach 95% salvage").
    /// </summary>
    public enum SalvageObjectiveState
    {
        /// <summary>
        /// The objective is ongoing, still reachable.
        /// </summary>
        InProgress = 0,

        /// <summary>
        /// The objective is reached.
        /// </summary>
        Reached = 1,

        /// <summary>
        /// The objective cannot be reached anymore.
        /// </summary>
        Failed = 2
    }
}
