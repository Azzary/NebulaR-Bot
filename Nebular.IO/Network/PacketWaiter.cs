using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nebular.Core.Network
{
    public class PacketWaiter
    {
        private readonly List<string> packetsToWaitFor;
        private readonly SemaphoreSlim semaphore;

        public PacketWaiter(List<string> packets)
        {
            packetsToWaitFor = new List<string>(packets);
            semaphore = new SemaphoreSlim(0, 1);
        }

        public bool IsWaiting => packetsToWaitFor.Count > 0;

        public async Task<bool> WaitForPacket(int timeout)
        {
            if (IsWaiting)
            {
                await semaphore.WaitAsync(timeout);
            }

            return packetsToWaitFor.Count == 0;
        }

        public void CheckPacket(string packet)
        {
            foreach (string expectedPacket in packetsToWaitFor)
            {
                if (packet.StartsWith(expectedPacket))
                {
                    packetsToWaitFor.Remove(expectedPacket);
                    if (packetsToWaitFor.Count == 0)
                    {
                        semaphore.Release();
                    }
                    break;
                }
            }
        }
    }
}
