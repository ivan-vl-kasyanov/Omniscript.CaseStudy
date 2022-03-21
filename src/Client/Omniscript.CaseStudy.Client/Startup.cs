using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Omniscript.CaseStudy.Client.DataAccess.Clients;
using Omniscript.CaseStudy.Client.DataAccess.Repositories;
using Omniscript.CaseStudy.Client.Setup.Logger;
using Omniscript.CaseStudy.Client.Setup.Swagger;
using Omniscript.CaseStudy.Client.Setup.Validation;

namespace Omniscript.CaseStudy.Client
{
    internal sealed class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogger()
                .AddSwagger()
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddValidation(Assembly.GetExecutingAssembly());

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddSingleton<QueueClient>();
            services.AddSingleton<ConsumerClient>();

            services.AddScoped<MessageRepository>();
        }

        public static void Configure(
            IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
            }

            if (!webHostEnvironment.IsProduction())
            {
                SwaggerSetupHelper.ConfigureSwagger(
                    applicationBuilder,
                    webHostEnvironment);
            }

            applicationBuilder.UseRouting();
            applicationBuilder.UseAuthorization();

            applicationBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SetupConsumerClient(applicationBuilder);

            if (!webHostEnvironment.IsProduction())
            {
                OpenBrowser("http://localhost:5001/swagger/index.html");
            }
        }

        private static void SetupConsumerClient(IApplicationBuilder applicationBuilder)
        {
            var consumerClient = applicationBuilder
                .ApplicationServices
                .GetService<ConsumerClient>();
            if (consumerClient == null)
            {
                var exceptionMessage = $"{nameof(ConsumerClient)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }
            consumerClient.Subscribe();
        }

        private static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
    }
}