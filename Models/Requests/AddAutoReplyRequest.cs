using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class AddAutoReplyRequest : AccountBase
	{
		[JsonPropertyName("auto_replies")]
		public AutoReply[] AutoReplies { get; set; }

		public AddAutoReplyRequest()
		{
		}
	}
}
