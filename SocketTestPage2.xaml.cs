using System;
using System.Collections.Generic;
using System.Collections;
using Xamarin.Forms;
using WebSocket.Portable;
using System.Threading.Tasks;
using WebSocket.Portable.Interfaces;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using dralloMultiPlayer.Messages;
using Drallo.ChallengeEngine.Activity.Event;
using Drallo.ChallengeEngine.Activity.Record;

namespace WebsocketTest
{
    public partial class SocketTestPage2 : ContentPage
    {
        ConnectionController connection;

        public SocketTestPage2()
        {
			try{
	            InitializeComponent();
				connectButton.Clicked += OnConnect;
				sendEventMessageButton.Clicked += SendActivityEvent;
				sendRecordMessageButton.Clicked += SendActivityRecord;
				testMultipleMessagesButton.Clicked += OnMultipleMessages;
				sendRegister.Clicked += OnRegister;
				sendDeregister.Clicked += OnDeregister;
				sendJoin.Clicked += OnJoin;

				connection = new ConnectionController("http://drallomultiplayermanager.azurewebsites.net/connect", "hansNoetig");
				connection.Closed += (closedMessage) => {};
				connection.InvitationReceived += (inviteMessage) => {};
				connection.JoinAcceptedReceived += (joinAcceptedMessage) => {};
				connection.Reconnecting += () => {};
				connection.Reconnected += () => {};
			}
			catch (Exception e){
				Debug.WriteLine (e.Message);
			}
        }

		private async void OnConnect(object sender, EventArgs e) 
		{
			await connection.Connect();

			Debug.WriteLine("on connect");
		}

		private void SendActivityEvent(object sender, EventArgs e) 
		{
			try
			{
				connection.Send(new ActivityEvent());
				Debug.WriteLine("sent 1 Activity Event");
			}
			catch(Exception ex) {
				Debug.WriteLine (ex.Message);
			}
		}

		private void SendActivityRecord(object sender, EventArgs e) 
		{
			try
			{
				connection.Send(new ActivityRecord());
				Debug.WriteLine("sent 1 Activity Record");
			}
			catch(Exception ex) {
				Debug.WriteLine (ex.Message);
			}

		}

        private async void OnMultipleMessages(object sender, EventArgs e) 
        {
			try
			{
				int amountOfMessagesToSend = 100;
				for(int i = 1; i <= amountOfMessagesToSend; i++)
				{
					await Task.Delay(TimeSpan.FromMilliseconds(100));
					connection.Send(new ActivityEvent());

				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine (ex.Message);
			}
	    }
				
		private void OnRegister(object sender, EventArgs e)
		{
			try
			{
				connection.Register("mydeviceid12341234", Guid.NewGuid());
			}
			catch(Exception ex)
			{
				Debug.WriteLine (ex.Message);
				
			}
		}

		private void OnDeregister (object sender, EventArgs e)
		{
			connection.Deregister ("mydeviceid12341234", Guid.NewGuid ());
		}

		private void OnJoin (object sender, EventArgs e)
		{
			connection.Join (Guid.NewGuid());
		}
    }
}

