using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pulse.Models;

namespace Pulse.Controllers
{
	[ApiController]
	[Route("api/v1/activate")]
	public class ActivateController : ControllerBase
	{
		private readonly ILogger<ActivateController> _logger;

		public ActivateController(ILogger<ActivateController> logger)
		{
			_logger = logger;
		}

		[HttpGet("activate")]
		public LoginResponse check([FromQuery] string activation_code)
		{
			return new LoginResponse();
		}

	}
}