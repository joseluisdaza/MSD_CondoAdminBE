using System;
using System.Collections.Generic;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Condominio.Data.MySql.Models;

public partial class CondominioContext : DbContext
{
    public CondominioContext()
    {
    }

    public CondominioContext(DbContextOptions<CondominioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyOwner> PropertyOwners { get; set; }

    public virtual DbSet<PropertyType> PropertyTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    // Nuevas entidades
    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<ExpensePayment> ExpensePayments { get; set; }

    public virtual DbSet<ServiceType> ServiceTypes { get; set; }

    public virtual DbSet<ServiceExpense> ServiceExpenses { get; set; }

    public virtual DbSet<ServicePayment> ServicePayments { get; set; }

    public virtual DbSet<ServiceExpensePayment> ServiceExpensePayments { get; set; }

    public virtual DbSet<DatabaseVersion> Versions { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //No implementation needed if using DI to provide connection string
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        // Payment Status
        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("payment_status");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.StatusDescription)
                .HasMaxLength(100)
                .HasColumnName("Status_Description");
        });

        // Expense Categories
        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("expense_categories");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Expenses - Actualizada
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("expenses");

            entity.HasIndex(e => e.CategoryId, "Category_Id");
            entity.HasIndex(e => e.PropertyId, "Property_Id");
            entity.HasIndex(e => e.StatusId, "Status_Id");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CategoryId).HasColumnName("Category_Id");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.InterestAmount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("Interest_Amount");
            entity.Property(e => e.InterestRate)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("Interest_Rate");
            entity.Property(e => e.PaymentLimitDate)
                .HasColumnType("datetime")
                .HasColumnName("Payment_Limit_Date");
            entity.Property(e => e.PropertyId).HasColumnName("Property_Id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.StatusId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Status_Id");

            entity.HasOne(d => d.Category).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expenses_ibfk_1");

            entity.HasOne(d => d.Property).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("expenses_ibfk_2");

            entity.HasOne(d => d.Status).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expenses_ibfk_3");
        });

        // Payments - Actualizada
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("payments");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("Payment_Date");
            entity.Property(e => e.ReceiveNumber)
                .HasMaxLength(100)
                .HasColumnName("Receive_Number");
            entity.Property(e => e.ReceivePhoto)
                .HasMaxLength(1000)
                .HasColumnName("Receive_Photo");
        });

        // Expense Payments - Nueva tabla intermedia
        modelBuilder.Entity<ExpensePayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("expense_payments");

            entity.HasIndex(e => e.ExpenseId, "Expense_Id");
            entity.HasIndex(e => e.PaymentId, "Payment_Id");

            entity.Property(e => e.ExpenseId).HasColumnName("Expense_Id");
            entity.Property(e => e.PaymentId).HasColumnName("Payment_Id");

            entity.HasOne(d => d.Expense).WithMany(p => p.ExpensePayments)
                .HasForeignKey(d => d.ExpenseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expense_payments_ibfk_1");

            entity.HasOne(d => d.Payment).WithMany(p => p.ExpensePayments)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expense_payments_ibfk_2");
        });

        // Service Types
        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("service_types");

            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .HasColumnName("Service_Name");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Service Expenses
        modelBuilder.Entity<ServiceExpense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("service_expenses");

            entity.HasIndex(e => e.ServiceTypeId, "Service_Type_Id");
            entity.HasIndex(e => e.StatusId, "Status_Id");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExpenseDate)
                .HasColumnType("datetime")
                .HasColumnName("Expense_Date");
            entity.Property(e => e.InterestAmount)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("Interest_Amount");
            entity.Property(e => e.PaymentLimitDate)
                .HasColumnType("datetime")
                .HasColumnName("Payment_Limit_Date");
            entity.Property(e => e.ServiceTypeId).HasColumnName("Service_Type_Id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.Status).HasDefaultValueSql("'1'");
            entity.Property(e => e.StatusId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Status_Id");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("Total_Amount");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.ServiceExpenses)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_expenses_ibfk_1");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.ServiceExpenses)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_expenses_ibfk_2");
        });

        // Service Payments
        modelBuilder.Entity<ServicePayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("service_payments");

            entity.HasIndex(e => e.StatusId, "Status_Id");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("Payment_Date");
            entity.Property(e => e.ReceiveNumber)
                .HasMaxLength(100)
                .HasColumnName("Receive_Number");
            entity.Property(e => e.ReceivePhoto)
                .HasMaxLength(1000)
                .HasColumnName("Receive_Photo");
            entity.Property(e => e.StatusId)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Status_Id");

            entity.HasOne(d => d.Status).WithMany(p => p.ServicePayments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_payments_ibfk_1");
        });

        // Service Expense Payments
        modelBuilder.Entity<ServiceExpensePayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("service_expense_payments");

            entity.HasIndex(e => e.ServiceExpenseId, "Service_Expense_Id");
            entity.HasIndex(e => e.PaymentId, "Payment_Id");

            entity.Property(e => e.ServiceExpenseId).HasColumnName("Service_Expense_Id");
            entity.Property(e => e.PaymentId).HasColumnName("Payment_Id");

            entity.HasOne(d => d.ServiceExpense).WithMany(p => p.ServiceExpensePayments)
                .HasForeignKey(d => d.ServiceExpenseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_expense_payments_ibfk_1");

            entity.HasOne(d => d.Payment).WithMany(p => p.ServiceExpensePayments)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_expense_payments_ibfk_2");
        });

        // Property
        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("property");

            entity.HasIndex(e => e.PropertyType, "Property_Type");

            entity.Property(e => e.Code).HasMaxLength(2);
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.LegalId)
                .HasMaxLength(100)
                .HasColumnName("Legal_Id");
            entity.Property(e => e.PropertyType).HasColumnName("Property_Type");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.Tower).HasMaxLength(20);

            entity.HasOne(d => d.PropertyTypeNavigation).WithMany(p => p.Properties)
                .HasForeignKey(d => d.PropertyType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("property_ibfk_1");
        });

        // Property Owner
        modelBuilder.Entity<PropertyOwner>(entity =>
        {
            entity.HasKey(e => new { e.PropertyId, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("property_owners");

            entity.HasIndex(e => e.UserId, "User_Id");

            entity.Property(e => e.PropertyId).HasColumnName("Property_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyOwners)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("property_owners_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.PropertyOwners)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("property_owners_ibfk_2");
        });

        // Property Type
        modelBuilder.Entity<PropertyType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("property_type");

            entity.Property(e => e.Bathrooms).HasDefaultValueSql("'0'");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.Rooms).HasDefaultValueSql("'0'");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.Type).HasMaxLength(100);
            entity.Property(e => e.WaterService)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Water_Service");
        });

        // Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("roles");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.RolName)
                .HasMaxLength(50)
                .HasColumnName("Rol_Name");
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "Login")
                .IsUnique();

            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("Last_Name");
            entity.Property(e => e.LegalId)
                .HasMaxLength(1000)
                .HasColumnName("Legal_Id");
            entity.Property(e => e.Login).HasMaxLength(30);
            entity.Property(e => e.Password)
                .HasMaxLength(1000)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("User_Name");
        });

        // User Role
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("user_roles");

            entity.HasIndex(e => e.UserId, "User_Id");

            entity.Property(e => e.RoleId).HasColumnName("Role_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_roles_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_roles_ibfk_2");
        });

        // Database Version
        modelBuilder.Entity<DatabaseVersion>(entity =>
        {
            entity.HasKey(e => e.VersionNumber).HasName("PRIMARY");
            entity.ToTable("versions");

            entity.Property(e => e.VersionNumber)
                .HasMaxLength(20)
                .HasColumnName("Version");
            entity.Property(e => e.LastUpdated)
                .HasColumnType("datetime")
                .HasColumnName("Last_Updated");
        });

        // Audit Log
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("audit_logs");

            entity.HasIndex(e => e.UserId, "idx_auditlogs_userid");
            entity.HasIndex(e => e.Timestamp, "idx_auditlogs_timestamp");
            entity.HasIndex(e => e.TableName, "idx_auditlogs_tablename");

            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .HasColumnName("Table_Name");
            entity.Property(e => e.Message).HasColumnType("TEXT");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("audit_logs_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
