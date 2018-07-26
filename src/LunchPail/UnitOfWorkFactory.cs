using System;
using System.Data;

namespace LunchPail
{
  public class UnitOfWorkFactory : IUnitOfWorkFactory
  {
    private IDbConnectionFactory dbConnectionFactory;

    public UnitOfWorkFactory(IDbConnectionFactory dbConnectionFactory)
    {
      this.dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
    }

    public IUnitOfWork Create()
    {
      return new UnitOfWork(dbConnectionFactory.CreateOpenConnection());
    }
  }
}