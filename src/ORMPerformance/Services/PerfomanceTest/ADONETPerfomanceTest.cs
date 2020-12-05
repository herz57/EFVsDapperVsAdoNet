using Dapper;
using ORMPerformance.Infrastructure.Dtos.EntityDtos;
using ORMPerformance.Infrastructure.Options;
using ORMPerformance.Services.Abstract;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using ORMPerformance.Infrastructure.Dtos;
using ORMPerformance.Data.Enums;
using System.Data;
using System.Threading.Tasks;
using ORMPerformance.Data.Domain;
using System.Linq;

namespace ORMPerformance.Services.PerfomanceTest
{
    public class ADONETPerfomanceTest : IORMPerformanceTest
    {
        private readonly IOptions<ConnectionStringsOptions> _connectionOptions;
        public ADONETPerfomanceTest(IOptions<ConnectionStringsOptions> connectionOptions)
        {
            _connectionOptions = connectionOptions;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int customerId = 1)
        {
            CustomerDto customer = new CustomerDto();
            var builder = new SqlBuilder();
            var query = builder.AddTemplate(@$"SELECT /**select**/ FROM [Customer] cus /**where**/");

            builder.Select("cus.Id");
            builder.Select("cus.Name");
            builder.Select("cus.ContactName");
            builder.Select("cus.Email");
            builder.Select("cus.ContactPhone");
            builder.Select("cus.CreateDate");
            builder.Select("cus.UpdateDate");

            builder.Where("cus.Id = @customerId");

            using (SqlConnection connection = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query.RawSql, connection);
                command.Parameters.Add(new SqlParameter("@customerId", customerId));
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        customer = new CustomerDto
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            ContactName = reader.GetString(2),
                            Email = reader.GetString(3),
                            ContactPhone = reader.GetString(4),
                            CreateDate = reader.GetDateTime(5),
                            UpdateDate = reader.GetDateTime(6)
                        };
                    }
                }

                reader.Close();
                return customer;
            }
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int size = 1000)
        {
            var orders = new List<OrderDto>();
            var builder = new SqlBuilder();
            var query = builder.AddTemplate(@$"SELECT top({size}) /**select**/ FROM [Order] ord");

            builder.Select("ord.Id");
            builder.Select("ord.Price");
            builder.Select("ord.Currency");
            builder.Select("ord.CustomerId");
            builder.Select("ord.OrderStatusId");
            builder.Select("ord.CreateDate");
            builder.Select("ord.UpdateDate");

            using (SqlConnection connection = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                connection.Open();
                SqlCommand commandOrders = new SqlCommand(query.RawSql, connection);
                SqlDataReader reader = await commandOrders.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        orders.Add(new OrderDto
                        {
                            Id = reader.GetInt32(0),
                            Price = reader.GetDouble(1),
                            Currency = (Currency)reader.GetInt32(2),
                            CustomerId = reader.GetInt32(3),
                            OrderStatusId = reader.GetInt32(4),
                            CreateDate = reader.GetDateTime(5),
                            UpdateDate = reader.GetDateTime(6)
                        });
                    }

                    reader.Close();
                }
                return orders;
            }
        }

        public async Task<List<CustomerWithRelationsDto>> GetCustomersWithOrdersAsync()
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

            using (SqlConnection connection = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                connection.Open();

                SqlCommand commandCustomer = new SqlCommand(query.RawSql, connection);
                SqlDataReader reader = await commandCustomer.ExecuteReaderAsync();

                var customers = new List<CustomerWithRelationsDto>();
                var customerDictionary = new Dictionary<int, CustomerWithRelationsDto>();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        CustomerWithRelationsDto customerEntry;
                        if (!customerDictionary.TryGetValue(reader.GetInt32(0), out customerEntry))
                        {
                            customerEntry = new CustomerWithRelationsDto
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ContactName = reader.GetString(2),
                                Email = reader.GetString(3),
                                ContactPhone = reader.GetString(4),
                                CreateDate = reader.GetDateTime(5),
                                UpdateDate = reader.GetDateTime(6)
                            };
                            customerEntry.OrdersDto = new List<OrderDto>();
                            customerDictionary.Add(reader.GetInt32(0), customerEntry);
                        }
                        (customerEntry.OrdersDto as List<OrderDto>).Add(new OrderDto
                        {
                            Id = reader.GetInt32(7),
                            Price = reader.GetDouble(8),
                            Currency = (Currency)reader.GetInt32(9),
                            CustomerId = reader.GetInt32(10),
                            OrderStatusId = reader.GetInt32(11),
                            CreateDate = reader.GetDateTime(12),
                            UpdateDate = reader.GetDateTime(13)
                        });
                    }

                    customers = customerDictionary.Values.Select(x => x).ToList();
                }

                reader.Close();
                return customers;
            }
        }

        public async Task<List<CustomerDto>> GetCustomersWithOrdersSeparatedAsync()
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

            using (SqlConnection connection = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                connection.Open();
                SqlCommand commandCustomers = new SqlCommand(query.RawSql, connection);
                SqlDataReader readerCustomer = await commandCustomers.ExecuteReaderAsync();

                var customers = new Dictionary<int, CustomerDto>();
                if (readerCustomer.HasRows)
                {
                    while (readerCustomer.Read())
                    {
                        customers.Add(readerCustomer.GetInt32(0), new CustomerDto
                        {
                            Id = readerCustomer.GetInt32(0),
                            Name = readerCustomer.GetString(1),
                            ContactName = readerCustomer.GetString(2),
                            Email = readerCustomer.GetString(3),
                            ContactPhone = readerCustomer.GetString(4),
                            CreateDate = readerCustomer.GetDateTime(5),
                            UpdateDate = readerCustomer.GetDateTime(6)
                        });
                    }
                }

                var customersIds = customers.Values.Select(x => x.Id).ToList();

                SqlCommand commandOrders = new SqlCommand($@"SELECT
                    ord.Id,
                    ord.Price,
                    ord.Currency,
                    ord.CustomerId,
                    ord.OrderStatusId,
                    ord.CreateDate,
                    ord.UpdateDate
                    FROM [Order] ord WHERE ord.CustomerId IN ({string.Join(',', customersIds)})", connection);

                SqlDataReader readerOrders = await commandOrders.ExecuteReaderAsync();
                List<OrderDto> orders = new List<OrderDto>();

                if (readerOrders.HasRows)
                {
                    while (readerOrders.Read())
                    {
                        orders.Add(new OrderDto
                        {
                            Id = readerOrders.GetInt32(0),
                            Price = readerOrders.GetDouble(1),
                            Currency = (Currency)readerOrders.GetInt32(2),
                            CustomerId = readerOrders.GetInt32(3),
                            OrderStatusId = readerOrders.GetInt32(4),
                            CreateDate = readerOrders.GetDateTime(5),
                            UpdateDate = readerOrders.GetDateTime(6)
                        });
                    }
                }

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

                readerOrders.Close();
                return result;
            }
        }

        // insert
        public async Task CreateCustomerAsync()
        {
            var customer = new Customer
            {
                Name = "ado create",
                ContactName = "ado create ado create ado create",
                Email = "adocreate@ado.ado",
                ContactPhone = "0503847839"
            };

            string sql = @"INSERT INTO [Customer] (Name, ContactName, Email, ContactPhone)
                VALUES(@Name, @ContactName, @Email, @ContactPhone)";

            using (SqlConnection connection = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@ContactName", customer.ContactName);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@ContactPhone", customer.ContactPhone);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateCustomerAsync(int customerId)
        {
            var customer = new Customer
            {
                Id = customerId,
                Name = "ado update",
                ContactName = "ado update ado update ado update",
                Email = "adoupdate@ado.ado",
                ContactPhone = "0503847839"
            };

            string sql = @"UPDATE [Customer] SET Name = @Name, ContactName = @ContactName, Email = @Email, ContactPhone = @ContactPhone
                        WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", customer.Id);
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@ContactName", customer.ContactName);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@ContactPhone", customer.ContactPhone);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            string sql = @"DELETE FROM [Customer] WHERE Id = @customerId";

            using (SqlConnection connection = new SqlConnection(_connectionOptions.Value.AppConnection))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@customerId", customerId));
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
