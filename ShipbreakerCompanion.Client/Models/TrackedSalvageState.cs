namespace ShipbreakerCompanion.Client.Models
{
    /// <summary>
    /// Contains the salvaging salvage of a ship at a given point.
    /// Matches a similar structure of the game itself.
    /// </summary>
    public struct TrackedSalvageState
    {
        /// <summary>
        /// Gets the total salvageable value of the ship.
        /// </summary>
        public float TotalSalvageableValue { get; }

        /// <summary>
        /// Gets the value that has been salvaged.
        /// </summary>
        public float SalvagedValue { get; }

        /// <summary>
        /// Gets the value that is beyond recovery.
        /// </summary>
        public float DestroyedValue { get; }

        /// <summary>
        /// Builds a salvage state with the given properties.
        /// </summary>
        /// <param name="totalSalvageableValue">Total salvageable value of the ship.</param>
        /// <param name="salvagedValue">Value that has been salvaged.</param>
        /// <param name="destroyedValue">Value that is beyond recovery.</param>
        public TrackedSalvageState(float totalSalvageableValue, float salvagedValue, float destroyedValue)
        {
            TotalSalvageableValue = totalSalvageableValue;
            SalvagedValue = salvagedValue;
            DestroyedValue = destroyedValue;
        }
    }
}
