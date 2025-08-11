using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("Chart")]
public class Chart
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Period { get; set; }

    [Required]
    public required string PeriodType { get; set; }

    [Required]
    public required string AirlineCode { get; set; }

    [Required]
    public required string AirlineName { get; set; }

    [Required]
    public required int TicketsSold { get; set; }

    [Required]
    public required double Revenue { get; set; }

}