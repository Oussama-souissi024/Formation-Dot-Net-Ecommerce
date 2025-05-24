using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Application.Coupons.Dtos
{
    public class UpdateCouponDto
    {
        [Required]
        public Guid Id { get; set; } // kiop154

        [Required]
        [StringLength(100)]
        public string CouponCode { get; set; } // OFF350

        [Required]
        [Range(0.01, 10000.00)]
        public decimal DiscountAmount { get; set; } // 350

        [Required]
        [Range(0.01, 10000.00)]
        public decimal MinAmount { get; set; }  // 3000
    }
}
