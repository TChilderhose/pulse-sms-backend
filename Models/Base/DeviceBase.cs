using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class DeviceBase : AccountBase
	{
		[Key, JsonPropertyName("device_id")]
		public long DeviceId { get; set; }

		[JsonIgnore]
		public Account Account { get; set; }
	}
}
