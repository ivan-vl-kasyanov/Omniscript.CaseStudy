using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

using Microsoft.Extensions.Logging;

using Omniscript.CaseStudy.Server.DataAccess.Repositories;
using Omniscript.CaseStudy.Server.Models.Customer.OrderCompleted;
using Omniscript.CaseStudy.Server.Models.MessageModels;

using RabbitMQ.Client;

namespace Omniscript.CaseStudy.Server.Handlers
{
    internal sealed class OrderCompletedHandler
    {
        private readonly ILogger<GetCustomersHandler> _logger;
        private readonly MessageRepository _messageRepository;
        private readonly CustomerRepository _customerRepository;

        public OrderCompletedHandler(
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
            if (messageType != typeof(OrderCompletedRequest))
            {
                return;
            }

            var message = (OrderCompletedRequest)messageRaw;
            if (message == null)
            {
                var logMessage = $"Message ID:{messageGuid}. Message is NULL.";
                _logger.LogError(logMessage);

                var messageErrorRequest = new ServerSimpleResponseMessageModel()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Severity = LogLevel.Error,
                    Message = logMessage
                };
                _messageRepository.QueueMessage(
                    messageGuid,
                    messageErrorRequest);

                return;
            }

            // TODO: need to create request validation in here.

            _customerRepository.UpdateCustomerPurchase(
                message.Email,
                message.OrderCreatedAt);

            var messageRequest = new ServerSimpleResponseMessageModel() { StatusCode = HttpStatusCode.NoContent };
            _messageRepository.QueueMessage(
                messageGuid,
                messageRequest);
        }
    }
}