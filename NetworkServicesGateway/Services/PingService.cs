using System.Net;
using System.Net.NetworkInformation;
namespace NetworkServicesGateway.Services
{
    public class PingService : IDisposable
    {
        private readonly Ping ping;
        private CancellationTokenSource cancellationTokenSource;
        private TimeSpan sendPingInterval;

        public PingService()
        {
            ping = new Ping();
            cancellationTokenSource = new();
            sendPingInterval = TimeSpan.FromSeconds(1);
        }

        public void RunService(string ipAdressStr)
        {
            if (!IPAddress.TryParse(ipAdressStr, out var ipAddress)) 
                throw new ArgumentException("Błędny adres IP", nameof(ipAdressStr));

            SendPing(ipAddress, cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();

            ping.SendAsyncCancel();
        }

        private void SendPing(IPAddress ipAdress, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var reply = ping.Send(ipAdress);
                System.Diagnostics.Debug.WriteLine(reply.RoundtripTime);

                Task.Delay(sendPingInterval, cancellationToken)
                        .GetAwaiter()
                        .OnCompleted(() => SendPing(ipAdress, cancellationToken));
            }
            catch
            {
            }
        }
    }
}
