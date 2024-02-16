using System.Security.Claims;
using BookingSystem.Api.Attributes.EndpointsFilters;
using BookingSystem.Api.Services.Concretes;
using BookingSystem.Api.Services.Interfaces;
using DeskLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mongo.Library.Clients;
using Mongo.Library.Repositories.Desk;
using Mongo.Library.Repositories.User;
using Mongo.Library.Settings;
using UserLibrary.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Environment.CurrentDirectory);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add Mongo Db configurations
builder.Services.Configure<DeskSettings>(settings =>
{
    settings.ConnectionString = builder.Configuration.GetSection("DeskSettings:ConnectionString").Value ?? string.Empty;
    settings.DatabaseName = builder.Configuration.GetSection("DeskSettings:DatabaseName").Value ?? string.Empty;
});

builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<DeskSettings>>().Value);
builder.Services.AddSingleton(sp => new DeskMongoClient(sp.GetRequiredService<DeskSettings>().ConnectionString));

builder.Services.Configure<UserSettings>(settings =>
{
    settings.ConnectionString = builder.Configuration.GetSection("UserSettings:ConnectionString").Value ?? string.Empty;
    settings.DatabaseName = builder.Configuration.GetSection("UserSettings:DatabaseName").Value ?? string.Empty;
});

builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<UserSettings>>().Value);
builder.Services.AddSingleton(sp => new UserMongoClient(sp.GetRequiredService<UserSettings>().ConnectionString));


// Add Authentication Services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

// Add services to the container.
builder.Services.AddScoped(typeof(IDeskRepository<>), typeof(DeskRepository<>));
builder.Services.AddScoped<IDeskService, DeskService>();
builder.Services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<CreateEmptyUser>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IGeoLocationService, GeoLocationService>();

const string bookingSystemSolution = "BookingSystem";

// Adding CORS policy to allow communication between frontends and API
builder.Services.AddCors(options =>  
{  
    options.AddPolicy(name: bookingSystemSolution,  
        policy  =>
        {
            policy.WithOrigins(
                    "",
                    "")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });  
});  
    
builder.Services.AddControllers();
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

app.UseCors(bookingSystemSolution);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
