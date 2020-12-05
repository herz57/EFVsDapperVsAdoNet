using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.Domain
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
