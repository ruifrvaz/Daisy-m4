using Daisy.Resources.Extensions;
using Daisy.Resources.Interfaces;
using Daisy.Resources.Pools;
using Daisy.Resources.Services;
using Daisy.Resources.Signals;
using Daisy.Transmitters.VectorStore.Services;

namespace Daisy.Transmitters.VectorStore
{
    public class FileUploadLoopback : ILoopBackTransmitter
    {
        private readonly IFileStoreService _vectorStoreService;

        public FileUploadLoopback()
        {
            _vectorStoreService = ServiceContainer.Instance.GetService<IFileStoreService>() as IFileStoreService;
        }

        public bool CanTransmit(Impulse impulse)
        {
            return impulse.Output.StartsWith("GitRepoFileLocation:");
        }

        public void TransmitLoopBack(Impulse impulse)
        {
            if (!CanTransmit(impulse))
            {
                return;
            }

            var filePath = impulse.Output.GetChainByKey("GitRepoFileLocation");

            var assistantId = "asst_VWtn2qYi5gaT0MPCppKeqLDq";

            var fileId = _vectorStoreService.UploadFileAsync(filePath).GetAwaiter().GetResult();

            if (fileId.Contains("error"))
            {
                impulse.Error = $"Error uploading file: {fileId}";
                Parallel.ForEach(ExternalTransmitters.Instance.Pool, p => p.Transmit(impulse));
            }

            var responseId = _vectorStoreService.LoadIntoCodeInterpreterAsync(assistantId, fileId).GetAwaiter().GetResult(); // convert to asyncronous on future iterations

            if (responseId.Contains("error"))
            {
                impulse.Error = $"Error uploading file: {fileId}";
                Parallel.ForEach(ExternalTransmitters.Instance.Pool, p => p.Transmit(impulse));
            }

            impulse.Input = impulse.Input.AddPrefix($"FileStored: {fileId} > ");

            System.Console.WriteLine("7. Loaded repository into code generator.");
            LoopBackReceivers.Instance.DispatchAsync(impulse);
        }
    }
}