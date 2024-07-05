using Microsoft.EntityFrameworkCore;
using MakeupShop.Data;
using Microsoft.AspNetCore.Identity;
using MakeupShop.Areas.Identity.Data;

namespace MakeupShop.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MakeupShopUser>>();
            IdentityResult roleResult, roleResult2;

            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }

            var roleCheck2 = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck2) { roleResult2 = await RoleManager.CreateAsync(new IdentityRole("User")); }

            MakeupShopUser user = await UserManager.FindByEmailAsync("admin@makeupshop.com");
            if (user == null)
            {
                var User = new MakeupShopUser();
                User.Email = "admin@makeupshop.com";
                User.UserName = "admin@makeupshop.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }


        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MakeupShopContext(serviceProvider.GetRequiredService<DbContextOptions<MakeupShopContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Makeup.Any() || context.Brand.Any())
                {
                    return;
                }

                context.Brand.AddRange(
                    new Brand
                    {
                        Name = "Essence",
                        Description = "Award-winning makeup brand known for providing innovative, cruelty-free beauty products at very affordable prices.",
                        ReleaseDate = DateTime.Parse("2001-2-14"),
                        Image = "photo"
                    }
                );
                context.SaveChanges();

                context.Makeup.AddRange(
                    new Makeup
                    {
                        Name = "DOUBLE TROUBLE MASCARA",
                        Description = "Bold, daring, and has a double-sided brush for a lash look that's got it all.",
                        Price = 5.99M,
                        Image = "photo",
                        Category = "Eyes",
                        BrandId = context.Brand.Single(b => b.Name == "Essence").Id
                    }
                );
                context.SaveChanges();

                context.Review.AddRange(
                    new Review
                    {
                        AppUser = "Random",
                        Comment = "SO NICE!! Love it.",
                        Rating = 5,
                        MakeupId = context.Makeup.Single(b => b.Name == "DOUBLE TROUBLE MASCARA").Id
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
