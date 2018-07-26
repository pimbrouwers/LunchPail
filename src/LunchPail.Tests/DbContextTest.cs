using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LunchPail.Tests
{
  public class DbContextTest
  {
    protected DbContext db;
    protected Mock<IUnitOfWorkFactory> unitOfWorkFactory;

    public DbContextTest()
    {
      unitOfWorkFactory = new Mock<IUnitOfWorkFactory>();

      var unitOfWork = new Mock<IUnitOfWork>();
      unitOfWorkFactory
        .Setup(u => u.Create())
        .Returns(unitOfWork.Object);

      db = new DbContext(unitOfWorkFactory.Object);
    }

    public class NewDbContext : DbContextTest
    {
      [Fact]
      public void Should_have_closed_state()
      {
        //Assert
        Assert.Equal(IDbContextState.Closed, db.State);
      }
    }

    public class OpenDbContext : DbContextTest
    {
      [Fact]
      public void Should_have_open_state()
      {
        //Arrange
        var uow = db.UnitOfWork;

        //Assert
        Assert.Equal(IDbContextState.Open, db.State);
      }
    }

    public class Commit : DbContextTest
    {
      [Fact]
      public void Should_commit_unitofwork_and_have_committed_state()
      {
        //Act
        db.Commit();

        //Assert
        Assert.Equal(IDbContextState.Comitted, db.State);
      }
    }

    public class Rollback : DbContextTest
    {
      [Fact]
      public void Should_rollback_unitofwork_and_have_rolledback_state()
      {
        //Act
        db.Rollback();

        //Assert
        Assert.Equal(IDbContextState.RolledBack, db.State);
      }
    }
  }
}