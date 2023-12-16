using System.IO;
using System.Security.Cryptography;

namespace PhpComposerInstaller {
    /// <summary>
    /// Checksum utility class.
    /// </summary>
    internal class Checksum {
        private readonly SHA256 sha256;

        /// <summary>
        /// Constructor, initializes the SHA256 utility.
        /// </summary>
        public Checksum() {
            sha256 = SHA256.Create();
        }

        /// <summary>
        /// Computes the SHA256 checksum of the given file.
        /// </summary>
        private byte[] HashFile(string filename) {
            using (FileStream stream = File.OpenRead(filename)) {
                return sha256.ComputeHash(stream);
            }
        }

        /// <summary>
        /// Converts the given byte array to a string.
        /// </summary>
        private string BytesToString(byte[] bytes) {
            var result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

        /// <summary>
        /// Checks if the given file has the given checksum.
        /// </summary>
        public bool Check(string filename, string expectedHash) {
            var hashBytes = HashFile(filename);
            var computedHash = BytesToString(hashBytes);
            return expectedHash.Equals(computedHash);
        }
    }
}
