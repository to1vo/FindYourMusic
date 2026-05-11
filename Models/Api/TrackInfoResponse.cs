using System.Text.Json.Serialization;

namespace FindYourMusic.Models.Api {
    public class TrackInfoResponse {
        public TrackInfo track { get; set; }
    }

    public class TrackInfo {
        public string name { get; set; }
        public string duration { get; set; }
        public Artist artist { get; set; }
        public Album album { get; set; }
    }

    public class Artist {
        public string name { get; set; }
    }

    public class Album {
        public string title { get; set; }
        public List<Image> image { get; set; }
    }

    public class Image {
        public string size { get; set; }
        [JsonPropertyName("#text")]
        public string url { get; set; }
    }
}
