namespace FindYourMusic.Models {
    public class DescribeTrackViewModel {
        private Track _trackInfo { get; set; }
        private List<CategoryGroup> _categoryGroups { get; set; }
        private List<Category> _categories { get; set; }
        private List<int> _previouslyAddedCategories { get; set; }

        public Track trackInfo { get {  
            return _trackInfo; 
        } } 

        public List<CategoryGroup> categoryGroups { get {
            return _categoryGroups;
        } }

        public List<Category> categories { get {
            return _categories;
        } }

        public List<int> previouslyAddedCategories { get {
            return _previouslyAddedCategories;
        } }

        public DescribeTrackViewModel(Track trackInfo, List<CategoryGroup> categoryGroups, List<Category> categories, List<int> previouslyAddedCategories) {
            _trackInfo = trackInfo;
            _categoryGroups = categoryGroups;
            _categories = categories;
            _previouslyAddedCategories = previouslyAddedCategories;
        }
    }
}
