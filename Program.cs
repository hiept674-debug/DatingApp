
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DatingAppAPI.Data;
using DatingAppAPI.Middleware;
using DatingAppAPI.Repositories;
using DatingAppAPI.Services;
using Serilog;
using System.Security.Claims;
using System.IO;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    // Khởi tạo builder
    var builder = WebApplication.CreateBuilder(args);

    // Cấu hình Serilog
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.File(
                Path.Combine(
                    context.HostingEnvironment.ContentRootPath,
                    "Logs",
                    "log-.txt"
                ),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                shared: true
            );
    });

    // SQL Server
    var connectionString = builder.Configuration
        .GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<AppDbContext>(
        options => options.UseSqlServer(connectionString)
    );

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"]!;

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(secretKey)
            ),

            RoleClaimType = ClaimTypes.Role
        };
    });

    // Swagger + JWT Authorize
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "DatingAppAPI",
            Version = "v1"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Nhập trực tiếp chuỗi Token thu được sau khi Login."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // Đăng ký Repository
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IAuthRepository, AuthRepository>();
    builder.Services.AddScoped<IFileRepository, FileRepository>();
    builder.Services.AddScoped<EmailService>();

    // Đăng ký Service
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IFileService, FileService>();
    builder.Services.AddScoped<IAvatarService, AvatarService>();
    // Controllers
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReact", policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
    // Build ứng dụng
    var app = builder.Build();
    app.UseCors("AllowReact");
    

    // Middleware
    app.UseExceptionHandling();
    app.UseRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Ứng dụng dừng bất thường khi khởi động.");
}
finally
{
    Log.CloseAndFlush();
}
