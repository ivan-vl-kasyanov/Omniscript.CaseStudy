using System;

using Omniscript.CaseStudy.Server.DataAccess.Clients;

namespace Omniscript.CaseStudy.Server.Handlers
{
    internal sealed class HandlersHelper : IDisposable
    {
        private readonly ConsumerClient _consumerClient;
        private readonly GetCustomersHandler _getCustomersHandler;
        private readonly CreateCustomerHandler _createCustomerHandler;
        private readonly UpdateAddressHandler _updateAddressHandler;
        private readonly OrderCompletedHandler _orderCompletedHandler;

        private readonly object _disposeLock = new();

        private bool disposedValue;

        public HandlersHelper(
            ConsumerClient consumerClient,
            GetCustomersHandler getCustomersHandler,
            CreateCustomerHandler createCustomerHandler,
            UpdateAddressHandler updateAddressHandler,
            OrderCompletedHandler orderCompletedHandler)
        {
            _consumerClient = consumerClient;
            _getCustomersHandler = getCustomersHandler;
            _createCustomerHandler = createCustomerHandler;
            _updateAddressHandler = updateAddressHandler;
            _orderCompletedHandler = orderCompletedHandler;
        }

        public void SubscribeHandlers()
        {
            _consumerClient.HandleMessage += _getCustomersHandler.MessageHandler;
            _consumerClient.HandleMessage += _createCustomerHandler.MessageHandler;
            _consumerClient.HandleMessage += _updateAddressHandler.MessageHandler;
            _consumerClient.HandleMessage += _orderCompletedHandler.MessageHandler;
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock (_disposeLock)
                    {
                        try
                        {
                            _consumerClient.HandleMessage -= _getCustomersHandler.MessageHandler;
                        }
                        catch { }
                        try
                        {
                            _consumerClient.HandleMessage -= _createCustomerHandler.MessageHandler;
                        }
                        catch { }
                        try
                        {
                            _consumerClient.HandleMessage -= _updateAddressHandler.MessageHandler;
                        }
                        catch { }
                        try
                        {
                            _consumerClient.HandleMessage -= _orderCompletedHandler.MessageHandler;
                        }
                        catch { }
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}