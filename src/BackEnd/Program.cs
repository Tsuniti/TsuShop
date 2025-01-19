using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using TsuShopWebApi.Controllers;
using TsuShopWebApi.Database;
using TsuShopWebApi.Interfaces;
using TsuShopWebApi.Options;
using TsuShopWebApi.Services;

var builder = WebApplication.CreateBuilder(args);
{
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
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtOptions>>().Value);
    
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<DatabaseOptions>>().Value);
    
builder.Services.Configure<FirstUserOptions>(builder.Configuration.GetSection("FirstUser"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<FirstUserOptions>>().Value);

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>();


builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddSingleton<IJwtGenerator, JwtGenerator>();

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
}



var app = builder.Build();
{
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
}

app.Run();