using HRMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Data
{
    public class HRMSDbContext : DbContext
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTransferHistory> EmployeeTransferHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee -> Department
            modelBuilder.Entity<Employee>()
                   .HasOne(e => e.Department)
                   .WithMany(d => d.Employees)
                   .HasForeignKey(e => e.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Constraint Department Name Unique
            modelBuilder.Entity<Department>()
                .HasIndex(d => d.DepartmentName)
                .IsUnique();

            // Employee -> Transfer History
            modelBuilder.Entity<EmployeeTransferHistory>()
                .HasOne(th => th.Employee)
                .WithMany(e => e.TransferHistories)
                .HasForeignKey(th => th.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // From Department -> Transfer History
            modelBuilder.Entity<EmployeeTransferHistory>()
                .HasOne(th => th.FromDepartment)
                .WithMany(d => d.TransfersFrom)
                .HasForeignKey(th => th.FromDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // To Department -> Transfer History
            modelBuilder.Entity<EmployeeTransferHistory>()
                .HasOne(th => th.ToDepartment)
                .WithMany(d => d.TransfersTo)
                .HasForeignKey(th => th.ToDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
