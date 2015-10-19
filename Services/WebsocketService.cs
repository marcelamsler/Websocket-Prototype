using System;
using WebSocket.Portable;
using System.Diagnostics;
using WebSocket.Portable.Interfaces;
using Newtonsoft.Json.Linq;

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

        public WebsocketService(string webSocketURI) 
		{
			this.webSocketURI = webSocketURI;
            websocketClient = new WebSocketClient();
            websocketClient.Opened += OnWebsocketOpened;
            websocketClient.Closed += OnWebsocketClosed;
            websocketClient.MessageReceived += OnWebsocketReceived;
            websocketClient.Error += OnWebsocketError;
            ConnectWithWebsocket(this.webSocketURI);
        }

        private async void ConnectWithWebsocket(string webSocketEndpoint) 
        {
            await websocketClient.OpenAsync(webSocketEndpoint);
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

        void OnWebsocketReceived(IWebSocketMessage frame)
        {
            Debug.WriteLine("Websocket Message Received: " + frame.ToString());

            var payload = frame.ToString();
            var indexOfBegin = payload.IndexOf("'");
            var message = payload.Substring(indexOfBegin + 1, payload.Length - 2);

            var parsedMessage = JObject.Parse(message);
            Received(parsedMessage.ToString());
        }

        void OnWebsocketError(Exception e)
        {
            Debug.WriteLine("Websocket Error" + e.Message);
            Error();
        }
    }
}

