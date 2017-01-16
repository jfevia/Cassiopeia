using System.Net;
using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class CassiopeiaSettings : ObservableObject
    {
        private string _reportedHostname;
        private int _reportedPort;
        private int _maximumPeerCount;
        private bool _requiresEncryption;
        private bool _useCompactResponse;

        public CassiopeiaSettings()
        {
            _maximumPeerCount = 30;
            _requiresEncryption = false;
            _useCompactResponse = true;
        }

        public string ReportedHostname
        {
            get { return _reportedHostname; }
            set { Set(nameof(ReportedHostname), ref _reportedHostname, value); }
        }

        public int ReportedPort
        {
            get { return _reportedPort; }
            set { Set(nameof(ReportedPort), ref _reportedPort, value); }
        }

        public int MaximumPeerCount
        {
            get { return _maximumPeerCount; }
            set { Set(nameof(MaximumPeerCount), ref _maximumPeerCount, value); }
        }

        public bool RequiresEncryption
        {
            get { return _requiresEncryption; }
            set { Set(nameof(RequiresEncryption), ref _requiresEncryption, value); }
        }

        public bool UseCompactResponse
        {
            get { return _useCompactResponse; }
            set { Set(nameof(UseCompactResponse), ref _useCompactResponse, value); }
        }
    }
}