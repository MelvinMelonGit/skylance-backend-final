using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("Aircraft")]
public class Aircraft
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(255)]
    public required string Airline { get; set; }

    [Required]
    [MaxLength(50)]
    public required string FlightNumber { get; set; }

    [Required]
    [MaxLength(50)]
    public required string AircraftBrand { get; set; }

    [Required]
    [MaxLength(255)]
    public required string AircraftModel { get; set; }

    [Required]
    public required int SeatCapacity { get; set; }

    //[Required]
    public virtual ICollection<Seat>? Seats { get; set; }
    
}