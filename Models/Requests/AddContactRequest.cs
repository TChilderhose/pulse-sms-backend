using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddContactRequest : AccountBase
	{
		[JsonPropertyName("contacts")]
		public Contact[] Contacts { get; set; }

		public AddContactRequest()
		{
		}
	}
}
