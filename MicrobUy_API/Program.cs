using FluentValidation;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.JwtFeatures;
using MicrobUy_API.Middleware;
using MicrobUy_API.Models;
using MicrobUy_API.Services.AccountService;
using MicrobUy_API.Services.TenantInstanceService;
using MicrobUy_API.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Neo4j.Driver;
using System.Text;
using System;

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

builder.Services.AddDbContext<TenantAplicationDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddDbContext<TenantInstanceDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddDbContext<IdentityProviderDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt => { opt.User.RequireUniqueEmail = false; }) 
    .AddEntityFrameworkStores<IdentityProviderDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<ITenantInstance, TenantInstance>();
builder.Services.AddScoped<IValidator<CreateInstanceRequestDto>, CreateInstanceRequestValidator>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateInstanceRequestValidator>();
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

//Node4j

builder.Services.AddSingleton(GraphDatabase.Driver(
           Environment.GetEnvironmentVariable("NEO4J_URI") ?? "neo4j+s://demo.neo4jlabs.com",
           AuthTokens.Basic(
               Environment.GetEnvironmentVariable("NEO4J_USER") ?? "movies",
               Environment.GetEnvironmentVariable("NEO4J_PASSWORD") ?? "movies"
           )
       ));
/*
IDriver client = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("micro", "forcepassword"));
var status1 = client.VerifyConnectivityAsync();  
using var session = client.AsyncSession();
var message = "hello, world";
var greeting = session.ExecuteWriteAsync(
    tx =>
    {
        var result = tx.RunAsync(
            "MATCH (n) RETURN count(n) ");

        return result;
    });
bool status = greeting.IsCompletedSuccessfully;

Console.WriteLine(greeting);
//var client = new BoltGraphClient(new Uri("http://localhost:7687"),null, "neo4j", "root",null,null);
//client.ConnectAsync();
//builder.Services.AddSingleton<IGraphClient>(client);
//builder.Services.AddDbContext<Neo4jDbContext>();
*/
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