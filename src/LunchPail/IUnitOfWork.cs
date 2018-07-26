using System;
using System.Data;

namespace LunchPail
{
  public enum IUnitOfWorkState
  {
    Closed,
    Open,
    Comitted,
    RolledBack
  }

  public interface IUnitOfWork
  {
    /// <summary>
    /// Represents the current state of the unit of work
    /// </summary>
    IUnitOfWorkState State { get; }

    /// <summary>
    /// Represents the current transaction
    /// </summary>
    IDbTransaction Transaction { get; }

    /// <summary>
    /// Commit Transaction
    /// Close Transaction.Connection
    /// Set State to IUnitOfWorkState.Comitted
    /// Dispose Transaction.Connect & Transaction
    /// </summary>
    void Commit();

    /// <summary>
    /// Rollback Transaction
    /// Close Transaction.Connection
    /// Set State to IUnitOfWorkState.RolledBack
    /// Dispose Transaction.Connect & Transaction
    /// </summary>
    void Rollback();
  }
}