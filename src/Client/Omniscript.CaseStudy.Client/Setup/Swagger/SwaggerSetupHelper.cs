using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Omniscript.CaseStudy.Client.Setup.Swagger
{
    internal static class SwaggerSetupHelper
    {
        private const string DefaultAssemblyVersion = "0.0.0.0";
        private const string SwaggerUrlTemplate = "/swagger/{0}/swagger.json";
        private const string SwaggerTitleTemplate = "{0} v{1}";

        private static readonly string _assemblyVersion;

        static SwaggerSetupHelper()
        {
            _assemblyVersion = Assembly
                .GetExecutingAssembly()
                .GetName()
                ?.Version
                ?.ToString() ?? DefaultAssemblyVersion;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var xmlDocuments = currentAssembly
                .GetReferencedAssemblies()
                .Union(new AssemblyName[] { currentAssembly.GetName() })
                .Select(assembly => Path.Combine(Path.GetDirectoryName(
                    AppContext.BaseDirectory) ?? String.Empty,
                    $"{assembly.Name}.xml"))
                .Where(file => File.Exists(file))
                .ToArray();

            return services.AddSwaggerGen(setup =>
            {
                Array.ForEach(xmlDocuments, (document) =>
                {
                    setup.IncludeXmlComments(
                        document,
                        true);
                });
                setup.SwaggerDoc(
                    _assemblyVersion,
                    new OpenApiInfo
                    {
                        Title = typeof(Startup).Namespace,
                        Version = _assemblyVersion
                    });
            });
        }

        public static void ConfigureSwagger(
            IApplicationBuilder applicationBuilder,
            IWebHostEnvironment webHostEnvironment)
        {
            if (!webHostEnvironment.IsProduction())
            {
                var swaggerUrl = String.Format(
                    SwaggerUrlTemplate,
                    _assemblyVersion);
                var swaggerTitle = String.Format(
                    SwaggerTitleTemplate,
                    typeof(Startup).Namespace,
                    _assemblyVersion);
                applicationBuilder.UseSwagger();
                applicationBuilder.UseSwaggerUI(setup => setup.SwaggerEndpoint(
                    swaggerUrl,
                    swaggerTitle));
            }
        }
    }
}