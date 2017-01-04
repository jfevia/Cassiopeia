using System.Windows.Input;
using Cassiopeia.Services;
using Cassiopeia.Views;
using GalaSoft.MvvmLight.Command;

namespace Cassiopeia.ViewModels
{
    internal class MainViewModel
    {
        public MainViewModel()
        {
            AddTorrentCommand = new RelayCommand(ShowAddTorrentCommandWindow);
        }

        private void ShowAddTorrentCommandWindow()
        {
            WindowService.ShowWindow<AddTorrentWindow>();
        }

        public ICommand AddTorrentCommand { get; }
    }
}