using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddConversationRequest : AccountBase
	{
		[JsonPropertyName("conversations")]
		public Conversation[] Conversations { get; set; }

		public AddConversationRequest()
		{
		}
	}
}
