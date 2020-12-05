using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddTemplateRequest : AccountBase
	{
		[JsonPropertyName("templates")]
		public Template[] Templates { get; set; }

		public AddTemplateRequest()
		{
		}
	}
}