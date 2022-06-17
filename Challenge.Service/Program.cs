namespace PlanetWars.Server
{
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    builder
                        .AddSystemsManager(configSource =>
                        {
                            configSource.ReloadAfter = TimeSpan.FromMinutes(5);
                            configSource.Path = "/challengeService";
                        })
                        .AddSecretsManager(configurator: options =>
                        {
                            options.PollingInterval = TimeSpan.FromMinutes(5);
                            options.KeyGenerator = (entry, key) => key.ToUpper();
                        });
                })
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                });
        }
    }
}