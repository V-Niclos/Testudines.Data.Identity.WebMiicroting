using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;
namespace IdetityNet10MySql.Data.Seeders;


public static class SeederUserRoles

{
    public static readonly string Test_Password = "1234abcd";
    public static readonly string Test_MailServer = "@test.com";
    public static string[] Test_RolesList { get { return rolesAddArray; } }
public static IReadOnlyList<SeederUsersAdd> Test_UsersList => usersAddList.AsReadOnly();

    private static string[] rolesAddArray = ["admin", "editor", "translator", "reader"];
    //..........................
    private static List<SeederUsersAdd> usersAddList =
[
  new()
    {
        userNameEmail = "admin"+Test_MailServer,
        userPaswod = Test_Password,
        userRole = ["Admin", "editor", "translator"]
    },
    new()
    {
        userNameEmail = "editor"+Test_MailServer,
           userPaswod = Test_Password,
        userRole = ["editor"]
    },
    new()
    {
        userNameEmail = "translator"+Test_MailServer,
             userPaswod = Test_Password,
        userRole = ["translator" ]
    },
        new()
    {
        userNameEmail = "reader"+Test_MailServer,
         userPaswod = Test_Password,
        userRole = ["reader"]
    },
      new()
    {
        userNameEmail = "notrole"+Test_MailServer,
         userPaswod = Test_Password,
        userRole = null
    },
    new()
    {
        // Ejemplo probando propiedades nulas
        userNameEmail = "invalid_user",
        userPaswod = null,
        userRole = null
    }
];
    //..........................
    public static async Task InitializeAsync(IServiceProvider services)
    {
      
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Crear los roles si no existen
        foreach (var role in rolesAddArray)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                EnsureSucceeded(result, $"creating role '{role}'");
            }
        }

        // 2. Crear usuarios y asignar múltiples roles
        try
        {
            foreach (var item in usersAddList)
            {
                // Evitar fallo si el email es nulo o vacío
                if (string.IsNullOrWhiteSpace(item.userNameEmail)) continue;

                var user = await userManager.FindByEmailAsync(item.userNameEmail);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        UserName = item.userNameEmail,
                        Email = item.userNameEmail,
                        EmailConfirmed = true
                    };

                    // Si tiene password usamos CreateAsync con password, si es null lo creamos sin password
                    IdentityResult createResult = !string.IsNullOrWhiteSpace(item.userPaswod)
                        ? await userManager.CreateAsync(user, item.userPaswod)
                        : await userManager.CreateAsync(user);

                    EnsureSucceeded(createResult, $"creating user '{item.userNameEmail}'");
                }

                // 3. Recorrer el array de roles si no es nulo
                if (item.userRole is { Length: > 0 })
                {
                    foreach (var role in item.userRole)
                    {
                        if (!await userManager.IsInRoleAsync(user, role))
                        {
                            var roleResult = await userManager.AddToRoleAsync(user, role);
                            EnsureSucceeded(roleResult, $"assigning role '{role}' to '{item.userNameEmail}'");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error class DbSeeder: {ex.Message}");
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

    //..........................
    //..........................
    //..........................



