using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UdritDhakal_SMS.Models.Entity;

namespace UdritDhakal_SMS.Infrastructure.EntityConfiguration
{
    public class StudentConfiguration : IEntityTypeConfiguration<StudentInfo>
    {
        public void Configure(EntityTypeBuilder<StudentInfo> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
               .ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(true);

            builder.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(true);

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(true);

            builder.Property(e => e.Gender)
                .HasMaxLength(30)
                .IsUnicode(true);

            builder.Property(e => e.Address)
                .HasMaxLength(30)
               .IsUnicode(true);

            builder.Property(e => e.Class)
                .HasMaxLength(30)
               .IsUnicode(true);

            builder.Property(e => e.Section)
                .HasMaxLength(30)
               .IsUnicode(true);

            builder.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(true);

            builder.HasOne(e => e.CourseInfo)
            .WithMany(pt => pt.StudentInfos)
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
