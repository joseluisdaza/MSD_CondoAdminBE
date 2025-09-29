using System;
using System.Collections.Generic;

namespace Condominio.Data.MySql.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int ExpenseId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string ReceivePhoto { get; set; } = null!;

    public virtual Expense Expense { get; set; } = null!;
}
