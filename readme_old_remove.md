------

## Install Identity Database (Microsoft .NET 10) using MySQL or MariaDB

### Objective

1. Create and test an Identity database based on the Microsoft .NET 10 standard using a MariaDB or MySQL server.
2. Run basic user and role tests: create, login, logout, delete, and component-level authorization control.

### Environment

* MySQL or MariaDB database server

* Microsoft .NET 10

* Windows 11 with WSL

* Visual Studio 2026

* Microting.EntityFrameworkCore.MySql Version=10.0.14

* Microsoft.AspNetCore.Identity.EntityFrameworkCore  Version=10.0.15

* 

  

---

### Step 1: Create the Blazor Project with Built-in Authentication

To keep things simple, we will use the **Blazor Web App** template, which will also serve for our initial tests.

* **Open Visual Studio 2026.** Create a new project using the **Blazor Web App** template.
* **Configure the project.** Select **.NET 10.0**, Interactive render mode **Server**, and Interactivity location **Global**.
* **Enable user accounts.** Under **Authentication type**, change *None* to **Individual Accounts**.
* **Create the project.** Visual Studio will generate the `Components/Account` folder containing the Login, Register, and User Profile pages.

---

### Step 2: Required NuGet Packages 

* Microsoft.EntityFrameworkCore.Design Version=10.0.14
* Microsoft.EntityFrameworkCore.Tools Version=10.0.14
* Microting.EntityFrameworkCore.MySql Version=10.0.14
* `Microsoft.EntityFrameworkCore` Version `10.0.14`
* Windows SDK (versión 10.0.x)

By default, the template installs SQL Server. Let's replace it with the Microting provider for MySQL/MariaDB.

**2.1 Open the Package Manager Console** (*Tools > NuGet Package Manager > Package Manager Console*) and run:



```powershell
dotnet package remove Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.14
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.14
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 10.0.14
dotnet add package Microting.EntityFrameworkCore.MySql --version 10.0.14
```

---

Note: The first command remove *Microsoft.EntityFrameworkCore.SqlServer* here not use MS SqlServer, we use MariaDB or MySql



**2.2 Verify the .Net 10 SDK**

Open a PowerShell terminal and check the installed SDK versions:

```bash
dotnet --list-sdks
```

You need find in reply list result some like 10.0.404 [C:\Program Files\dotnet\sdk] 

### 2.2.1. Pin the SDK Version (`global.json`)

To ensure consistent SDK selection across development environments, create a `global.json` file in your solution or project root directory:

```bash
dotnet new globaljson --sdk-version 10.0.404 --roll-forward latestFeature
```

This configuration selects the specified .NET 9 SDK feature band and allows compatible updates within that band. Verify that the generated `global.json` file matches the following structure:

```json
{
  "sdk": {
    "rollForward": "latestFeature",
    "version": "10.0.404"
  }
}
```

> **Note:** The specified SDK version (or a newer patch within the `10.0.404` band) must be installed on your machine.

### 2.2. Install the Entity Framework Core CLI

Install the global `dotnet-ef` command-line tool matching your EF Core package version:

If you need install in windows:

```bash
dotnet tool install --global dotnet-ef --version 10.0.404

```

If you need install in Linux ( script oficial ofMicrosoft )

```
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 10.0.404

```

Verify the installation:

```bash
dotnet ef --version
```

---

## 

### Step 3: Configure the Connection String in `appsettings.json`

Update **`appsettings.json`**. Replace the default connection string with your MySQL/MariaDB server details (`Server`, `Port`, `Database`, `User`, and `Password`):

```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=localhost;Port=3306;Database=your_database;User=your_user;Password=yourPassword;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

### Step 4: Adapt `Program.cs` for the New Database Environment

Modify **`Program.cs`**. Locate where `ApplicationDbContext` is registered (which originally uses `UseSqlServer`) and change it to `UseMySql`. Additionally, add `.AddRoles<IdentityRole>()` to enable role management.

> **Warning:** The code below includes simplified sign-in and password rules marked with `// TODO` for easy testing (e.g., `RequireConfirmedAccount = false` and `RequireDigit = false`). **Do not use these relaxed settings in production.**

```csharp
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using test_microtring_03.Components;
using test_microtring_03.Components.Account;
using test_microtring_03.Data;
using Microting.Web.Data.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

// TODO: Make sure this matches the 'IdentityConnection' key configured in Step 3
var connectionString = builder.Configuration.GetConnectionString("IdentityConnection")
    ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// TODO: WARNING - Simplified rules for testing purposes only. Tighten these for production.
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // Simplified password rules
    options.Password.RequiredLength = 5;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 3;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Run database seeder on startup (for testing in Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await DbSeeder.InitializeAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
```

*(Note: Setting `RequireConfirmedAccount = false` allows you to register users and log in immediately without configuring an email server).*

---

### Step 5: Generate Identity Tables in MySQL / MariaDB

1. **Delete old SQL Server migrations.** In Solution Explorer, open the `Data/Migrations` folder and **delete** the default migration files (since they were generated for SQL Server).
2. **Create and apply the new MySQL migration.** Run the following commands in the Package Manager Console:
3. be sure that your app compile before add migration
4. be sure your are in app project directory
5. If use Visual studio console be sure is selected as initial project.

Then in console run this commands, step by step

```powershell
Add-Migration IdentityMySql
Update-Database
```

This will generate all standard Identity tables in your MySQL/MariaDB database: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, etc.

---

### Step 6: Prepare Test Data (Database Seeder)

Create a seeder class to populate initial roles and test users. Create the file `Data/Seeders.cs` with the following code:

