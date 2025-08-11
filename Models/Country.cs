using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("Countries")]
public class Country
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public required string CountryCode { get; set; }

    [Required]
    public required int MobileCode { get; set; }
}