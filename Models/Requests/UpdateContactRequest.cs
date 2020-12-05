using System.Text.Json.Serialization;

namespace Pulse.Models
{
    public class UpdateContactRequest : ColorBase
    {
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public UpdateContactRequest()
		{
		}
    }
}
