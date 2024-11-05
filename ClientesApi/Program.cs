using System.Text.Json.Serialization;
using System;
using Serilog;
using ClientesApi.Data;
using Microsoft.EntityFrameworkCore;
using ClientesApi.Interfaces;
using ClientesApi.Services;
using Common.Services;
using Common.Models.Config;
using Microsoft.Extensions.Configuration;
using Common.Interfaces;

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
var apiUrl = builder.Configuration.GetValue<string>("ApiUrl");
var smtpConfig = new SmtpConfig()
{
    Email = builder.Configuration.GetValue<string>("SMTP:Email"),
    Password = builder.Configuration.GetValue<string>("SMTP:Password"),
    Host = builder.Configuration.GetValue<string>("SMTP:Host"),
    Port = builder.Configuration.GetValue<int>("SMTP:Port")
};


builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ApiConnectService>(s => new ApiConnectService(apiUrl));
builder.Services.AddScoped<EmailService>(s => new EmailService(smtpConfig));

//Provoca un crash 
//builder.Services.AddScoped<IApiConnectService, ApiConnectService>(s => new ApiConnectService(apiUrl));
//builder.Services.AddScoped<IEmailService, EmailService>(s => new EmailService(smtpConfig));
#endregion

#region Authentication
/*builder.Services.AddAuthentication(options =>
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
});*/
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
// Add services to the container.

builder.Services.AddControllers();
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
