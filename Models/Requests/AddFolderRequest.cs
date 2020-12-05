using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddFolderRequest : AccountBase
	{
		[JsonPropertyName("folders")]
		public Folder[] Folders { get; set; }

		public AddFolderRequest()
		{
		}
	}
}