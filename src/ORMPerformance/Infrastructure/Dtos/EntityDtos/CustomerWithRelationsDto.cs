using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Infrastructure.Dtos.EntityDtos
{
    public class CustomerWithRelationsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public IEnumerable<OrderDto> OrdersDto { get; set; }
        public List<CardDto> CardsDto { get; set; }
    }
}
