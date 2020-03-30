using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleDeviceC2DReceiver
{
    public class C2DMessageReceiver : BackgroundService
    {
        private readonly ILogger _logger;

        public C2DMessageReceiver(ILogger<C2DMessageReceiver> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            //Do your preparation (e.g. Start code) here
            _logger.LogInformation("Starting to monitor C2D Messages incoming into the device");
            
            while (!stopToken.IsCancellationRequested)
            {
                try
                {
                    var message = await Program.Client.ReceiveAsync();

                    if (message != null)
                    {
                        _logger.LogInformation($"Received a C2D message with MessageId {message.MessageId} and LockToken {message.LockToken}");

                        byte[] messagePayload = message.GetBytes();
                        string messagePayloadJson = Encoding.UTF8.GetString(messagePayload);

                        _logger.LogInformation($"Message decoded: {messagePayloadJson}");

                        //ok just ack the message
                        await Program.Client.CompleteAsync(message.LockToken);

                        _logger.LogInformation($"Sent the feedback for a C2D message with MessageId {message.MessageId} and LockToken {message.LockToken}");

                        _logger.LogInformation("Now waiting for the next C2D message!");
                    }
                    else
                    {
                        _logger.LogInformation("C2D Message Receiver timeouted, let's just continue again.");
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Error while receiving a C2D message");
                }
            }

            //Do your cleanup (e.g. Stop code) here
            _logger.LogInformation("Stopping to monitor C2D Messages incoming into the device");
        }
    }
}
