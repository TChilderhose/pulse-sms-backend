using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class Folder : ColorBase
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

		public Folder()
		{
		}
	}
}
