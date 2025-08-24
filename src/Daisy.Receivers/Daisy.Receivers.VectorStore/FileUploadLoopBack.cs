using Daisy.Resources.Abstracts;
using Daisy.Resources.Signals;
using System.Threading.Tasks;
using System;
using Daisy.Resources.Extensions;

namespace Daisy.Receivers.VectorStore
{
    public class FileUploadLoopBack : ALoopBackReceiver
    {
        public override bool CanReceive(Impulse impulse)
        {
            return impulse.Input.StartsWith("FileStored:", StringComparison.InvariantCultureIgnoreCase);
        }

        public async override Task ReceiveLoopBack(Impulse impulse)
        {
            if (CanReceive(impulse))
            {
                var fileId = impulse.Input.GetChainByKey("FileStored");

                Console.WriteLine($"8. Received file upload confirmation: {fileId}.");
                await base.ReceiveLoopBack(impulse);
            }
        }
    }
}