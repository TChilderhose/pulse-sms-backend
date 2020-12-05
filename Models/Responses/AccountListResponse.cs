using System.Linq;
using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AccountListResponse
	{
		[JsonPropertyName("devices")]
		public Device[] Devices { get; set; }

		[JsonPropertyName("messages")]
		public Message[] Messages { get; set; }

		[JsonPropertyName("conversations")]
		public Conversation[] Conversations { get; set; }

		[JsonPropertyName("drafts")]
		public Draft[] Drafts { get; set; }

		[JsonPropertyName("scheduled_messages")]
		public ScheduledMessage[] ScheduledMessages { get; set; }

		[JsonPropertyName("blacklists")]
		public Blacklist[] Blacklists { get; set; }

		public AccountListResponse()
		{
		}
	}
}
