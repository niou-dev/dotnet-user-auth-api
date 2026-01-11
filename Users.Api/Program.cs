using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Users.Application.Database;
using Users.Application.Models;
using Users.Application.Repository;
using Users.Application.Services;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);
var config =  builder.Configuration;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Add services to the container.

var connectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
if (string.IsNullOrEmpty(connectionString)) throw new Exception("Please specify a connection string");

builder.Services.AddSingleton<IDbConnectionFactory>(_ => 
    new NpgsqlConnectionFactory(connectionString));

builder.Services.AddSingleton<DbInitializer>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"];
var issuer  = jwtSection["Issuer"];
var audience = jwtSection["Audience"];
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("Admin",
        p => p.RequireClaim("admin", "true"));
});


builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// db init
var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();