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
            }
            else
            {
                options.UseSqlServer(connectionString);
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



        //===================================================RabbitMQ ====================================================

        var rabbitMqHost = configuration["RabbitMQ:Host"];
        var rabbitMqPort = configuration["RabbitMQ:Port"];
        string rabbitMqUsername = configuration["RabbitMQ:Username"]!;
        string rabbitMqPassword = configuration["RabbitMQ:Password"]!;

        // إعداد MassTransit مع RabbitMQ
        services.AddMassTransit(x =>
        {
            // إعداد Request/Response
            //x.AddRequestClient<T>();

            //Add Consumers
            //x.AddConsumer<PaymentTermTemplate>(); 


            // تسجيل المستهلكين من أكثر من Assembly
            //x.AddConsumers(typeof(SomeConsumer).Assembly, typeof(AnotherConsumer).Assembly);


            // تسجيل كل المستهلكين من نفس الـ Assembly بشكل تلقائي
            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host($"rabbitmq://{rabbitMqHost}:{rabbitMqPort}", h =>
                {
                    h.Username(rabbitMqUsername);
                    h.Password(rabbitMqPassword);
                });

                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                // ربط المستهلكين مع RabbitMQ تلقائياً
                cfg.ConfigureEndpoints(context);

                //// إعداد نقطة النهاية لاستقبال الرسائل
                //cfg.ReceiveEndpoint("send-command", e =>
                //{
                //    e.Consumer<PaymentTermTemplate>(context);
                //});

            });
        });

        services.AddScoped(typeof(IMessagePublisher<>), typeof(RabbitMqMessageService<>));

        return services;
    }
}
