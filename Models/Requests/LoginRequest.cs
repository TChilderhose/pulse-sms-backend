using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class LoginRequest
	{
		[JsonPropertyName("username")]
		public string Username { get; set; }

		[JsonPropertyName("password")]
		public string Password { get; set; }

		public LoginRequest()
		{
		}
	}
}
