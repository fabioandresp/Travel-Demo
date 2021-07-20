using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Travel.Security
{
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

            App.UseHttpsRedirection();
            App.UseStaticFiles();
            App.UseHttpsRedirection();
            App.UseRouting();
            App.UseAuthorization();

            //Complemento para mapear las rutas de las anotaciones, esto se hace por cada clase en runtime.
            App.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            App.Use(
                    async (context, next) =>
                    {
                        //Retornar código de error y llevar control de los requets incorrectos.
                        context.Response.StatusCode = 404;
                        await context.Response.BodyWriter.WriteAsync(null);

                        //Allow buffer on Body
                        context.Request.EnableBuffering();
                    }
                );

        }
    }
}
