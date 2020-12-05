using ORMPerformance.Data.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Infrastructure.Dtos.EntityDtos
{
    public class CardDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Number { get; set; }
        public int CCV { get; set; }
        public DateTime Exp { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
