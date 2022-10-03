namespace ShipbreakerCompanion.Client.Models
{
    /// <summary>
    /// Represents a ship that can be loaded onto a profile.
    /// </summary>
    public class Ship
    {
        /// <summary>
        /// Gets or sets the user-friendly name of the ship.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the preferred file name when written to a file system, without the extension.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the URL pointing to the ship file available for download.
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets a boolean indicating if the ship has a matching competitive category for speedrunning.
        /// </summary>
        public bool IsCompetitive { get; set; }

        /// <summary>
        /// Gets or sets the expected MD5 checksum of the ship file, as a string.
        /// </summary>
        public string ExpectedChecksum { get; set; }
    }
}
