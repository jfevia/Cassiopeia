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

        public MainViewModel()
        {
            NewTorrents = new ObservableCollection<Torrent>();
            Torrents = new ObservableCollection<Torrent>();
            Categories = new ObservableCollection<string>();
            CachedDownloadFolders = new ObservableCollection<string>();
            CachedCompletedDownloadFolders = new ObservableCollection<string>();
            AddTorrentsDialogCommand = new InputGestureCommand(ShowAddTorrentWindow, "Ctrl+N");
            OpenFileCommand = new InputGestureCommand(ShowOpenFileDialog, "Ctrl+O");
            AddTorrentsCommand = new RelayCommand<IDialog>(OnAddTorrents);
        }

        private void OnAddTorrents(IDialog dialog)
        {
            Torrents.AddRange(NewTorrents);
            dialog.CloseDialog();
        }

        public ObservableCollection<string> CachedCompletedDownloadFolders { get; set; }

        public ObservableCollection<string> CachedDownloadFolders { get; set; }

        public InputGestureCommand AddTorrentsDialogCommand { get; }
        public RelayCommand<IDialog> AddTorrentsCommand { get; }
        public InputGestureCommand OpenFileCommand { get; }

        public Torrent SelectedTorrent
        {
            get { return _selectedTorrent; }
            set { Set(nameof(SelectedTorrent), ref _selectedTorrent, value); }
        }

        public ObservableCollection<string> Categories { get; set; }

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