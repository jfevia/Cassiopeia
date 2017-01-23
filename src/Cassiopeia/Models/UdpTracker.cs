using System;

namespace Cassiopeia.Models
{
    internal class UdpTracker : Tracker
    {
        public UdpTracker(Uri uri) : base(uri)
        {
        }

        public override void Announce(AnnounceParameters parameters)
        {
            // TODO: Implement
            return;
        }

        public override void Scrape()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }
}