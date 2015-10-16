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
        WebsocketService websocket;

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

            websocket = new WebsocketService("ws://drallodmmprototype.azurewebsites.net/dmmSocketConnector");
            websocket.Opened += () => debugLabel.Text = "Connection Open";
            websocket.Closed += () => debugLabel.Text = "Connection closed";
            websocket.Error += () => debugLabel.Text = "Connection error";
            websocket.Received += (parsedMessage) => valueLabel.Text = parsedMessage;
        }


//        private async void OnMultipleMessages(object sender, EventArgs e) 
//        {
//            webSocket.FrameReceived += printMissingMessages;
//
//            for (int i = 0; i < MessageCount; i++)
//            {
//                await webSocket.SendAsync("{message: '"+ i + "'}");
//                Debug.WriteLine("send a message");
//                messages.Add(i);
//            }               
//                
//        }
//
//        private void printMissingMessages(IWebSocketFrame frame)
//        {
//
//            if (messages.Contains(int.Parse(parsedMessage[message].ToString()))) 
//            {
//                    MissingMessages++;
//            }
//            valueLabel.Text = MissingMessages.ToString();                   
//        }
//
//        private void printMessage(IWebSocketFrame frame) {
//            valueLabel.Text = frame.ToString();
//        }
//
    }
}

