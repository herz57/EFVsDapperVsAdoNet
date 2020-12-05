using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.Domain.Logging
{
    public class AuditLogDetail : ModifyDate, IEntity<int>
    {
        public int Id { get; set; }
        public int AuditLogId { get; set; }
        public string ValueFrom { get; set; }
        public string ValueTo { get; set; }
        public string FieldName { get; set; }

        public AuditLog AuditLog { get; set; }
    }
}
