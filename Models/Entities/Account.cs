using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pulse.Models
{
	public class Account : LoginResponse
	{
		[JsonPropertyName("password_hash")]
		public string PasswordHash { get; set; }

		[JsonPropertyName("real_name")]
		public string RealName { get; set; }

		public ICollection<AutoReply> AutoReplies { get; set; }
		public ICollection<Blacklist> Blacklists { get; set; }
		public ICollection<Contact> Contacts { get; set; }
		public ICollection<Conversation> Conversations { get; set; }
		public ICollection<Device> Devices { get; set; }
		public ICollection<Draft> Drafts { get; set; }
		public ICollection<Folder> Folders { get; set; }
		public ICollection<Message> Messages { get; set; }
		public ICollection<ScheduledMessage> ScheduledMessages { get; set; }
		public ICollection<Template> Templates { get; set; }
	}
}
