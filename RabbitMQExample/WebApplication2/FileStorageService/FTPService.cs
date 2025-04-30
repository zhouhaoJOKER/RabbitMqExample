using WebApplication2.FileStorage;

namespace WebApplication2.FileStorageService
{
    public class FTPService : IFileStorageService
    {
        private readonly IConfiguration _configuration;

        public FTPService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        FileStorageType IFileStorageService.StorageType => FileStorageType.FTP;

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
