using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

using Newtonsoft.Json;

using Omniscript.CaseStudy.Client.DataAccess.Clients;

namespace Omniscript.CaseStudy.Client.DataAccess.Repositories
{
    /// <summary>
    /// Message pipe repository.
    /// </summary>
    public sealed class MessageRepository
    {
        private readonly QueueClient _queueClient;

        /// <summary>
        /// Message pipe repository constructor.
        /// </summary>
        /// <param name="queueClient">Queue service client instance.</param>
        public MessageRepository(QueueClient queueClient)
        {
            _queueClient = queueClient;
        }

        /// <summary>
        /// Puts message into the message pipe.
        /// </summary>
        /// <typeparam name="T">Message type.</typeparam>
        /// <param name="message">Message instance.</param>
        /// <returns>GUID of the queued message.</returns>
        public Guid QueueMessage<T>(T message)
            where T : notnull
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
            if (message == null)
            {
                var exceptionMessage = $"{nameof(message)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }

            var messageGuid = Guid.NewGuid();
            var properties = _queueClient.Channel.CreateBasicProperties();
            properties.ContentType = MediaTypeNames.Application.Octet;
            properties.DeliveryMode = 2;
            properties.Headers = new Dictionary<string, object>()
            {
                { QueueClient.MessageIdHeaderName, messageGuid.ToString() },
                { QueueClient.MessageContentTypeHeaderName, typeof(T).FullName ?? String.Empty }
            };
            var serializedMessage = JsonConvert.SerializeObject(
                message,
                Formatting.None);
            var binaryMessage = Encoding
                .UTF8
                .GetBytes(serializedMessage);
            _queueClient
                .Channel
                .BasicPublish(
                    String.Empty,
                    QueueClient.RabbitMqClientToServerQueueName,
                    true,
                    properties,
                    binaryMessage);

            return messageGuid;
        }
    }
}