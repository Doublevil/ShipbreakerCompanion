using System.Collections.Generic;
using System.Threading.Tasks;
using ShipbreakerCompanion.Client.ViewModels;

namespace ShipbreakerCompanion.Client.Services
{
    /// <summary>
    /// Provides methods related to the ship list.
    /// </summary>
    public interface IShipService
    {
        /// <summary>
        /// Gets the list of competitive ships as ship view models.
        /// </summary>
        Task<ICollection<ShipViewModel>> GetCompetitiveShipsAsync();

        /// <summary>
        /// Determines if the given ship is available locally or if it needs to be downloaded before.
        /// </summary>
        /// <param name="ship">Target ship.</param>
        Task<bool> IsShipLocallyAvailableAsync(ShipViewModel ship);

        /// <summary>
        /// Downloads the file of the given ship from its download URL and write it in the application directory.
        /// </summary>
        /// <param name="ship">Target ship.</param>
        Task DownloadShipFileAsync(ShipViewModel ship);
    }
}
