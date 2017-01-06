using System.Windows;
using Cassiopeia.Models;

namespace Cassiopeia.Views
{
    public partial class AddTorrentsWindow : Window, IDialog
    {
        public AddTorrentsWindow()
        {
            InitializeComponent();
        }

        public void CloseDialog()
        {
            Close();
        }
    }
}