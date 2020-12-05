using ORMPerformance.Data.Domain;
using ORMPerformance.Infrastructure.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMPerformance.Data.EntityConfigurations
{
    public class CardEntityConfiguration : EntityTypeConfiguration<Card>
    {
        public override void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasKey(os => os.Id);

            builder.Property(os => os.CreateDate).HasDefaultValueSql("getutcdate()").ValueGeneratedOnAdd();
            builder.Property(os => os.UpdateDate).HasDefaultValueSql("getutcdate()").ValueGeneratedOnAddOrUpdate();

            builder.HasOne(o => o.Customer)
               .WithMany(o => o.Cards)
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
