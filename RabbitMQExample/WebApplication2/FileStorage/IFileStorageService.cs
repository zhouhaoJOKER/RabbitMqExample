namespace WebApplication2.FileStorage
{
    public interface IFileStorageService
    {
        FileStorageType StorageType { get; } // 标识当前服务的类型
        Task UploadFileAsync(string fileName, Stream fileStream);
        Task<Stream> DownloadFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
        Task<bool> FileExistsAsync(string fileName);
    }

    public enum FileStorageType
    {
        Local,
        FTP,
        MinIO
    }
}
