using Formationn_Ecommerce.Application.Coupons.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Application.Coupons.Interfaces
{
    public interface ICouponService
    {
        Task<CouponDto> AddAsync(CouponDto coupon);
        Task<CouponDto?> ReadByIdAsync(Guid couponId);
        Task<CouponDto?> GetCouponByCodeAsync(string couponCode);
        Task<IEnumerable<CouponDto>> ReadAllAsync();
        Task UpdateAsync(UpdateCouponDto updateCouponDto);
        Task DeleteAsync(Guid id);
    }
}
