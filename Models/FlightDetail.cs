using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using skylance_backend.Enum;

namespace skylance_backend.Models;

[Table("FlightDetails")]
public class FlightDetail
{
    [Key]
    [MaxLength(255)]
 
    public int Id { get; set; }

    [Required]
    [ForeignKey("AircraftId")]
    public virtual required Aircraft Aircraft { get; set; }

    [Required]
    [ForeignKey("OriginAirportId")]
    public virtual required Airport OriginAirport { get; set; }

    [Required]
    [ForeignKey("DestinationAirportId")]
    public virtual required Airport DestinationAirport { get; set; }

    [Required]
    public required DateTime DepartureTime { get; set; }

    [Required]
    public required DateTime ArrivalTime { get; set; }

    [Required]
    public required bool IsHoliday { get; set; }

    [Required]
    [MaxLength(50)]
    public required string FlightStatus { get; set; }

    [Required]
    public required int CheckInCount { get; set; }

    [Required]
    public required int SeatsSold { get; set; }

    [Required]
    public required double Distance { get; set; }

    public int? NumberOfCrew { get; set; }

    public Prediction? Prediction { get; set; }     

    public float? Probability { get; set; }
    public int? OverbookingCount { get; set; }

    [NotMapped] 
    public double Compensation => Distance / 4;

    [NotMapped]
    public double RebookingCompensation => 1.5*Distance / 4;


}