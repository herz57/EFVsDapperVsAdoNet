using ORMPerformance.Infrastructure.Dtos.EntityDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Infrastructure.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public List<OrderDto> OrdersDto { get; set; }
    }
}
