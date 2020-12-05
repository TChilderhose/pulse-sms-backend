using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddBlacklistRequest : AccountBase
	{
		[JsonPropertyName("blacklists")]
		public Blacklist[] Blacklists { get; set; }

		public AddBlacklistRequest()
		{
		}
	}
}
