using System.Net.NetworkInformation;

namespace NetworkServicesGateway.Models
{
    public enum NetworkServiceState
    {
        Started = 1,
        Running,
        Stopped,
        StoppedByException
    }

    public class NetworkActionResult
    {
        public NetworkServiceState State { get; set; }
        public string Message { get; set; } = default!;
        public static NetworkActionResult StartedResult => new NetworkActionResult()
        {
            State = NetworkServiceState.Started
        };
        public static NetworkActionResult RunningResult => new NetworkActionResult()
        {
            State = NetworkServiceState.Running
        };
        public static NetworkActionResult StoppedResult => new NetworkActionResult()
        {
            State = NetworkServiceState.Stopped
        };
        public static NetworkActionResult StoppedByExceptionResult => new NetworkActionResult()
        {
            State = NetworkServiceState.StoppedByException
        };
    }
}
