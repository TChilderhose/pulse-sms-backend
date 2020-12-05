using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddDraftRequest : AccountBase
	{
		[JsonPropertyName("drafts")]
		public Draft[] Drafts { get; set; }

		public AddDraftRequest()
		{
		}
	}
}
