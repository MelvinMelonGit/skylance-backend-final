using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("Airports")]
public class Airport
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(50)]
    public required string IataCode { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }

    [Required]
    [ForeignKey("CityId")]
    public virtual required City City { get; set; }

    [Required]
    [MaxLength(255)]
    public required string TimeZone { get; set; }
}
