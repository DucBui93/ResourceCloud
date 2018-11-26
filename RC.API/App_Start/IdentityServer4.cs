using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RC.Models;
using RC.Models.EntityModels;

namespace RC.API
{
    public partial class Startup 
    {
        public void IdentityServer4(IServiceCollection services)
        {
            #region get settings for external login
            //var externalLoginUseMicrosoft = Configuration.GetSection("ExternalLoginWithMicrosoft");
            //services.Configure<BaseAuthInfor>(externalLoginUseMicrosoft);
            //var externalLoginWithMicrosoft = externalLoginUseMicrosoft.Get<BaseAuthInfor>(); 
            #endregion

            var readServerAuthenticationConfig = Configuration.GetSection("ServerAuthenticationConfig");
            services.Configure<BaseAuthInfor>(readServerAuthenticationConfig);

            var serverAuthInforConfig = readServerAuthenticationConfig.Get<BaseAuthInfor>();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<User>();

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = serverAuthInforConfig.Authority;
                    options.RequireHttpsMetadata = serverAuthInforConfig.RequireHttpsMetadata;
                    options.ApiName = serverAuthInforConfig.APIName;
                });

            #region Try to implement login use external
            //    .AddJwtBearer(options =>
            //    {
            //        options.Audience = "https://localhost:5001/";
            //        options.Authority = "https://localhost:5001/identity/";
            //        options.RequireHttpsMetadata = false;
            //    })
            //    .AddMicrosoftAccount("Microsoft", options =>
            //    {
            //        //options.AuthorizationEndpoint = "";
            //        options.ClientId = externalLoginWithMicrosoft.Client;
            //        options.ClientSecret = externalLoginWithMicrosoft.SecretKey;
            //    })
            //    .AddJwtBearer("AzureAD", options =>
            //    {
            //        options.Audience = "https://localhost:5001/";
            //        options.Authority = "https://login.microsoftonline.com/eb971100-6f99-4bdc-8611-1bc8edd7f436/";
            //    });
            //services.AddAuthorization(options =>
            //{
            //    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
            //        JwtBearerDefaults.AuthenticationScheme,
            //        "AzureAD");
            //    defaultAuthorizationPolicyBuilder =
            //        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            //    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            //}); 
            #endregion
        }
    }
}
