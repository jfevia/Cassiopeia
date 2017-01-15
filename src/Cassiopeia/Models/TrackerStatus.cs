namespace Cassiopeia.Models
{
    internal enum TrackerStatus
    {
        Working,
        TimeOut,
        HostnameNotFound,
        ConnectionClosedByPeer,
        NoSuchHostKnown,
        Offline,
        InvalidResponse
    }
}