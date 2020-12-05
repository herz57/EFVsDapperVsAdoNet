using ORMPerformance.Data.Domain;
using ORMPerformance.Infrastructure.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.EntityConfigurations
{
    public class OrderEntityConfiguration : EntityTypeConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasIndex(o => o.Price);
            builder.HasIndex(o => o.Currency);

            builder.Property(o => o.CreateDate).HasDefaultValueSql("getutcdate()").ValueGeneratedOnAdd();
            builder.Property(o => o.UpdateDate).HasDefaultValueSql("getutcdate()").ValueGeneratedOnAddOrUpdate();

            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.OrderStatus)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
