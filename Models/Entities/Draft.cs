using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class Draft : DeviceBase
	{
		[JsonPropertyName("device_conversation_id")]
		public long DeviceConversationId { get; set; }

		[JsonPropertyName("data")]
		public string Data { get; set; }

		[JsonPropertyName("mime_type")]
		public string MimeType { get; set; }

		[JsonIgnore]
		public Conversation ConversationBody { get; set; }

		public Draft()
		{
		}
	}
}
