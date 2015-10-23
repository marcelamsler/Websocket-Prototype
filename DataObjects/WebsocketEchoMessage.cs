using System;
using Newtonsoft.Json;
namespace WebsocketTest
{
	public class WebsocketEchoMessage
	{
		[JsonProperty(PropertyName = "timestamp")]
		public string timestamp { get; set; }
		[JsonProperty(PropertyName = "message")]
		public string message{ get; set; }
		public WebsocketEchoMessage (string serializedJSON)
		{
		}

	}
}
	