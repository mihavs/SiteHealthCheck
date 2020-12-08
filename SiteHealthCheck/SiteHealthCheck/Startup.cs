using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteHealthCheck
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        { 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
        {
            string url = config["HealthCheckURL"];
            string intervalString = config["HealthCheckInterval"];
            int interval = 30;
            if (!int.TryParse(intervalString, out interval))
                interval = 30;
            string UseHEADInsteadOfGET = config["UseHEADInsteadOfGET"];

            HealthCheckFactory.Default = new HealthCheckFactory(url, interval, UseHEADInsteadOfGET.ToLower().Trim() == "true");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    context.Response.StatusCode = HealthCheckFactory.Default.GetSuccess() ? 200 : 500;
                });
            });
        }
    }
}
