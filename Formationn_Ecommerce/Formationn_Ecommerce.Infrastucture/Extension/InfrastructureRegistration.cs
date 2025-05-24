using Formationn_Ecommerce.Core.Interfaces;
using Formationn_Ecommerce.Core.Interfaces.External;
using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Core.Interfaces.Repositories.Base;
using Formationn_Ecommerce.Infrastucture.External.Mailing;
using Formationn_Ecommerce.Infrastucture.External.Payment;
using Formationn_Ecommerce.Infrastucture.Persistence;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository;
using Formationn_Ecommerce.Infrastucture.Persistence.Repository.Base;
using Formationn_Ecommercee.Core.Interfaces.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Infrastucture.Extension
{
    public static class InfrastructureRegistration
    {
        
        public static IServiceCollection AddInfrastructureRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            //Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<ICouponRepository, CouponRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            //Register EmailSender 
            services.AddScoped<IEmailSender, EmailSender>();

            //Register Stripe Payment Service
            services.AddScoped<IStripePayment, StripePayment>();

            services.AddScoped<IFileHelper, FileHelper>();

            return services;
        }

        
    }
}
