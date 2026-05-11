using System.Text.Json;
using FindYourMusic.Models;
using FindYourMusic.Models.Api;

namespace FindYourMusic.Services {
    public class APIService {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://ws.audioscrobbler.com/2.0/";

        public APIService(HttpClient httpClient, IConfiguration config) {
            _httpClient = httpClient;
            _apiKey = config["LastFm:APIKey"];
        }

        public async Task<List<TrackResult>> SearchTracks(string query) {
            var url = $"{_baseUrl}?method=track.search&track={query}&api_key={_apiKey}&format=json&limit=10";
            var json = await _httpClient.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<SearchResponse>(json);

            return data?.results?.trackmatches?.track?.Select(t => new TrackResult {
                name = t.name,
                artist = t.artist
            }).ToList() ?? new List<TrackResult>();
        }

        public async Task<Track?> GetTrackInfo(string track, string artist) {
            var url = $"{_baseUrl}?method=track.getInfo&api_key={_apiKey}&artist={artist}&track={track}&format=json";
            var json = await _httpClient.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<TrackInfoResponse>(json);

            if (data == null || data.track == null) return null;

            var imageUrl = data.track.album?.image?.FirstOrDefault(i => i.size == "extralarge")?.url;
            //var durationDecimal = Decimal.Round((Decimal.Parse(data.track.duration) / 1000) / 60, 2);
            int durationSeconds = Int32.Parse(data.track.duration) / 1000;
            int durationMinutes = durationSeconds / 60;
            int seconds = durationSeconds % 60;
            var duration = $"{durationMinutes}:{seconds:D2}";
            string? albumTitle = null;
            //uint? releaseYear = null;

            if(data.track.album != null) {
                albumTitle = data.track.album.title;
                //releaseYear = await GetAlbumReleaseYear(data.track.album.title, data.track.artist.name);
            }

            return new Track {
                name = data.track.name,
                artist = data.track.artist.name,
                duration = duration,
                //year = releaseYear,
                album = albumTitle,
                albumCoverImgUrl = imageUrl
            };
        }

        //private async Task<uint?> GetAlbumReleaseYear(string album, string artist) {
        //    var url = $"{_baseUrl}?method=album.getinfo&api_key={_apiKey}&artist={artist}&album={album}&format=json";
        //    var json = await _httpClient.GetStringAsync(url);
        //    var data = JsonSerializer.Deserialize<AlbumInfoResponse>(json);

        //    if(data.album.releaseDate == null) {
        //        return null;
        //    }

        //    try {
        //        var date = DateTime.ParseExact(
        //            data.album.releaseDate,
        //            "d MMM yyyy, HH:mm",
        //            CultureInfo.InvariantCulture
        //        );
        //        return (uint)date.Year;
        //    } catch {
        //        return null;
        //    }
        //}
    }
}
