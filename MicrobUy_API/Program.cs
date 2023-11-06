using Firebase.Auth.Providers;
using Firebase.Auth;
using FluentValidation;
using MicrobUy_API.Data;
using MicrobUy_API.Dtos;
using MicrobUy_API.JwtFeatures;
using MicrobUy_API.Middleware;
using MicrobUy_API.Services.AccountService;
using MicrobUy_API.Services.PostService;
using MicrobUy_API.Services.TenantInstanceService;
using MicrobUy_API.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var jwtSettings = builder.Configuration.GetSection("JwtSettings");


var firebaseProjectName = builder.Configuration.GetValue<string>("FireBaseSettings:LaboratorioNet");
builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig
{
    ApiKey = builder.Configuration.GetValue<string>("FireBaseSettings:apiKey"),
    AuthDomain = $"{firebaseProjectName}.firebaseapp.com",
    Providers = new FirebaseAuthProvider[]
    {
        new GoogleProvider()
    }
}));
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
    options.Authority = $"https://securetoken.google.com/{firebaseProjectName}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://securetoken.google.com/{firebaseProjectName}",
        ValidateAudience = true,
        ValidAudience = firebaseProjectName,
        ValidateLifetime = true
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<TenantAplicationDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddDbContext<TenantInstanceDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddDbContext<IdentityProviderDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt => { opt.User.RequireUniqueEmail = false; })
    .AddEntityFrameworkStores<IdentityProviderDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<ITenantInstance, TenantInstance>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IValidator<CreateInstanceRequestDto>, CreateInstanceRequestValidator>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateInstanceRequestValidator>();
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

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
