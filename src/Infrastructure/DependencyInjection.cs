using System.Reflection;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Infrastructure.Data.Interceptors;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using SharedKernel.Services.Messaging;
using CleanArchitecture.Infrastructure.DependencyInjections;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        var isPostgres = configuration.GetValue<bool>("IsPostgres");
        if (isPostgres)
        {
            connectionString = configuration.GetConnectionString("PostgresConnection");
        }

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            if (isPostgres)
            {
                options.UseNpgsql(connectionString);
                //.LogTo(Console.WriteLine, Logging.LogLevel.Information);
            }
            else
            {
                options.UseSqlServer(connectionString);
                //.LogTo(Console.WriteLine, Logging.LogLevel.Information);
            }

        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();


        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));



        //rabbitMQ
        if (configuration.GetValue<bool>("RabbitMQ:IsEnabled"))
         services.AddRabbitMqService(configuration);

        //openTelemetry
        if(configuration.GetValue<bool>("OpenTelemetry:IsEnabled"))
            services.AddOpenTelemetryService(configuration);



        return services;
    }
}
