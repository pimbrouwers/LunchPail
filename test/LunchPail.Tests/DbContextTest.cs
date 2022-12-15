using System;
using System.Data;
using Moq;
using Xunit;

namespace LunchPail.Tests
{
    public class DbContextTest
    {
        protected Mock<IDbConnection> connection;
        protected DbContext db;
        protected Mock<IDbConnectionFactory> dbConnectionFactory;
        protected Mock<IDbTransaction> transaction;

        public DbContextTest()
        {
            connection = new Mock<IDbConnection>();
            dbConnectionFactory = new Mock<IDbConnectionFactory>();
            transaction = new Mock<IDbTransaction>();

            connection
              .Setup(c => c.BeginTransaction())
              .Returns(transaction.Object);

            dbConnectionFactory
              .Setup(u => u.CreateOpenConnection())
              .Returns(connection.Object);

            db = new DbContext(dbConnectionFactory.Object);
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

            [Fact]
            public void Should_fail_commit_and_rollback()
            {
                //Arrange
                transaction
                  .Setup(t => t.Commit())
                  .Throws(new Exception("fake exception"));

                //Assert
                Assert.Throws<Exception>(() => db.Commit());
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