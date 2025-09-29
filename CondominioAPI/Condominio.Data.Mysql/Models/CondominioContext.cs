using System;
using System.Collections.Generic;
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //No implementation needed if using DI to provide connection string
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("expenses");

            entity.HasIndex(e => e.CategoryId, "Category_Id");

            entity.HasIndex(e => e.PropertyId, "Property_Id");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CategoryId).HasColumnName("Category_Id");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExpenseDate)
                .HasColumnType("datetime")
                .HasColumnName("Expense_Date");
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
            entity.Property(e => e.ReceiveNumber)
                .HasMaxLength(100)
                .HasColumnName("Receive_Number");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.Status).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.Category).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expenses_ibfk_1");

            entity.HasOne(d => d.Property).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("expenses_ibfk_2");
        });

        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("expense_categories");

            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.ExpenseId, "Expense_Id");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExpenseId).HasColumnName("Expense_Id");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("Payment_Date");
            entity.Property(e => e.ReceivePhoto)
                .HasMaxLength(1000)
                .HasColumnName("Receive_Photo");

            entity.HasOne(d => d.Expense).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ExpenseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_ibfk_1");
        });

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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.RolName)
                .HasMaxLength(50)
                .HasColumnName("Rol_Name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("Last_Name");
            entity.Property(e => e.LegalId)
                .HasMaxLength(1000)
                .HasColumnName("Legal_Id");
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
