using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FindYourMusic.Models {
    public class LoginViewModel {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        [DisplayName("Username")]
        public string username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string password { get; set; }
    }
}
