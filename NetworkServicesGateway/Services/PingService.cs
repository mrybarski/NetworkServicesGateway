using NetworkServicesGateway.Models;
using System.Net;
using System.Net.NetworkInformation;
namespace NetworkServicesGateway.Services
{
    public class PingService : IDisposable, INetworkService
    {
        private readonly Ping ping;
        private readonly Func<NetworkActionResult, Task> sendUpdate;
        private CancellationTokenSource cancellationTokenSource;
        private TimeSpan sendPingInterval;
        private bool working = false;

        public PingService(Func<NetworkActionResult, Task> sendUpdate)
        {
            ping = new Ping();
            cancellationTokenSource = new();
            sendPingInterval = TimeSpan.FromMilliseconds(50);
            this.sendUpdate = sendUpdate;
        }

        public void RunService(IPAddress ipAddress)
        {
            if (working) return;

            cancellationTokenSource.TryReset();

            working = true;
            sendUpdate?.Invoke(NetworkActionResult.StartedResult);
            SendPing(ipAddress, cancellationTokenSource.Token);
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

        private void SendPing(IPAddress addressIP, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    working = false;
                    sendUpdate?.Invoke(NetworkActionResult.StoppedResult);
                    return;
                }

                var reply = ping.Send(addressIP);

                sendUpdate?.Invoke(ParseResult(reply));

                Task.Delay(sendPingInterval, cancellationToken)
                        .GetAwaiter()
                        .OnCompleted(() => SendPing(addressIP, cancellationToken));
            }
            catch
            {
                working = false;
                sendUpdate?.Invoke(NetworkActionResult.StoppedByExceptionResult);
            }
        }

        private NetworkActionResult ParseResult(PingReply reply)
        {
            var result = NetworkActionResult.RunningResult;
            result.Message = reply.Status switch
            {
                IPStatus.Success => $"Reply from {reply.Address}: {reply.Status} bytes={reply.Buffer.Length} time={reply.RoundtripTime}ms TTL={reply.Options?.Ttl}",
                _ => reply.Status.ToString()
            };
            return result;
        }
    }
}
