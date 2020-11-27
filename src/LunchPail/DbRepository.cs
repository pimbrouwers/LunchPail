using System.Data;

namespace LunchPail
{
    public class DbRepository
    {
        private readonly IDbContext _dbContext;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public DbRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _connection = _dbContext.UnitOfWork.Transaction.Connection;
            _transaction = _dbContext.UnitOfWork.Transaction;
        }

        public IDbConnection Connection => _connection;

        public IDbTransaction Transaction => _transaction;
    }
}
