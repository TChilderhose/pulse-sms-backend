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
	[Route("api/v1/blacklists")]
	public class BlacklistsController : ControllerBase
	{
		private readonly ILogger<BlacklistsController> _logger;

		public BlacklistsController(ILogger<BlacklistsController> logger)
		{
			_logger = logger;
		}

		[HttpPost("add")]
		public async void add([FromBody] AddBlacklistRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Blacklists)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var blacklist in request.Blacklists)
				{
					dbContext.Blacklists.Add(blacklist);
					account.Blacklists.Add(blacklist);

					await FirebaseHelper.SendMessage(account, "added_blacklist", new
					{
						id = blacklist.DeviceId,
						phone_number = blacklist.PhoneNumber,
						phrase = blacklist.Phrase
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
				var account = await dbContext.Accounts
										.Include(a => a.Blacklists)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				var blacklist = account.Blacklists.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (blacklist == null)
					return;

				await FirebaseHelper.SendMessage(account, "removed_blacklist", new
				{
					id = device_id
				});

				dbContext.Blacklists.Add(blacklist);
				account.Blacklists.Add(blacklist);

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<Blacklist[]> list([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Blacklists)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new Blacklist[0];

				return account.Blacklists.ToArray();
			}
		}
	}
}
