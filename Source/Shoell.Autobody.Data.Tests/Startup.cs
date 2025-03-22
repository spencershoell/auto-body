using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shoell.Autobody.Services;
using Shoell.Shared.Interfaces;

namespace Shoell.Autobody.Data.Tests
{
    public class Startup
    {
        protected IConfiguration Configuration { get; set; } = null!;

        public void ConfigureHost(IHostBuilder hostBuilder) => hostBuilder
            .ConfigureHostConfiguration(builder => { })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.SetBasePath(context.HostingEnvironment.ContentRootPath)
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                Configuration = builder.Build();
            });

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<DatabaseInitializationService>();

            services.AddSingleton((sp) => Configuration);

            services.AddDbContext<AutobodyContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AutobodyContext")));

            services.AddScoped<IDateTimeProvider, AutobodyDateTimeProvider>();

            services
                .AddAutobodyAuthorization()
                .AddAutobodyRepositories();

            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
