using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using dotNet_base.Components.Filters;
using dotNet_base.Components.Response;
using dotNet_base.Components.Services.BackgroundQueueTask;
using dotNet_base.Components.Services.BackgroundTimedTask;
using dotNet_base.Models;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Parbad.Builder;
using Parbad.Storage.EntityFrameworkCore.Builder;

namespace dotNet_base
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BaseContext>(options => {
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
                if (Configuration["ComponentConfig:Environment"].Equals("Development")) {
                    options.EnableSensitiveDataLogging();
                }
            });

            ConfigControllerService(services);

            ConfigSwaggerService(services);

            ConfigAuthService(services);

            ConfigBackgroundQueueTaskService(services);

            ConfigBackgroundTimedTaskService(services);

            ConfigHangfireService(services);

            ConfigPayment(services);

            services.AddCors();
            services.AddMvc(options => { options.Filters.Add<UserAuthorizeFilter>(); })
                .AddFluentValidation();

            services.Configure<Components.ComponentConfig>(Configuration.GetSection("ComponentConfig"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
            app.UseHangfireDashboard();
            // }
            // else {
            // app.UseHttpsRedirection();
            // }
            // if (env.IsProduction()) {
            //     app.UseExceptionHandler("/error");
            // }

            app.UseStatusCodePages(async context => {
                if (context.HttpContext.Response.StatusCode == 401
                    && context.HttpContext.Response.ContentType != "application/json") {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(ResponseFormat.NotAuth().Value));
                }
                else if (context.HttpContext.Response.StatusCode == 403
                         && context.HttpContext.Response.ContentType != "application/json") {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(ResponseFormat.PermissionDeniedMsg("شما به این قسمت دسترسی ندارید.")
                            .Value));
                }
                else if (context.HttpContext.Response.StatusCode == 400) {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(ResponseFormat.BadRequestMsg("درخواست نامعتبر").Value));
                }
                else if (context.HttpContext.Response.StatusCode == 500) {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync(
                        JsonSerializer.Serialize(ResponseFormat.InternalError("مشکلی در سرور رخ داده است.").Value));
                }
            });

            app.UseSwagger();

            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/V1 User/swagger.json", "V1 User");
                options.SwaggerEndpoint("/swagger/V1 Admin/swagger.json", "V1 Admin");
            });

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseDirectoryBrowser();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void ConfigSwaggerService(IServiceCollection services)
        {
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("V1 User", new OpenApiInfo {Title = "Test application", Version = "V1 User"});
                options.SwaggerDoc("V1 Admin", new OpenApiInfo {Title = "Test application", Version = "V1 Admin"});
                options.AddSecurityDefinition("Token", new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Token",
                            },
                        },
                        new string[] { }
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        private void ConfigControllerService(IServiceCollection services)
        {
            services.AddControllers(options => { options.RespectBrowserAcceptHeader = true; })
                .ConfigureApiBehaviorOptions(options => {
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    options.InvalidModelStateResponseFactory =
                        context => ResponseFormat.BadRequestMsg("درخواست نامعتبر");
                });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
        }

        private void ConfigAuthService(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["ComponentConfig:Jwt:Issuer"],
                        ValidAudience = Configuration["ComponentConfig:Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(Configuration["ComponentConfig:Jwt:SecretKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(config => {
                config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                config.AddPolicy(Policies.User, Policies.UserPolicy());
                config.AddPolicy(Policies.IncompleteUser, Policies.IncompleteUserPolicy());
                config.AddPolicy(Policies.Owner, Policies.OwnerPolicy());
            });
        }

        private void ConfigBackgroundQueueTaskService(IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }

        private void ConfigBackgroundTimedTaskService(IServiceCollection services)
        {
            services.AddHostedService<TimedHostedService>();
            services.AddSingleton<IBackgroundTimedTask, BackgroundTimedTask>();
        }

        private void ConfigHangfireService(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(Configuration.GetConnectionString("DefaultConnection"),
                    new PostgreSqlStorageOptions {
                        QueuePollInterval = TimeSpan.FromSeconds(1),
                    }));

            services.AddHangfireServer();
        }

        private void ConfigPayment(IServiceCollection services)
        {
            services.AddParbad()
                .ConfigureGateways(gateways => {
                    gateways.AddZarinPal()
                        .WithAccounts(accounts => {
                            accounts.AddInMemory(account => {
                                account.MerchantId = Configuration["ComponentConfig:Payment:Zarinpal:MerchantId"];
                                account.IsSandbox =
                                    Convert.ToBoolean(Configuration["ComponentConfig:Payment:Zarinpal:Sandbox"]);
                            });
                        });
                }).ConfigureStorage(builder => {
                    builder.UseEfCore(options => {
                        var assemblyName = typeof(Startup).Assembly.GetName().Name;
                        options.ConfigureDbContext = db =>
                            db.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                                sql => sql.MigrationsAssembly(assemblyName));

                        // If you prefer to have a separate MigrationHistory table for Parbad, you can change the above line to this:
                        options.ConfigureDbContext = db =>
                            db.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"),
                                sql => {
                                    sql.MigrationsAssembly(assemblyName);
                                    sql.MigrationsHistoryTable("PaymentHistory");
                                });

                        options.DefaultSchema = "public";

                        options.PaymentTableOptions.Name = "PaymentTable";
                        options.PaymentTableOptions.Schema = "public";

                        options.TransactionTableOptions.Name = "TransactionTable";
                        options.TransactionTableOptions.Schema = "public";
                    });
                });
        }
    }
}