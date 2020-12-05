using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class UpdateAutoReplyRequest
	{
		[JsonPropertyName("type")]
		public string Type { get; set; }

		[JsonPropertyName("pattern")]
		public string Pattern { get; set; }

		[JsonPropertyName("response")]
		public string Response { get; set; }

		public UpdateAutoReplyRequest()
		{
		}
	}
}
