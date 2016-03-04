using System.ComponentModel;

namespace ChatClient.ViewModels
{
    class ObservableObject : INotifyPropertyChanged
    {
        // http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html

        /// <summary>
        /// Fires when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fire the PropertyChanged event.
        /// </summary>
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
