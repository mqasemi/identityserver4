using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuthorizationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var host= CreateHostBuilder(args).Build();
           using(var scope=host.Services.CreateScope()){
            var services = scope.ServiceProvider;

                   try
                {
                    var context = services.GetRequiredService<DBDataContext>();
                    var persistedGrantContext=services.GetRequiredService<PersistedGrantDbContext>();
                    var configurationContext= services.GetRequiredService<ConfigurationDbContext>();
                    var userManager=services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleMnager =services.GetRequiredService<RoleManager<IdentityRole>>();
                    context.Database.Migrate();
                    configurationContext.Database.Migrate();
                    persistedGrantContext.Database.Migrate();
                    Seed.SeedUsers(userManager,roleMnager,persistedGrantContext,configurationContext);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                }
           }
           host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
