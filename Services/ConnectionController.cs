using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using dralloMultiPlayer.Messages;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using WebSocket.Portable;
using WebSocket.Portable.Interfaces;
using Xamarin.Forms;


namespace WebsocketTest
{
	public class ConnectionController
	{
		public event Action<string> Closed;
		public event Action<string> InvitationReceived;
		public event Action<string> JoinAcceptedReceived;
		public event Action<string> JoinRejectedReceived;
		private const string connectionUri = "http://drallomultiplayermanager.azurewebsites.net/connect";
		private const string userName = "colbinator";
		private ConnectionService connectionService = new ConnectionService (connectionUri);
		private ObservableCollection<string> receivedMessagesCollection = new ObservableCollection<string> ();
		private JsonSerializerSettings jsonSerializerSettings;

		public ConnectionController ()
		{
			connectionService.Closed += OnConnectionClosed;
			connectionService.Error += OnConnectionError;
			connectionService.Received += OnMessageReceived;
			connectionService.Reconnecting += OnConnectionReconnecting;
			connectionService.Reconnected += OnConnectionReconnected;

			jsonSerializerSettings = new JsonSerializerSettings () { 
				TypeNameHandling = TypeNameHandling.All
			};

		}

		private void OnConnectionReconnecting ()
		{
			Debug.WriteLine ("Reconnecting...");
		}

		private void OnConnectionReconnected ()
		{
			Debug.WriteLine ("Reconnected! ");
		}

		public async Task Connect ()
		{
			try {
				await connectionService.Connect ();
			} catch (Exception e) {
				
				Debug.WriteLine ("could not connect: " + e.Message);
			}
		}

		void OnConnectionClosed ()
		{
			Debug.WriteLine ("Closed Websocket");
			Closed ("connection closed");
		}

		private void OnConnectionError (Exception e)
		{
			Debug.WriteLine ("Websocket Error: " + e.Message);
		}

		public async Task Send (ActivityRecord input)
		{
			string message = JsonConvert.SerializeObject (input, jsonSerializerSettings);
			await Send (message);
		}

		public async Task Send (ActivityEvent input)
		{
			string message = JsonConvert.SerializeObject (input, jsonSerializerSettings);
			await Send (message);
		}

		private async Task Send (string msg)
		{
			try {
				Debug.WriteLine ("--------> send: " + msg);
				await connectionService.Send (msg);
			} catch (Exception e) {
				Debug.WriteLine ("exception in Send() happened:  " + e.Message);
			}
		}

		public async void Register (string deviceId, Guid challengeId)
		{
			var registerMsg = new RegisterMessage (deviceId, challengeId);
			string message = JsonConvert.SerializeObject (registerMsg, jsonSerializerSettings);
			await connectionService.Send (message);
			Debug.WriteLine ("sent register message: " + message);
		}

		public async void Deregister (string deviceId, Guid multiplayerChallengeId)
		{
			var deregisterMsg = new DeregisterMessage (deviceId, multiplayerChallengeId);
			string message = JsonConvert.SerializeObject (deregisterMsg, jsonSerializerSettings);
			await connectionService.Send (message);
			Debug.WriteLine ("sent deregister message");
		}

		public async void Join (string userName, Guid challengeId)
		{
			var joinMsg = new JoinMessage (userName, challengeId);
			string message = JsonConvert.SerializeObject (joinMsg, jsonSerializerSettings);
			await connectionService.Send (message);
			Debug.WriteLine ("sent join message");
		}


		private void OnMessageReceived (string message)
		{ 
			
			object receivedObject;
			Debug.WriteLine ("<--------- received message:   " + message);
			try {
				receivedObject = JsonConvert.DeserializeObject (message, jsonSerializerSettings);
			} catch (JsonSerializationException serializationException) {
				Debug.WriteLine ("Exception caught while trying to deserialize message: " + serializationException.Message); 
				return;
			}
			try {
				var switchDictionary = new Dictionary<Type, Action> { 
					{ 
						typeof(InviteMessage), () => 
						{
							InvitationReceived ("invitation received: " + receivedObject.ToString ());
							Debug.WriteLine ("RECEIVED INVITE");
						} 
					}, 
					{ 
						typeof(JoinAcceptMessage), () => 
						{
							JoinAcceptedReceived ("join accepted: " + receivedObject.ToString ());
							Debug.WriteLine ("RECEIVED JOIN ACCEPT");
						} 
					}, 
					{ 
						typeof(JoinRejectMessage), () => 
						{
							JoinRejectedReceived ("join rejected: " + receivedObject.ToString ());
							Debug.WriteLine ("RECEIVED JOIN REJECT");
						} 
					},
				};
				if (switchDictionary.ContainsKey (receivedObject.GetType())) {
					switchDictionary [receivedObject.GetType()] ();
				}
				else{
					Debug.WriteLine("UNKNOWN TYPE RECEIVED");
				}
			} catch (Exception ex) {
				Debug.WriteLine (ex.Message);
			}
		}
	}
}
