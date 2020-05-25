using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpeedCheck.BusinessLogic;
using SpeedCheck.Binders;
using SpeedCheck.DAL.Models;
using SpeedCheck.DAL.Repositories;
using SpeedCheck.DAL.Repositories.Infrastructure;
using System.Text.Json.Serialization;
//using System.Text.Json;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;
//using Newtonsoft.Json;

namespace SpeedCheck
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
            services.AddTransient(typeof(IRepository<TrackingData>), typeof(FileRepository));
            services.AddTransient<ITrackService, TrackService>();
            services.AddControllers(x =>
            {
                x.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
            })//.AddNewtonsoftJson();

                .AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.Converters.Add(new LeadConverter());
            });
        }
        //public class DateJsonConverter : JsonConverter<DateTime>
        //{
        //    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(DateTime);

        //    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert,
        //        JsonSerializerOptions options)
        //    {
        //        DateTime dateTime = reader.GetDateTime();
        //        return new Date(dateTime.Year, dateTime.Month, dateTime.Day);
        //    }

        //    public override void Write(Utf8JsonWriter writer, Date value, JsonSerializerOptions options)
        //    {
        //        const string Format = "yyyy-MM-dd";
        //        writer.WriteStringValue(new DateTime(value.Year, value.Month, value.Day).ToString(Format));
        //    }
        //}
        public class LeadConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new System.NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jsonSerializer = new JsonSerializer
                {
                    DateFormatString = "dd.MM.yyyy"
                };

                return jsonSerializer.Deserialize<Models.TrackingData>(reader);
            }

            public override bool CanConvert(Type objectType) => objectType == typeof( Models.TrackingData );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
