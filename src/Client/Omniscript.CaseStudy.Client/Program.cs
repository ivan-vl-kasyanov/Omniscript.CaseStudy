
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Omniscript.CaseStudy.Client
{
    /// <summary>
    /// Main assembly class.
    /// </summary>
    public sealed class Program
    {
        /// <summary>
        /// Main program entry point.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseKestrel(options => { options.ListenLocalhost(5001); }); ;
                })
                .Build()
                .Run();
        }
    }
}