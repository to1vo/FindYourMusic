using System.Text.Json;

namespace FindYourMusic.Models {
    public class UserProfileViewModel {
        private List<TrackDescriptionItem> _recentDescriptionItems { get; set; } = [];
        private List<Track> _recentBookmarks { get; set; }
        private List<CategoryUsage> _favoriteCategories { get; set; }
        private int _tracksDescribed { get; set; }
        private int _bookmarkAmount { get; set; }

        public int bookmarkAmount { get {
                return _bookmarkAmount;
        } }

        public int tracksDescribed { get {
            return _tracksDescribed;
        } }

        public List<TrackDescriptionItem> recentDescriptionItems { get {
            return _recentDescriptionItems;
        } }

        public List<Track> recentBookmarks { get {
            return _recentBookmarks;
        } }

        public List<CategoryUsage> favoriteCategories { get {
            return _favoriteCategories; 
        } }

        public string favoriteCategoriesLabels { get {
            if(_favoriteCategories.Count != 0) {
                var categoryNames = _favoriteCategories.Select(c => c.name).ToList();
                return JsonSerializer.Serialize(categoryNames);
            } else {
                return "[]";
            }
        } }

        public string favoriteCategoriesData { get {
            if (_favoriteCategories.Count != 0) {
                var categoryAmounts = _favoriteCategories.Select(c => c.amount).ToList();
                return JsonSerializer.Serialize(categoryAmounts);
            } else {
                return "[]";
            }
        } }

        public UserProfileViewModel(int tracksDescribed, List<TrackDescription> recentDescriptions, List<Track> recentBookmarks, List<CategoryUsage> favoriteCategories, int bookmarkAmount) {
            foreach (var description in recentDescriptions) {
                _recentDescriptionItems.Add(new TrackDescriptionItem(description));
            }
            _recentBookmarks = recentBookmarks;
            _favoriteCategories = favoriteCategories;
            _tracksDescribed = tracksDescribed;
            _bookmarkAmount = bookmarkAmount;
        }
    }
}
