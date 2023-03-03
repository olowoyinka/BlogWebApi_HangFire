using BlogWebApi.Application.Interfaces.Persistence;
using BlogWebApi.Application.Interfaces.Services;
using BlogWebApi.Domain.Entities;
using BlogWebApi.Infrastructure.Common;
using BlogWebApi.Infrastructure.Persistence;
using BlogWebApi.Infrastructure.Persistence.Repositories;
using BlogWebApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Text;

namespace BlogWebApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddService()
                .AddPersistence(configuration)
                .AddAuth(configuration);

            return services;
        }



        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlogDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<BlogDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }



        public static IServiceCollection AddAuth(this IServiceCollection services,
                                                    IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(JwtSettings), jwtSettings);

            services.AddSingleton(Options.Create(jwtSettings));

            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    });

            List<string> allowroles = new List<string>() { "creator", "reader" };

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowAccess", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAssertion(context => context.User.HasClaim(c => allowroles.Contains(c.Value)));
                });

                options.AddPolicy("CreatorAccess", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAssertion(context => context.User.HasClaim(c => c.Value.Equals("creator")));
                });
            });

            return services;
        }



        public static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IJobTestService, JobTestService>();

            return services;
        }
    }
}