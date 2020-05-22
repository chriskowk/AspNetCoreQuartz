using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TB.AspNetCore.Data.Entity
{
    public class SchedulerDbContext : DbContext
    {
        public SchedulerDbContext(DbContextOptions<SchedulerDbContext> options)
            : base(options)
        {
        }

        public DbSet<ScheduleInfo> ScheduleInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.JobGroup).IsRequired().HasMaxLength(100).HasDefaultValue(string.Empty);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.JobName).IsRequired().HasMaxLength(100).HasDefaultValue(string.Empty);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.RunStatus).IsRequired().HasDefaultValue(0);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.CronExpress).IsRequired().HasMaxLength(2000).HasDefaultValue(string.Empty);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.Creator).IsRequired().HasMaxLength(100).HasDefaultValue(string.Empty);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.InterfaceCode).IsRequired().HasMaxLength(100).HasDefaultValue(string.Empty);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.ServiceCode).IsRequired().HasMaxLength(100).HasDefaultValue(string.Empty);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.TaskDescription).IsRequired().HasMaxLength(100).HasDefaultValue(string.Empty);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.Token).IsRequired().HasMaxLength(500).HasDefaultValue(string.Empty);
            modelBuilder.Entity<ScheduleInfo>()
                .Property(x => x.AppId).IsRequired().HasMaxLength(100).HasDefaultValue(string.Empty);
        }
    }
}
