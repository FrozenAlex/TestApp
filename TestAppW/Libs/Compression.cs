using Ionic.Zip;
using System.Diagnostics;
using System.IO;

namespace TestApp.Libs
{
    /// <summary>
    /// Class for working with zip file methods
    /// </summary>
    class Compression
    {
        /// <summary>
        /// Clears and unpacks zip to folder
        /// </summary>
        /// <param name="zipToUnpack">Source zip</param>
        /// <param name="unpackDirectory">Directory to extract to</param>
        /// <param name="password">Password if present</param>
        public static void Unpack(string zipToUnpack, string unpackDirectory, string password = null)
        {   
            if (Directory.Exists(unpackDirectory)) Directory.Delete(unpackDirectory, true);
            using (ZipFile zip = ZipFile.Read(zipToUnpack))
            {
                zip.Password = password;
                //zip.AlternateEncoding = System.Text.Encoding.UTF8;
                zip.ExtractProgress += Zip_ExtractProgress; // Set listener
                zip.ExtractAll(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public static void Pack(string dirToPack, string zipTarget, string password = null, string comment = null)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.Password = password;
                zip.AlternateEncoding = System.Text.Encoding.UTF8;
                zip.Comment = comment;
                if (password != null) zip.Encryption = EncryptionAlgorithm.WinZipAes256;
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                zip.AddDirectory(dirToPack);
                zip.Save(zipTarget);
            }
        }
        private static void Zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {

        }
    }
}
