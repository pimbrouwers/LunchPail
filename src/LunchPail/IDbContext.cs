using System.Data;

namespace LunchPail
{
    public enum IDbContextState
    {
        Closed,
        Open,
        Comitted,
        RolledBack
    }

    public interface IDbContext
    {
        /// <summary>
        /// Current state of the context
        /// </summary>
        IDbContextState State { get; }

        /// <summary>
        /// Current database connection
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Current database transaction
        /// </summary>
        IDbTransaction Transaction { get; }

        /// <summary>
        /// Current unit of work
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Commit IUnitOfWork
        /// Set State to IDbContextState.Committed
        /// Nullify UnitOfWork
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback IUnitOfWork
        /// Set State to IDbContextState.Rolledback
        /// Nullify UnitOfWork
        /// </summary>
        void Rollback();
    }
}