using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Schedule> Schedules => Set<Schedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("schedules");

            entity.HasKey(schedule => schedule.Id);

            entity.Property(schedule => schedule.Id)
                .HasColumnName("id");

            entity.Property(schedule => schedule.EmployeeName)
                .HasColumnName("employee_name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(schedule => schedule.EmployeeRegistration)
                .HasColumnName("employee_registration")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(schedule => schedule.Department)
                .HasColumnName("department")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(schedule => schedule.ShiftName)
                .HasColumnName("shift_name")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(schedule => schedule.StartTime)
                .HasColumnName("start_time")
                .IsRequired();

            entity.Property(schedule => schedule.EndTime)
                .HasColumnName("end_time")
                .IsRequired();

            entity.Property(schedule => schedule.WorkDate)
                .HasColumnName("work_date")
                .IsRequired();

            entity.Property(schedule => schedule.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(schedule => schedule.Notes)
                .HasColumnName("notes")
                .HasMaxLength(500);

            entity.Property(schedule => schedule.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(schedule => schedule.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp with time zone");
        });
    }
}
