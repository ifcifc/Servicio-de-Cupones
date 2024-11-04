using Microsoft.EntityFrameworkCore;
using Serilog;
using AppCupones.Data;
using System;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;
using AppCupones.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbAppContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection"));
});

#region Logger
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Logger(options => options.Filter
                            .ByIncludingOnly(evt => evt.Level == Serilog.Events.LogEventLevel.Error)
                            .WriteTo.File("Logs/Log-Error-.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(options => options.Filter
                            .ByIncludingOnly(evt => evt.Level == Serilog.Events.LogEventLevel.Information)
                            .WriteTo.File("Logs/Log-.txt", rollingInterval: RollingInterval.Day))
    .CreateLogger();
#endregion

#region Services
builder.Services.AddScoped<HashPasswordService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<GenerateCuponService>();
#endregion

#region Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:key"]))
    };
});
#endregion

#region JsonOptions
// Add services to the container.
builder.Services.AddControllers()
                //Para evitar las referencia ciclicas en los modelos.
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
#endregion

#region Default
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion