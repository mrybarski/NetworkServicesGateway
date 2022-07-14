using System.Net;

namespace NetworkServicesGateway.Services
{
    public enum NetworkServiceType
    {
        Ping = 1,
        TraceRoute
    }
    interface INetworkService
    {
        void RunService(IPAddress ipAddress);
        void StopService();
        void Dispose();
    }
}
