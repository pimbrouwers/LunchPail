using System.Data;
using Moq;
using Xunit;

namespace LunchPail.Tests
{
    public class DbConnectionFactoryTest
    {
        protected readonly Mock<IDbConnection> connection;
        protected readonly DbConnectionFactory connectionFactory;

        public DbConnectionFactoryTest()
        {
            connection = new Mock<IDbConnection>();
            connectionFactory = new DbConnectionFactory(ConnectionFactoryFn);
        }

        public IDbConnection ConnectionFactoryFn()
        {
            var c = connection.Object;
            c.Open();

            return c;
        }

        public class CreateOpenConnection : DbConnectionFactoryTest
        {
            [Fact]
            public void Should_create_open_connection()
            {
                //Arrange
                connection
                  .SetupGet(c => c.State)
                  .Returns(ConnectionState.Open);

                //Act
                var conn = connectionFactory.CreateOpenConnection();

                //Assert
                Assert.IsAssignableFrom<IDbConnection>(conn);
                Assert.Equal(ConnectionState.Open, conn.State);
            }
        }
    }
}