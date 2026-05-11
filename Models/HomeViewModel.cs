namespace FindYourMusic.Models {
    public class HomeViewModel {
        private List<Track> _recentlyDescribedTracks;
        private List<Track>? _recommendedTracks;

        public List<Track> recentlyDescribedTracks { get { return _recentlyDescribedTracks; } }

        public List<Track>? recommendedTracks { get { return _recommendedTracks; } }

        public HomeViewModel(List<Track> recentlyDescribedTracks, List<Track>? recommendedTracks) { 
            _recentlyDescribedTracks = recentlyDescribedTracks;
            _recommendedTracks = recommendedTracks;
        }
    }
}
