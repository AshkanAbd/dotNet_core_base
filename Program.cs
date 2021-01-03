using System;
using System.IO;
using dotNet_base.Models;
using dotNet_base.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dotNet_base
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "seed") {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                Console.WriteLine("Starting to seed database...");
                var databaseSeeder = new DatabaseSeeder();
                databaseSeeder.Production = configuration["ComponentConfig:Environment"].ToLower() == "production";
                databaseSeeder.InitSerivces();
                databaseSeeder.Services.AddDbContext<BaseContext>(options => {
                    options.UseNpgsql(databaseSeeder.Configuration.GetConnectionString("DefaultConnection"));
                    if (databaseSeeder.Configuration["ComponentConfig:Environment"].Equals("Development")) {
                        options.EnableSensitiveDataLogging();
                    }
                });
                databaseSeeder.SetupServices();

                var menoshContext = databaseSeeder.ServiceProvider.GetService<BaseContext>();
                databaseSeeder.StartWithDbContext(menoshContext);
            }
            else {
                CreateHostBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}