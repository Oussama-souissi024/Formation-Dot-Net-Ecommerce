using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Formationn_Ecommerce.Application.Order.Dtos;
using Formationn_Ecommerce.Application.Order.Interfaces;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Entities.Orders;
using Formationn_Ecommercee.Core.Interfaces.External;
using Formationn_Ecommerce.Core.Not_Mapped_Entities;
using Microsoft.AspNetCore.Http;

namespace Formationn_Ecommerce.Application.Order.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IStripePayment _stripePayment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public OrderServices(IOrderRepository orderRepository,
                             IMapper mapper,
                             IStripePayment stripePayment,
                             IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _stripePayment = stripePayment;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Méthodes de création
        
        public async Task<OrderHeaderDto?> AddOrderHeaderAsync(OrderHeaderDto orderHeaderDto)
        {
            var orderHeader = _mapper.Map<OrderHeader>(orderHeaderDto);
            var addedOrderHeader = await _orderRepository.AddOrderHeaderAsync(orderHeader);
            
            // S'assurer que l'en-tête de commande inclut ses détails
            var completeOrderHeader = await _orderRepository.GetByIdWithDetailsAsync(addedOrderHeader.Id);
            
            // Build dynamic base URL using current request context
            var httpContext = _httpContextAccessor.HttpContext;
            var baseUrl = httpContext != null ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}" : "https://localhost:7217";
            // Create StripeRequest with dynamic URLs
            var stripeRequest = new StripeRequest
            {
                OrderHeader = completeOrderHeader,
                OrderDetails = completeOrderHeader.OrderDetails,
                ApprovedUrl = $"{baseUrl}/Order/OrderConfirmation?id={addedOrderHeader.Id}",
                CancelUrl = $"{baseUrl}/Cart/Checkout"
            };
            
            try
            {
                var stripeResponse = await _stripePayment.CreateStripeSessionAsync(stripeRequest);
                if (stripeResponse != null)
                {
                    // La mise à jour de StripeSessionId est déjà gérée dans StripePayment.CreateStripeSessionAsync
                    return _mapper.Map<OrderHeaderDto>(addedOrderHeader);
                }
                else
                {
                    throw new Exception("Échec de la création de la session de paiement Stripe");
                }
            }
            catch (Exception ex)
            {
                // En cas d'échec, on retourne quand même la commande créée pour ne pas bloquer le processus
                // mais on pourrait ajouter un statut spécifique pour indiquer l'échec de l'initialisation du paiement
                return _mapper.Map<OrderHeaderDto>(addedOrderHeader);
            }
        }
        
        public async Task<IEnumerable<OrderDetailsDto>> AddOrderDetailsAsync(IEnumerable<OrderDetailsDto> orderDetailsDtoList)
        {
            var orderDetailsList = _mapper.Map<IEnumerable<OrderDetails>>(orderDetailsDtoList);
            return _mapper.Map<IEnumerable<OrderDetailsDto>>(await _orderRepository.AddOrderDetailsAsync(orderDetailsList));
        }
        
        #endregion
        
        #region Méthodes de récupération
        
        public IEnumerable<OrderHeaderDto> GetAllOrdersAsync()
        {
            var orderHeaders = _orderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);
        }
        
        public IEnumerable<OrderHeaderDto> GetOrdersByUserIdAsync(string userId)
        {
            var orderHeaders = _orderRepository.GetAllByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderHeaderDto>>(orderHeaders);
        }
        
        public async Task<OrderHeaderDto?> GetOrderByIdAsync(Guid orderHeaderId)
        {
            var orderHeader = await _orderRepository.GetByIdAsync(orderHeaderId);
            return _mapper.Map<OrderHeaderDto>(orderHeader);
        }
        
        #endregion
        
        #region Méthodes de mise à jour
        
        public async Task<bool?> UpdateOrderStatusAsync(Guid orderHeaderId, string newStatus)
        {
            return await _orderRepository.UpdateStatusAsync(orderHeaderId, newStatus);
        }
        
        public async Task<OrderHeaderDto?> ValidatePaymentAsync(Guid orderHeaderId)
        {
            try
            {
                // Valider le paiement avec Stripe
                var orderHeader = await _stripePayment.ValidateStripeSession(orderHeaderId);
                
                if (orderHeader != null)
                {
                    // La validation a réussi et le statut a été mis à jour dans StripePayment.ValidateStripeSession
                    return _mapper.Map<OrderHeaderDto>(orderHeader);
                }
                else
                {
                    throw new Exception("Échec de la validation du paiement Stripe");
                }
            }
            catch (Exception ex)
            {
                // En cas d'erreur, on peut journaliser et gérer l'erreur
                return null;
            }
        }
        
        public async Task<OrderHeaderDto?> UpdateOrderHeaderAsync(OrderHeaderDto orderHeaderDto)
        {
            var orderHeader = _mapper.Map<OrderHeader>(orderHeaderDto);
            var updatedOrderHeader = await _orderRepository.UpdateOrderHeaderAsync(orderHeader);
            return _mapper.Map<OrderHeaderDto>(updatedOrderHeader);
        }
        
        public async Task<bool> ProcessRefundAsync(Guid orderHeaderId)
        {
            try
            {
                var orderHeader = await _orderRepository.GetByIdAsync(orderHeaderId);
                
                if (orderHeader != null && !string.IsNullOrEmpty(orderHeader.PaymentIntentId))
                {
                    bool refundResult = await _stripePayment.StripeRefundOptions(orderHeader.PaymentIntentId);
                    
                    if (refundResult)
                    {
                        await _orderRepository.UpdateStatusAsync(orderHeaderId, "Refunded");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        #endregion
    }
}
