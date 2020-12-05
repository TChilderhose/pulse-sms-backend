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
	[Route("api/v1/folders")]
	public class FoldersController : ControllerBase
	{
		private readonly ILogger<FoldersController> _logger;

		public FoldersController(ILogger<FoldersController> logger)
		{
			_logger = logger;
		}

		private async Task<(Account, Folder)> GetFolder(PulseDbContext dbContext, long device_id, string account_id)
		{
			var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Folders)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

			if (account != null)
			{
				var folder = account.Folders.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (folder != null)
					return (account, folder);
			}
			return (null, null);
		}

		[HttpPost("add")]
		public async void add([FromBody] AddFolderRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Folders)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var folder in request.Folders)
				{
					dbContext.Folders.Add(folder);
					account.Folders.Add(folder);

					await FirebaseHelper.SendMessage(account, "added_folder", folder);
				}

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove/{device_id}")]
		public async void remove(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, folder) = await GetFolder(dbContext, device_id, account_id);

				if (folder == null)
					return;

				dbContext.Folders.Remove(folder);
				account.Folders.Remove(folder);

				await FirebaseHelper.SendMessage(account, "removed_folder", new
				{
					id = device_id
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update/{device_id}")]
		public async void update(long device_id, [FromQuery] string account_id, [FromBody] UpdateFolderRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, folder) = await GetFolder(dbContext, device_id, account_id);

				if (folder == null)
					return;

				folder.Name = request.Name;

				await FirebaseHelper.SendMessage(account, "updated_folder", folder);

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<Folder[]> list([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Folders)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new Folder[0];

				return account.Folders.ToArray();
			}
		}
	}
}
