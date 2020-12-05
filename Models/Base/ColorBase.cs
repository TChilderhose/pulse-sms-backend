using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class ColorBase : DeviceBase
	{
		[JsonPropertyName("color")]
		public int Color { get; set; }

		[JsonPropertyName("color_dark")]
		public int ColorDark { get; set; }

		[JsonPropertyName("color_light")]
		public int ColorLight { get; set; }

		[JsonPropertyName("color_accent")]
		public int ColorAccent { get; set; }

		public void CloneTo(ColorBase to)
		{
			to.Color = this.Color;
			to.ColorDark = this.ColorDark;
			to.ColorLight = this.ColorLight;
			to.ColorAccent = this.ColorAccent;
		}
	}
}
