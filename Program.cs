//#define Custom

using TG.Blazor.IndexedDB;
using FPSHome;
using FPSHome.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<InitializationService>();
#if Custom
builder.Services.AddTransient<CustomHttpClientHandler>();

builder.Services.AddHttpClient("customClient")
    .ConfigurePrimaryHttpMessageHandler<CustomHttpClientHandler>();

builder.Services.AddScoped<FandomService>();
#endif

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
