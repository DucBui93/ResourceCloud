using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RC.Models.EntityModels;
using RC.Models.Enums;

namespace RC.DataAccess
{
    public class RCDAtaContextSeed
    {
        public static void Initialize(IServiceProvider serviceProvider, RCDataContext context)
        {
            try
            {
                if (!context.Users.ToList().Any())
                {
                    Task.Run(() => CreateRolesAndUsers(serviceProvider, context)).Wait();
                }

            }
            catch (Exception e)
            {

            }
        }

        public static async Task CreateRolesAndUsers(IServiceProvider serviceProvider, RCDataContext context)
        {
            var roleManager = serviceProvider.GetService<RoleManager<Role>>();
            var userManager = serviceProvider.GetService<UserManager<User>>();

            // In Startup iam creating first Admin Role and creating a default Admin User 

            #region Create Roles

            await roleManager.CreateAsync(new Role("Admin"));

            #endregion

            #region Create User
            var user = new User();
            user.UserName = "Admin@gmail.com";
            user.Email = "Admin@gmail.com";
            user.FirstName = "AdminFirstName";
            user.LastName = "AdminLastName";
            user.Status = UserStatus.Active;
            string userPWD = "123@123Aa";

            await userManager.CreateAsync(user, userPWD);

            await context.SaveChangesAsync();

            #endregion
        }
    }
}