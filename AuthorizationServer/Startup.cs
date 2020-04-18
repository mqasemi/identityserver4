using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



namespace AuthorizationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {string devConnectionString=Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DBDataContext>(x =>
            {
                x.UseLazyLoadingProxies();
                x.UseSqlite(devConnectionString);
            });
             var builder=cofigurIdenttyServer(services);
        builder.AddConfigurationStore(options=>{
            options.ConfigureDbContext=x=> x.UseSqlite(devConnectionString);
        }).AddOperationalStore(options=>{
            options.ConfigureDbContext=x=> x.UseSqlite(devConnectionString);
        });

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        { string prodConnectionString=Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DBDataContext>(x =>
            {
               
                x.UseLazyLoadingProxies();
                x.UseSqlite(prodConnectionString);
            });
        
        var builder=cofigurIdenttyServer(services);
        builder.AddConfigurationStore(options=>{
            options.ConfigureDbContext=x=> x.UseSqlite(prodConnectionString);
        }).AddOperationalStore(options=>{
            options.ConfigureDbContext=x=> x.UseSqlite(prodConnectionString);
        });

            ConfigureServices(services);
        }
        public IIdentityServerBuilder cofigurIdenttyServer(IServiceCollection services){
             var builderIs4 = services.AddIdentityServer(options =>
               {
                   options.Events.RaiseErrorEvents = true;
                   options.Events.RaiseInformationEvents = true;
                   options.Events.RaiseFailureEvents = true;
                   options.Events.RaiseSuccessEvents = true;

               })
            //    .AddInMemoryIdentityResources(config.GetIdentityResources())
            //    .AddInMemoryApiResources(config.getApiResource())
            //    .AddInMemoryClients(config.GetClients())
               .AddAspNetIdentity<IdentityUser>();
            // not recommended for production - you need to store your key material somewhere secure
            builderIs4.AddDeveloperSigningCredential();
            return builderIs4;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            IdentityBuilder builder = services.AddIdentity<IdentityUser, IdentityRole>(option =>
             {
                 option.Password.RequireDigit = false;
                 option.Password.RequiredLength = 4;
                 option.Password.RequireUppercase = false;
                 option.Password.RequireNonAlphanumeric = false;

             });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<DBDataContext>();
            // builder.AddRoleManager<RoleManager<IdentityRole>>();
            // builder.AddRoleValidator<RoleValidator<IdentityRole>>();
            // builder.AddSignInManager<SignInManager<IdentityUser>>();
            builder.AddDefaultTokenProviders();


            // services.AddMvc(MvcOptions =>
            // {
            //     MvcOptions.EnableEndpointRouting = false;
            // });
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
             app.UseAuthorization();
            // app.UseStaticFiles();
            // app.UseMvcWithDefaultRoute();
            app.UseEndpoints(endpoints =>
                       {
                           endpoints.MapDefaultControllerRoute();
                       });




        }
    }
}
