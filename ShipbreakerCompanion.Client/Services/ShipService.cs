using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ShipbreakerCompanion.Client.Helpers;
using ShipbreakerCompanion.Client.Models;
using ShipbreakerCompanion.Client.ViewModels;

namespace ShipbreakerCompanion.Client.Services
{
    public class ShipService : IShipService
    {
        /// <summary>
        /// Gets the list of competitive ships as ship view models.
        /// </summary>
        public async Task<ICollection<ShipViewModel>> GetCompetitiveShipsAsync()
        {
            //todo: Use RestSharp to download from GitHub when it's uploaded there. In the meantime we read from disk
            await using var fs = new FileStream(@".\CompetitiveShips.json", FileMode.Open);
            var availableShips = await System.Text.Json.JsonSerializer.DeserializeAsync<Ship[]>(fs, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            //todo: See above

            var results = new List<ShipViewModel>();
            foreach (var ship in availableShips)
            {
                results.Add(new ShipViewModel(ship.Name, ship.DownloadUrl, GetLocalPathForShip(ship), ship.ExpectedChecksum));
            }
            return results;
        }

        /// <summary>
        /// Determines if the given ship is available locally or if it needs to be downloaded before.
        /// </summary>
        /// <param name="ship">Target ship.</param>
        public async Task<bool> IsShipLocallyAvailableAsync(ShipViewModel ship)
        {
            // If the file doesn't even exist locally, obviously it's not available.
            if (!File.Exists(ship.LocalFilePath))
                return false;

            // If the file exists and we don't have a checksum to check, consider it available.
            if (ship.ExpectedChecksum == null)
                return true;

            // Otherwise, we compare the checksum of the local file with the expected one.
            // If they don't match, consider the file isn't available locally. It should be overwritten with the correct one.
            string localFileChecksum = await ChecksumHelper.ComputeChecksumAsync(ship.LocalFilePath);
            return ship.ExpectedChecksum == localFileChecksum;
        }

        /// <summary>
        /// Downloads the file of the given ship from its download URL and write it in the application directory.
        /// </summary>
        /// <param name="ship">Target ship.</param>
        public async Task DownloadShipFileAsync(ShipViewModel ship)
        {
            Directory.CreateDirectory(PathHelper.GetCompanionCompetitiveShipsDirectoryPath());

            //todo: Use RestSharp when the ship files are uploaded
        }

        /// <summary>
        /// Builds and returns the absolute path to store the given ship file after download.
        /// </summary>
        /// <param name="ship">Target ship.</param>
        private string GetLocalPathForShip(Ship ship)
        {
            string directoryPath = ship.IsCompetitive
                ? PathHelper.GetCompanionCompetitiveShipsDirectoryPath()
                : PathHelper.GetCompanionShipsDirectoryPath();

            return Path.Combine(directoryPath, ship.FileName + ".ship");
        }

    }
}
