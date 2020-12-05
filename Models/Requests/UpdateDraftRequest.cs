using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class UpdateDraftRequest
	{
		[JsonPropertyName("data")]
		public string Data { get; set; }

		[JsonPropertyName("mime_type")]
		public string MimeType { get; set; }

		public UpdateDraftRequest()
		{
		}
	}
}
