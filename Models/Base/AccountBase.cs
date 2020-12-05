using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AccountBase
	{
		[JsonPropertyName("account_id")]
		public string AccountId { get; set; }
	}
}
