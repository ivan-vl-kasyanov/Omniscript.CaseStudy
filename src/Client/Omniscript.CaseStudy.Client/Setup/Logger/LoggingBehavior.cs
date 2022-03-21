using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using RestEase;

namespace Omniscript.CaseStudy.Client.Setup.Logger
{
    internal sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : IRequest<TResponse>
    {
        private const string ErrorLogFormat = "{message}\r\n{@request}";
        private const string LogFormat = "{@request}";

        private readonly ILogger _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            TResponse response;
            try
            {
                response = await next();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(
                    ex,
                    ErrorLogFormat,
                    ex.ToString(),
                    request);
                throw;
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogWarning(
                    ex,
                    ErrorLogFormat,
                    ex.ToString(),
                    request);
                throw;
            }
            catch (ApiException ex)
            {
                _logger.LogError(
                    ex,
                    ErrorLogFormat,
                    ex.ToString(),
                    request);
                throw;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(
                    ex,
                    ErrorLogFormat,
                    ex.ToString(),
                    request);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    ex,
                    ErrorLogFormat,
                    ex.ToString(),
                    request);
                throw;
            }

            _logger.LogInformation(
                LogFormat,
                request);

            return response;
        }
    }
}