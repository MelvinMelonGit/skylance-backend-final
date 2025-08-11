using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
namespace skylance_backend.Models;
[Table("AirlineRevenue")]
public class AirlineRevenue
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    [MaxLength(255)]
    public string Period { get; set; }
    [Required]
    [MaxLength(255)]
    public string PeriodType { get; set; }
    [Required]
    [MaxLength(255)]
    public string AirlineCode { get; set; }
    [Required]
    [MaxLength(255)]
    public string AirlineName { get; set; }
    [Required]
    public int TicketsSold { get; set; }
    [Required]
    public decimal Revenue { get; set; }
}