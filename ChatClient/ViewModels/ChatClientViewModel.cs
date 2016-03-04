using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ChatClient.Models;
using System.Windows.Input;
using System.Net;

namespace ChatClient.ViewModels
{
    /// <summary>
    /// Helper class for data binding in listview items.
    /// </summary>
    class MessageString
    {
        public string Message { get; set; }

        public MessageString(string message)
        {
            Message = message;
        }
    }

    /// <summary>
    /// ViewModel for ChatView.xaml.
    /// </summary>
    class ChatClientViewModel : ObservableObject
    {
        private TCPClient tcpClient = new TCPClient();
        private ObservableCollection<MessageString> history = new ObservableCollection<MessageString>();
        private string message;

        /// <summary>
        /// Get message history.
        /// </summary>
        public IEnumerable<MessageString> History
        {
            get { return history; }
        }

        /// <summary>
        /// Get and set a message to the server.
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
        /// Set server IP address and port number.
        /// </summary>
        public string IP { private get; set; }

        /// <summary>
        /// Construct a new ChatClientViewModel and add an event handler for messages from tcpClient.
        /// </summary>
        public ChatClientViewModel()
        {
            tcpClient.PropertyChanged += tcpClient_PropertyChanged;
            IP = "127.0.0.1:60003";
        }

        /// <summary>
        /// Get connect to server method.
        /// </summary>
        public ICommand ConnectCommand
        {
            get { return new DelegateCommand(connect); }
        }

        /// <summary>
        /// Get send message to server method.
        /// </summary>
        public ICommand SendCommand
        {
            get { return new DelegateCommand(send); }
        }

        /// <summary>
        /// Get disconnect method.
        /// </summary>
        public ICommand DisconnectCommand
        {
            get { return new DelegateCommand(tcpClient.Disconnect); }
        }

        /// <summary>
        /// Parse ip:port string and call connect on the tcpClient.
        /// </summary>
        private void connect()
        {
            var ipAndPort = IP.Split(':');
            if (ipAndPort.Length != 2)
            {
                history.Insert(0, new MessageString("Error! Address format must be a.b.c.d:port."));
                return;
            }

            // Convert ip and port
            IPAddress ip;
            if (!IPAddress.TryParse(ipAndPort[0], out ip))
            {
                history.Insert(0, new MessageString("Error! IP address must have format a.b.c.d."));
                return;
            }

            int port;
            if (!Int32.TryParse(ipAndPort[1], out port) || port > 65535)
            {
                history.Insert(0, new MessageString("Error! Port bust be an integer between 0 and 65535."));
                return;
            }

            tcpClient.IP = ip;
            tcpClient.Port = port;
            tcpClient.Connect();
        }

        /// <summary>
        /// Call send on tcpClient.
        /// </summary>
        private void send()
        {
            tcpClient.Send(Message);
            Message = "";
        }

        /// <summary>
        /// Event handler for messages from the tcpClient.
        /// </summary>
        private void tcpClient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            history.Insert(0, new MessageString(tcpClient.Message));
        }
    }
}
