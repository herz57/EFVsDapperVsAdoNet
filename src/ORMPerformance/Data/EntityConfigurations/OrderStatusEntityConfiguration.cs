using ORMPerformance.Data.Domain;
using ORMPerformance.Infrastructure.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.EntityConfigurations
{
    public class OrderStatusEntityConfiguration : EntityTypeConfiguration<OrderStatus>
    {
        public override void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.HasKey(os => os.Id);

            builder.HasIndex(os => os.Name);

            builder.Property(os => os.CreateDate).HasDefaultValueSql("getutcdate()").ValueGeneratedOnAdd();
            builder.Property(os => os.UpdateDate).HasDefaultValueSql("getutcdate()").ValueGeneratedOnAddOrUpdate();
        }
    }
}
