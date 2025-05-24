using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Application.Coupons.Dtos
{
    public class CreateCouponDto
    {
        [Required]
        [StringLength(100)]
        public string CouponCode { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public decimal DiscountAmount { get; set; }

        [Required]
        [Range(0.01, 10000.00)]
        public decimal MinAmount { get; set; }

    }
}
