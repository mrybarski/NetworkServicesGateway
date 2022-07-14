using Microsoft.AspNetCore.SignalR;
using NetworkServicesGateway.Hubs;
using NetworkServicesGateway.Models;
using NetworkServicesGateway.Services;
using System.Collections.Concurrent;
using System.Net;

namespace NetworkServicesGateway.Data
{
    public class NetworkServicesContext
    {
        private readonly IHubContext<NetworkUpdatesMonitorHub, INetworkUpdatesMonitor> updatesHub;
        private readonly ConcurrentDictionary<string, INetworkService> usersServices = new();

        public NetworkServicesContext(IHubContext<NetworkUpdatesMonitorHub, INetworkUpdatesMonitor> updatesHub)
        {
            this.updatesHub = updatesHub;
        }

        public void RunService(string connectionId, NetworkServiceType networkServiceType, IPAddress iPAddress)
        {
            if (usersServices.TryGetValue(connectionId, out var existedService))
            {
                existedService.RunService(iPAddress);
                return;
            }

            INetworkService service = networkServiceType switch
            {
                NetworkServiceType.Ping => new PingService(updatesHub.Clients.Client(connectionId).SendUpdate),
                NetworkServiceType.TraceRoute => new TracertService(updatesHub.Clients.Client(connectionId).SendUpdate),
                _ => throw new NotImplementedException()
            };

            if (!usersServices.TryAdd(connectionId, service))
                throw new InvalidOperationException("Service already exist");

            service.RunService(iPAddress);
        }

        public void StopService(string connectionId)
        {
            usersServices.TryRemove(connectionId, out var service);
            service?.StopService();
            service?.Dispose();
        }
    }
}
