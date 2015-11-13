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
			try{
	            InitializeComponent();
				connectButton.Clicked += OnConnect;
				sendMessageButton.Clicked += SendMessage;
				testMultipleMessagesButton.Clicked += OnMultipleMessages;
				sendRegister.Clicked += OnRegister;
				sendDeregister.Clicked += OnDeregister;
				sendJoin.Clicked += OnJoin;
				websocket = new WebsocketService();
				websocket.Closed += (closedMessage) => debugLabel.Text = closedMessage;
				websocket.Received += (parsedMessage) => debugLabel.Text = parsedMessage;
			}
			catch (Exception e){
				Debug.WriteLine (e.Message);
			}
        }

		private async void OnConnect(object sender, EventArgs e) 
		{
			
			await websocket.ConnectWithWebsocket();

			Debug.WriteLine("on connect");

		}

		private void SendMessage(object sender, EventArgs e) 
		{
			try
			{
				string x = "";
				for(var i = 0; i<(1024); i++) { 
					x += "M";
				}

				websocket.Send(x);
				Debug.WriteLine("sent 1 message");
				
			}
			catch(Exception ex)
			{
				Debug.WriteLine (ex.Message);

			}

		}


        private async void OnMultipleMessages(object sender, EventArgs e) 
        {
			try
			{
				int amountOfMessagesToSend = 1000;
				for(int i = 1; i <= amountOfMessagesToSend; i++)
				{
					await Task.Delay(TimeSpan.FromMilliseconds(100));
					websocket.Send("{message: \"MULTIPLE: NO: "+i+" \"}");
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine (ex.Message);
			}

    }
			
		private async void OnRegister(object sender, EventArgs e)
		{
			try
			{
				await websocket.Register();
			}
			catch(Exception ex)
			{
				Debug.WriteLine (ex.Message);
				
			}
		}

		private async void OnDeregister (object sender, EventArgs e)
		{
			await websocket.Deregister();
		}

		private async void OnJoin (object sender, EventArgs e)
		{
			await websocket.Join();
		}
    }
}

