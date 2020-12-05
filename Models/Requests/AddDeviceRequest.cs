using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddDeviceRequest : AccountBase
	{
		[JsonPropertyName("device")]
		public Device Device { get; set; }

		public AddDeviceRequest()
		{
		}
	}
}