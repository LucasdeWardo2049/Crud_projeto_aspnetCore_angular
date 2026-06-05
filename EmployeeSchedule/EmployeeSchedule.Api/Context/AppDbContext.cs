using EmployeeSchedule.Api.Modules.EmployeeModule.Entity;
using EmployeeSchedule.Api.Modules.ScheduleModule.Entity;
using EmployeeSchedule.Api.Modules.TaskModule.Entity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSchedule.Api.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Schedule> Schedules => Set<Schedule>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");

            entity.HasKey(employee => employee.Id);

            entity.Property(employee => employee.Id)
                .HasColumnName("id");

            entity.Property(employee => employee.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(employee => employee.Registration)
                .HasColumnName("registration")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(employee => employee.Department)
                .HasColumnName("department")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(employee => employee.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.Property(employee => employee.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            entity.HasIndex(employee => employee.Registration)
                .IsUnique();
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("schedules");

            entity.HasKey(schedule => schedule.Id);

            entity.Property(schedule => schedule.Id)
                .HasColumnName("id");

            entity.Property(schedule => schedule.EmployeeId)
                .HasColumnName("employee_id")
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

            entity.HasIndex(schedule => schedule.EmployeeId);

            entity.HasOne(schedule => schedule.Employee)
                .WithMany(employee => employee.Schedules)
                .HasForeignKey(schedule => schedule.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");

            entity.HasKey(task => task.Id);

            entity.Property(task => task.Id)
                .HasColumnName("id");

            entity.Property(task => task.EmployeeId)
                .HasColumnName("employee_id")
                .IsRequired();

            entity.Property(task => task.Title)
                .HasColumnName("title")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(task => task.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            entity.Property(task => task.IsDone)
                .HasColumnName("is_done")
                .IsRequired();

            entity.Property(task => task.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(task => task.CompletedAt)
                .HasColumnName("completed_at")
                .HasColumnType("timestamp with time zone");

            entity.HasIndex(task => task.EmployeeId);

            entity.HasOne(task => task.Employee)
                .WithMany(employee => employee.Tasks)
                .HasForeignKey(task => task.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
