using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MarketCrud.UI.Client
{
    public class Program
    {
        public const string APIServer = "http://localhost:5000";

        public static async Task Main(string[] args)
        {
            // create web assembly application
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // put blazor instance in DOM(web page source)
            builder.RootComponents.Add<App>("#app");

            // add scoped services
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(APIServer) });
            builder.Services.AddMudServices();
            builder.Services.AddMudBlazorSnackbar();
            builder.Services.AddMudBlazorDialog();

            // mud blazor service
            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
