using System;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceClientApp
{
    class Program
    {
        static RegistryManager registryManager;
        static string IoTHubServiceConnectionString = "HostName=IoT-Hub-FTA-Scenario.azure-devices.net;SharedAccessKeyName=c2d-test;SharedAccessKey=G8+DnLL9RENM1q68sCzy7i62ReF0FWr3UOtFzPJW63A=";
        public static ServiceClient Client = null;
        public static string TargetDevice = "c2d-test-device";

        static void Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
           .ConfigureServices(services =>
               {
                   services.AddLogging();
                   services.AddHostedService<C2DMessageFeedbackReceiver>();   
                   services.AddHostedService<C2DMessageSender>();
               }
           );

            hostBuilder.ConfigureLogging(
                loggingOptions => loggingOptions.AddConsole(opt => opt.TimestampFormat = "[HH:mm:ss] ").AddDebug());

            hostBuilder.UseConsoleLifetime();

            var host = hostBuilder.Build();

            ILogger logger = host.Services.GetService<ILogger<Program>>();

            logger.LogInformation("Connecting to IoT Hub Service interface");

            Client = ServiceClient.CreateFromConnectionString(IoTHubServiceConnectionString);

            logger.LogInformation("Start backgrund tasks");

            try
            {
                host.Run();
            }
            catch (OperationCanceledException ex)
            {
                logger.LogInformation("Exiting...");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while executing the app, see stack trace...");
            }
        }
    }
    
}
