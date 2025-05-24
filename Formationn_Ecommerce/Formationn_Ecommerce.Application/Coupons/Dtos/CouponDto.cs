using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Application.Coupons.Dtos
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal MinimumAmount { get; set; }

    }
}
