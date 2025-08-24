using Daisy.Resources.Interfaces;

namespace Daisy.Transmitters.VectorStore.Services
{
    public interface IFileStoreService : IDaisyService
    {
        public Task<string> UploadFileAsync(string filePath);

        public Task<string> LoadIntoVectorStoreAsync(string assistantId, string fileId);

        public Task<string> LoadIntoCodeInterpreterAsync(string assistantId, string fileId);

    }
}