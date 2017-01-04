using System.IO;
using System.Windows.Input;
using Cassiopeia.BitTorrent;
using Cassiopeia.Models;
using Cassiopeia.Services;
using Cassiopeia.Views;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace Cassiopeia.ViewModels
{
    internal class MainViewModel
    {
        public MainViewModel()
        {
            AddTorrentCommand = new RelayCommand(ShowAddTorrentWindow);
            OpenFileCommand = new InputGestureCommand(ShowOpenFileDialog, "Ctrl+O");
        }

        public ICommand AddTorrentCommand { get; }
        public InputGestureCommand OpenFileCommand { get; }

        public void ShowOpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Torrent Files (*.torrent)|*.torrent|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                var bEncode = BEncode.Decode(File.ReadAllBytes(openFileDialog.FileName));
                // TODO: Process bEncode
            }
        }

        private void ShowAddTorrentWindow()
        {
            WindowService.ShowWindow<AddTorrentWindow>();
        }
    }
}