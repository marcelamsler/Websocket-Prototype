using System;
using System.Collections.Generic;
using System.Collections;
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

        public SocketTestPage2()
        {
            InitializeComponent();
			connectButton.Clicked += OnConnect;
			sendMessageButton.Clicked += OnSingleMessage;
			testMultipleMessagesButton.Clicked += OnMultipleMessages;
			websocket = new WebsocketService("ws://drallodmmprototype.azurewebsites.net/dmmSocketConnector.ashx");
            websocket.Opened += () => debugLabel.Text = "Connection Open";
            websocket.Closed += () => debugLabel.Text = "Connection closed";
            websocket.Error += () => debugLabel.Text = "Connection error";
            websocket.Received += (parsedMessage) => valueLabel.Text = parsedMessage;
			websocket.RetryFailedEvent += () => debugLabel.Text = "Could not establish connection";
        }
		private void OnConnect(object sender, EventArgs e) 
		{
			
			websocket.ConnectWithWebsocket();

			Debug.WriteLine("on connect");



		}
		private void OnSingleMessage(object sender, EventArgs e) 
		{
			Debug.WriteLine("send 1 message");

		}
        private void OnMultipleMessages(object sender, EventArgs e) 
        {
			List<string> messages = new List<string>();
			for(int i = 0; i < 2; i++)
			{
				messages.Add ("{message: \"HIER "+i+" \"}");
			}


			websocket.sendMultiple(messages);

        }
			

    }
}

