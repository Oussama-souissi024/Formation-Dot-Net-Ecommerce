using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Formationn_Ecommerce.Application.Cart.Dtos;
using Formationn_Ecommerce.Application.Order.Dtos;
using Formationn_Ecommerce.Core.Entities.Cart;
using Formationn_Ecommerce.Entities.Orders;

namespace Formationn_Ecommerce.Application.Order.Mapping
{
    public class OrderMappingProfile : Profile 
    {
        public OrderMappingProfile()
        {
            // Map OrderHeader <-> OrderHeaderDto
            CreateMap<OrderHeader, OrderHeaderDto>()
                .ReverseMap();

            // Map OrderDetails <-> OrderDetailsDto
            CreateMap<OrderDetails, OrderDetailsDto>()
                .ReverseMap();
        }
    }
}
