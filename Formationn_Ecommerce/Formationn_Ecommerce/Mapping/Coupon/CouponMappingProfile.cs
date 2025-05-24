using AutoMapper;
using Formationn_Ecommerce.Application.Coupons.Dtos;
using Formationn_Ecommerce.Models.Coupon;

namespace Formationn_Ecommerce.Mapping.Coupon
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile()
        {
            CreateMap<CouponViewModel, CouponDto>();
            CreateMap<CouponDto, CouponViewModel>();

            CreateMap<CreateCouponViewModel, CouponDto>();
            CreateMap<CouponDto, CreateCouponViewModel>();

            CreateMap<DeleteCouponViewModel, CouponDto>();
            CreateMap<CouponDto, DeleteCouponViewModel>();
        }
    }
}
