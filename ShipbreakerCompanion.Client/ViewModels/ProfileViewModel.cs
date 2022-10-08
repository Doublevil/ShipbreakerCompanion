using PropertyChanged.SourceGenerator;

namespace ShipbreakerCompanion.Client.ViewModels
{
    /// <summary>
    /// View model for an in-game profile.
    /// </summary>
    public partial class ProfileViewModel
    {
        /// <summary>
        /// Name of the profile as it was set in-game.
        /// </summary>
        [Notify] private string _profileName;

        /// <summary>
        /// Absolute path to the profile save file.
        /// </summary>
        [Notify] private string _saveFilePath;

        /// <summary>
        /// Defines if the profile has a current ship file or not.
        /// </summary>
        [Notify] private bool _hasShip;

        /// <summary>
        /// Defines if the profile is enabled (can be accessed by the game) or has been disabled.
        /// </summary>
        [Notify] private bool _isEnabled;

        /// <summary>
        /// Builds a new profile instance with the given properties.
        /// </summary>
        /// <param name="profileName">Name of the profile in-game.</param>
        /// <param name="saveFilePath">Absolute path to the profile save file.</param>
        /// <param name="hasShip">Defines if the profile has a current ship file or not.</param>
        /// <param name="isEnabled">Defines if the profile is enabled (can be accessed by the game) or has been disabled.</param>
        public ProfileViewModel(string profileName, string saveFilePath, bool hasShip, bool isEnabled)
        {
            _profileName = profileName;
            _saveFilePath = saveFilePath;
            _hasShip = hasShip;
            _isEnabled = isEnabled;
        }
    }
}
