using System;
using WebSocket.Portable;
using System.Diagnostics;
using WebSocket.Portable.Interfaces;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebsocketTest
{
    public class WebsocketService
    {
        private WebSocketClient websocketClient;
        private string webSocketURI;

        public event Action Opened;
        public event Action Closed;
        public event Action Error;
        public event Action<string> Received;
		public event Action RetryFailedEvent;

        public WebsocketService(string webSocketURI) 
		{
			this.webSocketURI = webSocketURI;
            websocketClient = new WebSocketClient();
            websocketClient.Opened += OnWebsocketOpened;
            websocketClient.Closed += OnWebsocketClosed;
            websocketClient.MessageReceived += OnWebsocketReceived;
            websocketClient.Error += OnWebsocketError;
            
        }


		public async Task ConnectWithWebsocket(int numberOfRetries, int intervalInMillis) 
        {
			await TryToConnect(numberOfRetries, intervalInMillis);
        }

        void OnWebsocketOpened()
        {
            Debug.WriteLine("Opened Websocket");
			//websocketClient.AutoSendPongResponse = true;
            Opened();
        }

        void OnWebsocketClosed()
        {
            Debug.WriteLine("Closed Websocket");

            Closed();
        }


        void OnWebsocketError(Exception e)
        {
            Debug.WriteLine("Websocket Error" + e.Message);
            Error();
        }

		public async Task<int> SendMultiple (List<string> messages)
		{
			foreach(string message in messages){
				await websocketClient.SendAsync (message);
				Debug.WriteLine ("message sent");
			}
			Debug.WriteLine ("sent " + messages.ToArray().Length + " messages");
			return messages.ToArray ().Length;
		}


		int numberOfReceivedMessages = 0;
		void OnWebsocketReceived(IWebSocketMessage frame)
		{
			WebsocketEchoMessage echoMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<WebsocketEchoMessage> (frame.ToString());
			Debug.WriteLine (echoMessage.message + " received ");

			numberOfReceivedMessages++;
			Received ("Websocket message: "+numberOfReceivedMessages+" received: \n"+echoMessage.message + " \n\tat time: " + echoMessage.timestamp);
			Debug.WriteLine (echoMessage.message + " sent to debug  label ");
		}

		private async Task TryToConnect(int retries, int intervalInMillis)
		{
			int i = 0;
			while(i <= retries){
				try
				{
					Debug.WriteLine("retry: " + i + ":"); 
					await websocketClient.OpenAsync(webSocketURI);
					Debug.WriteLine("successful");
					return;
				}
				catch (Exception ex)
				{
					Debug.WriteLine("failed");


					if (retries == i) {
						Debug.WriteLine ("could not establish connection: "+ex.Message);
						RetryFailedEvent ();
						return;
					}
					intervalInMillis *= 2;
				}
				i++;
				await Task.Delay(intervalInMillis);
			}
		}
    }
}

