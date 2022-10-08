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
        /// Gets the Shipbreaker companion downloaded profile saves directory path. This is where profile saves
        /// downloaded by the tool are stored.
        /// </summary>
        public static string GetCompanionDownloadedProfileSavesDirectoryPath()
        {
            return Path.Combine(GetCompanionFilesBasePath(), "Saves");
        }

        /// <summary>
        /// Gets the local path of the full Open Shift profile save file. This is where the Companion will save
        /// the file after downloading it, so it can be re-used without downloading afterwards.
        /// </summary>
        public static string GetCompanionFullProfileSaveFilePath()
        {
            // Note: the file name is important. If it's different, the game won't read it.
            return Path.Combine(GetCompanionDownloadedProfileSavesDirectoryPath(), "vglpp3_2195076518.lpw");
        }

        /// <summary>
        /// Gets the path to the final directory where the profile saves are found.
        /// </summary>
        public static string GetSaveDirectoryCompletePath()
        {
            return Path.Combine(GetSaveDirectoryBasePath(), "Profiles");
        }

        /// <summary>
        /// Gets the path to the directory for disabled save files.
        /// </summary>
        public static string GetDisabledSaveDirectoryPath()
        {
            return Path.Combine(GetSaveDirectoryCompletePath(), "Disabled");
        }

        /// <summary>
        /// Determines whether the given file path is directly in the specified directory path.
        /// </summary>
        /// <param name="filePath">File path to test.</param>
        /// <param name="directoryPath">Directory path to test.</param>
        public static bool IsInFolder(string filePath, string directoryPath)
        {
            string fileDirectoryPath = Path.GetDirectoryName(filePath);
            if (fileDirectoryPath == null)
                return false;

            return string.Compare(
                Path.GetFullPath(fileDirectoryPath)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                Path.GetFullPath(directoryPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
