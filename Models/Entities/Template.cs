using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class Template : DeviceBase
	{
		[JsonPropertyName("text")]
		public string Text { get; set; }

		public Template()
		{
		}
	}
}
