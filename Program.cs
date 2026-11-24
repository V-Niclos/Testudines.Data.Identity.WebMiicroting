using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdetityNet10MySql.Components;
using IdetityNet10MySql.Components.Account;
using IdetityNet10MySql;
using IdetityNet10MySql.Data.Seeders;
using IdetityNet10MySql.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Register Extenion 
Program_ServiceCollectionExtensions.AddServiceCollection(builder.Services, builder.Configuration);

var app = builder.Build();

// 2. Do Seeders only if not development
await Program_ApplicationExtensions.InitializeSeedersAsync(app);

// 3. Config  pipeline HTTP
Program_ApplicationExtensions.ConfigurePipeline(app);

app.Run();
