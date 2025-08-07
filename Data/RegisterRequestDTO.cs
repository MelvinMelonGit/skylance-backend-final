using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skylance_backend.Data;

public class RegisterRequestDTO
{
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Password { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Salutation { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Gender { get; set; } = null!;

        [Required, MaxLength(255)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(255)]
        public string LastName { get; set; } = null!;

        [Required]
        public string NationalityId { get; set; } = null!;  // Assuming Country Id is string

        [Required]
        public string MobileCodeId { get; set; } = null!;   // Assuming Country Id is string

        [Required, MaxLength(255)]
        public string MobileNumber { get; set; } = null!;

        [Required, MaxLength(50)]
        public string MembershipTier { get; set; } = null!;

        [Required, MaxLength(255)]
        public string MembershipNumber { get; set; } = null!;

        [Required, MaxLength(255)]
        public string PassportNumber { get; set; } = null!;

        [Required]
        public DateOnly PassportExpiry { get; set; }

        [Required] public DateOnly DateOfBirth { get; set; }
}