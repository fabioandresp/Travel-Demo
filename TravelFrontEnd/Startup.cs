using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using Travel.Core;

namespace Travel.View.FrontEnd
{
    internal class BasicToken
    {
        public string Name { set; get; }

        public string[] Actions { set; get; }
    }


    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Este método mapea todos los controladores según su Anotación.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddRazorPages();

            services.AddCors(o =>
                    o.AddPolicy(CoreEnvironment.AllowAllHeaders, builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials())
                );
        }

        private IApplicationBuilder App = null;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            App = app;

            App.UseExceptionHandler("/Error");
            App.UseHsts();

            if (env.IsDevelopment())
            {
                App.UseDeveloperExceptionPage();
            }
            else
            {

            }

            CoreEnvironment.ProxyPass = Configuration.GetSection("Proxy:ProxyPass").Value;

            CoreEnvironment.ProxyBase = Configuration.GetSection("Proxy:ProxyBase").Value;


            var items = Configuration.GetSection("Proxy:Controllers").Get<List<BasicToken>>();

            foreach (var iKey in items)
            {
                if (iKey.Actions != null)
                {
                    foreach (var iAction in iKey.Actions)
                    {
                        CoreEnvironment.AddRoute(iKey.Name, iAction, CoreEnvironment.ProxyPass + "/" + iKey.Name + "/" + iAction);
                    }
                }
            }

            App.UseHttpsRedirection();
            App.UseStaticFiles();
            App.UseHttpsRedirection();
            App.UseRouting();
            App.UseCors();
            App.UseFileServer();

            App.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });


            App.Use(
                   async (context, next) =>
                   {
                       //Proxy Pass Implementation
                       //Retornar código de error y llevar control de los requets incorrectos.
                       
                       //Allow buffer on Body
                       context.Request.EnableBuffering();
                       try
                       {
                           //Permite manipular requets no manejados del lado del proxy controller.
                           ProxyController.HelperRequest(context, next);
                       }
                       catch (Exception e)
                       {
                           await context.Response.BodyWriter.WriteAsync(null);
                       }
                        
                   }
               );
        }
    }
}
