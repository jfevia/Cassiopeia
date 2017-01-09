using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class HttpSeed : ObservableObject
    {
        private string _name;

        public HttpSeed(string name)
        {
            Name = name;
        }

        public string Name
        {
            get { return _name; }
            set { Set(nameof(Name), ref _name, value); }
        }
    }
}