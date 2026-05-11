namespace FindYourMusic.Models {
    public class TrackDescriptionItem {
        public int trackId { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public string? albumCoverImgUrl { get; set; }
        public string categories { get; set; }
        public string emojis { get; set; }

        public TrackDescriptionItem(TrackDescription trackDescription) {
            trackId = trackDescription.track.id;
            title = trackDescription.track.name;
            artist = trackDescription.track.artist;
            albumCoverImgUrl = trackDescription.track.albumCoverImgUrl;
            for (int i = 0; i < trackDescription.categories.Count(); i++) {
                categories += (i == trackDescription.categories.Count() - 1) ? trackDescription.categories[i].name : trackDescription.categories[i].name + " ";
                emojis += (i == trackDescription.categories.Count() - 1) ? trackDescription.categories[i].emoji : trackDescription.categories[i].emoji + " ";
            }
        }
    }
}
