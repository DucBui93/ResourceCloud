using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace RC.API
{
    public partial class Startup
    {
        public void SwaggerConfig(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "KOKO API",
                    Description = "ASP.NET Core Web API tempalte",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "DucBV",
                        Email = "DucBui1009@gmail.com",
                        Url = "https://twitter.com/spboyer"
                    },
                    License = new License
                    {
                        Name = "No License",
                        Url = "https://example.com/license"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
