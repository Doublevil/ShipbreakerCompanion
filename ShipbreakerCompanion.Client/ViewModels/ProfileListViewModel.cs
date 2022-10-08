using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using PropertyChanged.SourceGenerator;
using ShipbreakerCompanion.Client.Commands;
using ShipbreakerCompanion.Client.Services;

namespace ShipbreakerCompanion.Client.ViewModels
{
    /// <summary>
    /// ViewModel for the profile list.
    /// </summary>
    public partial class ProfileListViewModel
    {
        private readonly IProfileService _profileService;
        private readonly IDialogCoordinator _dialogCoordinator;

        /// <summary>
        /// Boolean that indicates if the profiles are being loaded.
        /// </summary>
        [Notify] private bool _isLoading = true;

        /// <summary>
        /// Collection of available profiles.
        /// </summary>
        [Notify] private ObservableCollection<ProfileViewModel> _profiles = new();

        /// <summary>
        /// Currently selected profile.
        /// </summary>
        [Notify] private ProfileViewModel _selectedProfile;

        /// <summary>
        /// Boolean that defines whether the current profile can be changed or not.
        /// </summary>
        [Notify] private bool _canChangeProfile = true;

        /// <summary>
        /// Command used to toggle (disable or re-enable) a profile.
        /// </summary>
        public RelayCommand<ProfileViewModel> ToggleProfileCommand { get; }

        /// <summary>
        /// Command used to install the handy open shift maxed profile.
        /// </summary>
        public RelayCommand<ProfileListViewModel> InstallFullSaveCommand { get; }

        public ProfileListViewModel(IProfileService profileService, IDialogCoordinator dialogCoordinator)
        {
            _profileService = profileService;
            _dialogCoordinator = dialogCoordinator;
            ToggleProfileCommand = new RelayCommand<ProfileViewModel>(ToggleProfile);
            InstallFullSaveCommand = new RelayCommand<ProfileListViewModel>(InstallFullSaveAsync);
        }

        /// <summary>
        /// Toggles (disables or re-enables) the given profile.
        /// </summary>
        /// <param name="profile">Profile to toggle.</param>
        private void ToggleProfile(ProfileViewModel profile)
        {
            _profileService.ToggleProfile(profile);
            _ = InitializeAsync();
        }

        /// <summary>
        /// Downloads (if necessary) and installs the handy open shift maxed profile save.
        /// </summary>
        private async void InstallFullSaveAsync(ProfileListViewModel vm)
        {
            IsLoading = true;
            await _profileService.InstallFullSaveAsync();
            await InitializeAsync();

            await _dialogCoordinator.ShowMessageAsync(this, "Warning",
                $"The open shift maxed profile has been installed, but might not work if you already have an active Open Shift profile.{Environment.NewLine}If you do, you can disable it by selecting your old Open Shift profile and clicking the \"Disable this profile\" button.");
        }

        /// <summary>
        /// Initializes the profile list.
        /// </summary>
        public async Task InitializeAsync()
        {
            IsLoading = true;

            // Load profiles
            var profiles = await _profileService.ReadSavedProfilesAsync();
            Profiles = new ObservableCollection<ProfileViewModel>(profiles);

            // Select first profile if possible.
            if (Profiles.Count > 0)
                SelectedProfile = Profiles.First();

            IsLoading = false;
        }
    }
}
