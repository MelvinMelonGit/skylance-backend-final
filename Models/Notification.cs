using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("Notifications")]
public class Notification
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(255)]
    public virtual required OverbookingDetail OverbookingDetail { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string Message { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string NotificationType { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string NotificationStatus { get; set; }
}