```csharp
using Microsoft.AspNetCore.Identity;
using test_microtring_03.Data;

namespace Microting.Web.Data.Seeders;

public static class DbSeeder
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Add Test Roles
        string[] roleNames = { "admin", "editor", "translator", "user" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Add Test Users
        // Note: Passwords must comply with the rules defined in Program.cs (min 5 chars, at least 1 lowercase)
        var userSeeder = new List<(ApplicationUser User, string Password, string Role)>
        {
            (new ApplicationUser { UserName = "admin@test.com", Email = "admin@test.com", EmailConfirmed = true }, "test1234", "admin"),
            (new ApplicationUser { UserName = "editor@test.com", Email = "editor@test.com", EmailConfirmed = true }, "test1234", "editor"),
            (new ApplicationUser { UserName = "translator@test.com", Email = "translator@test.com", EmailConfirmed = true }, "test1234", "translator"),
            (new ApplicationUser { UserName = "user@test.com", Email = "user@test.com", EmailConfirmed = true }, "22Algadins", "user")
        };

        foreach (var item in userSeeder)
        {
            var existingUser = await userManager.FindByEmailAsync(item.User.Email!);

            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(item.User, item.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(item.User, item.Role);
                }
            }
        }
    }
}
```

---

### Step 7: Test Authorization Controls in Blazor Components

To verify that roles and authentication work properly across your Blazor application, create a test page or update an existing component (for example, `Components/Pages/Home.razor` or a new `Components/Pages/AuthTest.razor`).

#### 7.1 Page-Level & Component-Level Authorization Example

Create `Components/Pages/AuthTest.razor` with the following code:

```razor
@page "/auth-test"
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization

<h3>Authentication & Role Authorization Test</h3>

<AuthorizeView>
    <Authorized>
        <div class="alert alert-success">
            Welcome, <strong>@context.User.Identity?.Name</strong>! You are currently logged in.
        </div>

        <h4>Role-Based UI Visibility:</h4>

        <AuthorizeView Roles="admin" Context="adminContext">
            <div class="alert alert-danger">
                <strong>Admin Panel:</strong> Visible only to users with the <code>admin</code> role.
            </div>
        </AuthorizeView>

        <AuthorizeView Roles="admin, editor" Context="editorContext">
            <div class="alert alert-warning">
                <strong>Editor Tools:</strong> Visible to both <code>admin</code> and <code>editor</code> roles.
            </div>
        </AuthorizeView>

        <AuthorizeView Roles="translator" Context="translatorContext">
            <div class="alert alert-info">
                <strong>Translator Workspace:</strong> Visible only to the <code>translator</code> role.
            </div>
        </AuthorizeView>
    </Authorized>
    <NotAuthorized>
        <div class="alert alert-secondary">
            You are not logged in. Please <a href="Account/Login">log in</a> with one of the seeded test accounts.
        </div>
    </NotAuthorized>
</AuthorizeView>
```

> **Note:** When nesting `<AuthorizeView>` components inside another `<AuthorizeView>`, always assign a distinct `Context` name (e.g., `Context="adminContext"`) to avoid variable naming collisions in Razor.

#### 7.2 Restricting Entire Pages with `@attribute [Authorize]`

If you want to block access to an entire page for unauthorized users, add the `[Authorize]` attribute at the top of any `.razor` page:

```razor
@page "/admin-only"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize(Roles = "admin")]

<h3>Restricted Admin Page</h3>
<p>Only users with the <strong>admin</strong> role can access this route.</p>
```

#### 7.3 Verification Checklist

1. **Run the application** (`F5` in Visual Studio) and verify in MySQL/MariaDB that the `AspNetUsers` and `AspNetRoles` tables are populated.
2. **Navigate to `/auth-test`** while logged out to confirm the `NotAuthorized` message appears.
3. **Log in** as `admin@test.com` (password: `test1234`) and revisit `/auth-test` to confirm both the **Admin Panel** and **Editor Tools** sections are visible.
4. **Log out** and log in as `translator@test.com` (password: `test1234`) to verify that only the **Translator Workspace** appears, and that navigating to `/admin-only` redirects to the **Access Denied** page.
5. **Test User Registration & Deletion** via `/Account/Register` and `/Account/Manage/PersonalData` to confirm full database read/write operations.

---

### Step 8: Production Readiness Checklist (Hardening)

Before deploying this application to a production environment, revert the temporary test settings introduced in earlier steps to ensure proper security.

#### 8.1 Tighten Password & Sign-In Rules in `Program.cs`

Replace the relaxed test configuration in `Program.cs` with strong password policies and account confirmation requirements:

```csharp
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // Require email confirmation before allowing sign-in
    options.SignIn.RequireConfirmedAccount = true;

    // Strong password rules for production
    options.Password.RequiredLength = 10;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 4;

    // Lockout settings to protect against brute-force attacks
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();
```

#### 8.2 Replace the No-Op Email Sender

In `Program.cs`, replace `IdentityNoOpEmailSender` with a real email service implementation (`IEmailSender<ApplicationUser>`) using SMTP or a transactional email provider (such as SendGrid, MailKit, or Azure Communication Services) so users can receive account confirmation and password reset links.

#### 8.3 Disable or Clean Up Test Seeders

* Keep `DbSeeder.InitializeAsync(...)` wrapped inside `if (app.Environment.IsDevelopment())` in `Program.cs` so test accounts with weak passwords (`test1234`) are never created in production.
* Ensure no test users (`admin@test.com`, `editor@test.com`, etc.) exist in your production database.

#### 8.4 Secure Connection Strings

Never commit real database credentials into `appsettings.json`. Store the production `IdentityConnection` string using **Environment Variables**, **User Secrets** (during local development), or a secure vault service.
