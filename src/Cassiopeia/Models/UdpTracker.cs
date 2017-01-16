using System;

namespace Cassiopeia.Models
{
    internal class UdpTracker : Tracker
    {
        public UdpTracker(Uri uri) : base(uri)
        {
        }

        public override void AnnounceAsync(AnnounceParameters parameters)
        {
            // TODO: Implement
            return;
        }

        public override void ScrapeAsync()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }
    }
}