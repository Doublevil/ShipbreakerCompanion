using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ShipbreakerCompanion.Client.Helpers;
using ShipbreakerCompanion.Client.ViewModels;

namespace ShipbreakerCompanion.Client.Services
{
    /// <summary>
    /// Provides read and write operations on game files.
    /// </summary>
    public class ProfileService : IProfileService
    {
        /// <summary>
        /// Regex that identifies correct profile file names.
        /// Translates to: a string that starts with "vglpp" followed by one or more characters, followed by an underscore,
        /// followed by one or more numbers, followed by ".lpw" at the end of the string.
        /// </summary>
        private readonly Regex _legalFileNameRegex = new("^vglpp.+_[0-9]+[.]lpw$");

        /// <summary>
        /// Reads the profile save files from the game's save directory and returns them as fully loaded viewmodels.
        /// </summary>
        public async Task<ICollection<ProfileViewModel>> ReadSavedProfilesAsync()
        {
            string saveDirectory = PathHelper.GetSaveDirectoryCompletePath();
            var results = new List<ProfileViewModel>();
            foreach (var saveFilePath in Directory.GetFiles(saveDirectory, "*.lpw", SearchOption.TopDirectoryOnly))
            {
                // Disregard file names that are unusual and that would not be recognized by the game
                // (most likely user-made backup files).
                // Note that we don't do that in the Directory.GetFiles filter parameter because it has plenty of caveats
                // and does not support complex matching.
                string fileName = Path.GetFileName(saveFilePath);
                if (!_legalFileNameRegex.IsMatch(fileName))
                    continue;

                var profile = await ReadProfileAsync(saveFilePath);
                results.Add(profile);
            }

            return results;
        }

        /// <summary>
        /// Links the ship file at the given path to the specified profile.
        /// </summary>
        /// <param name="profile">Profile receiving the ship file.</param>
        /// <param name="shipFilePath">Path to the ship file to associate to the profile.</param>
        public async Task LinkShipFileAsync(ProfileViewModel profile, string shipFilePath)
        {
            string targetShipFilePath = GetShipFilePathForProfileFilePath(profile.SaveFilePath);
            await using Stream source = new FileStream(shipFilePath, FileMode.Open);
            await using Stream destination = new FileStream(targetShipFilePath, FileMode.Create);
            await source.CopyToAsync(destination);
        }

        /// <summary>
        /// Reads the given profile save file.
        /// </summary>
        /// <param name="path">Path to the profile save file to read.</param>
        /// <returns>A fully initialized ProfileViewModel.</returns>
        private async Task<ProfileViewModel> ReadProfileAsync(string path)
        {
            string profileName = await ReadProfileNameAsync(path);
            string shipFilePath = GetShipFilePathForProfileFilePath(path);
            bool hasShipFile = File.Exists(shipFilePath);
            return new ProfileViewModel(profileName, path, hasShipFile);
        }

        /// <summary>
        /// Reads the profile name in the profile save file at the given path.
        /// </summary>
        /// <param name="path">Path to the profile save file to read.</param>
        /// <returns>The in-game user-defined profile name as read from the file.</returns>
        private async Task<string> ReadProfileNameAsync(string path)
        {
            await using var stream = new FileStream(path, FileMode.Open);
            var buffer = new byte[1];
            
            // Skip the first 8 bytes and read 1 byte. This gives us the length of the profile name string.
            stream.Seek(8, SeekOrigin.Begin);
            _ = await stream.ReadAsync(buffer, 0, 1);
            int profileNameLength = buffer[0];

            // Read the profile name string from the file in a byte buffer
            buffer = new byte[profileNameLength];
            _ = await stream.ReadAsync(buffer, 0, profileNameLength);

            // Convert the byte buffer to an UTF8 string
            return Encoding.UTF8.GetString(buffer);
        }
        
        /// <summary>
        /// Given a profile file path, returns the matching ship file path.
        /// </summary>
        /// <param name="profileFilePath">The profile file absolute path.</param>
        private string GetShipFilePathForProfileFilePath(string profileFilePath)
        {
            // The game has a convention that the ship file always has the exact same name as the profile file,
            // but with the extension ".ship" instead of ".lpw".
            string fileName = Path.GetFileNameWithoutExtension(profileFilePath);
            string targetFileName = fileName + ".ship";
            string parentDirectory = Path.GetDirectoryName(profileFilePath);

            if (parentDirectory == null)
                throw new InvalidOperationException($"Cannot determine the parent directory of save file \"{profileFilePath}\".");

            return Path.Combine(parentDirectory, targetFileName);
        }
    }
}
