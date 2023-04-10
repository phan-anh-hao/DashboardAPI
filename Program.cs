/* Program.cs
 * Configure the services required by the application
 * Define the application's request processing flow
 * */

// Initialize a new instance WebApplicationBuilder
using DashboardAppAPI.Data;
using DashboardAppAPI.Repository;
using DashboardAppAPI.Model;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(option => option.AddDefaultPolicy(policy 
    => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddTransient<IDataAccess, DataAccess>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSetting"));

var secretKey = builder.Configuration["AppSetting:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

            ClockSkew = TimeSpan.Zero
        };
    }
);

var app = builder.Build(); // Configure host

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Middleware component
    app.UseSwaggerUI();     // Middleware component
}

app.UseHttpsRedirection();  // Middleware component

app.UseAuthentication();    // Middleware component

app.UseAuthorization();     // Middleware component

app.MapControllers();       // Middleware component

app.Run();
