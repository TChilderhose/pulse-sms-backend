using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class Contact : ColorBase
	{
		[JsonPropertyName("phone_number")]
		public string PhoneNumber { get; set; }

		[JsonPropertyName("id_matcher")]
		public string IdMatcher { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("contact_type")]
		public int ContactType { get; set; }

		public Contact()
		{
		}
	}
}
