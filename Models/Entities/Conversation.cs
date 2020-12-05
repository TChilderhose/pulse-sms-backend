using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class Conversation : UpdateConversationRequest
	{
		[JsonPropertyName("phone_numbers")]
		public string PhoneNumbers { get; set; }

		[JsonPropertyName("image_uri")]
		public string ImageUri { get; set; }

		[JsonPropertyName("id_matcher")]
		public string IdMatcher { get; set; }

		[JsonPropertyName("folder_id")]
		public long FolderId { get; set; }
				
		public ICollection<Draft> Drafts { get; set; }
		public ICollection<Message> Messages { get; set; }

		public Conversation()
		{
		}
	}
}
