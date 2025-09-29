using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Repositories;
using SurveyManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SurveyManagement.Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add DbContext
builder.Services.AddDbContext<SurveyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurveyDb")));

// ✅ Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IOptionRepository, OptionRepository>();
builder.Services.AddScoped<IResponseRepository, ResponseRepository>();
builder.Services.AddScoped<IUserSurveyRepository, UserSurveyRepository>();
builder.Services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();

// ✅ Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IOptionService, OptionService>();
builder.Services.AddScoped<IResponseService, ResponseService>();
builder.Services.AddScoped<IUserSurveyService, UserSurveyService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ✅ Register AutoMapper
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.UserProfileMapping));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.UserProfileProfile));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.ProductProfile));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.SurveyProfileMapping));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.QuestionProfileMapping));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.OptionProfileMapping));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.ResponseProfileMapping));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.UserSurveyProfileMapping));

// ✅ Add Controllers
builder.Services.AddControllers();

// ✅ JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

// ✅ Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RespondentOnly", policy => policy.RequireRole("Respondent"));
});

// ✅ Swagger / OpenAPI with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Survey Management API",
        Version = "v1",
        Description = "API for Survey Management System"
    });

    // Add JWT support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ✅ Run DbInitializer once
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SurveyDbContext>();
    DbInitializer.Initialize(context); // will check for existing data before seeding
}

// ✅ Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Survey Management API V1");
        c.RoutePrefix = string.Empty; // Swagger at root: https://localhost:7043/
    });
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Must be before Authorization
app.UseAuthorization();

app.MapControllers();

// ✅ Health-check endpoint
app.MapGet("/health", () => "Survey Management API is running...");

app.Run();
