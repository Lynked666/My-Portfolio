using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PAWS_NDV_PetLovers.Models.Records
{
    public class PasswordResetRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ResetCode { get; set; } = string.Empty;

        [Required]
        public DateTime Expiry { get; set; }

        // Foreign key to UserAccount
        [Required]
        public int UserAccountId { get; set; }

        [ForeignKey(nameof(UserAccountId))]
        public UserAccount UserAccount { get; set; }
    }
}
