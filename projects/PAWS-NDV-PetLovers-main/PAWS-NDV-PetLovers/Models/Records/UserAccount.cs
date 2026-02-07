using System.ComponentModel.DataAnnotations;

namespace PAWS_NDV_PetLovers.Models.Records
{
    public class UserAccount
    {
        [Key]
        public int acc_Id { get; set; }

        [Display(Name = "First Name")]
        public string? fname { get; set; }

        [Display(Name = "Last Name")]
        public string? lname { get; set; }

        [Display(Name = "Middle Name")]
        public string? mname { get; set; }

        [Display(Name = "Username")]
        public string? userName { get; set; }

        [Display(Name = "Password")]
        public string? passWord { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string? email { get; set; }

      /*  [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime? bdate { get; set; }*/

        [Display(Name = "Contact #")]
        [MaxLength(11)]
        [DataType(DataType.PhoneNumber)]
        public string? contact { get; set; }

        [Display(Name = "User-Type")]
        public string? userType { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime? dateCreated { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        public bool IsPasswordChanged { get; set; } // New property to track password change
    }
}
