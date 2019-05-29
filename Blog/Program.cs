using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            try
            { 

                var scope = host.Services.CreateScope();

                var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var usrMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                ctx.Database.EnsureCreated();

                var adminRole = new IdentityRole("Admin");

                if (!ctx.Roles.Any())
                {
                    //create Role
                    roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();


                }

                if (!ctx.Users.Any(user => user.UserName == "admin"))
                {
                    //create an Admin

                    var adminUser = new IdentityUser
                    {
                        UserName = "Admin",
                        Email = "admin@gmail.com"
                    };

                    usrMgr.CreateAsync(adminUser, "admin").GetAwaiter().GetResult(); ;
                    //add role to user
                    usrMgr.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            host.Run();
        }
            
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
