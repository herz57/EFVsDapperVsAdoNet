using ORMPerformance.Data.Domain.Logging;
using ORMPerformance.Infrastructure.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.EntityConfigurations
{
    public class AuditLogDetailEntityConfiguration : EntityTypeConfiguration<AuditLogDetail>
    {
        public override void Configure(EntityTypeBuilder<AuditLogDetail> builder)
        {
            builder.HasKey(ald => ald.Id);

            builder.HasIndex(ald => ald.ValueFrom);
            builder.HasIndex(ald => ald.ValueTo);
            builder.HasIndex(ald => ald.FieldName);

            builder.Property(ald => ald.CreateDate).HasComputedColumnSql("getutcdate()").ValueGeneratedOnAdd();
            builder.Property(ald => ald.UpdateDate).HasComputedColumnSql("getutcdate()").ValueGeneratedOnUpdate();

            builder.HasOne(ald => ald.AuditLog)
                .WithMany(al => al.AuditLogDetails)
                .HasForeignKey(ald => ald.AuditLogId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
