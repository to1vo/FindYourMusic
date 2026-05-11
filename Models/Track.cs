namespace FindYourMusic.Models {
    public class Track {
        public int id { get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string? duration { get; set; }
        public uint? year { get; set; }
        public string? album { get; set; }
        public string? albumCoverImgUrl { get; set; }
        public bool userAdded { get; set; } = false;
    }
}
