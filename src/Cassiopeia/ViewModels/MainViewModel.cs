using System;
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
        private ObservableCollection<string> _cachedCompletedDownloadFolders;
        private ObservableCollection<string> _cachedDownloadFolders;
        private ObservableCollection<string> _categories;
        private ObservableCollection<Torrent> _newTorrents;
        private ObservableCollection<Option> _options;
        private Torrent _selectedNewTorrent;
        private Torrent _selectedTorrent;
        private ObservableCollection<Session> _sessions;
        private ObservableCollection<Torrent> _torrents;
        private string _searchOptionCriteria;

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
                    new SessionCategory("Checking", new List<Torrent>())
                }),
                new Session("Trackers", new List<SessionCategory>
                {
                    new SessionCategory("http://torrent.ubuntu.com:6969/announce", new List<Torrent>
                    {
                        new Torrent {Name = "Torrent 1"}
                    }),
                    new SessionCategory("Error", new List<Torrent>())
                }),
                new Session("Categories", new List<SessionCategory>
                {
                    new SessionCategory("Ubuntu", new List<Torrent>
                    {
                        new Torrent {Name = "Torrent 1"}
                    }),
                    new SessionCategory("Unknown", new List<Torrent>())
                })
            };
            _options = new ObservableCollection<Option>
            {
                new Option("Environment", OptionType.General, new[]
                {
                    new Option("General"),
                    new Option("Keyboard & Shortcuts")
                }),
                new Option("Directories", OptionType.Directories, new[]
                {
                    new Option("Downloaded Files"),
                    new Option("Torrent Files")
                }),
                new Option("Connections", OptionType.Connections, new[]
                {
                    new Option("Listening Ports"),
                    new Option("Proxy Settings")
                }),
                new Option("Bandwidth", OptionType.Bandwidth, new[]
                {
                    new Option("Global Rate Limits"),
                    new Option("Download Rate Limits"),
                    new Option("Seeding Rate Limits")
                }),
                new Option("BitTorrent", OptionType.BitTorrent, new[]
                {
                    new Option("Initial Seeding"),
                    new Option("DHT"),
                    new Option("μTP")
                }),
                new Option("Advanced", OptionType.Advanced, new[]
                {
                    new Option("Disk I/O"),
                    new Option("Network"),
                    new Option("Peer Settings"),
                    new Option("Queue Settings"),
                    new Option("RSS")
                })
            };
            _categories = new ObservableCollection<string>();
            _cachedDownloadFolders = new ObservableCollection<string>();
            _cachedCompletedDownloadFolders = new ObservableCollection<string>();
            AddTorrentsDialogCommand = new InputGestureCommand(ShowAddTorrentDialog, "Ctrl+N");
            OptionsDialogCommand = new InputGestureCommand(ShowOptionsDialog, "Ctrl+P");
            OpenFileCommand = new InputGestureCommand(ShowOpenFileDialog, "Ctrl+O");
            AddTorrentsCommand = new RelayCommand<IDialog>(OnAddTorrents);
            SaveOptionsCommand = new RelayCommand(OnSaveOptions);
            SearchOptionCriteriaChangedCommand = new RelayCommand(OnSearchOptionCriteriaChanged);
            StartSelectedTorrentCommand = new InputGestureCommand(StartSelectedTorrent, CanExecuteStartSelectedTorrent,
                "Ctrl+S");
        }

        private void OnSearchOptionCriteriaChanged()
        {
            ApplyOptionsFilters();
        }

        private void ApplyOptionsFilters()
        {
            foreach (var option in _options)
                option.ApplyCriteria(SearchOptionCriteria, new Stack<Option>());
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

        public ObservableCollection<Option> Options
        {
            get { return _options; }
            set { Set(nameof(Options), ref _options, value); }
        }

        public ObservableCollection<string> CachedDownloadFolders
        {
            get { return _cachedDownloadFolders; }
            set { Set(nameof(CachedDownloadFolders), ref _cachedDownloadFolders, value); }
        }

        public InputGestureCommand AddTorrentsDialogCommand { get; }
        public InputGestureCommand OptionsDialogCommand { get; }
        public RelayCommand<IDialog> AddTorrentsCommand { get; }
        public RelayCommand SaveOptionsCommand { get; }
        public RelayCommand SearchOptionCriteriaChangedCommand { get; }
        public InputGestureCommand OpenFileCommand { get; }
        public InputGestureCommand StartSelectedTorrentCommand { get; }

        public Torrent SelectedTorrent
        {
            get { return _selectedTorrent; }
            set { Set(nameof(SelectedTorrent), ref _selectedTorrent, value); }
        }

        public string SearchOptionCriteria
        {
            get { return _searchOptionCriteria; }
            set { Set(nameof(SearchOptionCriteria), ref _searchOptionCriteria, value); }
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

        private void ShowOptionsDialog()
        {
            WindowService.ShowWindow<OptionsWindow>();
        }

        private void OnSaveOptions()
        {
            throw new NotImplementedException();
        }

        private bool CanExecuteStartSelectedTorrent()
        {
            return SelectedTorrent != null && SelectedTorrent.Status == TorrentStatus.Paused;
        }

        private void StartSelectedTorrent()
        {
            throw new NotImplementedException();
        }

        private void OnAddTorrents(IDialog dialog)
        {
            Torrents.AddRange(NewTorrents);
            dialog.CloseDialog();
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

        private void ShowAddTorrentDialog()
        {
            WindowService.ShowWindow<AddTorrentsWindow>();
        }
    }
}