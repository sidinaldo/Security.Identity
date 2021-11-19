using Application.Filters.Logger;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using Virtual.Api.Configuration;

namespace Virtual.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            CriarLogger();
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //services.AddDbContext<VirtualContext>(o => o.UseMySql(Configuration.GetConnectionString("DefaultConnectionMySql")));

            services.AddIdentityConfiguration(Configuration);

            services.AddAutoMapper(typeof(Startup));

            services.WebApiConfig();

            services.AddSwaggerConfig();

            services.ResolveDependencies();

            //services.AddConfigurationInitial(Configuration);
            //services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddContext(LogLevel.Error, Configuration.GetConnectionString("DefaultConnection"));

            app.UseDeveloperExceptionPage();
            app.UseAuthentication();
            app.UseApiConfiguration();
            app.UseSwaggerConfig();

        }

        private void CriarLogger()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("Logs/log.txt",
                rollOnFileSizeLimit: true)
            .CreateLogger();

            Log.Information("Hello, Serilog!");

            Log.CloseAndFlush();
        }

    }
}
