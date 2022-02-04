// A letöltött fájlok ellenőrzéséhez kell

using System.IO;
using System.Security.Cryptography;

namespace PhpComposerInstaller {
    internal class Checksum {
        private SHA256 _sha256;
        public Checksum() {
            _sha256 = SHA256.Create();
        }

        private byte[] HashFile(string filename) {
            using (FileStream stream = File.OpenRead(filename)) {
                return _sha256.ComputeHash(stream);
            }
        }

        private string BytesToString(byte[] bytes) {
            var result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

        public bool Check(string filename, string expectedHash) {
            var hashBytes = HashFile(filename);
            var computedHash = BytesToString(hashBytes);
            return expectedHash.Equals(computedHash);
        }
    }
}
