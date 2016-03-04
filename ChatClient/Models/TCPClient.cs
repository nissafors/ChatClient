using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;

namespace ChatClient.Models
{
    class TCPClient : INotifyPropertyChanged
    {
        private Socket socket;
        private IPEndPoint ipe;
        private Thread listen;
        private string message;

        /// <summary>
        /// Fires when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Observable message from this or server.
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChangedEvent("Message");
            }
        }

        /// <summary>
        /// Get and set the server port number.
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// Get or set the server IP address.
        /// </summary>
        public IPAddress IP { get; set; }

        /// <summary>
        /// Establish socket connection to the server.
        /// </summary>
        public void Connect()
        {
            if (socket != null && socket.Connected)
            {
                Message = "Disconnect before connecting to a new server!";
                return;
            }

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipe = new IPEndPoint(IP, Port);
            try
            {
                socket.Connect(ipe);
                listen = new Thread(new ThreadStart(messageListener));
                listen.Start();
                Message = "Connected!";
            }
            catch (Exception ex)
            {
                Message = "Error! Failed to connect to server.";
            }
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        public void Disconnect()
        {
            if (socket != null && socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Message = "Connection closed.";
            }
        }

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="msg">The message as a string.</param>
        public void Send(string msg)
        {
            if (msg == "")
                return;
            if (socket != null && socket.Connected)
                socket.Send(Encoding.UTF8.GetBytes(msg));
            else
                Message = "Can't send. No connection!";
        }

        /// <summary>
        /// Raise the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// Runs in its own thread, listening for incoming messages and
        /// outputting those to the session list.
        /// </summary>
        private void messageListener()
        {
            var bytes = new byte[1024];

            while (socket != null && socket.Connected)
            {
                try
                {
                    int recCount = socket.Receive(bytes);
                    string rec = Encoding.UTF8.GetString(bytes, 0, recCount);
                    Application.Current.Dispatcher.Invoke(new Action(() => { Message = rec; }));
                }
                catch (Exception e) { }
            }
        }
    }
}
