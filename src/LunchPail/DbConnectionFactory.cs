using System;
using System.Data;

namespace LunchPail
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateOpenConnection();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly Func<IDbConnection> _connectionFactoryFn;

        public DbConnectionFactory(Func<IDbConnection> connectionFactory)
        {
            _connectionFactoryFn = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public IDbConnection CreateOpenConnection() => _connectionFactoryFn();
    }
}