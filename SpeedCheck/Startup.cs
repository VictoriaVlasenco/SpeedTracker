using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpeedCheck.BusinessLogic;
using SpeedCheck.Binders;
using SpeedCheck.DAL.Models;
using SpeedCheck.DAL.Repositories;
using SpeedCheck.DAL.Repositories.Infrastructure;

namespace SpeedCheck
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(IRepository<TrackingData>), typeof(FileRepository));
            services.AddTransient<ITrackService, TrackService>();
            services.AddControllers(x =>
            {
                x.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
            }).AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.Converters.Add(new TrackingDataDateConverter());
            });
        }
             

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
