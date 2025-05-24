using Formationn_Ecommerce.Core.Interfaces.Repositories;
using Formationn_Ecommerce.Infrastucture.Extension;
using Formationn_Ecommerce.Infrastucture.Persistence;
using Formationn_Ecommerce.Infrastucture.Persistence.DbInitializer;
using Formationn_Ecommerce.Application.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Formationn_Ecommerce.Mapping.Coupon;
using Formationn_Ecommerce.Mapping.Category;
using Formationn_Ecommerce.Core.Not_Mapped_Entities;
using Microsoft.AspNetCore.Identity;
using Formationn_Ecommerce.Core.Entities.Identity;
using Formationn_Ecommerce.Core.Interfaces.External;
using Formationn_Ecommerce.Infrastucture.External.Mailing;
using Formationn_Ecommerce.Mapping.Product;
using Formationn_Ecommerce.Mapping.Home;
using Formationn_Ecommerce.Application.Cart.Mapping;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// add DBContext with explicit SQL Server configuration
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DeafaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddInfrastructureRegistration(builder.Configuration);
builder.Services.AddServiceRegistraion();

// Register all AutoMapper profiles in a single call
builder.Services.AddAutoMapper(
    // Couche Web
    typeof(CouponMappingProfile),
    typeof(CategoryMappingProfile),
    typeof(ProductMappingProfile),
    typeof(HomeMappingProfile),
    typeof(CartMappingProfile));

//Configuration des options d'email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();

Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize database with seed data
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var dbContext = services.GetRequiredService<ApplicationDbContext>();
//    // Use Wait() instead of await since this is not an async method
//    DbInitializer.InitializeDatabase(dbContext).Wait();
//}

app.Run();
