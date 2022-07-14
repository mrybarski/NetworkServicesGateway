using NetworkServicesGateway.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace NetworkServicesGateway.Services
{
    public class TracertService : IDisposable, INetworkService
    {
        private readonly Ping ping;
        private readonly Func<NetworkActionResult, Task> sendUpdate;
        private CancellationTokenSource cancellationTokenSource;

        private bool working = false;
        private byte[] testData = Encoding.ASCII.GetBytes("asdasdasdasdasdasd");

        public TracertService(Func<NetworkActionResult, Task> sendUpdate)
        {
            ping = new Ping();
            cancellationTokenSource = new();
            this.sendUpdate = sendUpdate;
        }

        public void RunService(IPAddress ipAddress)
        {
            if (working) return;

            cancellationTokenSource.TryReset();

            working = true;
            sendUpdate?.Invoke(NetworkActionResult.StartedResult);
            SendPing(ipAddress, cancellationTokenSource.Token);
            working = false;
            sendUpdate?.Invoke(NetworkActionResult.StoppedResult);
        }

        public void StopService()
        {
            if (!working) return;

            cancellationTokenSource?.Cancel();
            ping.SendAsyncCancel();
        }

        public void Dispose()
        {
            if (working)
                StopService();

            cancellationTokenSource?.Dispose();
            ping.Dispose();
        }

        private void SendPing(IPAddress addressIP, CancellationToken cancellationToken, int ttl = 1)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    sendUpdate?.Invoke(NetworkActionResult.StoppedResult);
                    working = false;
                    return;
                }

                var pingOpts = new PingOptions(ttl, true);
                var reply = ping.Send(addressIP, 1000, testData, pingOpts);

                sendUpdate?.Invoke(ParseResult(ttl, reply));

                if (reply.Status == IPStatus.TtlExpired)
                {
                    SendPing(addressIP, cancellationToken, ++ttl);
                }
            }
            catch
            {
                working = false;
                sendUpdate?.Invoke(NetworkActionResult.StoppedByExceptionResult);
            }
        }

        private NetworkActionResult ParseResult(int ttl, PingReply reply)
        {
            var printStatus = reply.Status != IPStatus.TtlExpired;
            var result = NetworkActionResult.RunningResult;
            result.Message = printStatus ? $"{ttl} - {reply.Address} - {reply.Status}" : $"{ttl} - {reply.Address}";
            return result;
        }
    }
}
