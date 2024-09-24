using NLog;
using NLog.Web;


// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure NLog
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();


    // Add services to the container.
    builder.Services.AddKeyVaultIfConfigured(builder.Configuration);

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddAPIServices();

    var app = builder.Build();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseOpenApi();
        app.UseSwaggerUi();

    }
    else
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();


        app.UseOpenApi();
        app.UseSwaggerUi();
        //app.UseSwaggerUi(settings =>
        //{
        //    settings.Path = "/api";
        //    settings.DocumentPath = "/api/specification.json";
        //});

    }

    app.UseHttpsRedirection();
    app.UseHealthChecks("/health");

    app.UseReDoc(c =>
    {
        c.Path = "/redoc";
        c.DocumentPath = "/swagger/v1/swagger.json";
        c.CustomStylesheetPath = "/redoc-styles.css"; // ÅÖÇÝÉ ãÓÇÑ ãáÝ CSS ÇáãÎÕÕ
    });

    //app.UseExceptionHandler(options => { });

    app.MapEndpoints();

    app.Run();


}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}


