using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.Domain
{
    public class Customer : ModifyDate, IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }

        private ICollection<Order> _orders;
        private ICollection<Card> _cards;

        public virtual ICollection<Order> Orders
        {
            get => _orders ??= new List<Order>();
            set => _orders = value;
        }

        public virtual ICollection<Card> Cards
        {
            get => _cards ??= new List<Card>();
            set => _cards = value;
        }
    }
}
