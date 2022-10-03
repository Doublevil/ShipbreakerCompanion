using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ShipbreakerCompanion.Client.Helpers
{
    /// <summary>
    /// Provides consistent methods related to computing file checksums.
    /// </summary>
    public static class ChecksumHelper
    {
        /// <summary>
        /// Computes and returns the checksum of the file at the given path, as a string.
        /// </summary>
        /// <param name="filePath">Path to the file to check.</param>
        public static async Task<string> ComputeChecksumAsync(string filePath)
        {
            using var md5 = MD5.Create();
            await using var stream = File.OpenRead(filePath);
            var localFileHashBytes = await md5.ComputeHashAsync(stream);
            return BitConverter.ToString(localFileHashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
