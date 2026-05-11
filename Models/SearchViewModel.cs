namespace FindYourMusic.Models {
    public class SearchViewModel {
        private List<CategoryGroup> _categoryGroups { get; set; }
        private List<Category> _categories { get; set; }
        private List<int> _previouslySelectedCategories { get; set; }

        public List<CategoryGroup> categoryGroups {
            get {
                return _categoryGroups;
            }
        }

        public List<Category> categories {
            get {
                return _categories;
            }
        }

        public List<int> previouslySelectedCategories { 
            get { 
                return _previouslySelectedCategories; 
            } 
        }

        public SearchViewModel(List<CategoryGroup> categoryGroups, List<Category> categories, List<int> previouslySelectedCategories) {
            _categoryGroups = categoryGroups;
            _categories = categories;
            _previouslySelectedCategories = previouslySelectedCategories;
        }
    }
}
