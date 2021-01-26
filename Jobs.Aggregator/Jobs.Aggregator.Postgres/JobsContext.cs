using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Jobs.Aggregator.Postgres
{
    public partial class JobsContext : DbContext
    {
        public JobsContext()
        {
        }

        public JobsContext(DbContextOptions<JobsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Jobs> Jobs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
// TODO
                optionsBuilder.UseNpgsql("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Jobs>(entity =>
            {
                entity.ToTable("jobs");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Company)
                    .HasColumnName("company")
                    .HasMaxLength(50);

                entity.Property(e => e.Scrapedate)
                    .HasColumnName("scrapedate")
                    .HasColumnType("date");

                entity.Property(e => e.Site)
                    .HasColumnName("site")
                    .HasMaxLength(50);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(500);

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(5000);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
