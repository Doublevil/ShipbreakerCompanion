using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
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
            string disabledSaveDirectory = PathHelper.GetDisabledSaveDirectoryPath();
            
            // Make sure the directories exist
            Directory.CreateDirectory(saveDirectory);
            Directory.CreateDirectory(disabledSaveDirectory);

            var results = new List<ProfileViewModel>();
            foreach (var saveFilePath in Directory.GetFiles(saveDirectory, "*.lpw", SearchOption.TopDirectoryOnly)
                         .Union(Directory.GetFiles(disabledSaveDirectory, "*.lpw")))
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
        /// Disables or re-enables the given profile.
        /// </summary>
        /// <param name="profile">Profile to disable or re-enable.</param>
        public void ToggleProfile(ProfileViewModel profile)
        {
            string sourceFilePath = profile.SaveFilePath;
            string fileName = Path.GetFileName(sourceFilePath);
            if (fileName == null)
                throw new InvalidOperationException($"Cannot get the file name for \"{profile.SaveFilePath}\".");

            string destinationDirectoryPath = profile.IsEnabled
                ? PathHelper.GetDisabledSaveDirectoryPath()
                : PathHelper.GetSaveDirectoryCompletePath();
            Directory.CreateDirectory(destinationDirectoryPath);

            string destinationFilePath = Path.Combine(destinationDirectoryPath, fileName);
            if (File.Exists(destinationFilePath))
            {
                // A file with the same name already exists in the other directory.
                // In this case, we will swap the files.
                // 3 steps are required to do that:
                // - Move one of the files with a temporary name.
                // - Move the other file with its definitive destination name.
                // - Rename the first file with its definite name.
                
                // We'll move the source file first.
                sourceFilePath = Path.Combine(destinationDirectoryPath, $"tmp_{Guid.NewGuid():D}.lpw");
                File.Move(profile.SaveFilePath, sourceFilePath);

                // Then we move the other one
                File.Move(destinationFilePath, profile.SaveFilePath);

                // The final step will be handled in the common code path after this condition.
            }

            File.Move(sourceFilePath, destinationFilePath);

            // Note that the profiles have to be reloaded after this operation, because if we swap two files,
            // there are several profiles impacted and we cannot really know.
        }

        /// <summary>
        /// Installs a maxed open shift save.
        /// </summary>
        public async Task InstallFullSaveAsync()
        {
            string filePath = PathHelper.GetCompanionFullProfileSaveFilePath();
            string fileName = Path.GetFileName(filePath);
            string destinationFilePath = Path.Combine(PathHelper.GetSaveDirectoryCompletePath(), fileName);
            if (!File.Exists(filePath))
                await DownloadFullSaveAsync();
            File.Copy(filePath!, destinationFilePath, true);
        }

        /// <summary>
        /// Downloads the maxed open shift profile save file to its local location.
        /// </summary>
        private async Task DownloadFullSaveAsync()
        {
            Directory.CreateDirectory(PathHelper.GetCompanionDownloadedProfileSavesDirectoryPath());

            using var restClient = new RestClient();
            //todo: Put URI in a config file instead of having it hardcoded
            var request = new RestRequest("https://github.com/Doublevil/ShipbreakerCompanion/blob/master/Saves/vglpp3_2195076518.lpw?raw=true");
            await using var downloadStream = await restClient.DownloadStreamAsync(request);

            //todo: Use specialized exception
            if (downloadStream == null)
                throw new Exception("Unable to download the profile save. No stream returned.");

            await using var fileStream = new FileStream(PathHelper.GetCompanionFullProfileSaveFilePath(), FileMode.Create);
            await downloadStream.CopyToAsync(fileStream);
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
            bool isEnabled = !PathHelper.IsInFolder(path, PathHelper.GetDisabledSaveDirectoryPath());
            return new ProfileViewModel(profileName, path, hasShipFile, isEnabled);
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
