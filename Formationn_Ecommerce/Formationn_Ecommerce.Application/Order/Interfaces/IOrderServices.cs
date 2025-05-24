using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formationn_Ecommerce.Application.Order.Dtos;
using Formationn_Ecommerce.Entities.Orders;

namespace Formationn_Ecommerce.Application.Order.Interfaces
{
    public interface IOrderServices
    {
        // Méthodes de création
        Task<OrderHeaderDto?> AddOrderHeaderAsync(OrderHeaderDto orderHeaderDto);
        Task<IEnumerable<OrderDetailsDto>?> AddOrderDetailsAsync(IEnumerable<OrderDetailsDto> orderDetailsDtoList);
        
        // Méthodes de récupération
        IEnumerable<OrderHeaderDto> GetAllOrdersAsync();
        IEnumerable<OrderHeaderDto> GetOrdersByUserIdAsync(string userId);
        Task<OrderHeaderDto?> GetOrderByIdAsync(Guid orderHeaderId);
        
        // Méthodes de mise à jour
        Task<bool?> UpdateOrderStatusAsync(Guid orderHeaderId, string newStatus);
        Task<OrderHeaderDto?> UpdateOrderHeaderAsync(OrderHeaderDto orderHeaderDto);
        
        // Méthodes de paiement Stripe
        Task<OrderHeaderDto?> ValidatePaymentAsync(Guid orderHeaderId);
        Task<bool> ProcessRefundAsync(Guid orderHeaderId);
    }
}
