using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace LunchPail.Tests
{
  public class UnitOfWorkFactoryTest
  {
    protected readonly Mock<IDbConnectionFactory> dbConnectionFactory;
    protected readonly UnitOfWorkFactory unitOfWorkFactory;

    public UnitOfWorkFactoryTest()
    {
      dbConnectionFactory = new Mock<IDbConnectionFactory>();

      var dbConnection = new Mock<IDbConnection>();

      dbConnectionFactory
        .Setup(d => d.CreateOpenConnection())
        .Returns(dbConnection.Object);

      unitOfWorkFactory = new UnitOfWorkFactory(dbConnectionFactory.Object);
    }

    public class Create : UnitOfWorkFactoryTest
    {
      [Fact]
      public void Should_return_new_unitofwork()
      {
        //Act
        var unitOfWork = unitOfWorkFactory.Create();

        //Assert
        Assert.IsAssignableFrom<IUnitOfWork>(unitOfWork);
      }
    }
  }
}