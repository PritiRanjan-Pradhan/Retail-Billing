using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetailPOS.Application.Interfaces;
using RetailPOS.Persistence.Contexts;
using RetailPOS.Persistence.Repositories;

namespace RetailPOS.Persistence.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> configureDbContext)
        {
            services.AddDbContext<RetailPosDbContext>(configureDbContext);
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<IUnitOfWork, RetailPOS.Persistence.UnitOfWork.UnitOfWork>();
            return services;
        }
    }
}
