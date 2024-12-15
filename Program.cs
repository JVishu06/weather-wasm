using wasm;
using wasm.Sevices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add Authorization services
builder.Services.AddAuthorizationCore();

// Register Root Components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register WeatherService and LocalStorage services
builder.Services.AddScoped<WeatherService>();
builder.Services.AddBlazoredLocalStorage();

// Add Custom Authentication and HTTP Handler services
builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

// Use a dynamic base address for HttpClient
var baseAddress = builder.HostEnvironment.IsProduction()
    ? "https://webapi-8j7b.onrender.com" // Production Render API
    : "https://localhost:5001";          // Local Development API

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(baseAddress)
});

// Add HTTP Client for authentication
builder.Services.AddHttpClient("Auth", opt => opt.BaseAddress =
    new Uri(baseAddress)) // Use the dynamic API URL for authentication
    .AddHttpMessageHandler<CustomHttpHandler>();

await builder.Build().RunAsync();
