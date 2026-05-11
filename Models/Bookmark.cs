namespace FindYourMusic.Models {
    public class Bookmark {
        public int id { get; set; }
        public int userId { get; set; }
        public int trackId { get; set; }
        public DateTime date { get; set; }
    }
}
