#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
// windows azure storage
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
#endregion

namespace treeDiM.FileTransfer
{
    public class FileTransferUtility
    {
        #region Upload / Download methods
        public static Guid UploadFile(string filePath)
        {
            Guid guid = Guid.NewGuid();
            CloudBlobContainer container = Container;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileNameFromGuid(guid, Path.GetExtension(filePath)));

            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                blockBlob.UploadFromStream(fileStream);
            }
            return guid;
        }
        public static string DownloadFile(Guid guid, string fileExt)
        {
            // build file path
            string filePath = Path.Combine(TempCacheDirectory, FileNameFromGuid(guid, fileExt));

            if (!File.Exists(filePath))
            {
                CloudBlobContainer container = Container;
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileNameFromGuid(guid, fileExt));

                using (var fileStream = System.IO.File.OpenWrite(filePath))
                {
                    blockBlob.DownloadToStream(fileStream);
                }
            }
            return filePath;
        }
        #endregion

        #region Checking blob existence
        public static bool BlobExist(Guid guid, string fileExt)
        {
            try
            {
                CloudBlobContainer container = Container;
                return container.GetBlockBlobReference(FileNameFromGuid(guid, fileExt)).Exists();
            }
            catch (Exception /*ex*/)
            {
                return false;
            }
        }
        #endregion

        #region Public helpers
        public static string FileNameFromGuid(Guid g, string fileExt)
        {
            return g.ToString().Replace("-", "_") + fileExt;
        }
        public static void ClearFileCache()
        {
            Directory.Delete(TempCacheDirectory, true);
        }
        #endregion

        #region Private helpers
        private static string TempCacheDirectory
        {
            get
            {
                string treeDiMPath = Path.Combine(Path.GetTempPath(), "treeDiM");
                if (!Directory.Exists(treeDiMPath))
                    Directory.CreateDirectory(treeDiMPath);
                string PLMPackLibPath = Path.Combine(treeDiMPath, "PLMPackLib");
                if (!Directory.Exists(PLMPackLibPath))
                    Directory.CreateDirectory(PLMPackLibPath);
                string fileCachePath = Path.Combine(PLMPackLibPath, "FileCache");
                if (!Directory.Exists(fileCachePath))
                    Directory.CreateDirectory(fileCachePath);
                return fileCachePath;
            }
        }
        private static CloudBlobContainer Container
        {
            get
            {
                var storageAccount = CloudStorageAccount.Parse(Settings.Default.StorageConnectionString);
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("documents");
                container.CreateIfNotExists();
                return container;
            }
        }
        #endregion
    }
}
