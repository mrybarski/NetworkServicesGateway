using Microsoft.AspNetCore.SignalR;
using NetworkServicesGateway.Data;
using NetworkServicesGateway.Models;
using NetworkServicesGateway.Services;
using System.Net;

namespace NetworkServicesGateway.Hubs
{
    public interface INetworkUpdatesMonitor
    {
        Task SendUpdate(NetworkActionResult result);
        Task SendMessage(string message);
    }

    public class NetworkUpdatesMonitorHub : Hub<INetworkUpdatesMonitor>
    {
        private readonly NetworkServicesContext _networkServicesContext;

        public NetworkUpdatesMonitorHub(NetworkServicesContext networkServicesContext)
        {
            _networkServicesContext = networkServicesContext;
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _networkServicesContext.StopService(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendUpdate(NetworkActionResult result)
        {
            await Clients.Caller.SendUpdate(result);
        }

        public Task StartNetworkTask(NetworkServiceType networkServiceType, string addressIp)
        {
            try
            {
                if (!IPAddress.TryParse(addressIp, out var ipAddress))
                {
                    Clients.Caller.SendMessage("Błędny adres IP");
                    return Task.CompletedTask;
                }
                _networkServicesContext.RunService(Context.ConnectionId, networkServiceType, ipAddress);
            }
            catch (Exception ex)
            {
                Clients.Caller.SendMessage($"Wystąpił błąd, ex: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public Task StopNetworkTask()
        {
            _networkServicesContext.StopService(Context.ConnectionId);

            return Task.CompletedTask;
        }
    }
}
