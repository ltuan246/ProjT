namespace KISS.Misc;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOption<TOptions>(this IServiceCollection services)
        where TOptions : class, new()
    {
        services.ConfigureOptions<OptionsSetup<TOptions>>();

        return services;
    }
}

public abstract record OptionsSetup<TOptions>(IConfiguration Configuration, string SectionName) : IConfigureOptions<TOptions>
    where TOptions : class, new()
{
    public void Configure(TOptions options)
    {
        Configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}