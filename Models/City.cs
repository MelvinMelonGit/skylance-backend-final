using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("Cities")]
public class City
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [ForeignKey("CountryId")]
    public virtual required Country Country { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }
}
