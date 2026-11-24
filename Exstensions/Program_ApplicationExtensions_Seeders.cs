//-----------------------------------------------------
// Program_ApplicationExtensions_Seeders.cs
// Seeder centralization
// RunProductionSeedersAsync-> All environmet
// RunDevelopmentSeedersAsync-> Only Environment.IsDevelopment
// 2026-11-23
//-----------------------------------------------------
using IdetityNet10MySql.Components;
using IdetityNet10MySql.Components.Account;
using IdetityNet10MySql.Data.Seeders;

namespace IdetityNet10MySql.Extensions;

public static partial class Program_ApplicationExtensions
{
    public static async Task InitializeSeedersAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        // 1. SEEDERS DE PRODUCCIÓN / SIEMPRE ACTIVOS
        // Datos maestros imprescindibles para que la app funcione (ej. Roles, Admin inicial, catálogos base)
        await RunProductionSeedersAsync(services);

        // 2. SEEDERS EXCLUSIVOS DE DESARROLLO
        // Datos de prueba, usuarios ficticios, volcado de test
        if (app.Environment.IsDevelopment())
        {
            await RunDevelopmentSeedersAsync(services);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static async Task RunProductionSeedersAsync(IServiceProvider services)
    {
        await SeederUserRoles.InitializeAsync(services);
        // await SeederCatalogosBase.InitializeAsync(services);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static async Task RunDevelopmentSeedersAsync(IServiceProvider services)
    {
        // await SeederUsuariosPrueba.InitializeAsync(services);
        // await SeederDatosFicticios.InitializeAsync(services);
    }

}