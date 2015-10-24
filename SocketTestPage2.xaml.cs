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
		private async void OnConnect(object sender, EventArgs e) 
		{
			
			await websocket.ConnectWithWebsocket(3, 500);

			Debug.WriteLine("on connect");

		}
		private async void OnSingleMessage(object sender, EventArgs e) 
		{
			try
			{
				List<string> messages = new List<string>();
				messages.Add ("{message: \"SINGLE MESSAGE\"}");
				await websocket.SendMultiple(messages);
				Debug.WriteLine("sent 1 message");
			}
			catch(Exception ex)
			{
				Debug.WriteLine (ex.Message);

			}

		}
        private async void OnMultipleMessages(object sender, EventArgs e) 
        {
			int amountOfMessagesSent = 0;
			try
			{
				int amountOfMessagesToSend = 100;
				List<string> messages = new List<string>();
				for(int i = 1; i <= amountOfMessagesToSend; i++)
				{
					messages.Add ("{message: \"MULTIPLE: NO: "+i+" \"}");
				}
				amountOfMessagesSent = await websocket.SendMultiple(messages);
			}
			catch(Exception ex)
			{
				Debug.WriteLine (ex.Message);

			}
			Debug.WriteLine (amountOfMessagesSent);

    }
			

    }
}

