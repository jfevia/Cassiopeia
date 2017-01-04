using System;
using System.Diagnostics;
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
            if (openFileDialog.ShowDialog() == true)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                BEncode.Decode(File.ReadAllBytes(openFileDialog.FileName));
                Console.WriteLine($"File read completed in {stopwatch.Elapsed}");
            }
        }

        private void ShowAddTorrentWindow()
        {
            WindowService.ShowWindow<AddTorrentWindow>();
        }
    }
}