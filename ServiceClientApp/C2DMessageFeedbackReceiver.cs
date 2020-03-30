using Microsoft.Azure.Devices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceClientApp
{
    public class C2DMessageFeedbackReceiver : BackgroundService
    {
        private readonly ILogger _logger;

        public C2DMessageFeedbackReceiver(ILogger<C2DMessageFeedbackReceiver> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            _logger.LogInformation("Starting to receive feedback for C2D Messages from the device");

            var feedbackReceiver = Program.Client.GetFeedbackReceiver();
            
            string targetDeviceId = Program.TargetDevice;

            while (!stopToken.IsCancellationRequested)
            {
                try
                {
                    var feedbackBatch = await feedbackReceiver.ReceiveAsync(TimeSpan.FromMinutes(2.0));

                    if (feedbackBatch == null)
                    {
                        _logger.LogInformation("FeedbackReceiver timeouted, let's just continue again.");
                        continue;
                    }

                    var feedbacks = feedbackBatch.Records.ToList();

                    _logger.LogInformation($"Received {feedbacks.Count} feedback for C2D message");

                    foreach (var feedback in feedbacks)
                    {
                        _logger.LogInformation($"Feedback from {feedback.DeviceId} for MessageId {feedback.OriginalMessageId} is {feedback.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while receiving feedback for C2D message");
                }          
            }
        }
    }
}
