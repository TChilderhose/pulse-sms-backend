using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class UpdateMessageRequest
	{
		[JsonPropertyName("type")]
		public int Type { get; set; }

		[JsonPropertyName("read")]
		public bool Read { get; set; }

		[JsonPropertyName("seen")]
		public bool Seen { get; set; }

		[JsonPropertyName("timestamp")]
		public long Timestamp { get; set; }

		public UpdateMessageRequest()
		{
		}
	}
}
