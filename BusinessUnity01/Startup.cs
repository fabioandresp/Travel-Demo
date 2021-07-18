using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Travel.Core;

namespace BusinessUnity01
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

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<Travel.Model.Context.TDBContext>(
                    opt => Travel.Model.Context.TDBContext.FactoryBuilder(opt)
            );

            services.AddCors(o =>
                    o.AddPolicy(CoreEnvironment.AllowAllHeaders, builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials())
                );
        }

        private IApplicationBuilder App = null;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Load from vars environment or json file
            CoreEnvironment.EncryptedConnection = Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;

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
            App.UseCors();

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
