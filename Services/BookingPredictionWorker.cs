using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Enum;

namespace skylance_backend.Services
{
    /// Background service that periodically looks for bookings
    /// without a Prediction and calls the MLService to fill them in.
    public class BookingPredictionWorker : BackgroundService
    {
        private readonly IServiceProvider _sp;

        public BookingPredictionWorker(IServiceProvider sp)
            => _sp = sp;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SkylanceDbContext>();
                    var ml = scope.ServiceProvider.GetRequiredService<MLService>();

                    // Grab any new bookings (Prediction == null)
                    var pending = await db.FlightBookingDetails
                                          .Where(b => b.Prediction == null)
                                          .ToListAsync(stoppingToken);

                    foreach (var booking in pending)
                    {
                        try
                        {
                            // Call Python service for this one booking
                            // (CallSingleAsync will POST /predict/{id})
                            var res = await ml.CallSingleAsync(booking.Id);
                            booking.Prediction = (Prediction)res.prediction;
                        }
                        catch
                        {
                            // swallowing so we retry next cycle
                        }
                    }

                    if (pending.Count > 0)
                        await db.SaveChangesAsync(stoppingToken);
                }
                catch
                {
                    // top‐level protection so the loop never dies
                }

                // wait 15 seconds before checking again
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
