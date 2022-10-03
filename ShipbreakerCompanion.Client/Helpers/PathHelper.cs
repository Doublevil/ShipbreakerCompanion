using System;
using System.IO;

namespace ShipbreakerCompanion.Client.Helpers
{
    /// <summary>
    /// Provides methods to get directory and file paths for the game files.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Gets the path to the base directory of the game's saves.
        /// </summary>
        public static string GetSaveDirectoryBasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "AppData",
                "LocalLow",
                "Blackbird Interactive",
                "Hardspace_ Shipbreaker",
                "Saves");
        }

        /// <summary>
        /// Gets the base Shipbreaker companion directory path, where files used by the application can be found.
        /// </summary>
        public static string GetCompanionFilesBasePath()
        {
            return Path.Combine(GetSaveDirectoryBasePath(), "ShipbreakerCompanion");
        }

        /// <summary>
        /// Gets the Shipbreaker companion ships directory path, where ship files can be found.
        /// </summary>
        public static string GetCompanionShipsDirectoryPath()
        {
            return Path.Combine(GetCompanionFilesBasePath(), "Ships");
        }

        /// <summary>
        /// Gets the Shipbreaker companion competitive ships directory path, which is a subdirectory of the one
        /// returned by <see cref="GetCompanionShipsDirectoryPath"/>, containing only the competitive ships.
        /// </summary>
        public static string GetCompanionCompetitiveShipsDirectoryPath()
        {
            return Path.Combine(GetCompanionShipsDirectoryPath(), "Competitive");
        }

        /// <summary>
        /// Gets the path to the final directory where the profile saves are found.
        /// </summary>
        public static string GetSaveDirectoryCompletePath()
        {
            return Path.Combine(GetSaveDirectoryBasePath(), "Profiles");
        }
    }
}
