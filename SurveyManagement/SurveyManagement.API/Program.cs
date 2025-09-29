using SurveyManagement.Domain.Interfaces;
using SurveyManagement.Infrastructure.Repositories;
using SurveyManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SurveyManagement.Application.Services;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add DbContext
builder.Services.AddDbContext<SurveyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurveyDb")));

// ✅ Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// ✅ Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IProductService, ProductService>();

// ✅ Register AutoMapper
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.UserProfileMapping));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.UserProfileProfile));
builder.Services.AddAutoMapper(typeof(SurveyManagement.Application.Mapping.ProductProfile));


// ✅ Add Controllers
builder.Services.AddControllers();

// ✅ Add Authorization
builder.Services.AddAuthorization();

// ✅ Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Survey Management API",
        Version = "v1",
        Description = "API for Survey Management System"
    });
});

var app = builder.Build();

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
app.UseAuthorization();

app.MapControllers();

// ✅ Health-check endpoint
app.MapGet("/health", () => "Survey Management API is running...");

app.Run();
