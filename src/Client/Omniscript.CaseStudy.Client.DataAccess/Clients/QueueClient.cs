using System;

using RabbitMQ.Client;

namespace Omniscript.CaseStudy.Client.DataAccess.Clients
{
    /// <summary>
    /// Queue service client.
    /// </summary>
    public sealed class QueueClient : IDisposable
    {
        /// <summary>
        /// Default RabbitMQ Client>>>Server queue name.
        /// </summary>
        public const string RabbitMqClientToServerQueueName = "Omniscript.CaseStudy | Client>>>Server";

        /// <summary>
        /// Default RabbitMQ Client&lt;&lt;&lt;Server queue name.
        /// </summary>
        public const string RabbitMqClientFromServerQueueName = "Omniscript.CaseStudy | Client<<<Server";

        /// <summary>
        /// Name of the "MessageId" message header.
        /// </summary>
        public const string MessageIdHeaderName = "MessageId";

        /// <summary>
        /// Name of the "MessageContentType" message header.
        /// </summary>
        public const string MessageContentTypeHeaderName = "MessageContentType";

        private const string DefaultRabbitMqHostAddress = "localhost";

        private readonly object _disposeLock = new();

        /// <summary>
        /// Queue channel.
        /// </summary>
        public IModel? Channel { get; private set; }

        private bool _disposedValue;
        private IConnection? _connection;

        /// <summary>
        /// Queue service client constructor.
        /// </summary>
        public QueueClient()
        {
            var factory = new ConnectionFactory() { HostName = DefaultRabbitMqHostAddress };
            try
            {
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                var exceptionMessage = "Unable to initialize the connection to the queue server.";

                throw new Exception(
                    exceptionMessage,
                    ex);
            }
            try
            {
                Channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                var exceptionMessage = "Unable to initialize the queue channel.";

                throw new Exception(
                    exceptionMessage,
                    ex);
            }
        }

        private void DisposeUnmanaged()
        {
            if (!_disposedValue)
            {
                lock (_disposeLock)
                {
                    DisposeChannel();
                    DisposeConnection();
                }

                _disposedValue = true;
            }
        }

        private void DisposeChannel()
        {
            if (Channel != null)
            {
                try
                {
                    Channel.Dispose();
                }
                catch { }
                Channel = null;
            }
        }

        private void DisposeConnection()
        {
            if (_connection != null)
            {
                try
                {
                    _connection.Dispose();
                }
                catch { }
                _connection = null;
            }
        }

        /// <summary>
        /// Disposes unmanaged resources.
        /// </summary>
        ~QueueClient()
        {
            DisposeUnmanaged();
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            DisposeUnmanaged();
            GC.SuppressFinalize(this);
        }
    }
}