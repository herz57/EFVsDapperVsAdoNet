using AutoMapper;
using AutoMapper.QueryableExtensions;
using ORMPerformance.Data.UnitOfWork;
using ORMPerformance.Infrastructure.Dtos.EntityDtos;
using ORMPerformance.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORMPerformance.Data.Domain;
using ORMPerformance.Infrastructure.Dtos;

namespace ORMPerformance.Services
{
    public class EntityFrameworkPerfomanceTest : IORMPerformanceTest
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EntityFrameworkPerfomanceTest(UnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // get customer by id
        public async Task<CustomerDto> GetCustomerByIdAsync(int customerId = 1)
        {
            return await _unitOfWork.CustomerRepository.TableNoTracking
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(x => x.Id == customerId);
        }

        public async Task<CustomerDto> FindCustomerByIdAsync(int customerId = 1)
        {
            var res = await _unitOfWork.CustomerRepository
                .FindByIdAsync(customerId);

            return _mapper.Map<CustomerDto>(res);
        }

        // get orders
        public async Task<List<OrderDto>> GetOrdersAsync(int size = 1000)
        {
            return await _unitOfWork.OrderRepository.TableNoTracking
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .Take(size)
                .ToListAsync();
        }

        // get customers with orders - include
        public async Task<List<CustomerWithRelationsDto>> GetCustomersWithOrdersByIncludeAsync()
        {
            var result = await _unitOfWork.CustomerRepository.TableNoTracking
                .Include(x => x.Orders)
                .ToListAsync();

            return _mapper.Map<List<CustomerWithRelationsDto>>(result);
        }

        // get customers with orders - projection (project to)
        public async Task<List<CustomerWithRelationsDto>> GetCustomersWithOrdersAsync()
        {
            return await _unitOfWork.CustomerRepository.TableNoTracking
                .ProjectTo<CustomerWithRelationsDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // get customers with orders - projection (select)
        public async Task<List<CustomerWithRelationsDto>> GetCustomersWithOrdersBySelectAsync()
        {
            return await _unitOfWork.CustomerRepository.TableNoTracking
                .Select(x => new CustomerWithRelationsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    ContactName = x.ContactName,
                    ContactPhone = x.ContactPhone,
                    CreateDate = x.CreateDate,
                    UpdateDate = x.UpdateDate,
                    OrdersDto = x.Orders.Select(x => new OrderDto
                    {
                        Id = x.Id,
                        Price = x.Price,
                        Currency = x.Currency,
                        OrderStatusId = x.OrderStatusId,
                        CreateDate = x.CreateDate,
                        UpdateDate = x.UpdateDate,
                        CustomerId = x.CustomerId,
                    })
                })
                .ToListAsync();
        }

        // get customers with orders - separated queries
        public async Task<List<CustomerDto>> GetCustomersWithOrdersSeparatedAsync()
        {
            var customers = await _unitOfWork.CustomerRepository.TableNoTracking
                .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                .ToDictionaryAsync(x => x.Id);

            var customersIds = customers.Values.Select(x => x.Id).ToList();

            var orders = await _unitOfWork.OrderRepository.TableNoTracking
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .Where(x => customersIds.Contains(x.CustomerId))
                .ToListAsync();

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

        // insert
        public async Task CreateCustomerAsync()
        {
            var customer = new Customer
            {
                Name = "ef insert",
                ContactName = "ef insert ef insert ef insert",
                Email = "efinsert@ef.ef",
                ContactPhone = "05038478393"
            };

            await _unitOfWork.CustomerRepository.InsertAsync(customer);
        }

        // update
        public async Task UpdateCustomerAsync(int customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.Table
                .SingleOrDefaultAsync(x => x.Id == customerId);

            customer.Name = "ef update";

            await _unitOfWork.CustomerRepository.UpdateAsync(customer);
        }

        // delete 
        public async Task DeleteCustomerAsync(Customer customer)
        {
            await _unitOfWork.CustomerRepository.DeleteAsync(customer);
        }
    }
}
