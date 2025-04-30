namespace WebApplication2.FileStorage
{
    public interface IFileStorageFactory
    {
        IFileStorageService GetFileStorageService(FileStorageType type);
    }
}
