using System.Collections.Generic;
using System.Threading.Tasks;
using ShipbreakerCompanion.Client.ViewModels;

namespace ShipbreakerCompanion.Client.Services
{
    /// <summary>
    /// Provides read and write operations on game files.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Reads the profile save files from the game's save directory and returns them as fully loaded viewmodels.
        /// </summary>
        Task<ICollection<ProfileViewModel>> ReadSavedProfilesAsync();

        /// <summary>
        /// Links the ship file at the given path to the specified profile.
        /// </summary>
        /// <param name="profile">Profile receiving the ship file.</param>
        /// <param name="shipFilePath">Path to the ship file to associate to the profile.</param>
        Task LinkShipFileAsync(ProfileViewModel profile, string shipFilePath);

        /// <summary>
        /// Disables or re-enables the given profile.
        /// </summary>
        /// <param name="profile">Profile to disable or re-enable.</param>
        void ToggleProfile(ProfileViewModel profile);

        /// <summary>
        /// Installs a maxed open shift save.
        /// </summary>
        Task InstallFullSaveAsync();
    }
}
