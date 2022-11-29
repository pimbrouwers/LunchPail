# LunchPail
[LunchPail](https://github.com/pimbrouwers/LunchPail) is a .NET Standard compliant Unit of Work implementation for ADO.NET. The UoW is the workhorse of the data context, so the project was dubbed "lunch pail" as a reference to blue collar athletes.

[![NuGet Version](https://img.shields.io/nuget/v/LunchPail.svg)](https://www.nuget.org/packages/LunchPail)
[![Build Status](https://travis-ci.org/pimbrouwers/LunchPail.svg?branch=master)](https://travis-ci.org/pimbrouwers/LunchPail)

## Getting Started

1. Register the context within your IoC container (.NET Core shown below using SQL Server):

```c#
//startup.cs
public void ConfigureServices(IServiceCollection services)
{
  //rest of code...
  
  //context
  services.AddTransient<IDbConnectionFactory>(options =>
  {
    var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"));

    return new DbConnectionFactory(() =>
    {
      var conn = new SqlConnection(builder.ConnectionString);

      conn.Open();
      return conn;
    });
  });
  services.AddScoped<IDbContext, DbContext>();

  //repositories (we'll add this later)  
}
```

2. Create a domain class to represents your database object.

```c#
public class Product
{
  public int Id { get; set; }
  public string Name { get; set; }
}
```

3. Create a repository with a dependency on `IDbContext`

```c#
public interface IProductRepository 
{
  Task<Product> Read (int id);
}

public class ProductRepository : DbRepository
{
  public ProductRepository(IDbContext dbContext) : base(dbContext) { }

  public Product Read(int id)
  {
    return Connection.QuerySingleOrDefault<Product>("select * from dbo.Product where Id = @id", new { id }, transaction: Transaction);
  }
}
```

4. Register the repository with your IoC container (.NET Core shown below):

```c#
//startup.cs
public void ConfigureServices(IServiceCollection services)
{
  //repositories
  services.AddScoped<IProductRepository, ProductRepository>();
}
```

5. With the context and repository registered, you're free to inject this into your controller or service layer.

> Note the invocation of `dbContext.Commit()` which _must_ occur after every interaction is complete to ensure proper disposal of the connection. This is true whether the interaction is a read or write.

```c#
public class ProductService 
{
  private readonly IDbContext dbContext;
  private readonly IProductRepository productRepository;

  public ProductService (
    IDbContext dbContext,
    IProductRepository productRepository)
  {
    this.dbContext = dbContext;
    this.productRepository = productRepository;
  }

  public Product Read(int id)
  {
    var product = productRepository.Read(id);
    dbContext.Commit();

    return product;
  }
}
```

Built with ♥ by [Pim Brouwers](https://github.com/pimbrouwers) in Toronto, ON. 
