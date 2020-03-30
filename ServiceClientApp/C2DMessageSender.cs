using Microsoft.Azure.Devices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceClientApp
{
    public class C2DMessageSender : BackgroundService
    {
        private readonly ILogger _logger;

        public C2DMessageSender(ILogger<C2DMessageSender> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            //Do your preparation (e.g. Start code) here
            _logger.LogInformation("Starting to send C2D Messages to the device");

            string targetDeviceId = Program.TargetDevice;

            int sequence = 0;

            while (!stopToken.IsCancellationRequested)
            {
                object c2dMessagePayload = new { Content = "test123" , Time = DateTime.UtcNow , Sequence = sequence++ };

                string c2dMessagePayloadJson = JsonConvert.SerializeObject(c2dMessagePayload);

                byte[] c2dMessagePayloadBbytes = Encoding.UTF8.GetBytes(c2dMessagePayloadJson);

                _logger.LogInformation($"Sending C2D Messages to {targetDeviceId}: {c2dMessagePayloadJson}");

                Message c2dMessage = new Message(c2dMessagePayloadBbytes);

                c2dMessage.ExpiryTimeUtc = DateTime.UtcNow.AddMinutes(5);
                

                c2dMessage.MessageId = Guid.NewGuid().ToString();
                c2dMessage.Ack = DeliveryAcknowledgement.Full;

                try
                {
                    await Program.Client.SendAsync(targetDeviceId, c2dMessage);

                    _logger.LogInformation($"Message sent with MessageId { c2dMessage.MessageId }");

                    _logger.LogInformation("Let's wait for the feedback...");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred while sending a C2D message!");
                }

                await Task.Delay(15000);
            }
        }
    }
}
