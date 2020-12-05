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
	[Route("api/v1/drafts")]
	public class DraftsController : ControllerBase
	{
		private readonly ILogger<DraftsController> _logger;

		public DraftsController(ILogger<DraftsController> logger)
		{
			_logger = logger;
		}

		private async Task<(Account, Draft)> GetDraft(PulseDbContext dbContext, long device_id, string account_id)
		{
			var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Drafts)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

			if (account != null)
			{
				var draft = account.Drafts.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (draft != null)
					return (account, draft);
			}
			return (null, null);
		}

		[HttpPost("add")]
		public async void add([FromBody] AddDraftRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Drafts)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var draft in request.Drafts)
				{
					dbContext.Drafts.Add(draft);
					account.Drafts.Add(draft);

					await FirebaseHelper.SendMessage(account, "added_draft", new
					{
						id = draft.DeviceId,
						conversation_id = draft.DeviceConversationId,
						data = draft.Data,
						mime_type = draft.MimeType
					});
				}

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update/{device_id}")]
		public async void update(long device_id, [FromQuery] string account_id, [FromBody] UpdateDraftRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, draft) = await GetDraft(dbContext, device_id, account_id);

				if (draft == null)
					return;

				draft.Data = request.Data;
				draft.MimeType = request.MimeType;

				await FirebaseHelper.SendMessage(account, "replaced_drafts", new
				{
					id = draft.DeviceId,
					conversation_id = draft.DeviceConversationId,
					data = draft.Data,
					mime_type = draft.MimeType
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove/{device_id}")]
		public async void remove(long device_id, [FromQuery] string android_device, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, draft) = await GetDraft(dbContext, device_id, account_id);

				if (draft == null)
					return;

				dbContext.Drafts.Remove(draft);
				account.Drafts.Remove(draft);

				await FirebaseHelper.SendMessage(account, "removed_drafts", new
				{
					id = draft.DeviceId
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<Draft[]> list([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Drafts)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new Draft[0];

				return account.Drafts.ToArray();
			}
		}
	}
}
