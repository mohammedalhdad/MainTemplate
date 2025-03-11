
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


namespace CleanArchitecture.Infrastructure.DependencyInjections
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetryService(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceName = configuration["OpenTelemetry:ServiceName"] ?? "MyApp";
            var otlpEndpoint = configuration["OpenTelemetry:Endpoint"] ?? "http://127.0.0.1:4317"; //  


            services.AddOpenTelemetry()
                    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                            .AddAspNetCoreInstrumentation()  // Trace incoming HTTP requests
                            .AddHttpClientInstrumentation()  //Trace outgoing HTTP requests
                            //.AddConsoleExporter()
                            .AddOtlpExporter(options =>
                                { options.Endpoint = new Uri(otlpEndpoint); }) // OTEL gRPC
                            ) 

                    .WithMetrics(metricsProviderBuilder => metricsProviderBuilder
                            .AddRuntimeInstrumentation() // قياس استهلاك الموارد
                            .AddAspNetCoreInstrumentation()
                            //.AddConsoleExporter()
                            .AddOtlpExporter(options =>
                            { options.Endpoint = new Uri(otlpEndpoint); }) // OTEL gRPC
                            );



            return services;
        }
    }
}
