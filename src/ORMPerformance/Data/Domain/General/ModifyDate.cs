using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ORMPerformance.Data.Domain
{
    public class ModifyDate
    {
        public DateTime? CreateDate { get; }
        public DateTime? UpdateDate { get; }
    }
}
