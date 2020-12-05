using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddDeviceResponse
	{
		[JsonPropertyName("id")]
		public long DeviceId { get; set; }

		public AddDeviceResponse()
		{
		}
	}
}
