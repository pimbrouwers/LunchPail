using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LunchPail
{
  public class DbConnectionFactory<TConnection> : IDbConnectionFactory where TConnection : IDbConnection, new()
  {
    private string connectionString;

    public DbConnectionFactory(string connectionString)
    {
      this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection CreateOpenConnection()
    {
      var conn = new TConnection();
      conn.ConnectionString = connectionString;

      try
      {
        if (conn.State != ConnectionState.Open)
        {
          conn.Open();
        }
      }
      catch (Exception exception)
      {
        throw new Exception("An error occured while connecting to the database. See innerException for details.", exception);
      }

      return conn;
    }
  }
}