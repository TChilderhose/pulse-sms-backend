using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AutoReply : DeviceBase
	{
		[JsonPropertyName("replyType")]
		public string ReplyType { get; set; }

		[JsonPropertyName("pattern")]
		public string Pattern { get; set; }

		[JsonPropertyName("response")]
		public string Response { get; set; }

		public AutoReply()
		{
		}
	}
}
