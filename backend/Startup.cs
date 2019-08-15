using System;
using System.Net;
using AutoMapper;
using backend.Services;
using Backend.Data;
using Backend.Services;
using Backend.Services.EmailService;
using Backend.Services.Exceptions;
using Backend.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Backend {
    public class Startup {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Startup> _logger;

        public Startup (IConfiguration configuration, ILogger<Startup> logger) {
            _configuration = configuration;
            _logger = logger;
        }

        public Startup (IConfiguration configuration) {
            _configuration = configuration;

        }

        public void ConfigureServices (IServiceCollection services) {
            services.AddOptions ();
            services.Configure<UsersSettings> (_configuration.GetSection (nameof (UsersSettings)));
            services.Configure<CacheSettings> (_configuration.GetSection (nameof (CacheSettings)));
            services.Configure<EmailSettings> (_configuration.GetSection (nameof (EmailSettings)));

            var userSettings = _configuration
                .GetSection (nameof (UsersSettings))
                .Get<UsersSettings> ();
            var cacheSettings = _configuration
                .GetSection (nameof (CacheSettings))
                .Get<CacheSettings> ();
            var emailSettings = _configuration
                .GetSection (nameof (EmailSettings))
                .Get<EmailSettings> ();

            services.AddScoped<UsersContext> ();
            services.AddScoped<IUserService, UserService> ();

            services.AddSingleton<IEmailService, EmailService> ();

            var mappingConfig = new MapperConfiguration (config => {
                config.AddProfile (new MappingProfile ());
            });
            IMapper mapper = mappingConfig.CreateMapper ();
            services.AddSingleton (mapper);

            services
                .AddEntityFrameworkNpgsql ()
                .AddDbContext<UsersContext> (options =>
                    options.UseNpgsql (
                        userSettings.ConnectionString,
                        providerOptions => {
                            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
                            // TODO not with postgres, investigate
                            // providerOptions.EnableRetryOnFailure (
                            //     maxRetryCount: userSettings.MaxRetryCount,
                            //     maxRetryDelay: TimeSpan.FromSeconds (userSettings.MaxRetryDelay),
                            //     errorCodesToAdd: null
                            // );
                            providerOptions.MigrationsAssembly (nameof (Backend));
                        }
                    )
                );

            services.AddDistributedRedisCache (options => {
                options.Configuration = cacheSettings.ConnectionString;
                options.InstanceName = cacheSettings.InstanceName;
            });

            services.AddCors ();

            services.Configure<RouteOptions> (options => {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer (options => {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.ClaimsIssuer = userSettings.Issuer;
                    options.Audience = userSettings.Audience;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = false,
                        ValidIssuer = userSettings.Issuer,
                        ValidateAudience = false,
                        ValidAudience = userSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = userSettings.SymmetricSecurityKey,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes (5)
                    };
                });

            // TODO we can enable helcheck
            // services.AddHealthChecks ()
            //     .AddNpgSql (userSettings.ConnectionString)
            //     .AddRedis (cacheSettings.ConnectionString)
            //     .AddSmtpHealthCheck (options => {
            //         options.Host = emailSettings.Server;
            //         options.Port = emailSettings.Port;
            //     });
            // services.AddHealthChecksUI ();

            services.AddMvc ()
                .AddJsonOptions (options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver ();
                    options.SerializerSettings.Converters.Add (new StringEnumConverter ());
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
        }

        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseHsts ();
            }

            // TODO we can enable helcheck
            // app.UseHealthChecks ("/hc", new HealthCheckOptions () {
            //     Predicate = _ => true,
            //         ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            // });
            // app.UseHealthChecksUI (config => config.UIPath = "/hc-ui");

            app.UseExceptionHandler (a => a.Run (async context => {
                var feature = context.Features.Get<IExceptionHandlerPathFeature> ();
                var ex = feature.Error;
                var code = HttpStatusCode.InternalServerError;

                if (ex is ServiceNotFoundException) {
                    code = HttpStatusCode.NotFound;
                    _logger.LogInformation (LoggingEvents.ItemNotFound, "{code} {message}", code, ex.Message);
                } else if (ex is ServiceException) {
                    code = HttpStatusCode.BadRequest;
                    _logger.LogWarning (LoggingEvents.ValidationError, "{code} {message}", code, ex.Message);
                } else {
                    _logger.LogError (LoggingEvents.UnknownError, "{code} {message}", code, ex.Message);
                }

                var result = JsonConvert.SerializeObject (new { error = ex.Message });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) code;
                await context.Response.WriteAsync (result);
            }));

            app.UseCors (builder => builder
                .AllowAnyOrigin ()
                .AllowAnyMethod ()
                .AllowAnyHeader ()
                .AllowCredentials ()
            );

            // TODO rewrite to async redirect?
            app.UseStatusCodePages (async context => {
                var response = context.HttpContext.Response;
                if (response.StatusCode == (int) HttpStatusCode.Unauthorized) {
                    response.Redirect ("/login");
                }
            });

            app.UseAuthentication ();

            DefaultFilesOptions options = new DefaultFilesOptions ();
            options.DefaultFileNames.Clear ();
            options.DefaultFileNames.Add ("index.html");
            app.UseDefaultFiles (options);
            app.UseStaticFiles ();

            app.Use (async (context, next) => {
                if (!context.User.Identity.IsAuthenticated && context.Request.Path.StartsWithSegments ("/private")) {
                    // TODO check this
                    context.Response.Redirect ("/login");
                    // context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    return;
                }

                // otherwise continue with the request pipeline
                await next ();
            });

            app.UseMvc (routes => {
                routes.MapRoute (
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults : new { controller = "Home", action = "Index" }
                );

                routes.MapRoute (
                    name: "fallback",
                    template: "{*slug}",
                    defaults : new { controller = "Home", action = "Index" }
                );
            });

            _logger.LogError (LoggingEvents.ProcessStarted, "Backend started!");
        }
    }
}