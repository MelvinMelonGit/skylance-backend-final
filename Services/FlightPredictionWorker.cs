using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using skylance_backend.Data;
using skylance_backend.Enum;
using skylance_backend.Models;

namespace skylance_backend.Services
{
    /// Background service that periodically looks for flights
    /// without a Probability and calls the MLService to fill them in.
    public class FlightPredictionWorker : BackgroundService
    {
        private readonly IServiceProvider _sp;

        public FlightPredictionWorker(IServiceProvider sp)
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
                    var pending = await db.FlightDetails
                                          .Where(b => b.Probability == null)
                                          .ToListAsync(stoppingToken);

                    foreach (var flight in pending)
                    {
                        try
                        {
                            // Call Python service for this one flight
                            // (CallSingleFlightAsync will POST /predict_f/{id})
                            var res = await ml.CallSingleFlightAsync(flight.Id);
                            flight.Probability = res.probability;
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