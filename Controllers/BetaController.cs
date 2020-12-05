using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pulse.Controllers
{
	[ApiController]
	[Route("api/v1/beta")]
	public class BetaController : ControllerBase
	{
		private readonly ILogger<BetaController> _logger;

		public BetaController(ILogger<BetaController> logger)
		{
			_logger = logger;
		}

		[HttpPost("register")]
		public void register([FromQuery] string account_id)
		{
			//Not needed
		}

		[HttpPost("remove")]
		public void remove([FromQuery] string account_id)
		{
			//Not needed
		}

	}
}
