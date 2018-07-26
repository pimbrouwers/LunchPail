using System;

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
    /// Represents the current state of the context
    /// </summary>
    IDbContextState State { get; }

    /// <summary>
    /// Represents the current unit of work
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