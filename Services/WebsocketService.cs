using System;
using WebSocket.Portable;
using System.Diagnostics;
using WebSocket.Portable.Interfaces;

namespace WebsocketTest
{
    public class WebsocketService
    {
        private WebSocketClient websocketClient;

        public WebsocketService()
        {
            websocketClient = new WebSocketClient();
            websocketClient.Opened += OnWebsocketOpened;
            websocketClient.Closed += OnWebsocketClosed;
            websocketClient.MessageReceived += OnWebsocketReceived;
            websocketClient.Error += OnWebsocketError;
        }

        private async void ConnectWithWebsocket(string webSocketEndpoint) 
        {
            await websocketClient.OpenAsync(webSocketEndpoint);
        }

        void OnWebsocketOpened()
        {
            Debug.WriteLine("Opened Websocket");
        }

        void OnWebsocketClosed()
        {
            Debug.WriteLine("Closed Websocket");
        }

        void OnWebsocketReceived(IWebSocketMessage obj)
        {
            Debug.WriteLine("Websocket Message Received: " + obj.ToString());
        }

        void OnWebsocketError(Exception e)
        {
            Debug.WriteLine("Websocket Error" + e.Message);
        }
    }
}

