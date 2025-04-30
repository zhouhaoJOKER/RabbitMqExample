using WebApplication2.FileStorage;

namespace WebApplication2.FileStorageService
{
    public class MinioService : IFileStorageService
    {
        FileStorageType IFileStorageService.StorageType => FileStorageType.MinIO;

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
