using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddScheduledMessageRequest : AccountBase
	{
		[JsonPropertyName("scheduled_messages")]
		public ScheduledMessage[] ScheduledMessages { get; set; }

		public AddScheduledMessageRequest()
		{
		}
	}
}