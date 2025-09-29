using System;
using System.Collections.Generic;

namespace Condominio.Data.MySql.Models;

public partial class Expense
{
    public int Id { get; set; }

    public string ReceiveNumber { get; set; } = null!;

    public int CategoryId { get; set; }

    public int? PropertyId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime PaymentLimitDate { get; set; }

    public decimal Amount { get; set; }

    public decimal? InterestAmount { get; set; }

    public decimal? InterestRate { get; set; }

    public string Description { get; set; } = null!;

    public int Status { get; set; }

    public DateTime ExpenseDate { get; set; }

    public virtual ExpenseCategory Category { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Property? Property { get; set; }
}
