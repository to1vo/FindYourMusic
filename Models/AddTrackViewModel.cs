using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FindYourMusic.Models {
    public class AddTrackViewModel {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Track name is required")]
        [DisplayName("Title")]
        public string title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Artist name is required")]
        [DisplayName("Artist")]
        public string artist { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [RegularExpression(@"^\d{1,2}:\d{2}$", ErrorMessage = "Use m:ss format, for example 2:32")]
        [DisplayName("Duration")]
        public string duration { get; set; }

        [DisplayName("Release year")]
        public uint? year { get; set; }

        [DisplayName("Album")]
        //[RegularExpression(@"\S", ErrorMessage = "Album value is invalid")]
        public string? album { get; set; }

        [DisplayName("Album cover")]
        public string? albumCoverImgUrl { get; set; }
    }
}
