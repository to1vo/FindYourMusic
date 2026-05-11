namespace FindYourMusic.Models {
    public class UserTrackCategory {
        public int id { get; set; }
        public int userId { get; set; }
        public int categoryId { get; set; }
        public int trackId { get; set; }
        public DateTime date { get; set; }
    }
}
