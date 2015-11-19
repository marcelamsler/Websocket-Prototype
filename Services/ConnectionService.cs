﻿using System;
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
	public class ConnectionService
	{
		public event Action<string> Received;
		public event Action<Exception> Error;
		public event Action Reconnecting;
		public event Action Reconnected;
		public event Action Closed;
		Connection connection;
		public ConnectionService (string connectionUri)
		{
			connection = new Connection (connectionUri);
			connection.Closed += Connection_Closed;
			connection.Error += Connection_Error;
			connection.Received += Connection_Received;
			connection.Reconnecting += Connection_Reconnecting;
			connection.Reconnected += Connection_Reconnected;
		}

		public async Task Send (string message)
		{
			await connection.Send (message);
		}
		
		public async Task Connect ()
		{
			await connection.Start();
		}

		private void Connection_Received (string obj)
		{
			Received (obj);
		}

		void Connection_Reconnected ()
		{
			Reconnected ();
		}
		
		public void Connection_Reconnecting ()
		{
			Reconnecting ();
		}

		public void Connection_Error (Exception obj)
		{
			Error (obj);
		}

		public void Connection_Closed ()
		{
			Closed ();
		}

	}
}

