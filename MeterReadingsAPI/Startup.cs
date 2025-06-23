using MeterReadingsDAL;
using MeterReadingsDAL.Repositories;
using MeterReadingsServiceLayer;
using MeterReadingsServiceLayer.Contracts;
using Microsoft.EntityFrameworkCore;
using System;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        //database setup
        _ = services.AddDbContext<MeterReadingDbContext>(options =>
        {
            _ = options.UseInMemoryDatabase("InMemoryAppDb");
        });
        services.AddScoped<IMeterReadingDbContext>(provider => provider.GetRequiredService<MeterReadingDbContext>());

        //application services
        services.AddScoped<IMeterReadingUploadService, MeterReadingUploadService>();
        services.AddSingleton<IMeterReadingValidatorService, MeterReadingValidatorService>();

        //DAL respositories
        services.AddScoped<IMeterReadingInsertRepository, MeterReadingInsertRepository>();
        services.AddScoped<ICustomerAccountReadRepository, CustomerAccountReadRepository>();
        services.AddScoped<IMeterReadingReadRepository,  MeterReadingReadRepository>();

        //add cors so the client can send requests
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("https://localhost:7046")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //seed test customer account data
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MeterReadingDbContext>();
            dbContext.SeedTestData();
        }

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}