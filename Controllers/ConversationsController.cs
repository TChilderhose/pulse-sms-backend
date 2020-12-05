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
	[Route("api/v1/conversations")]
	public class ConversationsController : ControllerBase
	{
		private readonly ILogger<ConversationsController> _logger;

		public ConversationsController(ILogger<ConversationsController> logger)
		{
			_logger = logger;
		}

		private async Task<(Account, Conversation)> GetConversation(PulseDbContext dbContext, long device_id, string account_id)
		{
			var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Conversations)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

			if (account != null)
			{
				var conversation = account.Conversations.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (conversation != null)
					return (account, conversation);
			}
			return (null, null);
		}

		[HttpPost("add")]
		public async void add([FromBody] AddConversationRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Conversations)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var conversation in request.Conversations)
				{
					dbContext.Conversations.Add(conversation);
					account.Conversations.Add(conversation);

					await FirebaseHelper.SendMessage(account, "added_conversation", new
					{
						id = conversation.DeviceId,
						color = conversation.Color,
						color_dark = conversation.ColorDark,
						color_light = conversation.ColorLight,
						color_accent = conversation.ColorAccent,
						led_color = conversation.LedColor,
						pinned = conversation.Pinned,
						read = conversation.Read,
						timestamp = conversation.Timestamp,
						title = conversation.Title,
						phone_numbers = conversation.PhoneNumbers,
						snippet = conversation.Snippet,
						ringtone = conversation.Ringtone,
						id_matcher = conversation.IdMatcher,
						mute = conversation.Mute,
						archive = conversation.Archive,
						folder_id = conversation.FolderId
					});
				}
				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update/{device_id}")]
		public async void update(long device_id, [FromQuery] string account_id, [FromBody] UpdateConversationRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (_, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				request.CloneTo(conversation);

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update_snippet/{device_id}")]
		public async void updateSnippet(long device_id, [FromQuery] string account_id, [FromBody] UpdateConversationRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (_, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				conversation.Snippet = request.Snippet;

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update_title/{device_id}")]
		public async void updateTitle(long device_id, [FromQuery] string account_id, [FromQuery] string title)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (_, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				conversation.Title = title;

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove/{device_id}")]
		public async void remove(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				dbContext.Conversations.Remove(conversation);
				account.Conversations.Remove(conversation);

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<Conversation[]> Get([FromQuery] string account_id, [FromQuery] int limit = -1, [FromQuery] int offset = -1)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Conversations)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new Conversation[0];

				var conversations = account.Conversations.OrderByDescending(c => c.Timestamp).ToArray();

				if (offset > 0)
					conversations = conversations.Skip(offset).ToArray();

				if (limit > 0)
					conversations = conversations.Take(limit).ToArray();

				return conversations;
			}
		}

		[HttpPost("read/{device_id}")]
		public async void read(long device_id, [FromQuery] string android_device, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				conversation.Read = true;

				await FirebaseHelper.SendMessage(account, "read_conversation", new
				{
					id = device_id,
					android_device,
					account_id
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("seen/{device_id}")]
		public async void seen(long device_id, [FromQuery] string account_id)
		{
			throw new NotImplementedException();
		}

		[HttpPost("seen")]
		public async void seen([FromQuery] string account_id)
		{
			throw new NotImplementedException();
		}

		[HttpPost("archive/{device_id}")]
		public async void archive(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (_, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				conversation.Archive = true;

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("unarchive/{device_id}")]
		public async void unarchive(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (_, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				conversation.Archive = false;

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("add_to_folder/{device_id}")]
		public async void addToFolder(long device_id, [FromQuery] long folder_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (_, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				conversation.FolderId = folder_id;

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove_from_folder/{device_id}")]
		public async void removeFromFolder(long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (_, conversation) = await GetConversation(dbContext, device_id, account_id);

				if (conversation == null)
					return;

				conversation.FolderId = -1L;

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("clean")]
		public async void clean([FromQuery] string account_id)
		{
			throw new NotImplementedException();
		}

		[HttpPost("cleanup_messages")]
		public async void cleanup([FromQuery] string account_id, [FromQuery] long conversation_id, [FromQuery] long timestamp)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
									.Include(a => a.Conversations)
									.ThenInclude(c => c.Messages)
									.Where(a => a.AccountId == account_id)
									.FirstOrDefaultAsync();

				if (account == null)
					return;

				var conversation = account.Conversations.Where(c => c.DeviceId == conversation_id).FirstOrDefault();

				if (conversation == null)
					return;

				var messages = conversation.Messages.Where(m => m.Timestamp < timestamp).ToList();

				foreach (var message in messages)
				{
					dbContext.Messages.Remove(message);
					account.Messages.Remove(message);
				}
			}
		}
	}
}
