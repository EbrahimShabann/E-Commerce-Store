using BulkyWeb_Api;
using BulkyWeb_Api.Authentication;
using BulkyWeb_Api.Authorization;
using BulkyWeb_Api.Filters;
using BulkyWeb_Api.MiddleWares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Numerics;
using System.Text;
using Bulky.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options=>
{
    options.Filters.Add<LogActivityfilter>();
    options.Filters.Add<PermissionBasedAuthorizationFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version="v2",
        Title="E-Commerce Store API",
        Description="This is the API for a bulky E-Commerce Store ",
        TermsOfService=new Uri( "https://www.google.com" ),
        Contact=new OpenApiContact
        {
            Name="Ebrahim Shaban",
            Email="ebrahim@domain.com",
            Url= new Uri("https://www.google.com")

        },
        License=new OpenApiLicense
        {
            Name="My License",
            Url= new Uri("https://www.google.com")
        }

    });
});
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>();

var JwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.AddSingleton(JwtOptions);
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer" , options =>
    {
        options.SaveToken=true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = JwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey))

        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeesOnly", builder =>
    {
        builder.RequireRole("Admin");
    });
});
    //.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<ProfilingMiddleware>();


app.MapControllers();

app.Run();
