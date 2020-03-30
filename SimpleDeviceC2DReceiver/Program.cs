using System;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleDeviceC2DReceiver
{
    class Program
    {
        static string DeviceConnectionString = "HostName=IoT-Hub-FTA-Scenario.azure-devices.net;DeviceId=c2d-test-device;SharedAccessKey=LTvMDOkteAw8jdNqOZyKoaCzRndEfgY7d84sL/tVnRg=";
        public static DeviceClient Client = null;

        static void Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                     .ConfigureServices(services =>
                     {
                         services.AddLogging();
                         services.AddHostedService<C2DMessageReceiver>();
                     }
                   );

            hostBuilder.ConfigureLogging(
                loggingOptions => loggingOptions.AddConsole(opt => opt.TimestampFormat = "[HH:mm:ss] ").AddDebug());

            hostBuilder.UseConsoleLifetime();

            var host = hostBuilder.Build();

            ILogger logger = host.Services.GetService<ILogger<Program>>();

            logger.LogInformation("Connecting to hub");

            Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString,
                  TransportType.Mqtt);

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
