using Makanak.Abstraction.IServices.Manager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.Services.BackgroundServices
{
    //  singelton class work behind the scene not in request
    // used IserviceProvider to create new scoped inside class
    public class BookingStatusWorker(IServiceProvider serviceProvider, ILogger<BookingStatusWorker> logger) : BackgroundService
    {
        // stopping Token : control that when the server stop it clean the worked he made and off
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // while the server run and not stop
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Booking Worker is checking for updates...");

                try
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();

                        await serviceManager.BookingService.ProcessAutomatedStatusesAsync();
                    }
                }
                catch (Exception ex) 
                {
                    logger.LogError(ex, "Error inside Booking Worker");
                }
                // Sleep for 10 minutes 
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
