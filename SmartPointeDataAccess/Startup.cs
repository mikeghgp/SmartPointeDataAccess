using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartPointe.DataAccess;
using SmartPointe.Settings;

namespace SmartPointe
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //_configuration.GetValue<string>("FeatureToggles:DeveloperExceptions")
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<NtreisDataContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("BMV");
                options.UseSqlServer(connectionString);
            });

            // Register the Configuration instance which MyOptions binds against.
            //services.Configure<SPOptions>(Configuration);
            services.Configure<SPOptions>(myOptions =>
            {
                myOptions.Option1 = Configuration.GetValue<string>("Ntreis:User");
                myOptions.Option2 = Configuration.GetValue<string>("Ntreis:Password");
                myOptions.Option3 = Configuration.GetValue<string>("Ntreis:BaseUrl");
                myOptions.Option4 = Configuration.GetValue<string>("Ntreis:BaseImageUrl");
                myOptions.Option5 = Configuration.GetValue<string>("Ntreis:BaseImageLargeUrl");
                myOptions.Option6 = Configuration.GetValue<string>("Ntreis:BaseImageHighResUrl");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
