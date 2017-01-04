using System;
using GalaSoft.MvvmLight.Command;

namespace Cassiopeia.Models
{
    internal class InputGestureCommand : RelayCommand
    {
        public InputGestureCommand(Action execute, Func<bool> canExecute, string inputGestureText)
            : base(execute, canExecute)
        {
            InputGestureText = inputGestureText;
        }

        public InputGestureCommand(Action execute, string inputGestureText) : base(execute)
        {
            InputGestureText = inputGestureText;
        }

        public string InputGestureText { get; }
    }
}