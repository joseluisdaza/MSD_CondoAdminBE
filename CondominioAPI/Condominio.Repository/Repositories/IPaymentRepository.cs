using Condominio.Data.MySql.Models;
using Condominio.Models;

namespace Condominio.Repository.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment?> GetByIdWithRelationsAsync(int id);
        Task<IEnumerable<Payment>> GetAllWithRelationsAsync();
        Task<Payment?> GetByReceiveNumberAsync(string receiveNumber);
        Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
