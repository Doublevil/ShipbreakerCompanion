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
        /// Builds a new profile instance with the given properties.
        /// </summary>
        /// <param name="profileName">Name of the profile in-game.</param>
        /// <param name="saveFilePath">Absolute path to the profile save file.</param>
        /// <param name="hasShip">Defines if the profile has a current ship file or not.</param>
        public ProfileViewModel(string profileName, string saveFilePath, bool hasShip)
        {
            _profileName = profileName;
            _saveFilePath = saveFilePath;
            _hasShip = hasShip;
        }
    }
}
