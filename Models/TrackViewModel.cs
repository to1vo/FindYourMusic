using System.Text.Json;

namespace FindYourMusic.Models {
    public class TrackViewModel {
        private Track _track { get; set; }
        private List<Category>? _userCategories { get; set; }
        private List<CategoryUsage> _top4Categories { get; set; }
        private List<CategoryUsage> _allUsedCategories { get; set; }
        private List<Track> _similarTracks { get; set; }
        private bool? _isBookmarked { get; set; }
        private List<Track>? _bookmarkedTracks { get; set; }

        public Track track { get { return _track; } }

        public List<Category>? userCategories { get { return _userCategories; } }

        public List<CategoryUsage> top4Categories { get { return _top4Categories; } }

        public List<CategoryUsage> allUsedCateogories { get { return _allUsedCategories; } } 

        public List<Track> similarTracks { get { return _similarTracks; } }

        public List<Track>? bookmarkedTracks { get { return _bookmarkedTracks; } }

        public bool? isBookMarked { get { return _isBookmarked; } }

        public string top4CategoriesData {
            get {
                if (_top4Categories.Count != 0) {
                    var categoryAmounts = _top4Categories.Select(c => c.amount).ToList();
                    return JsonSerializer.Serialize(categoryAmounts);
                } else {
                    return "[]";
                }
            }
        }

        public string top4CategoriesLabels {
            get {
                var categoryAmounts = _top4Categories.Select(c => c.name).ToList();
                return JsonSerializer.Serialize(categoryAmounts);
            }
        }

        public TrackViewModel(Track track, bool? isBookmarked, List<Category>? userCategories, List<CategoryUsage> top4Categories, List<CategoryUsage> allUsedCategories, List<Track> similarTracks, List<Track>? bookmarkedTracks) {
            _track = track;
            _userCategories = userCategories;
            _top4Categories = top4Categories;
            _allUsedCategories = allUsedCategories;
            _similarTracks = similarTracks;
            _isBookmarked = isBookmarked;
            _bookmarkedTracks = bookmarkedTracks;
        }
    }
}
