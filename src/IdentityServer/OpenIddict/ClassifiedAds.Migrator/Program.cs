using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Persistence;
using DbUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Reflection;

var builder = Host.CreateDefaultBuilder(args)
.UseClassifiedAdsLogger(configuration =>
{
    return new LoggingOptions();
})
.ConfigureServices((hostContext, services) =>
{
    var configuration = hostContext.Configuration;

    if (bool.TryParse(configuration["CheckDependency:Enabled"], out var enabled) && enabled)
    {
        NetworkPortCheck.Wait(configuration["CheckDependency:Host"], 5);
    }

    services.AddDbContext<OpenIddictDbContext>(options =>
    {
        var connectionString = configuration["ConnectionStrings:IdentityServer"];
        options.UseSqlServer(configuration["ConnectionStrings:IdentityServer"], sql =>
        {
            sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
        });

        // Register the entity sets needed by OpenIddict.
        options.UseOpenIddict();
    });
    //services.AddDbContext<AdsDbContext>(options =>
    //{
    //    var connectionString = configuration["ConnectionStrings:ClassifiedAds"];
    //    options.UseSqlServer(configuration["ConnectionStrings:ClassifiedAds"], sql =>
    //    {
    //        sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
    //    });
    //});
});

var app = builder.Build();
var configuration = app.Services.GetRequiredService<IConfiguration>();

Policy.Handle<Exception>().WaitAndRetry(
[
    TimeSpan.FromSeconds(10),
    TimeSpan.FromSeconds(20),
    TimeSpan.FromSeconds(30),
])
.Execute(() =>
{
    app.MigrateOpenIddictDb();

    var openIddictUpgrader = DeployChanges.To
    .SqlDatabase(configuration.GetConnectionString("IdentityServer"))
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

    var openIddictResult = openIddictUpgrader.PerformUpgrade();

    if (!openIddictResult.Successful)
    {
        throw openIddictResult.Error;
    }

    //app.MigrateAdsDb();
    //var migrateAdsUpgrader = DeployChanges.To
    //.SqlDatabase(configuration.GetConnectionString("ClassifiedAds"))
    //.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    //.LogToConsole()
    //.Build();

    //var migrateAdsResult = migrateAdsUpgrader.PerformUpgrade();

    //if (!migrateAdsResult.Successful)
    //{
    //    throw migrateAdsResult.Error;
    //}
});