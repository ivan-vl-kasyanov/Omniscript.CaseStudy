using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using Omniscript.CaseStudy.Client.DataAccess.Clients;
using Omniscript.CaseStudy.Client.DataAccess.Repositories;
using Omniscript.CaseStudy.Server.Models.Customer.GetCustomers;
using Omniscript.CaseStudy.Server.Models.Entities;
using Omniscript.CaseStudy.Server.Models.MessageModels;

using RabbitMQ.Client;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.GetCustomers
{
    internal sealed class GetCustomersCommandHandler
        : IRequestHandler<GetCustomersCommand, (HttpStatusCode StatusCode, int CustomersTotalCount, IEnumerable<CustomerModel> Customers)>
    {
        private readonly ILogger<GetCustomersCommandHandler> _logger;
        private readonly MessageRepository _messageRepository;
        private readonly ConsumerClient _consumerClient;

        private Guid? _messageGuid;
        private (HttpStatusCode StatusCode, LogLevel? Severity, string? Message, int CustomersTotalCount, IEnumerable<CustomerModel> Customers)? _messageResponse;

        public GetCustomersCommandHandler(
            ILogger<GetCustomersCommandHandler> logger,
            MessageRepository messageRepository,
            ConsumerClient consumerClient)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _consumerClient = consumerClient;
        }

        public async Task<(HttpStatusCode StatusCode, int CustomersTotalCount, IEnumerable<CustomerModel> Customers)> Handle(
            GetCustomersCommand request,
            CancellationToken cancellationToken)
        {
            _consumerClient.HandleMessage += MessageHandler;

            var messageRequest = new GetCustomersRequest()
            {
                Skip = request.Skip,
                Take = request.Take
            };
            _messageGuid = _messageRepository.QueueMessage(messageRequest);

            while (!_messageResponse.HasValue)
            {
                try
                {
                    await Task.Delay(
                        500,
                        cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    try
                    {
                        _consumerClient.HandleMessage -= MessageHandler;
                    }
                    catch { }
                    throw;
                }
            }

            if ((_messageResponse?.Severity != null) ||
                (!String.IsNullOrWhiteSpace(_messageResponse?.Message)))
            {
                _logger.Log(
                    _messageResponse?.Severity ?? LogLevel.Error,
                    _messageResponse?.Message);
            }

#pragma warning disable CS8629 // Nullable value type may be null.
            return (_messageResponse.Value.StatusCode, _messageResponse.Value.CustomersTotalCount, _messageResponse.Value.Customers);
#pragma warning restore CS8629 // Nullable value type may be null.
        }

        private void MessageHandler(
            IModel channel,
            ulong deliveryTag,
            Guid messageGuid,
            Type messageType,
            object messageRaw)
        {
            if (!_messageGuid.HasValue)
            {
                return;
            }
            if (messageGuid != _messageGuid.Value)
            {
                return;
            }

            var message = (ServerMultipleEntityResponseMessageModel)messageRaw;
            if (message == null)
            {
                var exceptionMessage = $"{nameof(message)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }
            var customersTotalCount = message?.Metadata == null
                ? 0
                : Convert.ToInt32(message.Metadata);
            var customers = message
                ?.Entities
                ?.Select(customer => CustomerConverter((JObject)customer))
                ?.OrderByDescending(customer => customer.CreatedAt)
                ?? Enumerable.Empty<CustomerModel>();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _messageResponse = (message.StatusCode, message.Severity, message.Message, customersTotalCount, customers);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        private static CustomerModel CustomerConverter(JObject customerRaw)
        {
            var id = (long)customerRaw.GetValue("Id");
            var email = (string)customerRaw.GetValue("Email");
            var addressRaw = (JObject)customerRaw.GetValue("Address");
            var createdAt = ((DateTimeOffset)customerRaw.GetValue("CreatedAt")).ToUniversalTime();
            var isArchived = (bool)customerRaw.GetValue("IsArchived");
            var purchasedAt = ((DateTimeOffset?)customerRaw.GetValue("PurchasedAt"))?.ToUniversalTime();
            var address = AddressConverter(addressRaw);
            var customer = new CustomerModel(
                id,
                email,
                address,
                createdAt,
                isArchived,
                purchasedAt);

            return customer;
        }

        private static AddressModel AddressConverter(JObject customerRaw)
        {
            var country = (string)customerRaw.GetValue("Country");
            var city = (string)customerRaw.GetValue("City");
            var street = (string)customerRaw.GetValue("Street");
            var address = new AddressModel(
                country,
                city,
                street);

            return address;
        }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8604 // Possible null reference argument.
    }
}