using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

namespace Omniscript.CaseStudy.Client.Setup.Validation
{
    internal sealed class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        public FluentValidationBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        public Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _validator.ValidateAndThrow(request);

            return next();
        }
    }
}