using System;
using System.Collections.Generic;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Condominio.Data.Mysql.Models;

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

    // Nuevas entidades v2.0 - Resources e Incidents
    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<ResourceCost> ResourceCosts { get; set; }

    public virtual DbSet<ResourceBooking> ResourceBookings { get; set; }

    public virtual DbSet<IncidentType> IncidentTypes { get; set; }

    public virtual DbSet<IncidentCost> IncidentCosts { get; set; }

    public virtual DbSet<Incident> Incidents { get; set; }

    // Reports - v2.1
    public virtual DbSet<Style> Styles { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportRole> ReportRoles { get; set; }

    public virtual DbSet<ReportHeader> ReportHeaders { get; set; }

    public virtual DbSet<ReportSection> ReportSections { get; set; }

    public virtual DbSet<ReportFooter> ReportFooters { get; set; }

    public virtual DbSet<ReportAudit> ReportAudits { get; set; }

    public virtual DbSet<ReportParam> ReportParams { get; set; }

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

        // Resources - v2.0
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("resources");

            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.Photo).HasMaxLength(1000);
        });

        // Resource Costs - v2.0
        modelBuilder.Entity<ResourceCost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("resource_costs");

            entity.HasIndex(e => e.ResourceId, "Resource_Id");

            entity.Property(e => e.ResourceId).HasColumnName("Resource_Id");
            entity.Property(e => e.BookingPrice)
                .HasPrecision(10, 2)
                .HasColumnName("Booking_Price");
            entity.Property(e => e.BookingWarrantyCost)
                .HasPrecision(10, 2)
                .HasColumnName("Booking_Warranty_Cost");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");

            entity.HasOne(d => d.Resource)
                .WithMany(p => p.ResourceCosts)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resource_costs_ibfk_1");
        });

        // Resource Bookings - v2.0
        modelBuilder.Entity<ResourceBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("resource_bookings");

            entity.HasIndex(e => e.ResourceId, "Resource_Id");
            entity.HasIndex(e => e.UserId, "User_Id");
            entity.HasIndex(e => e.PropertyId, "Property_Id");
            entity.HasIndex(e => e.StatusId, "Status_Id");

            entity.Property(e => e.ResourceId).HasColumnName("Resource_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.PropertyId).HasColumnName("Property_Id");
            entity.Property(e => e.StatusId).HasColumnName("Status_Id");
            entity.Property(e => e.BookingDate)
                .HasColumnType("datetime")
                .HasColumnName("Booking_Date");
            entity.Property(e => e.BookingEndDate)
                .HasColumnType("datetime")
                .HasColumnName("Booking_End_Date")
                .IsRequired(false); // Allow NULL values
            entity.Property(e => e.BookingPrice)
                .HasPrecision(10, 2)
                .HasColumnName("Booking_Price");
            entity.Property(e => e.BookingWarrantyCost)
                .HasPrecision(10, 2)
                .HasColumnName("Booking_Warranty_Cost");
            entity.Property(e => e.BookingDescription)
                .HasMaxLength(500)
                .HasColumnName("Booking_Description");
            entity.Property(e => e.BookingPhoto)
                .HasMaxLength(1000)
                .HasColumnName("Booking_Photo");

            entity.HasOne(d => d.Resource)
                .WithMany(p => p.ResourceBookings)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resource_bookings_ibfk_1");

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resource_bookings_ibfk_2");

            entity.HasOne(d => d.Property)
                .WithMany()
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resource_bookings_ibfk_3");

            entity.HasOne(d => d.Status)
                .WithMany()
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resource_bookings_ibfk_4");
        });

        // Incident Types - v2.0
        modelBuilder.Entity<IncidentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("incident_types");

            entity.Property(e => e.Type).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Incident Costs - v2.0
        modelBuilder.Entity<IncidentCost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("incident_costs");

            entity.HasIndex(e => e.IncidentTypeId, "Incident_Type_Id");

            entity.Property(e => e.IncidentTypeId).HasColumnName("Incident_Type_Id");
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(d => d.IncidentType)
                .WithMany(p => p.IncidentCosts)
                .HasForeignKey(d => d.IncidentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incident_costs_ibfk_1");
        });

        // Incidents - v2.0
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("incidents");

            entity.HasIndex(e => e.IncidentTypeId, "Incident_Type_Id");
            entity.HasIndex(e => e.UserId, "User_Id");
            entity.HasIndex(e => e.PropertyId, "Property_Id");
            entity.HasIndex(e => e.StatusId, "Status_Id");

            entity.Property(e => e.IncidentTypeId).HasColumnName("Incident_Type_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.PropertyId).HasColumnName("Property_Id");
            entity.Property(e => e.StatusId).HasColumnName("Status_Id");
            entity.Property(e => e.IncidentDate)
                .HasColumnType("datetime")
                .HasColumnName("Incident_Date");
            entity.Property(e => e.IncidentDescription)
                .HasMaxLength(500)
                .HasColumnName("Incident_Description");
            entity.Property(e => e.IncidentPhoto)
                .HasMaxLength(1000)
                .HasColumnName("Incident_Photo");

            entity.HasOne(d => d.IncidentType)
                .WithMany(p => p.Incidents)
                .HasForeignKey(d => d.IncidentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incidents_ibfk_1");

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incidents_ibfk_2");

            entity.HasOne(d => d.Property)
                .WithMany()
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incidents_ibfk_3");

            entity.HasOne(d => d.Status)
                .WithMany()
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incidents_ibfk_4");
        });

        // Reports - v2.1
        modelBuilder.Entity<Style>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("styles");

            entity.Property(e => e.StyleName)
                .HasMaxLength(100)
                .HasColumnName("Style_Name");
            entity.Property(e => e.Bold).HasDefaultValueSql("'0'");
            entity.Property(e => e.Italic).HasDefaultValueSql("'0'");
            entity.Property(e => e.Underline).HasDefaultValueSql("'0'");
            entity.Property(e => e.FontSize)
                .HasDefaultValueSql("'12'")
                .HasColumnName("Font_Size");
            entity.Property(e => e.FontColor)
                .HasMaxLength(20)
                .HasDefaultValueSql("'#000000'")
                .HasColumnName("Font_Color");
            entity.Property(e => e.BackgroundColor)
                .HasMaxLength(20)
                .HasDefaultValueSql("'#FFFFFF'")
                .HasColumnName("Background_Color");
            entity.Property(e => e.HorizontalAlignment)
                .HasMaxLength(20)
                .HasDefaultValueSql("'left'")
                .HasColumnName("Horizontal_Alignment");
            entity.Property(e => e.VerticalAlignment)
                .HasMaxLength(20)
                .HasDefaultValueSql("'top'")
                .HasColumnName("Vertical_Alignment");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.WidthPercentage)
                .HasDefaultValueSql("'100'")
                .HasColumnName("Width_Percentage");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("reports");

            entity.HasIndex(e => e.ReportName, "Report_Name").IsUnique();

            entity.Property(e => e.ReportName)
                .HasMaxLength(50)
                .HasColumnName("Report_Name");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(150)
                .HasColumnName("Display_Name");
            entity.Property(e => e.TitleStyleId)
                .HasDefaultValueSql("'-1'")
                .HasColumnName("Title_Style");
            entity.Property(e => e.DisplayHeader)
                .HasDefaultValueSql("'1'")
                .HasColumnName("Display_Header");
            entity.Property(e => e.DisplayFooter)
                .HasDefaultValueSql("'1'")
                .HasColumnName("Display_Footer");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");

            entity.HasOne(d => d.TitleStyle)
                .WithMany()
                .HasForeignKey(d => d.TitleStyleId)
                .HasConstraintName("reports_ibfk_1");
        });

        modelBuilder.Entity<ReportRole>(entity =>
        {
            entity.HasKey(e => new { e.ReportId, e.RoleId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("report_roles");

            entity.HasIndex(e => e.RoleId, "Role_Id");

            entity.Property(e => e.ReportId).HasColumnName("Report_Id");
            entity.Property(e => e.RoleId).HasColumnName("Role_Id");

            entity.HasOne(d => d.Report)
                .WithMany(p => p.ReportRoles)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_roles_ibfk_1");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.ReportRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_roles_ibfk_2");
        });

        modelBuilder.Entity<ReportHeader>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("report_headers");

            entity.HasIndex(e => e.ReportId, "Report_Id");
            entity.HasIndex(e => e.StyleId, "Style_Id");

            entity.Property(e => e.ReportId).HasColumnName("Report_Id");
            entity.Property(e => e.DisplayOrder)
                .HasColumnName("Display_Order");
            entity.Property(e => e.StyleId)
                .HasDefaultValueSql("'-1'")
                .HasColumnName("Style_Id");
            entity.Property(e => e.DisplayContent)
                .HasColumnType("text")
                .HasColumnName("Display_Content");
            entity.Property(e => e.IsQuery)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Is_Query");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");

            entity.HasOne(d => d.Report)
                .WithMany(p => p.ReportHeaders)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_headers_ibfk_1");

            entity.HasOne(d => d.Style)
                .WithMany(p => p.ReportHeaders)
                .HasForeignKey(d => d.StyleId)
                .HasConstraintName("report_headers_ibfk_2");
        });

        modelBuilder.Entity<ReportSection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("report_sections");

            entity.HasIndex(e => e.ReportId, "Report_Id");
            entity.HasIndex(e => e.StyleId, "Style_Id");
            entity.HasIndex(e => e.HeaderStyleId, "Header_Style_Id");

            entity.Property(e => e.ReportId).HasColumnName("Report_Id");
            entity.Property(e => e.DisplayOrder)
                .HasColumnName("Display_Order");
            entity.Property(e => e.StyleId)
                .HasDefaultValueSql("'-1'")
                .HasColumnName("Style_Id");
            entity.Property(e => e.HeaderStyleId)
                .HasDefaultValueSql("'-1'")
                .HasColumnName("Header_Style_Id");
            entity.Property(e => e.DisplayContent)
                .HasColumnType("text")
                .HasColumnName("Display_Content");
            entity.Property(e => e.IsQuery)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Is_Query");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");

            entity.HasOne(d => d.Report)
                .WithMany(p => p.ReportSections)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_sections_ibfk_1");

            entity.HasOne(d => d.Style)
                .WithMany(p => p.ReportSections)
                .HasForeignKey(d => d.StyleId)
                .HasConstraintName("report_sections_ibfk_2");

            entity.HasOne(d => d.HeaderStyle)
                .WithMany(p => p.ReportSectionsHeaderStyle)
                .HasForeignKey(d => d.HeaderStyleId)
                .HasConstraintName("report_sections_ibfk_3");
        });

        modelBuilder.Entity<ReportFooter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("report_footers");

            entity.HasIndex(e => e.ReportId, "Report_Id");
            entity.HasIndex(e => e.StyleId, "Style_Id");

            entity.Property(e => e.ReportId).HasColumnName("Report_Id");
            entity.Property(e => e.DisplayOrder)
                .HasColumnName("Display_Order");
            entity.Property(e => e.StyleId)
                .HasDefaultValueSql("'-1'")
                .HasColumnName("Style_Id");
            entity.Property(e => e.DisplayContent)
                .HasColumnType("text")
                .HasColumnName("Display_Content");
            entity.Property(e => e.IsQuery)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Is_Query");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");

            entity.HasOne(d => d.Report)
                .WithMany(p => p.ReportFooters)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_footers_ibfk_1");

            entity.HasOne(d => d.Style)
                .WithMany(p => p.ReportFooters)
                .HasForeignKey(d => d.StyleId)
                .HasConstraintName("report_footers_ibfk_2");
        });

        modelBuilder.Entity<ReportAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("report_audits");

            entity.HasIndex(e => e.ReportId, "Report_Id");
            entity.HasIndex(e => e.UserId, "User_Id");

            entity.Property(e => e.ReportId).HasColumnName("Report_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.Parameters).HasColumnType("text");
            entity.Property(e => e.ExecutionDate)
                .HasColumnType("datetime")
                .HasColumnName("Execution_Date");

            entity.HasOne(d => d.Report)
                .WithMany(p => p.ReportAudits)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_audits_ibfk_1");

            entity.HasOne(d => d.User)
                .WithMany(p => p.ReportAudits)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_audits_ibfk_2");
        });

        modelBuilder.Entity<ReportParam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("report_params");

            entity.HasIndex(e => e.ReportId, "Report_Id");

            entity.Property(e => e.ReportId).HasColumnName("Report_Id");
            entity.Property(e => e.ParamName)
                .HasMaxLength(100)
                .HasColumnName("Param_Name");
            entity.Property(e => e.ParamType)
                .HasMaxLength(50)
                .HasColumnName("Param_Type");
            entity.Property(e => e.ParamDescription)
                .HasMaxLength(500)
                .HasColumnName("Param_Description");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");

            entity.HasOne(d => d.Report)
                .WithMany(p => p.ReportParams)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("report_params_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
