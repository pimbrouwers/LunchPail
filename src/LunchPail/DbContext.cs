using System;
using System.Data;

namespace LunchPail
{
    public class DbContext : IDbContext, IDisposable
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IUnitOfWork _unitOfWork;

        public DbContext(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbContextState State { get; private set; } = IDbContextState.Closed;

        public IDbConnection Connection =>
          _connection ?? (_connection = OpenConnection());

        public IDbTransaction Transaction =>
          _transaction ?? (_transaction = Connection.BeginTransaction());

        public IUnitOfWork UnitOfWork =>
          _unitOfWork ?? (_unitOfWork = new UnitOfWork(Transaction));

        public void Commit()
        {
            try
            {
                UnitOfWork.Commit();
                State = IDbContextState.Comitted;
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                Reset();
            }
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }

        public void Rollback()
        {
            try
            {
                UnitOfWork.Rollback();
                State = IDbContextState.RolledBack;
            }
            finally
            {
                Reset();
            }
        }

        private IDbConnection OpenConnection()
        {
            State = IDbContextState.Open;
            return _connectionFactory.CreateOpenConnection();
        }

        private void Reset()
        {
            Connection?.Close();
            Connection?.Dispose();
            Transaction?.Dispose();

            _connection = null;
            _transaction = null;
            _unitOfWork = null;
        }
    }
}