using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class UpdateFolderRequest : ColorBase
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

		public UpdateFolderRequest()
		{
		}
	}
}
