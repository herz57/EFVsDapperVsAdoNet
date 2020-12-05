using ORMPerformance.Data.Domain;
using ORMPerformance.Data.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.EntityConfigurations
{
    public class AuditLogEntityConfiguration : EntityTypeConfiguration<AuditLog>
    {
        public override void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(al => al.Id);

            builder.HasIndex(al => al.TableName);
            builder.HasIndex(al => al.Action);

            builder.Property(al => al.CreateDate).HasComputedColumnSql("getutcdate()").ValueGeneratedOnAdd();
            builder.Property(al => al.UpdateDate).HasComputedColumnSql("getutcdate()").ValueGeneratedOnUpdate();
        }
    }
}
