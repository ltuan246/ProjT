namespace KISS.QueryBuilder.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

        IConfigurationSection section = configuration.GetSection("ConnectionStrings");

        IServiceCollection services = new ServiceCollection();
        services.Configure<DbContextOptions>(section);
        services.AddSingleton<DapperContext>();
        services.AddScoped(typeof(GenericRepository<>));
        var serviceProvider = services.BuildServiceProvider();
        var repo = serviceProvider.GetService<GenericRepository<User>>();


        var builder = Builders<ComponentTest>.Filter;
        var filter = builder.And(builder.Eq(t => t.AsString, "a"), builder.Eq(t => t.AsString, "b"));
        var result = filter.Render();

        repo!.Find(filter);
        var rs = repo.GetList();
    }

    public class User
    {
        public int Id { get; set; }
    }

}