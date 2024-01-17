using System;
using Azure.Identity;
using MedvetitleBot.Gateway.Interfaces;
using MedvetitleBot.Gateway.Options;
using MedvetitleBot.Gateway.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MedvetitleBot.Gateway.Startup))]
namespace MedvetitleBot.Gateway
{
    public class Startup : FunctionsStartup
    {
        private IConfigurationRoot _functionConfig;
        private GatewayOptions _gatewayOptions = new();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _functionConfig = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.Configure<GatewayOptions>(_functionConfig.GetSection(nameof(GatewayOptions)));
            _functionConfig.GetSection(nameof(GatewayOptions)).Bind(_gatewayOptions);

            builder.Services.AddAzureClients(clientBuilder => {
                clientBuilder.UseCredential(new DefaultAzureCredential());

                if (Uri.TryCreate(_gatewayOptions.TableServiceConnection, UriKind.Absolute, out var tableServiceUri))
                    clientBuilder.AddTableServiceClient(tableServiceUri);
                else
                    clientBuilder.AddTableServiceClient(_gatewayOptions.TableServiceConnection);
            });

            builder.Services.AddAutoMapper(typeof(Startup));

            builder.Services.AddScoped<IStorageRepository, StorageRepository>();
        }
    }
}

