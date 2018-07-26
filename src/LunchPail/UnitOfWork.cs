using System;
using System.Data;

namespace LunchPail
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly IDbConnection connection;
    private IDbTransaction transaction;

    public UnitOfWork(IDbConnection connection)
    {
      this.connection = connection;
    }

    public IUnitOfWorkState State { get; private set; } = IUnitOfWorkState.Closed;

    public IDbTransaction Transaction
    {
      get
      {
        if (transaction == null)
        {
          transaction = connection.BeginTransaction();
          State = IUnitOfWorkState.Open;
        }

        return transaction;
      }
    }

    public void Commit()
    {
      try
      {
        Transaction.Commit();
        Transaction.Connection?.Close();

        State = IUnitOfWorkState.Comitted;
      }
      catch
      {
        Transaction.Rollback();
        throw;
      }
      finally
      {
        Reset();
      }
    }

    public void Rollback()
    {
      try
      {
        Transaction.Rollback();
        Transaction.Connection?.Close();

        State = IUnitOfWorkState.RolledBack;
      }
      catch
      {
        throw;
      }
      finally
      {
        Reset();
      }
    }

    private void Reset()
    {
      Transaction?.Dispose();
      Transaction?.Connection?.Dispose();
      transaction = null;
    }
  }
}