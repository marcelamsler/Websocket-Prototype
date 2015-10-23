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
		private const int NUMBER_OF_RETRIES = 3;
		private const int INTERVAL_IN_MILLIS = 500;

        public WebsocketService(string webSocketURI) 
		{
			this.webSocketURI = webSocketURI;
            websocketClient = new WebSocketClient();
            websocketClient.Opened += OnWebsocketOpened;
            websocketClient.Closed += OnWebsocketClosed;
            websocketClient.MessageReceived += OnWebsocketReceived;
            websocketClient.Error += OnWebsocketError;
            
        }

        public void ConnectWithWebsocket() 
        {
			TryToConnect(NUMBER_OF_RETRIES, INTERVAL_IN_MILLIS);
        }

        void OnWebsocketOpened()
        {
            Debug.WriteLine("Opened Websocket");
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

		public void sendMultiple (List<string> messages)
		{
			foreach(string message in messages){
				websocketClient.SendAsync (message);
				//await Task.Delay (1000);
			}
			Debug.WriteLine ("sent " + messages.ToArray().Length + " messages");
		}

		int numberOfReceivedMessages = 0;

		void OnWebsocketReceived(IWebSocketMessage frame)
		{
			WebsocketEchoMessage echoMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<WebsocketEchoMessage> (frame.ToString());


			numberOfReceivedMessages++;
//			Received ("Websocket message: "+numberOfReceivedMessages+" received: \n"+echoMessage.message + " \n\tat time: " + echoMessage.timestamp);
			Received ("Websocket message: "+numberOfReceivedMessages+"");
		}

		private async void TryToConnect(int retries, int intervalInMillis)
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
				catch
				{
					Debug.WriteLine("failed");
					if (retries == i) {
						Debug.WriteLine ("could not establish connection");
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

