using Formationn_Ecommerce.Application.Products.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Formationn_Ecommerce.Models.Order
{
    /// <summary>
    /// Modèle de vue principal pour l'affichage des commandes
    /// </summary>
    public class OrderViewModel
    {
        public Guid Id { get; set; }
        
        // Propriété supplémentaire pour correspondre à la vue OrderDetail.cshtml
        public Guid OrderHeaderId { get; set; }
        
        [Display(Name = "Utilisateur")]
        public string UserId { get; set; }
        
        [Display(Name = "Code coupon")]
        public string CouponCode { get; set; }
        
        [Display(Name = "Remise")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Discount { get; set; }
        
        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal OrderTotal { get; set; }
        
        [Display(Name = "Nom")]
        public string Name { get; set; }
        
        [Display(Name = "Téléphone")]
        public string Phone { get; set; }
        
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        
        [Display(Name = "Date de commande")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime OrderTime { get; set; }
        
        [Display(Name = "Statut")]
        public string Status { get; set; }
        
        // ID Stripe pour le suivi des paiements
        public string PaymentIntentId { get; set; }
        public string StripeSessionId { get; set; }
        
        // Collection des articles de la commande
        public List<OrderDetailsViewModel> OrderDetails { get; set; } = new List<OrderDetailsViewModel>();
    }
}
