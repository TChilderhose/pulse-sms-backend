using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pulse.Helpers;
using Pulse.Models;

namespace Pulse.Controllers
{
	[ApiController]
	[Route("api/v1/messages")]
	public class MessagesController : ControllerBase
	{
		private readonly ILogger<MessagesController> _logger;

		public MessagesController(ILogger<MessagesController> logger)
		{
			_logger = logger;
		}

		private async Task<(Account, Message)> GetMessage(PulseDbContext dbContext, long device_id, string account_id)
		{
			var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Messages)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

			if (account != null)
			{
				var message = account.Messages.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (message != null)
					return (account, message);
			}
			return (null, null);
		}

		[HttpPost("add")]
		public async void add([FromBody] AddMessagesRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Messages)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var message in request.Messages)
				{
					dbContext.Messages.Add(message);
					account.Messages.Add(message);

					await FirebaseHelper.SendMessage(account, "added_message", new
					{
						id = message.DeviceId,
						conversationId = message.DeviceConversationId,
						type = message.MessageType,
						timestamp = message.Timestamp,
						read= message.Read,
						seen = message.Seen,
						sent_device = message.SentDevice,
						data = message.Data,
						mime_type = message.MimeType,
						from = message.MessageFrom,
						color = message.Color
					});
				}

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update/{device_id}")]
		public async void update(long device_id, [FromQuery] string account_id, [FromBody] UpdateMessageRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, message) = await GetMessage(dbContext, device_id, account_id);

				if (message == null)
					return;

				message.MessageType = request.Type;
				message.Read = request.Read;
				message.Seen = request.Seen;
				message.Timestamp = request.Timestamp;

				await FirebaseHelper.SendMessage(account, "updated_message", new
				{
					id = message.DeviceId,
					type = message.MessageType,
					message.Read,
					message.Seen,
					timestamp = message.Timestamp
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update_type/{device_id}")]
		public async void updateType(long device_id, [FromQuery] string account_id, [FromQuery] int message_type)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, message) = await GetMessage(dbContext, device_id, account_id);

				if (message == null)
					return;

				message.MessageType = message_type;

				await FirebaseHelper.SendMessage(account, "update_message_type", new
				{
					id = device_id,
					message_type
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove/{device_id}")]
		public async void remove(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, message) = await GetMessage(dbContext, device_id, account_id);

				if (message == null)
					return;

				dbContext.Messages.Remove(message);
				account.Messages.Remove(message);

				await FirebaseHelper.SendMessage(account, "removed_message", new
				{
					id = device_id
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("cleanup")]
		public async void cleanup([FromQuery] string account_id, [FromQuery] long timestamp)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Messages)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				var messages = account.Messages.Where(m => m.Timestamp < timestamp).ToList();

				foreach (var message in messages)
				{
					dbContext.Messages.Remove(message);
					account.Messages.Remove(message);
				}

				await FirebaseHelper.SendMessage(account, "cleanup_messages", new
				{
					timestamp
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<Message[]> list([FromQuery] string account_id, [FromQuery] long conversation_id, [FromQuery] int limit, [FromQuery] int offset)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Messages)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new Message[0];

				var messages = account.Messages.OrderByDescending(c => c.Timestamp).ToArray();

				if (offset > 0)
					messages = messages.Skip(offset).ToArray();

				if (limit > 0)
					messages = messages.Take(limit).ToArray();

				return messages;
			}
		}
	}
}