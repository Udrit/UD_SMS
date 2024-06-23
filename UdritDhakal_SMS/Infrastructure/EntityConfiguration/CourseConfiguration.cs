using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UdritDhakal_SMS.Models.Entity;

namespace UdritDhakal_SMS.Infrastructure.EntityConfiguration
{
    public class CourseConfiguration : IEntityTypeConfiguration<CourseInfo>
    {
        public void Configure(EntityTypeBuilder<CourseInfo> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
               .ValueGeneratedOnAdd();

            builder.Property(e => e.CourseName)
                .HasMaxLength(200)
                .IsUnicode(true);

            builder.Property(e => e.CourseDescription)
                .HasMaxLength(500)
                .IsUnicode(true);

            builder.HasMany(e => e.StudentInfos)
                .WithOne(pt => pt.CourseInfo)
                .HasForeignKey(e => e.CourseId);

            builder.Property(e => e.IsActive)
           .HasDefaultValue(true);

            builder.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.CreatedBy)
                .IsRequired()
                .IsUnicode(true);
            builder.Property(e => e.ModifiedDate)
                .HasColumnType("datetime");

            builder.Property(e => e.ModifiedBy)
                .IsUnicode(true);


        }
    }
}
