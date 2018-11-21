using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using RC.DataAccess;
using RC.Models;
using RC.Models.EntityModels;

namespace RC.API
{
    public partial class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _hostingEnvironment.ConfigureNLog("nlog.config");

            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appSettings.json", true, true)
                .AddJsonFile($"appSettings.{hostingEnvironment.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            if (hostingEnvironment.IsDevelopment())
            {
                // Secret Manager let developers store configuration settings in secrets.json file, 
                // which isn’t checked-in the source control. 
                // The secrets.json file is stored in AppData folder
                //
                // Windows: %APPDATA%\microsoft\UserSecrets\<userSecretsId>\secrets.json
                // Linux: ~/.microsoft/usersecrets/<userSecretsId>/secrets.json
                // Mac: ~/.microsoft/usersecrets/<userSecretsId>/secrets.json
                // The value of userSecretsId comes from the value specified in .csproj file
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var jwtsection = Configuration.GetSection("JwtIssuerSettings");
            services.Configure<JwtIssuerSettings>(jwtsection);

            services.AddSingleton(Configuration);
            services.AddOptions();

            services.AddCors();

            ConfigIdentityDb(services);
            ConfigAppCookie(services);
            SwaggerConfig(services);
            
            MVCConfig(services);
            DependenceInjection(services);
            IdentityServer4(services);

            // configure strongly typed settings objects
            // configure jwt authentication
            var jwtSetting = jwtsection.Get<JwtIssuerSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSetting.SecretKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential();

            services.AddSingleton<IConfiguration>(Configuration);
            ServiceCollection = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                #region Seed data
                var context = serviceProvider.GetService<RCDataContext>();
                context.Database.EnsureCreated();

                InitializeData(serviceProvider, context);

                #endregion
            }
            else
            {
                app.UseHsts();
            }

            #region ErrorHandling

            ConfigureErrorHandling(app, env, loggerFactory);

            #endregion


            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                            var errorHandler = context.Features.Get<IExceptionHandlerFeature>();
                            if (errorHandler != null)
                            {
                                Log.Error(errorHandler.Error.Message, errorHandler.Error);

                                context.Response.Headers.Add("Application-Error", "Something went wrong. Please contact your system administrator.");
                                // CORS
                                context.Response.Headers.Add("access-control-expose-headers", "Application-Error");
                                await context.Response.WriteAsync("Something went wrong. Please contact your system administrator.").ConfigureAwait(false);

                                System.Diagnostics.Trace.TraceError(errorHandler.Error.Message);

                            }
                        });
                });

            #region File static

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory()),
                RequestPath = new PathString(Configuration["BaseUrl"])
            });
            #endregion

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "KOKO API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseAuthentication();
            //app.UseIdentityServer();

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void GetRegisteredServices(IApplicationBuilder app, IServiceCollection services)
        {
            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table>");
                sb.Append("<thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead>");
                sb.Append("<tbody>");

                foreach (var service in services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{service.ServiceType.FullName}</td>");
                    sb.Append($"<td>{service.Lifetime}</td>");
                    sb.Append($"<td>{service.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</tbody>");
                sb.Append("</table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }

        public void ConfigureErrorHandling(IApplicationBuilder app, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            hostingEnvironment.ConfigureNLog("nlog.config");

            //foreach (DatabaseTarget target in LogManager.Configuration.AllTargets.Where(t => t is DatabaseTarget))
            //{
            //    target.ConnectionString = Configuration.GetConnectionString("NLogDb");
            //}

            // LogManager.ReconfigExistingLoggers();
            LogManager.Configuration.Variables["connectionString"] = Configuration.GetConnectionString("DefaultConnection");

            if (hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                GetRegisteredServices(app, ServiceCollection);
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseStatusCodePages(async context =>
                {
                    context.HttpContext.Response.ContentType = "text/plain";
                    await context.HttpContext.Response.WriteAsync(
                        "Status code page, status code: " +
                        context.HttpContext.Response.StatusCode);
                });
            }
        }

        private IServiceCollection ServiceCollection { get; set; }

        protected IConfigurationRoot Configuration { get; }
    }
}
