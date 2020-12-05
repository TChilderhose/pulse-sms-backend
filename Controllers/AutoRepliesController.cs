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
	[Route("api/v1/auto_replies")]
	public class AutoRepliesController : ControllerBase
	{
		private readonly ILogger<AutoRepliesController> _logger;

		public AutoRepliesController(ILogger<AutoRepliesController> logger)
		{
			_logger = logger;
		}

		private async Task<(Account, AutoReply)> GetAutoReply(PulseDbContext dbContext, long device_id, string account_id)
		{
			var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.AutoReplies)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

			if (account != null)
			{
				var o = account.AutoReplies.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (o != null)
					return (account, o);
			}
			return (null, null);
		}

		[HttpPost("add")]
		public async void add([FromBody] AddAutoReplyRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.AutoReplies)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var autoReply in request.AutoReplies)
				{
					dbContext.AutoReplies.Add(autoReply);
					account.AutoReplies.Add(autoReply);

					await FirebaseHelper.SendMessage(account, "added_auto_reply", new
					{
						device_id = autoReply.DeviceId,
						type = autoReply.ReplyType,
						pattern = autoReply.Pattern,
						response = autoReply.Response
					});
				}

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove/{device_id}")]
		public async void remove(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, autoReply) = await GetAutoReply(dbContext, device_id, account_id);

				if (autoReply == null)
					return;

				dbContext.AutoReplies.Remove(autoReply);
				account.AutoReplies.Remove(autoReply);

				await FirebaseHelper.SendMessage(account, "removed_auto_reply", new
				{
					id = autoReply.DeviceId
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update/{device_id}")]
		public async void update(long device_id, [FromQuery] string account_id, [FromBody] UpdateAutoReplyRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, autoReply) = await GetAutoReply(dbContext, device_id, account_id);

				if (autoReply == null)
					return;

				autoReply.ReplyType = request.Type;
				autoReply.Pattern = request.Pattern;
				autoReply.Response = request.Response;

				await FirebaseHelper.SendMessage(account, "added_auto_reply", new
				{
					device_id = autoReply.DeviceId,
					type = autoReply.ReplyType,
					pattern = autoReply.Pattern,
					response = autoReply.Response
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<AutoReply[]> list([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.AutoReplies)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new AutoReply[0];

				return account.AutoReplies.ToArray();
			}
		}
	}
}