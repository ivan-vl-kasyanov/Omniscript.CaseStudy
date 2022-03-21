using System.Linq;
using System.Reflection;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Omniscript.CaseStudy.Client.Setup.Validation
{
    internal static class ValidationHelper
    {
        public static IServiceCollection AddValidation(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            var validationAssemblies = AssemblyScanner.FindValidatorsInAssemblies(
                assemblies,
                true);
            foreach (var assembly in validationAssemblies)
            {
                var validatorInterface = assembly.InterfaceType;
                var validatorType = assembly.ValidatorType;

                var requestType = validatorInterface
                    .GetGenericArguments()
                    .Single();
                var requestInterface = requestType.GetInterface(typeof(IRequest<>).Name);
                if (requestInterface == null)
                {
                    continue;
                }
                var responseType = requestInterface
                    .GetGenericArguments()
                    .Single();

                var pipelineBehaviorInterface = typeof(IPipelineBehavior<,>).MakeGenericType(
                    requestType,
                    responseType);
                var pipelineBehaviorType = typeof(FluentValidationBehavior<,>).MakeGenericType(
                    requestType,
                    responseType);

                services.AddSingleton(
                    validatorInterface,
                    validatorType);
                services.AddSingleton(
                    pipelineBehaviorInterface,
                    pipelineBehaviorType);
            }

            return services;
        }
    }
}