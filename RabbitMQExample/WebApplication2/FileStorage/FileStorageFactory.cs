namespace WebApplication2.FileStorage
{
    public class FileStorageFactory : IFileStorageFactory
    {
        private readonly IEnumerable<IFileStorageService> _storages;

        public FileStorageFactory(IEnumerable<IFileStorageService> storages)
        {
            _storages = storages;
        }

        public IFileStorageService GetFileStorageService(FileStorageType type)
        {
            return _storages.Single(d => d.StorageType == type);
        }
    }
}
