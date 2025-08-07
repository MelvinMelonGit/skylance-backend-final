using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("AppUserSessions")]
public class AppUserSession
{
    [Key]
    [MaxLength(255)]
    public required string Id { get; set; }
    
    [Required]
    public required DateTime SessionExpiry { get; set; }
    
    [Required]
    [ForeignKey("AppUserId")]
    public virtual required AppUser AppUser { get; set; }
}