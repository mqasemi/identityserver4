using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string devConnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DBDataContext>(x =>
            {
                x.UseLazyLoadingProxies();
                x.UseSqlite(devConnectionString);
            });
          

             var builderIs4 = cofigurIdenttyServer(services);
            
            builderIs4.AddDeveloperSigningCredential();
            
            builderIs4.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                               builder.UseSqlite(devConnectionString,
                                   sql => sql.MigrationsAssembly(migrationsAssembly));
            }
            ).AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                           builder.UseSqlite(devConnectionString,
                               sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
             var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string prodConnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DBDataContext>(x =>
            {

                x.UseLazyLoadingProxies();
                x.UseSqlite(prodConnectionString);
            });

            var builder = cofigurIdenttyServer(services);
            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                               builder.UseSqlite(prodConnectionString,
                                   sql => sql.MigrationsAssembly(migrationsAssembly));
            }
            ).AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                           builder.UseSqlite(prodConnectionString,
                               sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            ConfigureServices(services);
        }
        public IIdentityServerBuilder cofigurIdenttyServer(IServiceCollection services)
        {
             IdentityBuilder builder = services.AddIdentity<IdentityUser, IdentityRole>(option =>
             {
                 option.Password.RequireDigit = false;
                 option.Password.RequiredLength = 4;
                 option.Password.RequireUppercase = false;
                 option.Password.RequireNonAlphanumeric = false;

             });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<DBDataContext>();
            var builderIs4 = services.AddIdentityServer(options =>
              {
                  options.Events.RaiseErrorEvents = true;
                  options.Events.RaiseInformationEvents = true;
                  options.Events.RaiseFailureEvents = true;
                  options.Events.RaiseSuccessEvents = true;

              })
              .AddAspNetIdentity<IdentityUser>();
            
            builderIs4.AddDeveloperSigningCredential();
            builder.AddDefaultTokenProviders();
            return builderIs4;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddCors(options=>{
                options.AddDefaultPolicy(
                    builder=>{
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.WithOrigins("http://localhost:4200");
                    }
                );
            });
          //  var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

           
            
            //builder.AddRoleManager<RoleManager<IdentityRole>>();
            // builder.AddRoleValidator<RoleValidator<IdentityRole>>();
            // builder.AddSignInManager<SignInManager<IdentityUser>>();

        /*     var builderIs4 = services.AddIdentityServer(options =>
              {
                  options.Events.RaiseErrorEvents = true;
                  options.Events.RaiseInformationEvents = true;
                  options.Events.RaiseFailureEvents = true;
                  options.Events.RaiseSuccessEvents = true;

              })
              //    .AddInMemoryIdentityResources(config.GetIdentityResources())
              //    .AddInMemoryApiResources(config.getApiResource())
              //    .AddInMemoryClients(config.GetClients())
              .AddAspNetIdentity<IdentityUser>()
              .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder1 =>
                               builder1.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                                   sql => sql.MigrationsAssembly(migrationsAssembly));
            }
            ).AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder1 =>
                           builder1.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                               sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            // not recommended for production - you need to store your key material somewhere secure
            builderIs4.AddDeveloperSigningCredential();
 */


         


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
            app.UseCors();
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
