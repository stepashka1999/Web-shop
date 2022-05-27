using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web_shop.DataAccess.Data;
using Web_shop.DataAccess.Initializer;
using Web_shop.DataAccess.Repository;
using Web_shop.Models;
using Web_shop.Utility;
using Web_shop.Utility.BrainTree;

namespace Web_shop
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();

            services.AddScoped<IRepository<Category>, CategoryRepository>();
            services.AddScoped<IRepository<Product>, ProductRepository>();
            services.AddScoped<IRepository<ApplicationUser>, ApplicationUserRepository>();
            services.AddScoped<IRepository<ApplicationType>, ApplicationTypeRepository>();
            services.AddScoped<IRepository<InquiryHeader>, InquiryHeaderRepository>();
            services.AddScoped<IRepository<InquiryDetail>, InquiryDetailRepository>();
            services.AddScoped<IRepository<OrderHeader>, OrderHeaderRepository>();
            services.AddScoped<IRepository<OrderDetail>, OrderDetailRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddHttpContextAccessor();
            services.AddSession(opt =>
            {
                opt.IdleTimeout = System.TimeSpan.FromMinutes(10);
                opt.Cookie.HttpOnly = true;
                opt.Cookie.IsEssential = true;
            });

            services.AddAuthentication()
                .AddFacebook(options =>
                {
                    options.AppId = "267080392231775";
                    options.AppSecret = "3f0ccf73281c1c2e7b124fc91db9c823";
                });

            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
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
            dbInitializer.Initialize();

            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}