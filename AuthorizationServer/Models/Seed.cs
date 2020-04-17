using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AuthorizationServer.Models
{
    public class Seed
    {
         public static void SeedUsers(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager)
        {

            if (!userManager.Users.Any())
            {
                
                var userData = System.IO.File.ReadAllText("Models/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<IdentityUser>>(userData);
                var roles=new List<IdentityRole>{
                    new IdentityRole{Name="Member"},
                    new IdentityRole{Name="Admin"},
                    new IdentityRole{Name="Moderator"},
                    new IdentityRole{Name="VIP"}};
                foreach(var role in roles){
                     roleManager.CreateAsync(role).Wait();
                }
                foreach (var user in users)
                {
                   userManager.CreateAsync(user,"password").Wait();
                   userManager.AddToRoleAsync(user,"Member");
                }
                var adminUser=new IdentityUser{UserName="admin"};
                var result=userManager.CreateAsync(adminUser,"password").Result;
                if(result.Succeeded){
                    var createdAdmin=userManager.FindByNameAsync("admin").Result;
                    userManager.AddToRolesAsync(createdAdmin,new []{"Admin","Moderator"});
                }

               
            }
        }
    }
}