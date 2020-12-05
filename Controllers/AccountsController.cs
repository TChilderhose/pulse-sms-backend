using System;
using System.Linq;
using System.Threading.Tasks;
using Pulse.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pulse.Models;

namespace Pulse.Controllers
{
	[ApiController]
	[Route("api/v1/accounts")]
	public class AccountsController : ControllerBase
	{
		private readonly ILogger<AccountsController> _logger;

		public AccountsController(ILogger<AccountsController> logger)
		{
			_logger = logger;
		}

		[HttpPost("signup")]
		public async Task<SignupResponse> signup([FromBody] SignupRequest request)
		{
			var account = new Account();

			account.AccountId = Guid.NewGuid().ToString("N");
			account.Salt1 = Crypto.GenerateSalt();
			account.Salt2 = Crypto.GenerateSalt();
			account.PasswordHash = Crypto.GetPasswordHash(request.Password, account.Salt1);

			account.Username = request.Username;
			account.RealName = request.RealName;
			account.PhoneNumber = request.PhoneNumber;

			using (var dbContext = new PulseDbContext())
			{
				await dbContext.Accounts.AddAsync(account);
				await dbContext.SaveChangesAsync();
			}

			return account.ForceType<SignupResponse>();
		}

		[HttpPost("login")]
		public async Task<LoginResponse> login([FromBody] LoginRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts.Where(a => a.Username == request.Username).FirstOrDefaultAsync();
				if (account != null && account.PasswordHash == Crypto.GetPasswordHash(request.Password, account.Salt1))
				{
					return account.ForceType<LoginResponse>();
				}
			}
			return null;
		}

		[HttpPost("remove_account")]
		public async void remove([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				await FirebaseHelper.SendMessage(account, "removed_account", new
				{
					id = account_id
				});

				dbContext.Accounts.Remove(account);

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("clean_account")]
		public async void clean([FromQuery] string account_id)
		{
			throw new NotImplementedException();
		}

		[HttpGet("count")]
		public async Task<AccountCountResponse> count([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Messages)
										.Include(a => a.Conversations)
										.Include(a => a.Drafts)
										.Include(a => a.ScheduledMessages)
										.Include(a => a.Blacklists)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new AccountCountResponse();

				return new AccountCountResponse()
				{
					DeviceCount = account.Devices.Count(),
					MessageCount = account.Messages.Count(),
					ConversationCount = account.Conversations.Count(),
					DraftCount = account.Drafts.Count(),
					ScheduledCount = account.ScheduledMessages.Count(),
					BlacklistCount = account.Blacklists.Count()
				};
			}
		}

		[HttpGet]
		public async Task<AccountListResponse> list([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Messages)
										.Include(a => a.Conversations)
										.Include(a => a.Drafts)
										.Include(a => a.ScheduledMessages)
										.Include(a => a.Blacklists)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new AccountListResponse();

				return new AccountListResponse()
				{
					Devices = account.Devices.ToArray(),
					Messages = account.Messages.ToArray(),
					Conversations = account.Conversations.ToArray(),
					Drafts = account.Drafts.ToArray(),
					ScheduledMessages = account.ScheduledMessages.ToArray(),
					Blacklists = account.Blacklists.ToArray()
				};
			}
		}

		[HttpPost("update_setting")]
		public async void updateSetting([FromQuery] string account_id, [FromQuery] string pref, [FromQuery] string type, [FromQuery] string value)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();
				if (account != null)
				{
					switch (pref)
					{
						case "base_theme":
							account.BaseTheme = value;
							break;

						case "apply_primary_color_toolbar":
							account.ApplyPrimaryColorToToolbar = bool.Parse(value);
							break;

						case "global_primary_color":
							account.ColorLight = int.Parse(value);
							break;

						case "global_primary_dark_color":
							account.ColorDark = int.Parse(value);
							break;

						case "global_accent_color":
							account.ColorAccent = int.Parse(value);
							break;

						case "apply_theme_globally":
							account.UseGlobalTheme = bool.Parse(value);
							break;

						case "conversation_categories":
							account.ConversationCategories = bool.Parse(value);
							break;

						case "message_timestamp":
							account.MessageTimestamp = bool.Parse(value);
							break;

						case "bubble_style":
							account.RounderBubbles = value == "circle";
							break;

						default:
							Console.WriteLine(pref);
							break;
					}

					await FirebaseHelper.SendMessage(account, "update_setting", new
					{
						pref, 
						type, 
						value
					});

					await dbContext.SaveChangesAsync();
				}
			}
		}

		[HttpPost("dismissed_notification")]
		public async void dismissedNotification([FromQuery] string account_id, [FromQuery] string device_id, [FromQuery] long id) //conversationId
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Conversations)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				await FirebaseHelper.SendMessage(account, "dismissed_notification", new
				{
					device_id,
					id
				});
			}
		}

		[HttpGet("view_subscription")]
		public async Task<AccountListResponse> viewSubscription([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Messages)
										.Include(a => a.Conversations)
										.Include(a => a.Drafts)
										.Include(a => a.ScheduledMessages)
										.Include(a => a.Blacklists)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new AccountListResponse();

				return new AccountListResponse()
				{
					Devices = account.Devices.ToArray(),
					Messages = account.Messages.ToArray(),
					Conversations = account.Conversations.ToArray(),
					Drafts = account.Drafts.ToArray(),
					ScheduledMessages = account.ScheduledMessages.ToArray(),
					Blacklists = account.Blacklists.ToArray()
				};
			}
		}

		[HttpPost("update_subscription")]
		public void updateSubscription([FromQuery] string account_id, [FromQuery] int subscription_type, [FromQuery] long subscription_expiration)
		{
			//Not needed
		}
	}
}
