using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Models;

[Table("AppUsers")]
public class AppUser
{
    [Key]
    [MaxLength(255)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(255)]
    public required string Email { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string Password { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Salutation { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Gender { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string FirstName { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string LastName { get; set; }
    
    [Required]
    [ForeignKey("NationalityId")]
    public virtual required Country Nationality { get; set; }
    
    [Required]
    [ForeignKey("MobileCodeId")]
    public virtual required Country MobileCode { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string MobileNumber { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string MembershipTier { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string MembershipNumber { get; set; }
    
    [Required]
    [MaxLength(255)]
    public required string PassportNumber { get; set; }
    
    [Required]
    public required DateOnly PassportExpiry { get; set; }
    
    [Required]
    public required DateOnly DateOfBirth { get; set; }
}