using ORMPerformance.Infrastructure.Dtos;
using ORMPerformance.Infrastructure.Dtos.EntityDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ORMPerformance.Services.Abstract
{
    public interface IORMPerformanceTest
    {
        Task<CustomerDto> GetCustomerByIdAsync(int customerId = 1);
        Task<List<OrderDto>> GetOrdersAsync(int size = 10000);
        Task<List<CustomerWithRelationsDto>> GetCustomersWithOrdersAsync();
        Task<List<CustomerDto>> GetCustomersWithOrdersSeparatedAsync();
        Task CreateCustomerAsync();
        Task UpdateCustomerAsync(int customerId);
    }
}
