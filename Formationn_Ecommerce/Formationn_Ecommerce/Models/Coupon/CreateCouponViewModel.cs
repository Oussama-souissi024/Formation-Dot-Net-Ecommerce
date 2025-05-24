using System.ComponentModel.DataAnnotations;

namespace Formationn_Ecommerce.Models.Coupon
{
    public class CreateCouponViewModel
    {
        [Required(ErrorMessage = "Coupon code is required")]
        [Display(Name = "Coupon Code")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Coupon code must be between 3 and 50 characters")]
        public string CouponCode { get; set; }


        [Required(ErrorMessage = "Discount amount is required")]
        [Display(Name = "Discount Amount")]
        [Range(0.01, 10000, ErrorMessage = "Discount amount must be between 0.01 and 10,000")]
        public decimal DiscountAmount { get; set; }

        [Required(ErrorMessage = "Minimum amount is required")]
        [Display(Name = "Minimum Purchase Amount")]
        [Range(0, 10000, ErrorMessage = "Minimum amount must be between 0 and 10,000")]
        public decimal MinimumAmount { get; set; }
    }
}
