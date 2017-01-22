using System;
using GalaSoft.MvvmLight;

namespace Cassiopeia.Models
{
    internal class LoggableException : ObservableObject
    {
        private DateTime _dateTime;
        private Exception _exception;

        public DateTime DateTime
        {
            get { return _dateTime; }
            set { Set(nameof(DateTime), ref _dateTime, value); }
        }

        public Exception Exception
        {
            get { return _exception; }
            set { Set(nameof(Exception), ref _exception, value); }
        }
    }
}