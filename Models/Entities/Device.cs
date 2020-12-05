using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class Device : DeviceBase
	{
		[JsonPropertyName("info")]
		public string Info { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("primary")]
		public bool Primary { get; set; }

		[JsonPropertyName("fcm_token")]
		public string FcmToken { get; set; }

		public Device()
		{
		}
	}
}
