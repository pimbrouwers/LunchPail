using System;

namespace LunchPail
{
  public class DbContext : IDbContext
  {
    private IUnitOfWorkFactory unitOfWorkFactory;

    private IUnitOfWork unitOfWork;

    public DbContext(IUnitOfWorkFactory unitOfWorkFactory)
    {
      this.unitOfWorkFactory = unitOfWorkFactory;
    }

    public IDbContextState State { get; private set; } = IDbContextState.Closed;

    public IUnitOfWork UnitOfWork
    {
      get
      {
        if (unitOfWork == null)
        {
          unitOfWork = unitOfWorkFactory.Create();
          State = IDbContextState.Open;
        }

        return unitOfWork;
      }
    }

    public void Commit()
    {
      try
      {
        UnitOfWork.Commit();
        State = IDbContextState.Comitted;
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
        UnitOfWork.Rollback();
        State = IDbContextState.RolledBack;
      }
      finally
      {
        Reset();
      }
    }

    private void Reset()
    {
      unitOfWork = null;
    }
  }
}