using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Shoell.Autobody.Data;
using Shoell.Autobody.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services
    .AddAutobodyAuthorization()
    .AddAutobodyRepositories();

builder.Services.AddDbContext<AutobodyContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("AutobodyContext"))
    );

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddOData(opt =>
    {
        opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(10000);

        opt.TimeZone = TimeZoneInfo.Utc;

        opt.AddRouteComponents("odata", AutobodyDataModel.GetEntityDataModel());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
var corsDomains = configuration.GetValue<string>("corsDomains");
var domains = corsDomains?
    .Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
    .Select(e => e.TrimEnd('/'))
    .ToArray() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins(domains)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use odata route debug, /$odata
    app.UseODataRouteDebug();

    // If you want to use /$openapi, enable the middleware.
    //app.UseODataOpenApi();

    // Add OData /$query middleware
    app.UseODataQueryRequest();

    // Add the OData Batch middleware to support OData $Batch
    app.UseODataBatching();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
