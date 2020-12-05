using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class Blacklist : DeviceBase
	{
		[JsonPropertyName("phone_number")]
		public string PhoneNumber { get; set; }

		[JsonPropertyName("phrase")]
		public string Phrase { get; set; }

		public Blacklist()
		{
		}
	}
}
