using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Infrastructure.Dtos.EntityDtos
{
    public class OrderStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
