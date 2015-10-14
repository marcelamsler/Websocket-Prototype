using System;
using System.Collections.Generic;

using Xamarin.Forms;
using WebSocket.Portable;
using System.Threading.Tasks;
using WebSocket.Portable.Interfaces;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace WebsocketTest
{
    public partial class SocketTestPage2 : ContentPage
    {
        WebSocketClient client;

        int MessageCount { get; set; }

        int MissingMessages
        {
            get;
            set;
        }

        List<int> messages = new List<int>();

        public SocketTestPage2()
        {
            InitializeComponent();

            client = new WebSocketClient();

            MessageCount = 100;
        }


        private async void OnConnect(object sender, EventArgs e) 
        {
            await client.OpenAsync("ws://drallodmmprototype.azurewebsites.net/dmmSocketConnector.ashx");

            Debug.WriteLine("connected");
        }

        private async void OnSingleMessage(object sender, EventArgs e) 
        {
            client.FrameReceived += printMessage;

            await client.SendAsync("{message: 'Bob'}");
            Debug.WriteLine("send one message");
        }

        private async void OnMultipleMessages(object sender, EventArgs e) 
        {
            client.FrameReceived += printMissingMessages;

            for (int i = 0; i < MessageCount; i++)
            {
                await client.SendAsync("{message: '"+ i + "'}");
                Debug.WriteLine("send a message");
                messages.Add(i);
            }


                
        }

        private void printMissingMessages(IWebSocketFrame frame)
        {
            var payload = frame.ToString();
            var indexOfBegin = payload.IndexOf("'");
            var message = payload.Substring(indexOfBegin + 1, payload.Length - 2 );

            var parsedMessage = JObject.Parse(message);
            if (messages.Contains(int.Parse(parsedMessage[message].ToString()))) 
            {
                    MissingMessages++;
            }
            valueLabel.Text = MissingMessages.ToString();                   
        }

        private void printMessage(IWebSocketFrame frame) {
            valueLabel.Text = frame.ToString();
        }

    }
}

