using Microsoft.AspNetCore.HttpLogging;
using UserManagement.Api;
using UserManagement.Infrastructure;
using Serilog;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var services = builder.Services;
{
    services.AddControllers();

    services.AddMongo(builder.Configuration);

    services.AddJwtAuthentication(builder.Configuration);

    services.AddEndpointsApiExplorer();

    services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
    services.AddSwaggerOptions();
    services.AddSwaggerGen();

    services.AddHttpLogging(o => {
        o.LoggingFields = HttpLoggingFields.All;

        o.RequestHeaders.Add("Authorization");

        o.RequestBodyLogLimit = 4096;
        o.ResponseBodyLogLimit = 4096;
        }
    );

    services.AddApiVersioning();
}

var app = builder.Build();
{
    app.UseHttpsRedirection();

    app.UseHttpLogging();

    app.UseExceptionHandler("/error");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseApiVersioning();

    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        var apiDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            foreach (var description in apiDescriptionProvider.ApiVersionDescriptions)
                o.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
        });
    }

    app.Run();
}
