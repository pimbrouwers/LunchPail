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
    protected readonly Mock<IDbTransaction> transaction;
    protected readonly UnitOfWork unitOfWork;

    public UnitOfWorkTest()
    {
      transaction = new Mock<IDbTransaction>();

      unitOfWork = new UnitOfWork(transaction.Object);
    }

    public class NewUnitOfWork : UnitOfWorkTest
    {
      [Fact]
      public void Should_have_open_state()
      {
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

      [Fact]
      public void Should_fail_commit_and_rollback()
      {
        //Arrange
        transaction
          .Setup(t => t.Commit())
          .Throws(new Exception("fake exception"));

        //Assert
        Assert.Throws<Exception>(() => unitOfWork.Commit());
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