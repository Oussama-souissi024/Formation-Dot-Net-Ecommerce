


using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;
using Formationn_Ecommerce.Entities.Orders;

namespace Formationn_Ecommerce.Core.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<OrderHeader>, IRepository<OrderDetails>
    {
        Task<OrderHeader> AddOrderHeaderAsync(OrderHeader orderHeader);
        Task<IEnumerable<OrderDetails>> AddOrderDetailsAsync(IEnumerable<OrderDetails>  orderDetailsList);
        IEnumerable<OrderHeader?> GetAllAsync();
        IEnumerable<OrderHeader?> GetAllByUserIdAsync(string UserId);
        Task<OrderHeader?> GetByIdAsync(Guid orderHeaderId);
        // Nouvelle méthode pour récupérer un OrderHeader avec ses OrderDetails
        Task<OrderHeader?> GetByIdWithDetailsAsync(Guid orderHeaderId);
        Task<bool?> UpdateStatusAsync(Guid orderHeaderId, string newStatus);
        Task<OrderHeader?> UpdateOrderHeaderAsync(OrderHeader orderHeader);

    }
}
