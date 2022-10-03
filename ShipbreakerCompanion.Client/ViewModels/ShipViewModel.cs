using PropertyChanged.SourceGenerator;

namespace ShipbreakerCompanion.Client.ViewModels
{
    /// <summary>
    /// ViewModel for ships that can be loaded onto a profile.
    /// </summary>
    public partial class ShipViewModel
    {
        /// <summary>
        /// User-friendly name of the ship.
        /// </summary>
        [Notify] private string _name;

        /// <summary>
        /// URL pointing to the ship file to download.
        /// </summary>
        [Notify] private string _downloadUrl;

        /// <summary>
        /// Absolute path of the local ship file when downloaded.
        /// </summary>
        [Notify] private string _localFilePath;

        /// <summary>
        /// Checksum of the ship file, to ensure integrity.
        /// </summary>
        [Notify] private string _expectedChecksum;

        /// <summary>
        /// Defines if the ship is currently being loaded onto a profile.
        /// </summary>
        [Notify] private bool _isLoading;

        /// <summary>
        /// Defines if the ship has already been loaded onto the selected profile.
        /// </summary>
        [Notify] private bool _isAlreadyLoaded;

        public ShipViewModel(string name, string downloadUrl, string localFilePath, string expectedChecksum)
        {
            _name = name;
            _downloadUrl = downloadUrl;
            _localFilePath = localFilePath;
            _expectedChecksum = expectedChecksum;
        }
    }
}
