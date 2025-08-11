using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace skylance_backend.Models
{
    public class Seat
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("FlightDetailId")]
        public virtual FlightDetail? FlightDetail { get; set; }
        public  string? SeatNumber { get; set; } // e.g., "12A"
        public bool IsAssigned { get; set; } = false;
    }
}