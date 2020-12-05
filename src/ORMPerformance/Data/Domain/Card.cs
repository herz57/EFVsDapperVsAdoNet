using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.Domain
{
    public class Card : ModifyDate, IEntity<int>
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string Number { get; set; }
        public int CCV { get; set; }
        public DateTime Exp { get; set; }
    }
}
