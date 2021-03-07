using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace EventBusRabbitMQ
{
    public class RabbitMqConnection : IRabbitMqConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private readonly bool _disposed;

        public RabbitMqConnection(IConnectionFactory connectionFactory, bool disposed=false)
        {
            _connectionFactory = connectionFactory;
            _disposed = disposed;
            if (!IsConnected)
                TryConnect();
        } 

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        public void Dispose()
        {
            if (_disposed)
                return;
            try
            {
                _connection.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public IModel CreateModel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("no rabbit connection");
            return _connection.CreateModel();
        }
        public bool TryConnect()
        {
            while (!IsConnected)
                _connection = _connectionFactory.CreateConnection();
            return IsConnected;
        }
    }
}