using AutoMapper;
using Formationn_Ecommerce.Application.Coupons.Dtos;
using Formationn_Ecommerce.Core.Entities.Coupon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Application.Coupons.Mapping
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            //Mapping entre entité et DTO
            CreateMap<Coupon, CouponDto>();
            CreateMap<CouponDto, Coupon>();

            //Mapping pour opération de création
            CreateMap<CreateCouponDto, Coupon>();

            //Mapping pour les opération de mise à jour
            CreateMap<UpdateCouponDto, Coupon>();
        }     
    }
}
