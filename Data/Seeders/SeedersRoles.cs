using Microsoft.AspNetCore.Identity;

namespace Testudines.Data.Identity.WebMiicroting.Data.Seeders
{

    public static class DbSeeder
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = ["admin", "editor", "translator", "user"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    EnsureSucceeded(result, $"creating role '{role}'");
                }
            }

            var users = new (string Email, string Password, string Role)[]
            {
            ("admin@test.local", "DevOnly-Admin-123", "admin"),
            ("editor@test.local", "DevOnly-Editor-123", "editor"),
            ("translator@test.local", "DevOnly-Translator-123", "translator"),
            ("user@test.local", "DevOnly-User-123", "user")
            };

            foreach (var item in users)
            {
                var user = await userManager.FindByEmailAsync(item.Email);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        UserName = item.Email,
                        Email = item.Email,
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(user, item.Password);
                    EnsureSucceeded(createResult, $"creating user '{item.Email}'");
                }

                if (!await userManager.IsInRoleAsync(user, item.Role))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, item.Role);
                    EnsureSucceeded(roleResult, $"assigning role '{item.Role}' to '{item.Email}'");
                }
            }
        }

        private static void EnsureSucceeded(IdentityResult result, string operation)
        {
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Identity failed while {operation}: {errors}");
            }
        }
    }
}
