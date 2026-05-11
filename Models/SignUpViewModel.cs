using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FindYourMusic.Models {
    public class SignUpViewModel {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username has to be at least 3 characters long")]
        [MaxLength(15, ErrorMessage = "Username can be maximum of only 15 characters long")]
        [DisplayName("Username")]
        public string username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Password has to be at least 5 characters long")]
        [DisplayName("Password")]
        public string password { get; set; }

        [Required(ErrorMessage = "Repeat password")]
        [DataType(DataType.Password)]
        [DisplayName("Password confirmation")]
        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string passwordConfirmation { get; set; }
    }
}
