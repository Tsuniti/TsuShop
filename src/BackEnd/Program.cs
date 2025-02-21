using System.Reflection;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using TsuShopWebApi.Controllers;
using TsuShopWebApi.Database;
using TsuShopWebApi.Entities;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Options;
using TsuShopWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавление поддержки переменных окружения
Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


//Регистрируем конфигурацию
//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
// builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtOptions>>().Value);

var jwtOptions = new JwtOptions
{
    SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY"),
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
    TtlMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_TTL_MINUTES"))
};

builder.Services.AddSingleton<IOptions<JwtOptions>>(Options.Create(jwtOptions));


// builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
// builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<DatabaseOptions>>().Value);

var databaseOptions = new DatabaseOptions
{
    ConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
};
builder.Services.AddSingleton<IOptions<DatabaseOptions>>(Options.Create(databaseOptions));

// builder.Services.Configure<AdminOptions>(builder.Configuration.GetSection("AdminUser"));
// builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AdminOptions>>().Value);

var adminOptions = new AdminOptions
{
    Username = Environment.GetEnvironmentVariable("ADMIN_USERNAME"),
    Password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD"),
    CartId = Environment.GetEnvironmentVariable("ADMIN_CART_ID")
};
builder.Services.AddSingleton<IOptions<AdminOptions>>(Options.Create(adminOptions));

// builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
// builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EmailOptions>>().Value);

var emailOptions = new EmailOptions
{
    SmtpClientHost = Environment.GetEnvironmentVariable("EMAIL_SMTP_CLIENT_HOST"),
    SmtpClientPort = int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_CLIENT_PORT")),
    Email = Environment.GetEnvironmentVariable("EMAIL_ADDRESS"),
    AppPassword = Environment.GetEnvironmentVariable("EMAIL_APP_PASSWORD")
};
builder.Services.AddSingleton<IOptions<EmailOptions>>(Options.Create(emailOptions));

var azureBlobStorageOptions = new AzureBlobStorageOptions
{
    ConnectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONNECTION_STRING"),
    ContainerName = Environment.GetEnvironmentVariable("AZURE_BLOB_STORAGE_CONTAINER_NAME")
};
builder.Services.AddSingleton<IOptions<AzureBlobStorageOptions>>(Options.Create(azureBlobStorageOptions));


builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>();


builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

var serviceProvider = builder.Services.BuildServiceProvider();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters =
            serviceProvider.GetRequiredService<IJwtGenerator>().TokenValidationParameters;
    });


builder.Services.AddControllers();


// Регистрируем сваггер
builder.Services.AddSwaggerGen(config =>
{
    // xml comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    config.IncludeXmlComments(xmlPath);

    // Input for JWT access token at swagger
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });

    // Inserting written jwt-token to headers
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
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


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();
{
    app.UseCors("AllowFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

    // Swagger
    app.UseSwagger();
    app.UseSwaggerUI(config =>
        {
            config.RoutePrefix = string.Empty;
            config.SwaggerEndpoint("swagger/v1/swagger.json", "TsuShop");
        }
    );

    app.MapControllers();
}

app.Run();