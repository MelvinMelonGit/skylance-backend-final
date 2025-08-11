using skylance_backend.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("CheckInDetails")]
public class CheckInDetail
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [ForeignKey("AppUserId")]
    public virtual required AppUser AppUser { get; set; }
    
    [Required]
    [ForeignKey("FlightBookingDetailId")]
    public virtual required FlightBookingDetail FlightBookingDetail { get; set; }
    
    [Required]
    public required DateTime CheckInTime { get; set; }
    
    [Required]
    public required DateTime BoardingTime { get; set; }
    
    [Required]
    [MaxLength(50)]
    public virtual required Seat SeatNumber { get; set; }
    
    [Required]
    public required string Gate { get; set; }
    
    [Required]
    public required string Terminal { get; set; }
    public BoardingStatus BoardingStatus { get; set; }
}