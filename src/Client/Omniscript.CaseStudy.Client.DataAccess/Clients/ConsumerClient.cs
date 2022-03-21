using System;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Omniscript.CaseStudy.Server.Models;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Omniscript.CaseStudy.Client.DataAccess.Clients
{
    /// <summary>
    /// Message consumer client.
    /// </summary>
    public sealed class ConsumerClient
    {
        /// <summary>
        /// Consumed message handler.
        /// </summary>
        /// <param name="channel">Queue channel.</param>
        /// <param name="deliveryTag">Delivery identifier.</param>
        /// <param name="messageGuid">Message identifier.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="message">Message body.</param>
        public delegate void MessageHandler(
            IModel channel,
            ulong deliveryTag,
            Guid messageGuid,
            Type messageType,
            object message);

        /// <summary>
        /// Message consumed event.
        /// </summary>
        public event MessageHandler? HandleMessage;

        private readonly ILogger<ConsumerClient> _logger;
        private readonly QueueClient _queueClient;

        /// <summary>
        /// Message consumer client constructor.
        /// </summary>
        /// <param name="logger">Default logger instance.</param>
        /// <param name="queueClient">Queue service client instance.</param>
        public ConsumerClient(
            ILogger<ConsumerClient> logger,
            QueueClient queueClient)
        {
            _logger = logger;
            _queueClient = queueClient;
        }

        /// <summary>
        /// Subscribes for an answers from the server.
        /// </summary>
        public void Subscribe()
        {
            if (_queueClient == null)
            {
                var exceptionMessage = $"{nameof(QueueClient)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }
            if (_queueClient.Channel == null)
            {
                var exceptionMessage = $"{nameof(_queueClient.Channel)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }

            var consumer = new EventingBasicConsumer(_queueClient.Channel);
            consumer.Received += (
                chanel,
                basicDeliverEventArgs) => ReceiveHandler(
                    chanel,
                    basicDeliverEventArgs);

            _queueClient
                .Channel
                .BasicConsume(
                    QueueClient.RabbitMqClientFromServerQueueName,
                    true,
                    String.Empty,
                    false,
                    false,
                    null,
                    consumer);
        }

        private void ReceiveHandler(
            object? chanel,
            BasicDeliverEventArgs basicDeliverEventArgs)
        {
            (Guid MessageGuid, Type MessageType, object Message) consumeMessageResult;
            try
            {
                consumeMessageResult = ConsumeMessage(
                    chanel,
                    basicDeliverEventArgs);
            }
            catch (Exception ex)
            {
                var logMessage = "Consuming message error.";
                _logger.LogWarning(
                    ex,
                    logMessage);

                return;
            }

            try
            {
                HandleMessage?.Invoke(
#pragma warning disable CS8604 // Possible null reference argument.
                    _queueClient.Channel,
#pragma warning restore CS8604 // Possible null reference argument.
                    basicDeliverEventArgs.DeliveryTag,
                    consumeMessageResult.MessageGuid,
                    consumeMessageResult.MessageType,
                    consumeMessageResult.Message);
            }
            catch (Exception ex)
            {
                var logMessage = "Handling message error.";
                _logger.LogWarning(
                    ex,
                    logMessage);

                return;
            }
        }

        private static (Guid MessageGuid, Type MessageType, object Message) ConsumeMessage(
            object? chanel,
            BasicDeliverEventArgs? basicDeliverEventArgs)
        {
            (var messageGuid, var messageContentType) = ValidateMessage(basicDeliverEventArgs);

            var binaryMessage = basicDeliverEventArgs
                ?.Body
                .ToArray() ?? Array.Empty<byte>();
            var serializedMessage = Encoding
                .UTF8
                .GetString(binaryMessage);
            var message = JsonConvert.DeserializeObject(
                serializedMessage,
                messageContentType);
            if (message == null)
            {
                var loggerMessage = $"Cannot deserialize body of the message ID:{messageGuid}.";

                throw new ArgumentException(loggerMessage);
            }

            return (messageGuid, messageContentType, message);
        }

        private static (Guid MessageGuid, Type MessageContentType) ValidateMessage(BasicDeliverEventArgs? basicDeliverEventArgs)
        {
            if (basicDeliverEventArgs == null)
            {
                var exceptionMessage = "NULL message received.";

                throw new ArgumentException(exceptionMessage);
            }

            if (basicDeliverEventArgs.BasicProperties == null)
            {
                var exceptionMessage = "Message with no properties received.";

                throw new ArgumentException(exceptionMessage);
            }

            if (basicDeliverEventArgs.BasicProperties.Headers == null)
            {
                var exceptionMessage = "Message with no headers received.";

                throw new ArgumentException(exceptionMessage);
            }
            if (!basicDeliverEventArgs.BasicProperties.Headers.Any())
            {
                var exceptionMessage = "Message with empty headers received.";

                throw new ArgumentException(exceptionMessage);
            }

            if (!basicDeliverEventArgs.BasicProperties.Headers.ContainsKey(QueueClient.MessageIdHeaderName))
            {
                var exceptionMessage = "Message without ID received.";

                throw new ArgumentException(exceptionMessage);
            }
            if (basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageIdHeaderName] == null)
            {
                var exceptionMessage = "Message with NULL ID received.";

                throw new ArgumentException(exceptionMessage);
            }
            if (basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageIdHeaderName].GetType() != typeof(byte[]))
            {
                var exceptionMessage =
                    "Message with unknown ID type " +
                    basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageIdHeaderName].GetType().FullName +
                    " received.";

                throw new ArgumentException(exceptionMessage);
            }
            string messageGuidRaw;
            try
            {
                messageGuidRaw = Encoding
                    .UTF8
                    .GetString((byte[])basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageIdHeaderName]);
            }
            catch (Exception ex)
            {
                var exceptionMessage = "Unable to decode message ID.";

                throw new ArgumentException(
                    exceptionMessage,
                    ex);
            }
            if (String.IsNullOrWhiteSpace(messageGuidRaw))
            {
                var exceptionMessage = "Message with empty ID received.";

                throw new ArgumentException(exceptionMessage);
            }
            if (!Guid.TryParse(
                messageGuidRaw,
                out var messageGuid))
            {
                var exceptionMessage =
                    "Received message ID is not GUID: \"" +
                    (string)basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageIdHeaderName] +
                    "\".";

                throw new ArgumentException(exceptionMessage);
            }
            if (messageGuid == Guid.Empty)
            {
                var exceptionMessage = "Message with empty GUID received.";

                throw new ArgumentException(exceptionMessage);
            }

            if (!basicDeliverEventArgs.BasicProperties.Headers.ContainsKey(QueueClient.MessageContentTypeHeaderName))
            {
                var exceptionMessage = $"Message ID:{messageGuid} received without {QueueClient.MessageContentTypeHeaderName} header.";

                throw new ArgumentException(exceptionMessage);
            }
            if (basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageContentTypeHeaderName] == null)
            {
                var exceptionMessage = $"Message ID:{messageGuid} received with NULL {QueueClient.MessageContentTypeHeaderName} header.";

                throw new ArgumentException(exceptionMessage);
            }
            if (basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageContentTypeHeaderName].GetType() != typeof(byte[]))
            {
                var exceptionMessage =
                    $"Message ID:{messageGuid} received with unknown type: \"" +
                    basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageContentTypeHeaderName].GetType().FullName +
                    "\".";

                throw new ArgumentException(exceptionMessage);
            }
            string messageContentTypeRaw;
            try
            {
                messageContentTypeRaw = Encoding
                    .UTF8
                    .GetString((byte[])basicDeliverEventArgs.BasicProperties.Headers[QueueClient.MessageContentTypeHeaderName]);
            }
            catch (Exception ex)
            {
                var exceptionMessage = "Unable to decode message content type.";

                throw new ArgumentException(
                    exceptionMessage,
                    ex);
            }
            if (String.IsNullOrWhiteSpace(messageContentTypeRaw))
            {
                var exceptionMessage = "Message with empty content type received.";

                throw new ArgumentException(exceptionMessage);
            }
            var modelsAssemblyName = typeof(MessageTypeResolver).Assembly.FullName;
            var messageContentTypeLocation = $"{messageContentTypeRaw}, {modelsAssemblyName}";
            Type messageContentType;
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                messageContentType = Type.GetType(
                    messageContentTypeLocation,
                    true);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            catch (Exception ex)
            {
                var exceptionMessage = "Unable to reflect message content type.";

                throw new ArgumentException(
                    exceptionMessage,
                    ex);
            }

            if (basicDeliverEventArgs.Body.IsEmpty)
            {
                var exceptionMessage = $"Message ID:{messageGuid} received with en empty body.";

                throw new ArgumentException(exceptionMessage);
            }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return (messageGuid, messageContentType);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }
    }
}