using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.Domain.Logging
{
    public class AuditLog : ModifyDate, IEntity<int>
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public int CreateUserId { get; set; }
        public int UpdateUserId { get; set; }


        private ICollection<AuditLogDetail> _auditLogDetails;

        public virtual ICollection<AuditLogDetail> AuditLogDetails
        {
            get => _auditLogDetails ??= new List<AuditLogDetail>();
            set => _auditLogDetails = value;
        }
    }
}
