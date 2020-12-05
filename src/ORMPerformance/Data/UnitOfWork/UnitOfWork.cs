using ORMPerformance.Data;
using ORMPerformance.Data.Domain;
using ORMPerformance.Data.Repository;
using ORMPerformance.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;

namespace ORMPerformance.Data.UnitOfWork
{
    public class UnitOfWork
    {
        public readonly AppDbContext Context;

        private EfRepository<Customer> _customerRepository;
        private EfRepository<Order> _orderRepository;
        private EfRepository<OrderStatus> _orderStatusRepository;

        private bool disposed;

        public UnitOfWork(AppDbContext context)
        {
            Context = context;
        }

        public EfRepository<Customer> CustomerRepository
        {
            get
            {
                if (_customerRepository == null)
                    _customerRepository = new EfRepository<Customer>(Context);
                return _customerRepository;
            }
        }

        public EfRepository<Order> OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new EfRepository<Order>(Context);
                return _orderRepository;
            }
        }

        public EfRepository<OrderStatus> OrderStatusRepository
        {
            get
            {
                if (_orderStatusRepository == null)
                    _orderStatusRepository = new EfRepository<OrderStatus>(Context);
                return _orderStatusRepository;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            disposed = true;
        }
    }
}