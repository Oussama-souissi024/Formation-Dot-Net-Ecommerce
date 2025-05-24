using Formationn_Ecommerce.Application.Authentication.Interfaces;
using Formationn_Ecommerce.Application.Authentication.Services;
using Formationn_Ecommerce.Application.Cart.Interfaces;
using Formationn_Ecommerce.Application.Cart.Mapping;
using Formationn_Ecommerce.Application.Cart.Servecies;
using Formationn_Ecommerce.Application.Categories.Interfaces;
using Formationn_Ecommerce.Application.Categories.Mapping;
using Formationn_Ecommerce.Application.Categories.Services;
using Formationn_Ecommerce.Application.Coupons.Interfaces;
using Formationn_Ecommerce.Application.Coupons.Mapping;
using Formationn_Ecommerce.Application.Coupons.Services;
using Formationn_Ecommerce.Application.Order.Interfaces;
using Formationn_Ecommerce.Application.Order.Mapping;
using Formationn_Ecommerce.Application.Order.Services;
using Formationn_Ecommerce.Application.Products.Interfaces;
using Formationn_Ecommerce.Application.Products.Mapping;
using Formationn_Ecommerce.Application.Products.Services;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace Formationn_Ecommerce.Application.Common.Extension
{
    public static class ServicesRegistration
    {
        public static void AddServiceRegistraion(this IServiceCollection services)
        {
            services.AddScoped<ICouponService, CouponServices>();
            services.AddScoped<ICategoryService, CategoryServices>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IProductServices, ProductServices>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderServices, OrderServices>();

            services.AddAutoMapper(typeof(CouponProfile),
                                   typeof(CategoryProfile),
                                   typeof(ProductProfile),
                                   typeof(CartMappingProfile),
                                   typeof(OrderMappingProfile));
        }
    }
}
