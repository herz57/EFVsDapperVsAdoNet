using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using ORMPerformance.Data.Domain;
using ORMPerformance.Data.Enums;

namespace ORMPerformance.Data.DataSeed
{
    public class DataSeed
    {
        public static async Task Seed(AppDbContext context)
        {
            Random rand = new Random();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var generatedData = LoadJson("../../../mock_data.json");

            var fullNames = generatedData["full_names"];
            var userNames = generatedData["user_names"];
            var emails = generatedData["emails"];
            var phones = generatedData["phones"];
            var orderStatuses = generatedData["order_statuses"];
            var addresses = generatedData["addresses"];
            var cities = generatedData["cities"];
            var counties = generatedData["counties"];
            var countries = generatedData["countries"];
            var postCodes = generatedData["post_codes"];

            int orderStatusesCount = orderStatuses.Count();
            int customersCount = 100;
            int deliveryDetailsCount;
            int ordersCount = 10000;
            int cardsCount = 1000;

            if (!await context.Set<Customer>().AnyAsync())
            {
                var customersList = new List<Customer>();
                for (int i = 0; i < customersCount; i++)
                {
                    customersList.Add(new Customer
                    {
                        Name = fullNames[rand.Next(fullNames.Count())].ToString(),
                        ContactName = fullNames[rand.Next(fullNames.Count())].ToString(),
                        Email = emails[rand.Next(emails.Count())].ToString(),
                        ContactPhone = phones[rand.Next(phones.Count())].ToString()
                    });
                }

                await context.BulkInsertAsync(customersList);
            }

            if (!await context.Set<OrderStatus>().AnyAsync())
            {
                var orderStatusesList = new List<OrderStatus>();
                for (int i = 0; i < orderStatusesCount; i++)
                {
                    orderStatusesList.Add(new OrderStatus
                    {
                        Name = orderStatuses[i].ToString()
                    });
                }

                await context.BulkInsertAsync(orderStatusesList);
            }

            if (!await context.Set<Order>().AnyAsync())
            {
                var ordersList = new List<Order>();
                var customerIds = await context.Set<Customer>().Select(x => x.Id).ToListAsync();
                var orderStatusIds = await context.Set<OrderStatus>().Select(x => x.Id).ToListAsync();
                for (int i = 0; i < ordersCount; i++)
                {
                    ordersList.Add(new Order
                    {
                        Price = Math.Round(rand.Next(100, 900) + rand.NextDouble(), 1),
                        Currency = (Currency)rand.Next(2),
                        CustomerId = customerIds[rand.Next(customerIds.Count())],
                        OrderStatusId = orderStatusIds[rand.Next(orderStatusIds.Count())],
                    });
                }

                await context.BulkInsertAsync(ordersList);
            }

            if (!await context.Set<Card>().AnyAsync())
            {
                var cardsList = new List<Card>();
                var customerIds = await context.Set<Customer>().Select(x => x.Id).ToListAsync();
                for (int i = 0; i < cardsCount; i++)
                {
                    cardsList.Add(new Card
                    {
                        CustomerId = customerIds[rand.Next(customerIds.Count())],
                        Number = $"{rand.Next(1000, 9999)}{rand.Next(1000, 9999)}{rand.Next(1000, 9999)}{rand.Next(1000, 9999)}",
                        CCV = rand.Next(10, 1000),
                        Exp = DateTime.UtcNow
                    });
                }

                await context.BulkInsertAsync(cardsList);
            }

            Console.WriteLine(string.Format("\nSeeding has finished, time taken: {0}s", Math.Round(stopWatch.Elapsed.TotalSeconds, 1)));
        }

        private static JObject LoadJson(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string data = r.ReadToEnd();
                return JObject.Parse(data);
            }
        }
    }
}
