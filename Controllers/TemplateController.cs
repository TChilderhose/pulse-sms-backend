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
	[Route("api/v1/templates")]
	public class TemplateController : ControllerBase
	{
		private readonly ILogger<TemplateController> _logger;

		public TemplateController(ILogger<TemplateController> logger)
		{
			_logger = logger;
		}

		private async Task<(Account, Template)> GetTemplate(PulseDbContext dbContext, long device_id, string account_id)
		{
			var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Templates)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

			if (account != null)
			{
				var template = account.Templates.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (template != null)
					return (account, template);
			}
			return (null, null);
		}

		[HttpPost("add")]
		public async void add([FromBody] AddTemplateRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Templates)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var template in request.Templates)
				{
					dbContext.Templates.Add(template);
					account.Templates.Add(template);

					await FirebaseHelper.SendMessage(account, "added_template", template);
				}

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove/{device_id}")]
		public async void remove(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, template) = await GetTemplate(dbContext, device_id, account_id);

				if (template == null)
					return;

				dbContext.Templates.Remove(template);
				account.Templates.Remove(template);

				await FirebaseHelper.SendMessage(account, "removed_template", new
				{
					id = device_id
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update/{device_id}")]
		public async void update(long device_id, [FromQuery] string account_id, [FromBody] UpdateTemplateRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, template) = await GetTemplate(dbContext, device_id, account_id);

				if (template == null)
					return;

				template.Text = request.Text; 
				
				await FirebaseHelper.SendMessage(account, "updated_template", template);

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<Template[]> list([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Templates)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new Template[0];

				return account.Templates.ToArray();
			}
		}
	}
}