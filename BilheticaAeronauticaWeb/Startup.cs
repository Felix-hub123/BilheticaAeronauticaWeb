using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Identity;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.Extensions.Azure;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;

namespace BilheticaAeronauticaWeb
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
            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

          
            services.AddRazorPages();
            services.AddControllersWithViews();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IBlobHelper, BlobHelper>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddTransient<SeedDb>();
            services.AddScoped<IAeroportoRepository, AeroportoRepository>();
            services.AddScoped<IAviaoRepository, AviaoRepository>();
            services.AddScoped<IPassageiroRepository, PassageiroRepository>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IVooRepository, VooRepository>();


            services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, BilheticaAeronauticaWeb.Helper.EmailSender>();

                   
        }
           

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();


            app.UseAuthorization();

           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
