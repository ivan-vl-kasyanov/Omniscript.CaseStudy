using System;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Omniscript.CaseStudy.Server.DataAccess.Clients;
using Omniscript.CaseStudy.Server.DataAccess.Repositories;
using Omniscript.CaseStudy.Server.Handlers;
using Omniscript.CaseStudy.Server.Setup.Logger;

namespace Omniscript.CaseStudy.Server
{
    internal sealed class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogger()
                .AddMediatR(Assembly.GetExecutingAssembly());

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
            services.AddSingleton<DatabaseClient>();

            services.AddScoped<MessageRepository>();
            services.AddScoped<CustomerRepository>();

            services.AddSingleton<GetCustomersHandler>();
            services.AddSingleton<CreateCustomerHandler>();
            services.AddSingleton<UpdateAddressHandler>();
            services.AddSingleton<OrderCompletedHandler>();
            services.AddSingleton<HandlersHelper>();
        }

        public static void Configure(
            IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
            }

            applicationBuilder.UseRouting();
            applicationBuilder.UseAuthorization();

            SetupConsumerClient(applicationBuilder);
            SetupDatabaseClient(applicationBuilder);
            SetupHandlersHelper(applicationBuilder);
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

        private static void SetupDatabaseClient(IApplicationBuilder applicationBuilder)
        {
            var databaseClient = applicationBuilder
                .ApplicationServices
                .GetService<DatabaseClient>();
            if (databaseClient == null)
            {
                var exceptionMessage = $"{nameof(DatabaseClient)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }
            databaseClient.RecreateDatabase();
        }

        private static void SetupHandlersHelper(IApplicationBuilder applicationBuilder)
        {
            var handlersHelper = applicationBuilder
                .ApplicationServices
                .GetService<HandlersHelper>();
            if (handlersHelper == null)
            {
                var exceptionMessage = $"{nameof(HandlersHelper)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }
            handlersHelper.SubscribeHandlers();
        }
    }
}