using WebApplication2.FileStorage;

namespace WebApplication2.FileStorageService
{
    public class LocalFileService : IFileStorageService
    {
        public LocalFileService()
        {
            
        }

        FileStorageType IFileStorageService.StorageType => FileStorageType.Local;

        Task IFileStorageService.DeleteFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        Task<Stream> IFileStorageService.DownloadFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        Task<bool> IFileStorageService.FileExistsAsync(string fileName)
        {
            throw new NotImplementedException();
        }

         
        Task IFileStorageService.UploadFileAsync(string fileName, Stream fileStream)
        {
            throw new NotImplementedException();
        }
    } 
}
