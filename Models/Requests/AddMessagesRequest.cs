using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddMessagesRequest : AccountBase
	{
		[JsonPropertyName("messages")]
		public Message[] Messages { get; set; }

		public AddMessagesRequest()
		{
		}
	}
}