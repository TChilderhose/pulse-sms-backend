using System;
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
	[Route("api/v1/scheduled_messages")]
	public class ScheduledMessageController : ControllerBase
	{
		private readonly ILogger<ScheduledMessageController> _logger;

		public ScheduledMessageController(ILogger<ScheduledMessageController> logger)
		{
			_logger = logger;
		}

		private async Task<(Account, ScheduledMessage)> GetScheduledMessage(PulseDbContext dbContext, long device_id, string account_id)
		{
			var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.ScheduledMessages)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

			if (account != null)
			{
				var message = account.ScheduledMessages.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (message != null)
					return (account, message);
			}
			return (null, null);
		}

		[HttpPost("add")]
		public async void add([FromBody] AddScheduledMessageRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.ScheduledMessages)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var message in request.ScheduledMessages)
				{
					dbContext.ScheduledMessages.Add(message);
					account.ScheduledMessages.Add(message);

					await FirebaseHelper.SendMessage(account, "added_scheduled_message", new
					{
						id = message.DeviceId,
						to = message.To,
						data = message.Data,
						mimeType = message.MimeType,
						timestamp = message.Timestamp,
						title = message.Title,
						repeat = message.Repeat
					});
				}

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update/{device_id}")]
		public async void update(long device_id, [FromQuery] string account_id, [FromBody] UpdateScheduledMessageRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, message) = await GetScheduledMessage(dbContext, device_id, account_id);

				if (message == null)
					return;

				message.To = request.To;
				message.Data = request.Data;
				message.MimeType = request.MimeType;
				message.Timestamp = request.Timestamp;
				message.Title = request.Title;
				message.Repeat = request.Repeat;

				await FirebaseHelper.SendMessage(account, "updated_scheduled_message", new
				{
					id = message.DeviceId,
					to = message.To,
					data = message.Data,
					mimeType = message.MimeType,
					timestamp = message.Timestamp,
					title = message.Title,
					repeat = message.Repeat
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove/{device_id}")]
		public async void remove(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, message) = await GetScheduledMessage(dbContext, device_id, account_id);

				if (message == null)
					return;

				dbContext.ScheduledMessages.Remove(message);
				account.ScheduledMessages.Remove(message);

				await FirebaseHelper.SendMessage(account, "removed_scheduled_message", new
				{
					id = message.DeviceId
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<ScheduledMessage[]> list([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.ScheduledMessages)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new ScheduledMessage[0];

				return account.ScheduledMessages.ToArray();
			}
		}

	}
}
