using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using SuperShop.Helpers;
using Syncfusion.Blazor;
using System.Globalization;
using System.Text;

namespace BilheticaAeronauticaWeb
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // =========================================================
            // BASE DE DADOS
            //
            // Development -> SQL Server local
            // Production  -> PostgreSQL
            // =========================================================
            services.AddDbContext<DataContext>(options =>
            {
                var connectionString =
                    Configuration.GetConnectionString("DefaultConnection");

                if (_env.IsDevelopment())
                {
                    options.UseSqlServer(connectionString);
                }
                else
                {
                    options.UseNpgsql(connectionString);
                }
            });


            // =========================================================
            // IDENTITY
            // =========================================================
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();


            // =========================================================
            // AUTENTICAÇÃO
            // =========================================================
            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                Configuration["Tokens:Issuer"],

                            ValidAudience =
                                Configuration["Tokens:Audience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        Configuration["Tokens:Key"]
                                    )
                                )
                        };
                });


            // =========================================================
            // MVC / RAZOR
            // =========================================================
            services.AddRazorPages();
            services.AddControllersWithViews();


            // =========================================================
            // HELPERS
            // =========================================================
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IBlobHelper, BlobHelper>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IEMailHelper, EMailHelper>();


            // =========================================================
            // DATABASE SEED
            // =========================================================
            services.AddTransient<SeedDb>();


            // =========================================================
            // REPOSITÓRIOS
            // =========================================================
            services.AddScoped<IAeroportoRepository, AeroportoRepository>();
            services.AddScoped<IAviaoRepository, AviaoRepository>();
            services.AddScoped<IPassageiroRepository, PassageiroRepository>();
            services.AddScoped<IVooRepository, VooRepository>();
            services.AddScoped<ILugarRepository, LugarRepository>();
            services.AddScoped<IBilheteRepository, BilheteRepository>();


            // =========================================================
            // SERVIÇOS
            // =========================================================
            services.AddScoped<IBilheteService, BilheteService>();
            services.AddScoped<IVooService, VooService>();


            // =========================================================
            // BLAZOR / COMPONENTES
            // =========================================================
            services.AddMudServices();
            services.AddSyncfusionBlazor();


            // =========================================================
            // HTTP CONTEXT
            // =========================================================
            services.AddHttpContextAccessor();


            // =========================================================
            // FLUENT VALIDATION
            // =========================================================
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining
                <VooViewModelValidator>();


            // =========================================================
            // SOFT DELETE
            // =========================================================
            services.AddScoped<SoftDeleteInterceptor>();


            // =========================================================
            // CULTURA PORTUGUESA
            // =========================================================
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("pt-PT")
                };

                options.DefaultRequestCulture =
                    new RequestCulture("pt-PT");

                options.SupportedCultures =
                    supportedCultures;

                options.SupportedUICultures =
                    supportedCultures;
            });


            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/NotAuthorized";
            });
        }


        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            // =========================================================
            // ERROS
            // =========================================================
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Error");
                app.UseHsts();
            }


            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern:
                        "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }
    }
}