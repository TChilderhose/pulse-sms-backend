using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class SignupRequest
	{
		[JsonPropertyName("name")]
		public string Username { get; set; }

		[JsonPropertyName("real_name")]
		public string RealName { get; set; }

		[JsonPropertyName("password")]
		public string Password { get; set; }

		[JsonPropertyName("phone_number")]
		public string PhoneNumber { get; set; }

		public SignupRequest()
		{
		}
	}
}
