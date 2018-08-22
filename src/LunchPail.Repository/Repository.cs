using Dapper;
using Sequel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LunchPail
{
  public abstract class Repository<TEntity> where TEntity : class
  {
    private readonly IDbContext dbContext;
    protected readonly ISqlMapper<TEntity> sqlMapper;

    public Repository(
      IDbContext dbContext,
      ISqlMapper<TEntity> sqlMapper)
    {
      this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      this.sqlMapper = sqlMapper ?? throw new ArgumentNullException(nameof(sqlMapper));
    }

    private IDbConnection Db =>
      dbContext.UnitOfWork.Transaction.Connection;

    private IDbTransaction Transaction =>
      dbContext.UnitOfWork.Transaction;

    /// <summary>
    /// Check entity existence based on key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected async Task<bool> EntityExists(object key)
    {
      var sql = new SqlBuilder()
        .Select("count(1)")
        .From(sqlMapper.Table)
        .Where($"{sqlMapper.Key} = @key")
        .ToSql();

      return await ExecuteScalar<bool>(sql, new { key });
    }

    /// <summary>
    /// Create entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>scope_identity() of inserted</returns>
    protected async Task<int> CreateEntity(TEntity entity)
    {
      return await Db.QuerySingleOrDefaultAsync<int>($"{sqlMapper.CreateSql.ToSql()}; select scope_identity();", entity, Transaction);
    }

    /// <summary>
    /// Read entity (throws exception if more than 1 record found)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="commandTimeout"></param>
    /// <returns>Entity or null</returns>
    protected async Task<TEntity> ReadEntity(object key)
    {
      return await QuerySingleOrDefault(sqlMapper.ReadSql.Where($"{sqlMapper.Key} = @key").ToSql(), new { key });
    }

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    protected async Task<bool> UpdateEntity(TEntity entity)
    {
      return (await Execute(sqlMapper.UpdateSql.ToSql(), entity)) == 1;
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="transaction"></param>
    /// <param name="commandTimeout"></param>
    /// <returns></returns>
    protected async Task<bool> DeleteEntity(TEntity entity)
    {
      return (await Execute(sqlMapper.DeleteSql.ToSql(), entity)) == 1;
    }

    /// <summary>
    /// Lists all entities
    /// </summary>
    /// <returns>Entities or null</returns>
    protected async Task<IEnumerable<TEntity>> ListEntities()
    {
      return await Query(sqlMapper.ReadSql.ToSql());
    }

    /// <summary>
    /// Finds all entities by a specific field
    /// </summary>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <returns>Entities or null</returns>
    protected async Task<IEnumerable<TEntity>> FindBy(string field, object value)
    {
      return await Query(sqlMapper.ReadSql.Where($"{field} = @value").ToSql(), new { value });
    }

    /// <summary>
    /// Finds all entities by a specific field using the LIKE predicate filter
    /// ** Assumes suffix wildcarding has been done prior to invocation
    /// </summary>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <returns>Entities or null</returns>
    protected async Task<IEnumerable<TEntity>> FindLike(string field, object value)
    {
      return await Query(sqlMapper.ReadSql.Where($"{field} like @value").ToSql(), new { value });
    }

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns>Number of rows affected</returns>
    protected async Task<int> Execute(string sql, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text)
    {
      return await Db.ExecuteAsync(sql, param, Transaction, commandTimeout: commandTimeout, commandType: commandType);
    }

    /// <summary>
    /// Execute scalar
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<T> ExecuteScalar<T>(string sql, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text)
    {
      return await Db.ExecuteScalarAsync<T>(sql, param, Transaction, commandTimeout: commandTimeout, commandType: commandType);
    }

    /// <summary>
    /// First or default
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns>Record or null</returns>
    protected async Task<TEntity> QueryFirstOrDefault(string sql, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text)
    {
      return await Db.QueryFirstOrDefaultAsync<TEntity>(sql, param, Transaction, commandTimeout: commandTimeout, commandType: commandType);
    }

    /// <summary>
    /// Single or default
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns>Record or throws exception</returns>
    protected async Task<TEntity> QuerySingleOrDefault(string sql, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text)
    {
      return await Db.QuerySingleOrDefaultAsync<TEntity>(sql, param, Transaction, commandTimeout: commandTimeout, commandType: commandType);
    }

    /// <summary>
    /// Query
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<IEnumerable<TEntity>> Query(string sql, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text)
    {
      return await Db.QueryAsync<TEntity>(sql, param, Transaction, commandTimeout: commandTimeout, commandType: commandType);
    }

    /// <summary>
    /// Multimap 2
    /// </summary>
    /// <typeparam name="TSecond"></typeparam>
    /// <param name="sql"></param>
    /// <param name="map"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<IEnumerable<TEntity>> Query<TSecond>(string sql, Func<TEntity, TSecond, TEntity> map, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text, string splitOn = "Id")
    {
      return await Db.QueryAsync(sql, map, param, Transaction, commandTimeout: commandTimeout, commandType: commandType, splitOn: splitOn);
    }

    /// <summary>
    /// Multimap 3
    /// </summary>
    /// <typeparam name="TSecond"></typeparam>
    /// <typeparam name="TThird"></typeparam>
    /// <param name="sql"></param>
    /// <param name="map"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<IEnumerable<TEntity>> Query<TSecond, TThird>(string sql, Func<TEntity, TSecond, TThird, TEntity> map, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text, string splitOn = "Id")
    {
      return await Db.QueryAsync(sql, map, param, Transaction, commandTimeout: commandTimeout, commandType: commandType, splitOn: splitOn);
    }

    /// <summary>
    /// Multimap 4
    /// </summary>
    /// <typeparam name="TSecond"></typeparam>
    /// <typeparam name="TThird"></typeparam>
    /// <typeparam name="TFourth"></typeparam>
    /// <param name="sql"></param>
    /// <param name="map"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<IEnumerable<TEntity>> Query<TSecond, TThird, TFourth>(string sql, Func<TEntity, TSecond, TThird, TFourth, TEntity> map, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text, string splitOn = "Id")
    {
      return await Db.QueryAsync(sql, map, param, Transaction, commandTimeout: commandTimeout, commandType: commandType, splitOn: splitOn);
    }

    /// <summary>
    /// Multimap 5
    /// </summary>
    /// <typeparam name="TSecond"></typeparam>
    /// <typeparam name="TThird"></typeparam>
    /// <typeparam name="TFourth"></typeparam>
    /// <typeparam name="TFifth"></typeparam>
    /// <param name="sql"></param>
    /// <param name="map"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<IEnumerable<TEntity>> Query<TSecond, TThird, TFourth, TFifth>(string sql, Func<TEntity, TSecond, TThird, TFourth, TFifth, TEntity> map, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text, string splitOn = "Id")
    {
      return await Db.QueryAsync(sql, map, param, Transaction, commandTimeout: commandTimeout, commandType: commandType, splitOn: splitOn);
    }

    /// <summary>
    /// Multimap 6
    /// </summary>
    /// <typeparam name="TSecond"></typeparam>
    /// <typeparam name="TThird"></typeparam>
    /// <typeparam name="TFourth"></typeparam>
    /// <typeparam name="TFifth"></typeparam>
    /// <typeparam name="TSixth"></typeparam>
    /// <param name="sql"></param>
    /// <param name="map"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<IEnumerable<TEntity>> Query<TSecond, TThird, TFourth, TFifth, TSixth>(string sql, Func<TEntity, TSecond, TThird, TFourth, TFifth, TSixth, TEntity> map, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text, string splitOn = "Id")
    {
      return await Db.QueryAsync(sql, map, param, Transaction, commandTimeout: commandTimeout, commandType: commandType, splitOn: splitOn);
    }

    /// <summary>
    /// Multimap 7
    /// </summary>
    /// <typeparam name="TSecond"></typeparam>
    /// <typeparam name="TThird"></typeparam>
    /// <typeparam name="TFourth"></typeparam>
    /// <typeparam name="TFifth"></typeparam>
    /// <typeparam name="TSixth"></typeparam>
    /// <typeparam name="TSeventh"></typeparam>
    /// <param name="sql"></param>
    /// <param name="map"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    protected async Task<IEnumerable<TEntity>> Query<TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string sql, Func<TEntity, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEntity> map, object param = null, int commandTimeout = 30, CommandType commandType = CommandType.Text, string splitOn = "Id")
    {
      return await Db.QueryAsync(sql, map, param, Transaction, commandTimeout: commandTimeout, commandType: commandType, splitOn: splitOn);
    }
  }
}