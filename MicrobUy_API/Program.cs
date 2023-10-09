using FluentValidation;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.JwtFeatures;
using MicrobUy_API.Middleware;
using MicrobUy_API.Models;
using MicrobUy_API.Tenancy;
using MicrobUy_API.TenantInstanceService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["validIssuer"],
        ValidAudience = jwtSettings["validAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(jwtSettings.GetSection("securityKey").Value))
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContextFactory<TenantAplicationDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddDbContextFactory<TenantInstanceDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<TenantAplicationDbContext>();
builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<ITenantInstance, TenantInstance>();
builder.Services.AddScoped<IValidator<CreateInstanceRequestDto>, CreateInstanceRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateInstanceRequestValidator>();
builder.Services.AddScoped<JwtHandler>();

//CORS
var misReglasCors = "ReglasCors";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: misReglasCors, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(misReglasCors);
app.UseHttpsRedirection();
app.UseMiddleware<TenantResolver>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
