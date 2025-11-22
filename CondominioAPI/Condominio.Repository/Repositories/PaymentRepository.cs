using Condominio.Data.MySql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominio.Repository.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(CondominioContext context) : base(context) { }

        public async Task<Payment?> GetByIdWithRelationsAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.ExpensePayments)
                    .ThenInclude(ep => ep.Expense)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetAllWithRelationsAsync()
        {
            return await _context.Payments
                .Include(p => p.ExpensePayments)
                    .ThenInclude(ep => ep.Expense)
                .ToListAsync();
        }

        public async Task<Payment?> GetByReceiveNumberAsync(string receiveNumber)
        {
            return await _context.Payments
                .Include(p => p.ExpensePayments)
                    .ThenInclude(ep => ep.Expense)
                .FirstOrDefaultAsync(p => p.ReceiveNumber == receiveNumber);
        }

        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Include(p => p.ExpensePayments)
                    .ThenInclude(ep => ep.Expense)
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .ToListAsync();
        }
    }
}
