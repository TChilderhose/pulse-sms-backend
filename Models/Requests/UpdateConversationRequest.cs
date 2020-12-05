using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class UpdateConversationRequest : ColorBase
	{
		[JsonPropertyName("led_color")]
		public int LedColor { get; set; }

		[JsonPropertyName("pinned")]
		public bool Pinned { get; set; }

		[JsonPropertyName("read")]
		public bool Read { get; set; }

		[JsonPropertyName("timestamp")]
		public long Timestamp { get; set; }

		[JsonPropertyName("title")]
		public string Title { get; set; }

		[JsonPropertyName("snippet")]
		public string Snippet { get; set; }

		[JsonPropertyName("ringtone")]
		public string Ringtone { get; set; }

		[JsonPropertyName("mute")]
		public bool Mute { get; set; }

		[JsonPropertyName("archive")]
		public bool Archive { get; set; }

		[JsonPropertyName("private_notifications")]
		public bool PrivateNotifications { get; set; }

		public UpdateConversationRequest()
		{
		}

		public void CloneTo(UpdateConversationRequest to)
		{
			base.CloneTo(to);
			to.LedColor = this.LedColor;
			to.Pinned = this.Pinned;
			to.Read = this.Read;
			to.Timestamp = this.Timestamp;
			to.Title = this.Title;
			to.Snippet = this.Snippet;
			to.Ringtone = this.Ringtone;
			to.Mute = this.Mute;
			to.Archive = this.Archive;
			to.PrivateNotifications = this.PrivateNotifications;
		}
	}
}
