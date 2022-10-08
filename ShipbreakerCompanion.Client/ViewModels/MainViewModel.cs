using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged.SourceGenerator;
using ShipbreakerCompanion.Client.Commands;
using ShipbreakerCompanion.Client.Services;

namespace ShipbreakerCompanion.Client.ViewModels
{
    /// <summary>
    /// View model for the main window.
    /// </summary>
    public partial class MainViewModel
    {
        private readonly IProfileService _profileService;
        private readonly IShipService _shipService;
        private readonly IDialogCoordinator _dialogCoordinator;

        /// <summary>
        /// Boolean that indicates whether or not the application is loading its content.
        /// </summary>
        [Notify] private bool _isLoading;
        
        public ProfileListViewModel ProfileListVm { get; set; }
        public ShipListViewModel ShipListVm { get; set; }
        public CurrentSalvageViewModel CurrentSalvageVm { get; set; }

        /// <summary>
        /// Gets the version number of the application.
        /// </summary>
        public string VersionNumber => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown version";

        /// <summary>
        /// Command used to load a ship into a profile.
        /// </summary>
        public ICommand LoadShipCommand { get; }

        public MainViewModel(IProfileService profileService, IShipService shipService, IDialogCoordinator dialogCoordinator)
        {
            _profileService = profileService;
            _shipService = shipService;
            _dialogCoordinator = dialogCoordinator;

            LoadShipCommand = new RelayCommand<ShipViewModel>(
                s => { _ = LoadShipAsync(s); }
            );

            ProfileListVm = new ProfileListViewModel(_profileService, _dialogCoordinator);
            ShipListVm = new ShipListViewModel(_shipService);
            CurrentSalvageVm = new CurrentSalvageViewModel();
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public async Task InitializeAsync()
        {
            var profilesTask = ProfileListVm.InitializeAsync();
            var shipsTask = ShipListVm.InitializeAsync();

            await Task.WhenAll(profilesTask, shipsTask).ConfigureAwait(false);
        }

        /// <summary>
        /// Enables features intended for development.
        /// </summary>
        public void EnableDebugMode()
        {
            ShipListVm.ShowChecksum = true;
        }

        /// <summary>
        /// Loads the given ship into the selected profile.
        /// </summary>
        /// <param name="ship">Ship to load.</param>
        public async Task LoadShipAsync(ShipViewModel ship)
        {
            if (ProfileListVm.SelectedProfile == null)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "No profiles",
                    "This feature is unavailable because no profile saves could be found. Please check your Hardspace: Shipbreaker save files and restart the application.");
                return;
            }

            bool successful = false;
            ShipListVm.CanLoadShips = false;
            ProfileListVm.CanChangeProfile = false;
            try
            {
                ship.IsLoading = true;
                var isShipAvailableLocally = await _shipService.IsShipLocallyAvailableAsync(ship);
                if (!isShipAvailableLocally)
                    await _shipService.DownloadShipFileAsync(ship);
                await _profileService.LinkShipFileAsync(ProfileListVm.SelectedProfile, ship.LocalFilePath);

                successful = true;
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Error",
                    $"An error ({ex.GetType().Name}) occured while loading the ship.");
            }

            // Reset the loading indicators
            ShipListVm.CanLoadShips = true;
            ProfileListVm.CanChangeProfile = true;
            ship.IsLoading = false;

            if (successful)
            {
                // Set the "IsAlreadyLoaded" boolean to True for a while, then set it back to False.
                // Using Task.Delay(...).ContinueWith should have been preferable but it triggered a
                // random delay in bindings that I cannot explain (updating UI from non-UI thread?).
                ship.IsAlreadyLoaded = true;
                await Task.Delay(2000);
                ship.IsAlreadyLoaded = false;
            }
        }
    }
}
