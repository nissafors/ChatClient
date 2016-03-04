using System;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    class DelegateCommand : ICommand
    {
        // http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html

        private readonly Action action;

        /// <summary>
        /// Construct a new DelegateCommand instance.
        /// </summary>
        /// <param name="action">The action to be performed by this DelegateCommand.</param>
        public DelegateCommand(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// Execute action.
        /// </summary>
        public void Execute(object parameter)
        {
            action();
        }

        /// <summary>
        /// Indicate if the action can be performed.
        /// This implementation always returns true.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Supposed to fire when CanExecute changes. Unused here, so suppress warnings about that.
        /// </summary>
#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }
}
