using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("EmployeeSessions")]
public class EmployeeSession
{
    [Key]
    [MaxLength(255)]
    public required string Id { get; set; }
    
    [Required]
    public required DateTime SessionExpiry { get; set; }
    
    [Required]
    [ForeignKey("EmployeeId")]
    public virtual required Employee Employee { get; set; }
}