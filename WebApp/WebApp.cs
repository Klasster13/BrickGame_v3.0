using Common.Interfaces;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApp;

public class WebApp : IView
{
    private readonly WebApplication _app;


    public WebApp(IModel model)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSingleton<IModel, Race.Impl.Race>();

        builder.Services.ConfigureServices(model);
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug(); // TODO need? 
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        builder.Services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "BrickGame API",
                Description = "Implementation of Web API for brick game projects.",
                Contact = new OpenApiContact
                {
                    Name = "kristieh",
                    Email = "kristieh@student.21-school.com",
                    Url = new Uri("https://t.me/klasster13")
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swagger.IncludeXmlComments(xmlPath);
        });

        _app = builder.Build();

        if (_app.Environment.IsDevelopment())
        {
            _app.MapOpenApi();
        }

        _app.UseSwagger();
        _app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BrickGameAPI");
            c.RoutePrefix = "api/swagger";
        });

        _app.UseRouting();
        _app.MapControllers();
    }


    public async Task StartAsync() => await _app.RunAsync();
    public async Task StopAsync() => await _app.StopAsync();


}

public static class DependencyConfiguration
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IModel model)
    {
        services.AddSingleton<IModel, Race.Impl.Race>();
        return services;
    }
}


internal class Program
{
    private static async Task Main(string[] args)
    {
        var model = new Race.Impl.Race();
        var web = new WebApp(model);
        await web.StartAsync();
    }
}