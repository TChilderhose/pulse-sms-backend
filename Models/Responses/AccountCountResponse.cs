using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AccountCountResponse
	{
		[JsonPropertyName("device_count")]
		public int DeviceCount { get; set; }

		[JsonPropertyName("message_count")]
		public int MessageCount { get; set; }

		[JsonPropertyName("conversation_count")]
		public int ConversationCount { get; set; }

		[JsonPropertyName("draft_count")]
		public int DraftCount { get; set; }

		[JsonPropertyName("scheduled_count")]
		public int ScheduledCount { get; set; }

		[JsonPropertyName("blacklist_count")]
		public int BlacklistCount { get; set; }

		public AccountCountResponse()
		{
		}
	}
}
