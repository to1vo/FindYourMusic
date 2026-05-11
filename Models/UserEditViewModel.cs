using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FindYourMusic.Models {
    public class UserEditViewModel {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username has to be at least 3 characters long")]
        [DisplayName("Username")]
        public string username { get; set; }

        public UserEditViewModel() { }

        public UserEditViewModel(string username) {
            this.username = username;
        }
    }
}
