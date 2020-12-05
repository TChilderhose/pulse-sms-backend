using FirebaseNet.Messaging;
using Pulse.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pulse.Helpers
{
	public static class FirebaseHelper
	{
		private static FCMClient _client;

		public static void Init(string serverKey)
		{
			_client = new FCMClient(serverKey);
		}

		public static Task<IFCMResponse> SendMessage(Account account, string operationName, object contents)
		{
			return SendMessage(account.Devices.Select(d => d.FcmToken).ToList(), operationName, contents);
		}

		public static Task<IFCMResponse> SendMessage(List<string> toIds, string operationName, object contents)
		{
			return _client.SendMessageAsync(new FirebaseNet.Messaging.Message()
			{
				RegistrationIds = toIds,
				Data = new Dictionary<string, string> {
					{ "operation", operationName },
					{ "contents", JsonSerializer.Serialize(contents) }
				}
			});
		}
	}
}
