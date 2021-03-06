using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TradeTracker.Application.Common.Interfaces;
using TradeTracker.Application.Common.Services;

namespace TradeTracker.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped<ICostBasisCalculator, CostBasisCalculator>();
            services.AddScoped<IPositionCalculator, PositionCalculator>();
            services.AddScoped<IPositionService, PositionService>();

            return services;
        }
    }
}
