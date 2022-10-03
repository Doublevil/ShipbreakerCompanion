using System;
using System.Threading.Tasks;
using System.Windows.Input;
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

        [Notify] private bool _isLoading;
        
        public ProfileListViewModel ProfileListVm { get; set; }
        public ShipListViewModel ShipListVm { get; set; }

        public ICommand LoadShipCommand { get; }

        public MainViewModel(IProfileService profileService, IShipService shipService)
        {
            _profileService = profileService;
            _shipService = shipService;

            LoadShipCommand = new RelayCommand<ShipViewModel>(
                s => { _ = LoadShipAsync(s); }
            );

            ProfileListVm = new ProfileListViewModel(_profileService);
            ShipListVm = new ShipListViewModel(_shipService);
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
                return; //todo: show message instead of doing nothing

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
            catch (Exception)
            {
                //todo: Handle exceptions
            }

            // Reset the loading indicators
            ShipListVm.CanLoadShips = true;
            ProfileListVm.CanChangeProfile = true;
            ship.IsLoading = false;

            if (successful)
            {
                // Set the "IsAlreadyLoaded" boolean to True for a while, then set it back to False.
                // Using Task.Delay(...).ContinueWith should have been preferable but it triggered a
                // random delay in bindings that I cannot explain.
                ship.IsAlreadyLoaded = true;
                await Task.Delay(2000);
                ship.IsAlreadyLoaded = false;
            }
        }
    }
}
