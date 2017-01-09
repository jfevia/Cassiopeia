using System.Collections.Generic;
using System.Linq;
using Cassiopeia.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class FileItem : ObservableObject
    {
        private long _size;
        private string _md5;
        private ObservableCollection<string> _path;
        private Priority _downloadPriority;
        private double _progress;

        public FileItem()
        {
            Path = new ObservableCollection<string>();
        }

        public long Size
        {
            get { return _size; }
            set { Set(nameof(Size), ref _size, value); }
        }

        public ObservableCollection<string> Path
        {
            get { return _path; }
            set { Set(nameof(Path), ref _path, value); }
        }

        public string Md5
        {
            get { return _md5; }
            set { Set(nameof(Md5), ref _md5, value); }
        }

        public double Progress
        {
            get { return _progress; }
            set { Set(nameof(Progress), ref _progress, value); }
        }

        public Priority DownloadPriority
        {
            get { return _downloadPriority; }
            set { Set(nameof(DownloadPriority), ref _downloadPriority, value); }
        }

        public string Name
        {
            get { return Path.Count > 0 ? Path.Last() : null; }
        }
    }
}