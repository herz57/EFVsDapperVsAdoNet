using ORMPerformance.Data.Repository;
using ORMPerformance.Infrastructure.Dtos.EntityDtos;
using ORMPerformance.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using ORMPerformance.Data.Domain;
using ORMPerformance.Infrastructure.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Mapper;
using SqlMapper = Dapper.SqlMapper;
using ORMPerformance.Infrastructure.Dtos;

namespace ORMPerformance.Services
{
    public class DapperPerfomanceTest : IORMPerformanceTest
    {
        private readonly IOptions<ConnectionStringsOptions> _connectionOptions;
        public DapperPerfomanceTest(IOptions<ConnectionStringsOptions> connectionOptions)
        {
            _connectionOptions = connectionOptions;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int customerId = 1)
        {
            using (IDbConnection conn = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                var builder = new SqlBuilder();
                var query = builder.AddTemplate(@$"SELECT /**select**/ FROM [Customer] cus where cus.Id = @customerId");

                builder.Select("cus.Id");
                builder.Select("cus.Name");
                builder.Select("cus.ContactName");
                builder.Select("cus.Email");
                builder.Select("cus.ContactPhone");
                builder.Select("cus.CreateDate");
                builder.Select("cus.UpdateDate");

                return await conn.QuerySingleAsync<CustomerDto>(query.RawSql, new { customerId });
            }
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int size = 1000)
        {
            using (IDbConnection conn = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                var builder = new SqlBuilder();
                var query = builder.AddTemplate(@$"SELECT top({size}) /**select**/ FROM [Order] ord");

                builder.Select("ord.Id");
                builder.Select("ord.Price");
                builder.Select("ord.Currency");
                builder.Select("ord.CustomerId");
                builder.Select("ord.OrderStatusId");
                builder.Select("ord.CreateDate");
                builder.Select("ord.UpdateDate");

                var orders = (await conn.QueryAsync<OrderDto>(query.RawSql)).ToList();
                return orders;
            }
        }

        public async Task<List<CustomerWithRelationsDto>> GetCustomersWithOrdersAsync()
        {
            var customers = new List<CustomerWithRelationsDto>();
            using (IDbConnection conn = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                var builder = new SqlBuilder();
                var query = builder.AddTemplate(@$"SELECT /**select**/ FROM [Customer] cus /**innerjoin**/");

                builder.Select("cus.Id");
                builder.Select("cus.Name");
                builder.Select("cus.ContactName");
                builder.Select("cus.Email");
                builder.Select("cus.ContactPhone");
                builder.Select("cus.CreateDate");
                builder.Select("cus.UpdateDate");

                builder.Select("ord.Id");
                builder.Select("ord.Price");
                builder.Select("ord.Currency");
                builder.Select("ord.CustomerId");
                builder.Select("ord.OrderStatusId");
                builder.Select("ord.CreateDate");
                builder.Select("ord.UpdateDate");

                builder.InnerJoin("[Order] ord on cus.Id = ord.CustomerId");

                var customerDictionary = new Dictionary<int, CustomerWithRelationsDto>();
                customers = (await conn.QueryAsync<CustomerWithRelationsDto, OrderDto, CustomerWithRelationsDto>(query.RawSql, (cus, ord) =>
                {
                    CustomerWithRelationsDto customerEntry;
                    if (!customerDictionary.TryGetValue(cus.Id, out customerEntry))
                    {
                        customerEntry = cus;
                        customerEntry.OrdersDto = new List<OrderDto>();
                        customerDictionary.Add(cus.Id, customerEntry);
                    }

                    (customerEntry.OrdersDto as List<OrderDto>).Add(ord);
                    return customerEntry;
                }, splitOn: "Id")).ToList();

                return customers;
            }
        }

        public async Task<List<CustomerDto>> GetCustomersWithOrdersSeparatedAsync()
        {
            using (IDbConnection conn = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                var builder = new SqlBuilder();
                var query = builder.AddTemplate(@$"SELECT /**select**/ FROM [Customer] cus");

                builder.Select("cus.Id");
                builder.Select("cus.Name");
                builder.Select("cus.ContactName");
                builder.Select("cus.Email");
                builder.Select("cus.ContactPhone");
                builder.Select("cus.CreateDate");
                builder.Select("cus.UpdateDate");

                var customers = (await conn.QueryAsync<CustomerDto>(query.RawSql)).ToDictionary(x => x.Id);
                var customersIds = customers.Values.Select(x => x.Id).ToList();

                var orders = await conn.QueryAsync<OrderDto>(@"SELECT
                    ord.Id,
                    ord.Price,
                    ord.Currency,
                    ord.CustomerId,
                    ord.OrderStatusId,
                    ord.CreateDate,
                    ord.UpdateDate
                    FROM [Order] ord WHERE ord.CustomerId IN @customersIds", new { customersIds });

                foreach (var order in orders)
                {
                    CustomerDto customerEntry;
                    if (customers.TryGetValue(order.CustomerId, out customerEntry))
                    {
                        if (customerEntry.OrdersDto is null)
                        {
                            customerEntry.OrdersDto = new List<OrderDto>();
                        }
                        customerEntry.OrdersDto.Add(order);
                    }
                }

                var result = customers.Values.Select(x => x).ToList();
                return result;
            }
        }

        // insert
        public async Task CreateCustomerAsync()
        {
            using (IDbConnection conn = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                var customer = new Customer
                {
                    Name = "dapper insert",
                    ContactName = "dapper insert dapper insert dapper insert",
                    Email = "dapperinsert@dapper.dapper",
                    ContactPhone = "0503847839"
                };

                await conn.ExecuteAsync(@"INSERT INTO [Customer] (Name, ContactName, Email, ContactPhone)
                VALUES(@Name, @ContactName, @Email, @ContactPhone)", customer);
            }
        }

        // update
        public async Task UpdateCustomerAsync(int customerId)
        {
            using (IDbConnection conn = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                var customers = new Customer
                {
                    Id = customerId,
                    Name = "dapper update",
                    ContactName = "dapper update dapper update dapper update",
                    Email = "dapperupdate@dapper.dapper",
                    ContactPhone = "0503847839"
                };

                string sql = @"UPDATE [Customer] SET Name = @Name, ContactName = @ContactName, Email = @Email, ContactPhone = @ContactPhone
                        WHERE Id = @Id";

                await conn.ExecuteAsync(sql, customers);
            }
        }

        // delete
        public async Task DeleteCustomerAsync(int customerId)
        {
            using (IDbConnection conn = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                string sql = "DELETE FROM [Customer] WHERE Id = @customerId";
                await conn.ExecuteAsync(sql, new { customerId });
            }
        }
    }
}
