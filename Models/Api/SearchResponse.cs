namespace FindYourMusic.Models.Api {
    public class SearchResponse {
        public Results results { get; set; }
    }

    public class Results {
        public TrackMatches trackmatches { get; set; }
    }
}
