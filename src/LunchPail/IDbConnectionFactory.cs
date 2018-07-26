using System;
using System.Data;

namespace LunchPail
{
  public interface IDbConnectionFactory
  {
    IDbConnection CreateOpenConnection();
  }
}