using IdetityNet10MySql.Components;
using IdetityNet10MySql.Components.Account;
using IdetityNet10MySql.Data.Seeders;

namespace IdetityNet10MySql.Extensions;

public static partial class Program_ApplicationExtensions
{
    //----------------------------------------
    // public 
    public static WebApplication ConfigurePipeline( WebApplication app)
    {
        ConfigureDevelopment(app);
        ConfigureProduction(app);
        ConfigureCommonMiddlewares(app);

        return app;
    }
   
    private static void ConfigureDevelopment(WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;

        app.UseMigrationsEndPoint();
    }

    private static void ConfigureProduction(WebApplication app)
    {
        if (app.Environment.IsDevelopment()) return;

        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    private static void ConfigureCommonMiddlewares(WebApplication app)
    {
        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        app.UseHttpsRedirection();
        app.UseAntiforgery();
        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapAdditionalIdentityEndpoints();
    }
}
