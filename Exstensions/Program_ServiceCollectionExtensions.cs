using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdetityNet10MySql.Components.Account;
using IdetityNet10MySql;

namespace IdetityNet10MySql.Extensions;

public static class Program_ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceCollection( IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Add_DatabaseContext(configuration)
            .Add_ScopedServices(configuration)
            .Add_IdentityConfiguration()
            .Add_BlazorConfiguration()
            .Add_SingletonServices(configuration);
    }

    private static IServiceCollection Add_BlazorConfiguration(this IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        return services;
    }
    //----------------------------------------
    //  context
    //----------------------------------------
    private static IServiceCollection Add_DatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityConnection")
            ?? throw new InvalidOperationException("Connection string 'IdentityConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }
    //----------------------------------------
    // Identity
    //----------------------------------------
    private static IServiceCollection Add_IdentityConfiguration(this IServiceCollection services)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredUniqueChars = 1;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        return services;
    }
    //----------------------------------------
    // Scopped Services
    //----------------------------------------
    private static IServiceCollection Add_ScopedServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registra aquí tus servicios Scoped
        return services;
    }
    //----------------------------------------
    // Sigleton services
    //----------------------------------------
    private static IServiceCollection Add_SingletonServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registra aquí tus servicios Singleton
        return services;
    }
}
