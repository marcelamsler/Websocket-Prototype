using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;


namespace WebsocketTest
{
    public class WebsocketService
    {
		private const string webSocketURI = "http://drallodmmprototype.azurewebsites.net/connect";

		public event Action<string> Closed;
        public event Action<string> Received;

		private const string challengeId = "45435-2435-245-2345-234";
		private const string deviceId = "123123123";
		private const string userName = "colbinator";
		Connection connection = new Connection (webSocketURI);
		ObservableCollection<string> mycollection = new ObservableCollection<string> ();
		JsonSerializerSettings jsonSerializerSettings; 

		public WebsocketService(){
			connection.Closed += OnWebsocketClosed;
			connection.Reconnected += OnWebsocketReconnected;
			connection.Error += OnWebsocketError;
			connection.Received += OnWebsocketReceived;
			mycollection.CollectionChanged += Mycollection_CollectionChanged;

			jsonSerializerSettings = new JsonSerializerSettings() { 
				TypeNameHandling = TypeNameHandling.All
			};

		}

		void Mycollection_CollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
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

        void OnWebsocketClosed()
        {
            Debug.WriteLine("Closed Websocket");


			Closed ("oops, connection closed");

        }


        private async void OnWebsocketError(Exception e)
        {
			try {
				Debug.WriteLine ("Websocket Error: " + e.Message);
            	
				connection.Stop ();
				await connection.Start ();
			} catch (Exception ex) {
				Debug.WriteLine ("Websocket Error handling failed: " + ex.Message);
			}

        }

		public async void Send(string msg)
		{
			
			try{
				EchoWithTimestamp echo = new EchoWithTimestamp (msg);

				string message = JsonConvert.SerializeObject (echo, jsonSerializerSettings);

				Debug.WriteLine("--------> send: "+ message);
				await connection.Send(message);
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

			string message = JsonConvert.SerializeObject (registerMsg, jsonSerializerSettings);

			await connection.Send(message);
			Debug.WriteLine ("sent register message: " +  message);
		}

		public async Task Deregister ()
		{
			var deregisterMsg = new DeregisterMessage ();
			deregisterMsg.challengeId = challengeId;
			deregisterMsg.deviceId = deviceId;
			
			string message = JsonConvert.SerializeObject (deregisterMsg, jsonSerializerSettings);

			await connection.Send(message);

			Debug.WriteLine ("sent deregister message");
		}

		public async Task Join ()
		{
			var joinMsg = new JoinMessage ();
			joinMsg.challengeId = challengeId;
			joinMsg.userName = userName;

			string message = JsonConvert.SerializeObject (joinMsg, jsonSerializerSettings);
			await connection.Send(message);

			Debug.WriteLine ("sent join message");
		}

		 void OnWebsocketReceived(string message)
		{
			try
			{
				mycollection.Add (message);
			}
			catch(Exception e)
			{
				Debug.WriteLine ("Received Exception:  " + e.Message);
			}
		}
		List<string> errors = new List<string>();
		void HandleCollectionChanged(string message){ 
			Debug.WriteLine ("<--------- received message:   "+ message);

			var receivedObject = JsonConvert.DeserializeObject (message,  jsonSerializerSettings);

			if (receivedObject is EchoWithTimestamp) 
			{
				Debug.WriteLine ("RECEIVED ECHO");
			} 
			else if (receivedObject is InviteMessage) 
			{
				Debug.WriteLine ("RECEIVED INVITE");
			} 
			else 
			{
				Debug.WriteLine ("RECEIVED UNKNOWN TYPE");
			}
			Debug.WriteLine(receivedObject.ToString());
			Received ("for you, cellphone");
		}
    }

}

