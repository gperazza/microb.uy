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
using MicrobUy_API.Data.SeedData;
using MicrobUy_API.Services.GeneralDataService;
using Neo4j.Driver;
using MicrobUy_API.Data.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MicrobUy_API.Services.StatisticsService;
using AutoMapper;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MicrobUY API",
        Version = "v1",
        Description = "Esta API sirve las funcionalidades de la Web MicrobUY y de la aplicación Mobile"
    });

    //Set the comments path for the Swagger JSON and UI
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});
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
builder.Services.AddScoped<INeo4jUsersRepository, Neo4jUsersRepository>();//neo4j
builder.Services.AddScoped<IGeneralDataService, GeneralDataService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IValidator<CreateInstanceRequestDto>, CreateInstanceRequestValidator>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateInstanceRequestValidator>();
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
    
//NEO4J
builder.Services.AddSingleton(GraphDatabase.Driver(
    builder.Configuration.GetValue<string>("ConnectionString-neo4j:NEO4J_URI") ?? "bolt://localhost",
    AuthTokens.Basic(
        builder.Configuration.GetValue<string>("ConnectionString-neo4j:NEO4J_USER") ?? "micro",
         builder.Configuration.GetValue<string>("ConnectionString-neo4j:NEO4J_PASSWORD") ?? "forcepassword"
    )
));

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


app.UseSwagger();
app.UseSwaggerUI();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicrobUY v1"));


//Seed de la Base de Datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TenantAplicationDbContext>;
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>;
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>;
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>;
    var neo4jdriver = scope.ServiceProvider.GetRequiredService<IDriver>;

    SeedTematica.RunSeedTematica(context.Invoke());
    SeedCity.RunSeedCity(context.Invoke());
    Thread.Sleep(3000);
    SeedPlataformUsers.RunSeedUserPlataform(context.Invoke(), userManager.Invoke(), mapper.Invoke());
    Thread.Sleep(3000);
    SeedInstancia.RunSeedInstances(context.Invoke(), mapper.Invoke());
    Thread.Sleep(3000);
    SeedUsers userSeed = new SeedUsers();
    userSeed.RunSeedUsers(context.Invoke(), userManager.Invoke(), mapper.Invoke(), config.Invoke(), neo4jdriver.Invoke());
    Thread.Sleep(3000);
    SeedPosts postsSeed = new SeedPosts();
    postsSeed.RunSeedPostsAndFollowUser(context.Invoke(), mapper.Invoke(), config.Invoke(), neo4jdriver.Invoke());
}

app.UseCors(misReglasCors);
app.UseHttpsRedirection();
app.UseMiddleware<TenantResolver>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
