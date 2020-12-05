using ORMPerformance.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Infrastructure.Dtos.EntityDtos
{
    public class OrderWithRelationsDto
    {
        public int Id { get; set; }
        public int DeliveryDetailId { get; set; }
        public double Price { get; set; }
        public Currency Currency { get; set; }
        public int CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public CustomerDto CustomerDto { get; set; }
    }
}
