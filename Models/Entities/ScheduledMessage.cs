using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class ScheduledMessage : DeviceBase
	{
		[JsonPropertyName("to")]
		public string To { get; set; }

		[JsonPropertyName("data")]
		public string Data { get; set; }

		[JsonPropertyName("mime_type")]
		public string MimeType { get; set; }

		[JsonPropertyName("timestamp")]
		public long Timestamp { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("repeat")]
		public int Repeat { get; set; }

		public ScheduledMessage()
		{
		}
	}
}
