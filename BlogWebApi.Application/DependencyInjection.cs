using BlogWebApi.Application.Interfaces.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BlogWebApi.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration Configuration)
        {
            services
                .AddSwaggerDoc()
                .AddCorsPolicy();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddHangfireServer();

            return services;
        }


        public static IServiceCollection AddSwaggerDoc(this IServiceCollection services)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BlogWeb API Documentation",
                    Version = "v1",
                    Description = "This is a private document, for BlogWebAPI internal use only." +
                                 "\nFor API consumers, always ensure that you handle 500 errors." +
                                 "These indicate a Server Error, and we would like to know about these ASAP."
                });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[0] }
                };

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()}
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });

            return services;
        }


        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddPolicy("AllowAll", options => options
                   .AllowAnyMethod()
                   .AllowAnyOrigin()
                   .AllowAnyHeader());
            });

            return services;
        }
    }
}