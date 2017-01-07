using System.Collections.Generic;
using System.IO;
using Cassiopeia.BitTorrent;
using Cassiopeia.Collections.ObjectModel;
using Cassiopeia.Converters;
using Cassiopeia.Models;
using Cassiopeia.Services;
using Cassiopeia.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace Cassiopeia.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private ObservableCollection<Torrent> _newTorrents;
        private Torrent _selectedNewTorrent;
        private ObservableCollection<Torrent> _torrents;
        private Torrent _selectedTorrent;
        private ObservableCollection<string> _cachedDownloadFolders;
        private ObservableCollection<string> _cachedCompletedDownloadFolders;
        private ObservableCollection<Session> _sessions;
        private ObservableCollection<string> _categories;

        public MainViewModel()
        {
            _newTorrents = new ObservableCollection<Torrent>();
            _torrents = new ObservableCollection<Torrent>();
            _sessions = new ObservableCollection<Session>
            {
                new Session("Torrents", new List<SessionCategory>
                {
                    new SessionCategory("Downloading", new List<Torrent>()),
                    new SessionCategory("Seeding", new List<Torrent>
                    {
                        new Torrent {Name = "Torrent 1"}
                    }),
                    new SessionCategory("Active", new List<Torrent>()),
                    new SessionCategory("Paused", new List<Torrent>()),
                    new SessionCategory("Queued", new List<Torrent>()),
                    new SessionCategory("Error", new List<Torrent>()),
                    new SessionCategory("Checking", new List<Torrent>()),
                }),
                new Session("Trackers", new List<SessionCategory>
                {
                    new SessionCategory("http://torrent.ubuntu.com:6969/announce", new List<Torrent>
                    {
                        new Torrent {Name = "Torrent 1"}
                    }),
                    new SessionCategory("Error", new List<Torrent>())
                })
            };
            _categories = new ObservableCollection<string>();
            _cachedDownloadFolders = new ObservableCollection<string>();
            _cachedCompletedDownloadFolders = new ObservableCollection<string>();
            AddTorrentsDialogCommand = new InputGestureCommand(ShowAddTorrentWindow, "Ctrl+N");
            OpenFileCommand = new InputGestureCommand(ShowOpenFileDialog, "Ctrl+O");
            AddTorrentsCommand = new RelayCommand<IDialog>(OnAddTorrents);
        }

        private void OnAddTorrents(IDialog dialog)
        {
            Torrents.AddRange(NewTorrents);
            dialog.CloseDialog();
        }

        public ObservableCollection<string> CachedCompletedDownloadFolders
        {
            get { return _cachedCompletedDownloadFolders; }
            set { Set(nameof(CachedCompletedDownloadFolders), ref _cachedCompletedDownloadFolders, value); }
        }

        public ObservableCollection<Session> Sessions
        {
            get { return _sessions; }
            set { Set(nameof(Sessions), ref _sessions, value); }
        }

        public ObservableCollection<string> CachedDownloadFolders
        {
            get { return _cachedDownloadFolders; }
            set { Set(nameof(CachedDownloadFolders), ref _cachedDownloadFolders, value); }
        }

        public InputGestureCommand AddTorrentsDialogCommand { get; }
        public RelayCommand<IDialog> AddTorrentsCommand { get; }
        public InputGestureCommand OpenFileCommand { get; }

        public Torrent SelectedTorrent
        {
            get { return _selectedTorrent; }
            set { Set(nameof(SelectedTorrent), ref _selectedTorrent, value); }
        }

        public ObservableCollection<string> Categories
        {
            get { return _categories; }
            set { Set(nameof(Categories), ref _categories, value); }
        }

        public Torrent SelectedNewTorrent
        {
            get { return _selectedNewTorrent; }
            set { Set(nameof(SelectedNewTorrent), ref _selectedNewTorrent, value); }
        }

        public ObservableCollection<Torrent> NewTorrents
        {
            get { return _newTorrents; }
            set { Set(nameof(NewTorrents), ref _newTorrents, value); }
        }

        public ObservableCollection<Torrent> Torrents
        {
            get { return _torrents; }
            set { Set(nameof(Torrents), ref _torrents, value); }
        }

        public void ShowOpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Torrent Files (*.torrent)|*.torrent|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                var bEncode = BEncode.Decode(File.ReadAllBytes(openFileDialog.FileName));
                var torrent = TorrentConverter.ConvertFromBEncode(bEncode);
                if (torrent != null)
                {
                    NewTorrents.Add(torrent);
                    SelectedNewTorrent = torrent;
                }
            }
        }

        private void ShowAddTorrentWindow()
        {
            WindowService.ShowWindow<AddTorrentsWindow>();
        }
    }
}