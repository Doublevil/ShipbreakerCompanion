using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PropertyChanged.SourceGenerator;
using ShipbreakerCompanion.Client.Services;

namespace ShipbreakerCompanion.Client.ViewModels
{
    /// <summary>
    /// ViewModel for the profile list.
    /// </summary>
    public partial class ProfileListViewModel
    {
        private readonly IProfileService _profileService;

        [Notify] private bool _isLoading = true;
        [Notify] private ObservableCollection<ProfileViewModel> _profiles = new();
        [Notify] private ProfileViewModel _selectedProfile;
        [Notify] private bool _canChangeProfile = true;
        
        public ProfileListViewModel(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Initializes the profile list.
        /// </summary>
        public async Task InitializeAsync()
        {
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
