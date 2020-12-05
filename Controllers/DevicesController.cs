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
	[Route("api/v1/devices")]
	public class DevicesController : ControllerBase
	{
		private readonly ILogger<DevicesController> _logger;

		public DevicesController(ILogger<DevicesController> logger)
		{
			_logger = logger;
		}

		[HttpPost("add")]
		public async Task<AddDeviceResponse> add([FromBody] AddDeviceRequest request)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Where(a => a.AccountId == request.AccountId)
										.FirstOrDefaultAsync();

				if (account == null)
					return null;

				dbContext.Devices.Add(request.Device);
				account.Devices.Add(request.Device);

				await dbContext.SaveChangesAsync();
			}

			return request.Device.ForceType<AddDeviceResponse>();
		}

		[HttpPost("update/{id}")]
		public async void update(long id, [FromQuery] string account_id, [FromQuery] string name, [FromQuery] string fcm_token)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				var device = account.Devices.Where(d => d.DeviceId == id).FirstOrDefault();
				if (device == null)
					return;

				device.AccountId = account_id;
				device.Name = name;
				device.FcmToken = fcm_token;

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("remove/{id}")]
		public async void remove(int id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				var device = account.Devices.Where(d => d.DeviceId == id).FirstOrDefault();
				if (device == null)
					return;

				dbContext.Devices.Remove(device);
				account.Devices.Remove(device);

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpPost("update_primary")]
		public async void updatePrimary([FromQuery] string new_primary_device_id, [FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				var device = account.Devices.Where(d => d.Primary).FirstOrDefault();
				if (device != null)
					device.Primary = false;

				device = account.Devices.Where(d => d.DeviceId == long.Parse(new_primary_device_id)).FirstOrDefault();
				if (device != null)
					device.Primary = true;

				await dbContext.SaveChangesAsync();
			}
		}

		[HttpGet("devices")]
		public async Task<Device[]> list([FromQuery] string account_id)
		{
			using (var dbContext = new PulseDbContext())
			{
				var account = await dbContext.Accounts
										.Include(a => a.Devices)
										.Where(a => a.AccountId == account_id)
										.FirstOrDefaultAsync();

				if (account != null)
					return account.Devices.ToArray();				
			}

			return null;
		}
	}
}
