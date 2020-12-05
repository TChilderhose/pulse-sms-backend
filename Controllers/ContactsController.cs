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
	[Route("api/v1/contacts")]
	public class ContactsController : ControllerBase
	{
		private readonly ILogger<ContactsController> _logger;

		public ContactsController(ILogger<ContactsController> logger)
		{
			_logger = logger;
		}

		private async Task<(Account, Contact)> GetContact(PulseDbContext dbContext, long device_id, string account_id)
		{
			var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Include(a => a.Contacts)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

			if (account != null)
			{
				var contact = account.Contacts.Where(c => c.DeviceId == device_id).FirstOrDefault();
				if (contact != null)
					return (account, contact);
			}
			return (null, null);
		}

		[HttpPost("add")]
		public async void add([FromBody] AddContactRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Contacts)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return;

				foreach (var contact in request.Contacts)
				{
					dbContext.Contacts.Add(contact);
					account.Contacts.Add(contact);

					await FirebaseHelper.SendMessage(account, "added_contact", new
					{
						phone_number = contact.PhoneNumber,
						name = contact.Name,
						type = contact.ContactType,
						color = contact.Color,
						color_dark = contact.ColorDark,
						color_light = contact.ColorLight,
						color_accent = contact.ColorAccent
					});
				}

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update_device_id")]
		public async void update([FromQuery] string phone_number, [FromQuery] long device_id, [FromQuery] string account_id, [FromBody] UpdateContactRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, contact) = await GetContact(dbContext, device_id, account_id);

				if (contact == null)
					return;

				contact.PhoneNumber = request.PhoneNumber;
				contact.Name = request.Name;

				await FirebaseHelper.SendMessage(account, "updated_contact", new
				{
					device_id = device_id,
					phone_number = contact.PhoneNumber,
					name = contact.Name,
					type = contact.ContactType,
					color = contact.Color,
					color_dark = contact.ColorDark,
					color_light = contact.ColorLight,
					color_accent = contact.ColorAccent
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove_device_id")]
		public async void remove([FromQuery] string phone_number, [FromQuery] long device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var (account, contact) = await GetContact(dbContext, device_id, account_id);

				if (contact == null)
					return;

				dbContext.Contacts.Remove(contact);
				account.Contacts.Remove(contact);

				await FirebaseHelper.SendMessage(account, "removed_contact", new
				{
					device_id = device_id,
					phone_number = phone_number,
				});

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("clear")]
		public async void clear([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Contacts)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				var contacts = account.Contacts.ToArray();
				foreach (var contact in contacts)
				{
					dbContext.Contacts.Remove(contact);
					account.Contacts.Remove(contact);
				}
				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet]
		public async Task<Contact[]> list([FromQuery] string account_id, [FromQuery] int limit, [FromQuery] int offset)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Contacts)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account == null)
					return new Contact[0];

				var contacts = account.Contacts.ToArray();

				if (offset > 0)
					contacts = contacts.Skip(offset).ToArray();

				if (limit > 0)
					contacts = contacts.Take(limit).ToArray();

				return contacts;
			}
		}
	}
}
