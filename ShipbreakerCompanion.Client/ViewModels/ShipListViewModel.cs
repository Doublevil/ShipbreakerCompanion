using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PropertyChanged.SourceGenerator;
using ShipbreakerCompanion.Client.Commands;
using ShipbreakerCompanion.Client.Helpers;
using ShipbreakerCompanion.Client.Services;

namespace ShipbreakerCompanion.Client.ViewModels
{
    /// <summary>
    /// ViewModel for the ship list.
    /// </summary>
    public partial class ShipListViewModel
    {
        private readonly IShipService _shipService;
        [Notify] private bool _isLoading = true;
        [Notify] private ObservableCollection<ShipViewModel> _availableShips;
        [Notify] private bool _canLoadShips = true;
        [Notify] private bool _showChecksum;

        public ICommand CopyChecksumCommand { get; }

        public ShipListViewModel(IShipService shipService)
        {
            _shipService = shipService;
            CopyChecksumCommand = new RelayCommand<ShipViewModel>(ship =>
            {
                CopyChecksumToClipboard(ship).ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Initializes the profile list.
        /// </summary>
        public async Task InitializeAsync()
        {
            // Load ships
            var ships = await _shipService.GetCompetitiveShipsAsync();
            AvailableShips = new ObservableCollection<ShipViewModel>(ships);

            IsLoading = false;
        }

        /// <summary>
        /// Debug feature that allows to copy the checksum of the local ship file to the clipboard.
        /// Useful to determine the expectedChecksum when adding new ships to the list.
        /// </summary>
        /// <param name="ship">Target ship.</param>
        public async Task CopyChecksumToClipboard(ShipViewModel ship)
        {
            // Don't do anything if the local file doesn't exist.
            if (!File.Exists(ship.LocalFilePath))
                return;

            // Not very clean to use a System.Windows thingy here but for the purpose of a debug feature, I'm willing
            // to make this sacrifice.
            Clipboard.SetText(await ChecksumHelper.ComputeChecksumAsync(ship.LocalFilePath));
        }
    }
}
