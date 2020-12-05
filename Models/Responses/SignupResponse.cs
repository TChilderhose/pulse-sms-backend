using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class SignupResponse : AccountBase
	{
		[JsonPropertyName("salt1")]
		public string Salt1 { get; set; }

		[JsonPropertyName("salt2")]
		public string Salt2 { get; set; }

		public SignupResponse()
		{
		}
	}
}
