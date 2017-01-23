using System;
using System.Collections.Generic;

namespace Cassiopeia.Models
{
    internal class AnnounceEventArgs : EventArgs
    {
        public AnnounceEventArgs(TrackerStatus status, List<Peer> peers)
        {
            Status = status;
            Peers = peers;
        }

        public TrackerStatus Status { get; }
        public List<Peer> Peers { get; }
    }
}