using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.Extensions.Logging;

using Omniscript.CaseStudy.Client.DataAccess.Clients;
using Omniscript.CaseStudy.Client.DataAccess.Repositories;
using Omniscript.CaseStudy.Server.Models.Customer.UpdateAddress;
using Omniscript.CaseStudy.Server.Models.MessageModels;

using RabbitMQ.Client;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.UpdateAddress
{
    internal sealed class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, HttpStatusCode>
    {
        private readonly ILogger<UpdateAddressCommandHandler> _logger;
        private readonly MessageRepository _messageRepository;
        private readonly ConsumerClient _consumerClient;

        private Guid? _messageGuid;
        private (HttpStatusCode StatusCode, LogLevel? Severity, string? Message)? _messageResponse;

        public UpdateAddressCommandHandler(
            ILogger<UpdateAddressCommandHandler> logger,
            MessageRepository messageRepository,
            ConsumerClient consumerClient)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _consumerClient = consumerClient;
        }

        public async Task<HttpStatusCode> Handle(
            UpdateAddressCommand request,
            CancellationToken cancellationToken)
        {
            _consumerClient.HandleMessage += MessageHandler;

            var messageRequest = new UpdateAddressRequest(
                request.CustomerId,
                request.Address);
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

            return _messageResponse?.StatusCode ?? HttpStatusCode.InternalServerError;
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

            var message = (ServerSimpleResponseMessageModel)messageRaw;

            _messageResponse = (message.StatusCode, message.Severity, message.Message);
        }
    }
}