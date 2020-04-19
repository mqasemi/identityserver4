using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AuthorizationServer.Models
{
    public class Seed
    {
        public static void SeedUsers(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        PersistedGrantDbContext persistedGrantDb, ConfigurationDbContext configurationDb)
        {

            if (!userManager.Users.Any())
            {

                var userData = System.IO.File.ReadAllText("Models/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<IdentityUser>>(userData);
                var roles = new List<IdentityRole>{
                    new IdentityRole{Name="Member"},
                    new IdentityRole{Name="Admin"},
                    new IdentityRole{Name="Moderator"},
                    new IdentityRole{Name="VIP"}};
                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }
                foreach (var user in users)
                {
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "Member");
                }
                var adminUser = new IdentityUser { UserName = "admin" };
                var result = userManager.CreateAsync(adminUser, "password").Result;
                if (result.Succeeded)
                {
                    var createdAdmin = userManager.FindByNameAsync("admin").Result;
                    userManager.AddToRolesAsync(createdAdmin, new[] { "Admin", "Moderator" });
                }


            }

            if (!configurationDb.Clients.Any())
            {
                foreach (var client in config.GetClients())
                {
                    configurationDb.Clients.Add(client.ToEntity());
                }
                configurationDb.SaveChanges();
            }

            if (!configurationDb.IdentityResources.Any())
            {
                foreach (var resource in config.GetIdentityResources())
                {
                    configurationDb.IdentityResources.Add(resource.ToEntity());
                }
                configurationDb.SaveChanges();
            }

            if (!configurationDb.ApiResources.Any())
            {
                foreach (var resource in config.getApiResource())
                {
                    configurationDb.ApiResources.Add(resource.ToEntity());
                }
                configurationDb.SaveChanges();
            }

        }
    }
}