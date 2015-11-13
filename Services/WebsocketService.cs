using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using dralloMultiPlayer.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocket.Portable;
using WebSocket.Portable.Interfaces;
using Xamarin.Forms;
using Microsoft.AspNet.SignalR.Client;
using System.Collections.ObjectModel;
namespace WebsocketTest
{
    public class WebsocketService
    {
		private const string webSocketURI = "http://drallodmmprototype.azurewebsites.net/connect";

        public event Action Reconnected;
        public event Action Closed;
        public event Action Error;
        //public event Action<string> Received;
		public event Action<string> RetryFailedEvent;
		private bool keepAliveActive = true;
		private const string challengeId = "45435-2435-245-2345-234";
		private const string deviceId = "123123123";
		private const string userName = "colbinator";
		Connection connection = new Connection (webSocketURI);
		ObservableCollection<string> mycollection = new ObservableCollection<string> ();

		public WebsocketService(){
			connection.Closed += OnWebsocketClosed;
			connection.Reconnected += OnWebsocketReconnected;
			connection.Error += OnWebsocketError;
			connection.Received += OnWebsocketReceived;
			mycollection.CollectionChanged += Mycollection_CollectionChanged;
		}

		void Mycollection_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			foreach (string m in e.NewItems) 
			{
				HandleCollectionChanged (m);
			}
		}


		public async Task ConnectWithWebsocket() 
        {
			try{
				await connection.Start();
			}
			catch (Exception e) {
				
				Debug.WriteLine ("could not connect");
			}
        }

        void OnWebsocketReconnected()
        {
            Debug.WriteLine("Reconnected Websocket");
            //Reconnected();
        }

        async void OnWebsocketClosed()
        {
            Debug.WriteLine("Closed Websocket");
			//Closed();
        }


        void OnWebsocketError(Exception e)
        {
            Debug.WriteLine("Websocket Error" + e.Message);
            //Error();
        }

		public void Send(string msg)
		{
			
			try{
				EchoWithTimestamp echo = new EchoWithTimestamp (msg);
				var msgFrame = new MessageFrame("echo", echo);
				string message = JsonConvert.SerializeObject (msgFrame);

				Debug.WriteLine(message);
				connection.Send(message);
			}
			catch (Exception e){
				Debug.WriteLine ("exception in Send() happened"+ e.Message);
			}

		}
		
		public async Task Register ()
		{
			var registerMsg = new RegisterMessage ();
			registerMsg.challengeId = challengeId;
			registerMsg.deviceId = deviceId;
			var registerMessage = new MessageFrame("register", registerMsg);

			string message = JsonConvert.SerializeObject (registerMessage);

			await connection.Send(message);
			Debug.WriteLine ("sent register message: " +  message);
		}

		public async Task Deregister ()
		{
			var deregisterMsg = new DeregisterMessage ();
			deregisterMsg.challengeId = challengeId;
			deregisterMsg.deviceId = deviceId;
			var deregisterMessage = new MessageFrame ("deregister", deregisterMsg);
			
			string message = JsonConvert.SerializeObject (deregisterMessage);

			await connection.Send(message);

			Debug.WriteLine ("sent deregister message");
		}

		public async Task Join ()
		{
			var joinMsg = new JoinMessage ();
			joinMsg.challengeId = challengeId;
			joinMsg.userName = userName;
			var joinMessage = new MessageFrame ("join", joinMsg);

			string message = JsonConvert.SerializeObject (joinMessage);
			await connection.Send(message);

			Debug.WriteLine ("sent join message");
		}

		 void OnWebsocketReceived(string message)
		{
			mycollection.Add (message);

		}

		void HandleCollectionChanged(string message){
			Debug.WriteLine ("recevid message:"+ message);

			var messageFrame = JsonConvert.DeserializeObject<MessageFrame>(message);
			object typedMessage;

			switch (messageFrame.messageType)
			{
			case "invite":
				typedMessage = JsonConvert.DeserializeObject<JoinMessage>(messageFrame.data.ToString());
				break;
			case "echo":
				typedMessage = JsonConvert.DeserializeObject<EchoWithTimestamp>(messageFrame.data.ToString());
				break;
			default:
				break;
			}

			//Received ("Websocket message: "+000+" received: \n"+message + "");
		}
    }

}

