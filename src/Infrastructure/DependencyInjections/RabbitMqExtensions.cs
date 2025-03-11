using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Services.Messaging;

namespace CleanArchitecture.Infrastructure.DependencyInjections;

public  static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMqService(this IServiceCollection services, IConfiguration configuration)
    {
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
