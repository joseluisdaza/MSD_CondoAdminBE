using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class CreateExpensePaymentRequest
{
    [Required(ErrorMessage = "El ID del gasto es requerido")]
    public int ExpenseId { get; set; }
}