using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace LunchPail.Tests
{
  public class UnitOfWorkTest
  {
    protected readonly Mock<IDbConnection> dbConnection;
    protected readonly UnitOfWork unitOfWork;

    public UnitOfWorkTest()
    {
      dbConnection = new Mock<IDbConnection>();

      var transaction = new Mock<IDbTransaction>();

      dbConnection
        .Setup(d => d.BeginTransaction())
        .Returns(transaction.Object);

      unitOfWork = new UnitOfWork(dbConnection.Object);
    }

    public class NewUnitOfWork : UnitOfWorkTest
    {
      [Fact]
      public void Should_have_closed_state()
      {
        //Assert
        Assert.Equal(IUnitOfWorkState.Closed, unitOfWork.State);
      }
    }

    public class OpenUnitOfWork : UnitOfWorkTest
    {
      [Fact]
      public void Should_have_open_state()
      {
        //Arrange
        var transaction = unitOfWork.Transaction;

        //Assert
        Assert.Equal(IUnitOfWorkState.Open, unitOfWork.State);
      }
    }

    public class Commit : UnitOfWorkTest
    {
      [Fact]
      public void Should_commit_transaction_and_have_committed_state()
      {
        //Act
        unitOfWork.Commit();

        //Assert
        Assert.Equal(IUnitOfWorkState.Comitted, unitOfWork.State);
      }
    }

    public class Rollback : UnitOfWorkTest
    {
      [Fact]
      public void Should_rollback_transaction_and_have_rolledback_state()
      {
        //Act
        unitOfWork.Rollback();

        //Assert
        Assert.Equal(IUnitOfWorkState.RolledBack, unitOfWork.State);
      }
    }
  }
}