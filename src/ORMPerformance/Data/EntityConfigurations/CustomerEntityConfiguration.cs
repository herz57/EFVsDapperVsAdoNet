using ORMPerformance.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.EntityConfigurations
{
    public class CustomerEntityConfiguration : EntityTypeConfiguration<Customer>
    {
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.ContactName);
            builder.HasIndex(c => c.Email);
            builder.HasIndex(c => c.ContactPhone);

            builder.Property(c => c.CreateDate).HasDefaultValueSql("getutcdate()").ValueGeneratedOnAdd();
            builder.Property(c => c.UpdateDate).HasDefaultValueSql("getutcdate()").ValueGeneratedOnAddOrUpdate();
        }
    }
}
