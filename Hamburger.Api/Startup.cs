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
                        policy.WithOrigins("https://localhost:44373")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("X-Total-Count");
                    });
            });

            // Model State Validation
            /*services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });*/

            // Auto Mapper
            services.AddAutoMapper(typeof(UserMapperProfile));

            // Entity Framework
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(AppSettings.Database.ConnectionString));

            // For Identity
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hamburger.Api", Version = "v1" });
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

            app.UseCors(_appCors);

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
