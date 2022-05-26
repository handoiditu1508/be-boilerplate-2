using Hamburger.Helpers;
using Hamburger.Helpers.Abstractions;
using Hamburger.Models.Entities;
using Hamburger.Models.UserService;
using Hamburger.Repository.Abstraction.Repositories;
using Hamburger.Repository.EF;
using Hamburger.Repository.EF.Repositories;
using Hamburger.Services.Abstractions.FileStorage;
using Hamburger.Services.Abstractions.LoggingService;
using Hamburger.Services.Abstractions.UserService;
using Hamburger.Services.FileStorage;
using Hamburger.Services.LoggingService;
using Hamburger.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hamburger.Api
{
    public class Startup
    {
        private readonly string _appCors = "AppCors";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(_appCors,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("X-Total-Count");
                    });
            });

            // Auto Mapper
            services.AddAutoMapper(typeof(UserMapperProfile));

            // Entity Framework
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(AppSettings.Database.ConnectionString));

            // For Identity
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            });

            // Add Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = EnvironmentVariable.AspNetCoreEnvironment == "Production";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = AppSettings.Jwt.Issuer,
                    ValidAudience = AppSettings.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Jwt.Secret))
                };
            });

            // Add role claims authorization
            services.AddAuthorization(options =>
            {
                foreach (var permissionPolicy in PermissionClaimPolicies.ClaimValues)
                {
                    options.AddPolicy(permissionPolicy.Key, policy => policy.RequireClaim(CustomClaimTypes.Permission, permissionPolicy.Value));
                }
            });

            services.AddTransient<IHttpHelper, HttpHelper>();
            services.AddSingleton<IValidationHelper, ValidationHelper>();
            services.AddSingleton<ILoggingService, LoggingService>();
            services.AddSingleton<IFileStorageService, FileStorageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILoginSessionRepository, LoginSessionRepository>();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hamburger.Api", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });

                // Add support for swagger endpoint description
                // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hamburger.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(_appCors);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
