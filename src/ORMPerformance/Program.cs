using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ORMPerformance.Data;
using ORMPerformance.Data.Domain;
using ORMPerformance.Data.Repository;
using ORMPerformance.Data.UnitOfWork;
using ORMPerformance.Infrastructure.Mappings;
using ORMPerformance.Infrastructure.Options;
using ORMPerformance.Services;
using ORMPerformance.Services.Abstract;
using ORMPerformance.Services.PerfomanceTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ORMPerformance.Data.DataSeed;

namespace ORMPerformance
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile("appsettings.json")
             .Build();

            var connString = builder.GetConnectionString("AppConnection");
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer(connString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }

    public class Program
    {
        public static IConfigurationRoot _configuration;

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var perfomanceTestByEF = serviceProvider.GetRequiredService<EntityFrameworkPerfomanceTest>();
            var perfomanceTestByDappeer = serviceProvider.GetRequiredService<DapperPerfomanceTest>();
            var perfomanceTestByADONET = serviceProvider.GetRequiredService<ADONETPerfomanceTest>();
            var unitOfWork = serviceProvider.GetRequiredService<UnitOfWork>();

            Stopwatch stopWatch = new Stopwatch();
            string option;
            var operations = new Dictionary<string, string> 
            {
                { "a", "Get customer by id" },
                { "b", "Get orders" },
                { "c", "Get customers with orders (ef by include)" },
                { "d", "Get customers with orders (ef by projection)" },
                { "e", "Get customers with orders - separated queries, manual mapping" },
                { "f", "Insert customer" },
                { "g", "Update customer" },
                { "h", "Delete customer" }
            };

            while (true)
            {
                Console.WriteLine("\n\n ORM perfomance testing options:");
                foreach(var op in operations)
                {
                    Console.WriteLine($"  {op.Key}: {op.Value}");
                }

                Console.Write("\nEnter option: ");
                option = Console.ReadLine();
                List<double> queryTimeByEFList = new List<double>(), queryTimeByDappeerList = new List<double>(), queryTimeByADONETList = new List<double>();

                switch (option)
                {
                    case "a": // Get customer by id
                        for (int i = 0; i < 100; i++)
                        {
                            stopWatch.Start();
                            var resultByEF = perfomanceTestByEF.GetCustomerByIdAsync().Result;
                            queryTimeByEFList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByDappeer = perfomanceTestByDappeer.GetCustomerByIdAsync().Result;
                            queryTimeByDappeerList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByADONET = perfomanceTestByADONET.GetCustomerByIdAsync().Result;
                            queryTimeByADONETList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();
                        }
                        break;
                    case "b": // Get orders
                        for (int i = 0; i < 100; i++)
                        {
                            stopWatch.Start();
                            var resultByEF = perfomanceTestByEF.GetOrdersAsync(10000).Result;
                            queryTimeByEFList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByDappeer = perfomanceTestByDappeer.GetOrdersAsync(10000).Result;
                            queryTimeByDappeerList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByADONET = perfomanceTestByADONET.GetOrdersAsync(10000).Result;
                            queryTimeByADONETList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();
                        }
                        break;
                    case "c": // Get customers with orders - by include
                        for (int i = 0; i < 100; i++)
                        {
                            stopWatch.Start();
                            var resultByEF = perfomanceTestByEF.GetCustomersWithOrdersByIncludeAsync().Result;
                            queryTimeByEFList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByDappeer = perfomanceTestByDappeer.GetCustomersWithOrdersAsync().Result;
                            queryTimeByDappeerList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByADONET = perfomanceTestByADONET.GetCustomersWithOrdersAsync().Result;
                            queryTimeByADONETList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();
                        }
                        break;
                    case "d": // Get customers with orders -  by projection
                        for (int i = 0; i < 100; i++)
                        {
                            stopWatch.Start();
                            var resultByEF = perfomanceTestByEF.GetCustomersWithOrdersAsync().Result;
                            queryTimeByEFList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByDappeer = perfomanceTestByDappeer.GetCustomersWithOrdersAsync().Result;
                            queryTimeByDappeerList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByADONET = perfomanceTestByADONET.GetCustomersWithOrdersAsync().Result;
                            queryTimeByADONETList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();
                        }
                        break;
                    case "e": // Get customers with orders - separated queries, manual mapping
                        for (int i = 0; i < 100; i++)
                        {
                            stopWatch.Start();
                            var resultByEF = perfomanceTestByEF.GetCustomersWithOrdersSeparatedAsync().Result;
                            queryTimeByEFList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByDappeer = perfomanceTestByDappeer.GetCustomersWithOrdersSeparatedAsync().Result;
                            queryTimeByDappeerList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            var resultByADONET = perfomanceTestByADONET.GetCustomersWithOrdersSeparatedAsync().Result;
                            queryTimeByADONETList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();
                        }
                        break;
                    case "f": // Insert customer
                        for (int i = 0; i < 10; i++)
                        {
                            stopWatch.Start();
                            perfomanceTestByEF.CreateCustomerAsync().Wait();
                            queryTimeByEFList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            perfomanceTestByDappeer.CreateCustomerAsync().Wait();
                            queryTimeByDappeerList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            perfomanceTestByADONET.CreateCustomerAsync().Wait();
                            queryTimeByADONETList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();
                        }
                        break;
                    case "g": // Update customer
                        for (int i = 0; i < 10; i++)
                        {
                            var customerId = unitOfWork.CustomerRepository.Table.OrderByDescending(x => x.Id).FirstOrDefaultAsync().Result.Id;
                            stopWatch.Start();
                            perfomanceTestByEF.UpdateCustomerAsync(customerId).Wait();
                            queryTimeByEFList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            perfomanceTestByDappeer.UpdateCustomerAsync(customerId).Wait();
                            queryTimeByDappeerList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            stopWatch.Start();
                            perfomanceTestByADONET.UpdateCustomerAsync(customerId).Wait();
                            queryTimeByADONETList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();
                        }
                        break;
                    case "h": // Delete customer
                        for (int i = 0; i < 100; i++)
                        {
                            var customer = unitOfWork.CustomerRepository.Table.OrderByDescending(x => x.Id).FirstOrDefaultAsync().Result;
                            stopWatch.Start();
                            perfomanceTestByEF.DeleteCustomerAsync(customer).Wait();
                            queryTimeByEFList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            customer = unitOfWork.CustomerRepository.Table.OrderByDescending(x => x.Id).FirstOrDefaultAsync().Result;
                            stopWatch.Start();
                            perfomanceTestByDappeer.DeleteCustomerAsync(customer.Id).Wait();
                            queryTimeByDappeerList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();

                            customer = unitOfWork.CustomerRepository.Table.OrderByDescending(x => x.Id).FirstOrDefaultAsync().Result;
                            stopWatch.Start();
                            perfomanceTestByADONET.DeleteCustomerAsync(customer.Id).Wait();
                            queryTimeByADONETList.Add(Math.Round(stopWatch.Elapsed.TotalMilliseconds, 2));
                            stopWatch.Reset();
                        }
                        break;
                    default:
                        continue;
                }

                double efResult = 0, dapperResult = 0, ADOResult = 0;
                if (queryTimeByEFList.Count() > 0)
                {
                    efResult = Math.Round(queryTimeByEFList.Average(), 2);
                }
                if (queryTimeByDappeerList.Count() > 0)
                {
                    dapperResult = Math.Round(queryTimeByDappeerList.Average(), 2);
                }
                if (queryTimeByADONETList.Count() > 0)
                {
                    ADOResult = Math.Round(queryTimeByADONETList.Average(), 2);
                }
                Console.WriteLine($"\n {operations.GetValueOrDefault(option)}. request time taken: \n\n  EF: {efResult} ms \n  Dapper: {dapperResult} ms \n  ADO.NET: {ADOResult} ms");
                stopWatch.Reset();
            }
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = _configuration.GetConnectionString("AppConnection");
            services.AddDbContext<AppDbContext>(options =>
                options
                    .UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
                    .UseSqlServer(connectionString, b => b.MigrationsAssembly("ORMPerformance")));

            services.Configure<ConnectionStringsOptions>(_configuration.GetSection(ConnectionStringsOptions.ConnectionStrings));
            services.AddAutoMapper(typeof(MappingProfile).GetTypeInfo().Assembly);
            services.AddTransient<AppDbContext, AppDbContext>();
            services.AddTransient<UnitOfWork, UnitOfWork>();
            services.AddTransient<EntityFrameworkPerfomanceTest>();
            services.AddTransient<DapperPerfomanceTest>();
            services.AddTransient<ADONETPerfomanceTest>();


            using (var serviceScope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
                {
                    context.Database.Migrate();
                    DataSeed.Seed(context).Wait();
                }
            }
        }
    }
}
