using System.Text.Json.Serialization;

namespace Pulse.Models
{
    public class Message : DeviceBase
    {
        [JsonPropertyName("device_conversation_id")]
        public long DeviceConversationId { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }

        [JsonPropertyName("message_type")]
        public int MessageType { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("read")]
        public bool Read { get; set; }

        [JsonPropertyName("seen")]
        public bool Seen { get; set; }

        [JsonPropertyName("message_from")]
        public string MessageFrom { get; set; }

        [JsonPropertyName("color")]
        public int Color { get; set; }

        [JsonPropertyName("sent_device")]
        public long SentDevice { get; set; }

        [JsonPropertyName("sim_stamp")]
        public string SimStamp { get; set; }

        [JsonIgnore]
        public Conversation ConversationBody { get; set; }

        public Message()
		{
		}
    }
}
