using ORMPerformance.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ORMPerformance.Data.Domain
{
    public class Order : ModifyDate, IEntity<int>
    {
        public int Id { get; set; }
        public int DeliveryDetailId { get; set; }
        public double Price { get; set; }
        public Currency Currency { get; set; }
        public int CustomerId { get; set; }
        public int OrderStatusId { get; set; }

        public Customer Customer { get; set; }
        public OrderStatus OrderStatus { get; set; }

    }
}

