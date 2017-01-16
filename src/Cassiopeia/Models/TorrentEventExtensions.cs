namespace Cassiopeia.Models
{
    public static class TorrentEventExtensions
    {
        public static string ToAnnounceParameter(this TorrentEvent torrentEvent)
        {
            switch (torrentEvent)
            {
                case TorrentEvent.Started:
                    return "started";
                case TorrentEvent.Stopped:
                    return "stopped";
                case TorrentEvent.Completed:
                    return "completed";
                default:
                    return string.Empty;
            }
        }
    }
}