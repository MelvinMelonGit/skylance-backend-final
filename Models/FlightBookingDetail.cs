using skylance_backend.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace skylance_backend.Models;

[Table("FlightBookingDetails")]
public class FlightBookingDetail
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("FlightDetailId")]
    public virtual required FlightDetail FlightDetail { get; set; }

    [Required]
    [ForeignKey("BookingDetailId")]
    public virtual required BookingDetail BookingDetail { get; set; }

    [Required]
    public required double BaggageAllowance { get; set; }
    public double BaggageChecked { get; set; }
    public DateTime BookingDate { get; set; }
    public TravelPurpose? TravelPurpose { get; set; }
    public Class? Class { get; set; }

    [MaxLength(50)]
    public virtual Seat? SeatNumber { get; set; }

    [Required]
    public required bool RequireSpecialAssistance { get; set; }

    [Required]
    [MaxLength(50)]
    public required BookingStatus BookingStatus { get; set; }

    [Required]
    [MaxLength(50)]
    public required int Fareamount { get; set; }

    public Prediction? Prediction { get; set; }
    public SpecialRequest? SpecialRequest { get; set; }

}