using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Serilog.Core;

namespace Omniscript.CaseStudy.Client.Setup.Logger
{
    internal static class LoggerHelper
    {
        public static IServiceCollection AddLogger(this IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IDestructuringPolicy, DestructuringPolicy>());
            services.TryAddEnumerable(
                ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>),
                typeof(LoggingBehavior<,>)));

            return services;
        }
    }
}