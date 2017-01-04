using System.Windows;

namespace Cassiopeia.Services
{
    internal static class WindowService
    {
        public static bool? ShowWindow<T>() where T : Window, new()
        {
            var window = new T();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
            return window.DialogResult;
        }
    }
}