using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class UpdateTemplateRequest
	{
		[JsonPropertyName("text")]
		public string Text { get; set; }

		public UpdateTemplateRequest()
		{
		}
	}
}
