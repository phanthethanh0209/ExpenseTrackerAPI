
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Mapper;
using ExpenseTrackerAPI.Repositories;
using ExpenseTrackerAPI.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ExpenseTrackerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                OpenApiSecurityScheme jwtSecurityScheme = new()
                {
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Enter your JWT Access Token",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {jwtSecurityScheme, Array.Empty<String>() }
                });
            });


            // Read config JWT from appsetting.json
            IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");
            byte[] secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            // config Authentication with JWT
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                    };
                });


            // register Database
            IConfigurationRoot cf = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            builder.Services.AddDbContext<MyDBContext>(opt => opt.UseSqlServer(cf.GetConnectionString("MyDB")));


            // register unit of work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            // register Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IExpenseService, ExpenseService>();


            // register auto mapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));


            // register fluentvalidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<ExpenseRequestDTO>();


            builder.Services.AddHttpContextAccessor();


            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
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
    }
}
