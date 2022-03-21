using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

using Microsoft.Extensions.Logging;

using Omniscript.CaseStudy.Server.DataAccess.Repositories;
using Omniscript.CaseStudy.Server.Models.Customer.GetCustomers;
using Omniscript.CaseStudy.Server.Models.Entities;
using Omniscript.CaseStudy.Server.Models.MessageModels;

using RabbitMQ.Client;

namespace Omniscript.CaseStudy.Server.Handlers
{
    internal sealed class GetCustomersHandler
    {
        private readonly ILogger<GetCustomersHandler> _logger;
        private readonly MessageRepository _messageRepository;
        private readonly CustomerRepository _customerRepository;

        public GetCustomersHandler(
            ILogger<GetCustomersHandler> logger,
            MessageRepository messageRepository,
            CustomerRepository customerRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _customerRepository = customerRepository;
        }

        [SuppressMessage(
            "Style",
            "IDE0060:Remove unused parameter",
            Justification = "Needed for signature.")]
        public void MessageHandler(
            IModel channel,
            ulong deliveryTag,
            Guid messageGuid,
            Type messageType,
            object messageRaw)
        {
            if (messageType != typeof(GetCustomersRequest))
            {
                return;
            }

            var message = (GetCustomersRequest)messageRaw;
            if (message == null)
            {
                var logMessage = $"Message ID:{messageGuid}. Message is NULL.";
                _logger.LogError(logMessage);

                var messageErrorRequest = new ServerMultipleEntityResponseMessageModel(
                    HttpStatusCode.InternalServerError,
                    LogLevel.Error,
                    logMessage,
                    typeof(CustomerModel).FullName ?? String.Empty,
                    null,
                    null,
                    null);
                _messageRepository.QueueMessage(
                    messageGuid,
                    messageErrorRequest);

                return;
            }
            if ((message.Skip ?? 0) < 0)
            {
                var logMessage = $"Message ID:{messageGuid}. Skip cannot be less than 0.";
                _logger.LogWarning(logMessage);

                var messageErrorRequest = new ServerMultipleEntityResponseMessageModel(
                    HttpStatusCode.BadRequest,
                    LogLevel.Warning,
                    logMessage,
                    typeof(CustomerModel).FullName ?? String.Empty,
                    null,
                    null,
                    null);
                _messageRepository.QueueMessage(
                    messageGuid,
                    messageErrorRequest);

                return;
            }
            if ((message.Take ?? Int32.MaxValue) < 1)
            {
                var logMessage = $"Message ID:{messageGuid}. Take must to be greater than 0.";
                _logger.LogWarning(logMessage);

                var messageErrorRequest = new ServerMultipleEntityResponseMessageModel(
                    HttpStatusCode.BadRequest,
                    LogLevel.Warning,
                    logMessage,
                    typeof(CustomerModel).FullName ?? String.Empty,
                    null,
                    null,
                    null);
                _messageRepository.QueueMessage(
                    messageGuid,
                    messageErrorRequest);

                return;
            }

            (var customersTotalCount, var customers) = _customerRepository.GetCustomers(
                message.Skip ?? 0,
                message.Take ?? Int32.MaxValue);

            var messageRequest = new ServerMultipleEntityResponseMessageModel(
                HttpStatusCode.OK,
                null,
                null,
                typeof(CustomerModel).FullName ?? String.Empty,
                customers,
                typeof(int).FullName,
                customersTotalCount);
            _messageRepository.QueueMessage(
                messageGuid,
                messageRequest);
        }
    }
}