using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Pulse
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseUrls("http://0.0.0.0:5000");
					webBuilder.UseKestrel();
					webBuilder.UseStartup<Startup>();
				});
	}
}
