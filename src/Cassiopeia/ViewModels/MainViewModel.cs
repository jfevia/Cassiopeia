using System.IO;
using System.Windows.Input;
using Cassiopeia.BitTorrent;
using Cassiopeia.Converters;
using Cassiopeia.Models;
using Cassiopeia.Services;
using Cassiopeia.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cassiopeia.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private Torrent _torrent;

        public MainViewModel()
        {
            AddTorrentCommand = new RelayCommand(ShowAddTorrentWindow);
            OpenFileCommand = new InputGestureCommand(ShowOpenFileDialog, "Ctrl+O");
        }

        public ICommand AddTorrentCommand { get; }
        public InputGestureCommand OpenFileCommand { get; }

        public Torrent Torrent
        {
            get { return _torrent; }
            private set { Set(nameof(Torrent), ref _torrent, value); }
        }

        public void ShowOpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Torrent Files (*.torrent)|*.torrent|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                var bEncode = BEncode.Decode(File.ReadAllBytes(openFileDialog.FileName));
                //var serializer = new JsonSerializer();
                //serializer.Formatting = Formatting.Indented;
                //serializer.NullValueHandling = NullValueHandling.Ignore;
                //using (var sw = new StreamWriter(@"bencode.txt"))
                //using (JsonWriter writer = new JsonTextWriter(sw))
                //{
                //    serializer.Serialize(writer, bEncode);
                //}
                Torrent = TorrentConverter.ConvertFromBEncode(bEncode);
            }
        }

        private void ShowAddTorrentWindow()
        {
            WindowService.ShowWindow<AddTorrentWindow>();
        }
    }
}