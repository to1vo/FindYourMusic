namespace FindYourMusic.Models {
    public class SearchResultsViewModel {
        private List<TrackDescriptionItem> _trackDescriptionItems { get; set; } = [];
        private PaginatedList<TrackDescriptionItem> _paginatedTrackDescriptionItems { get; set; }
        private List<int>? _bookmarkedTracks { get; set; }
        private string _searchDescription;
        private int _totalPagesNumber;
        private int _pageIndex;

        public PaginatedList<TrackDescriptionItem> searchResults {
            get {
                return _paginatedTrackDescriptionItems;
            }
        }

        public List<int>? bookmarkedTracks { get { return _bookmarkedTracks; } }

        public string searchDescription { get { return _searchDescription; } }

        public int totalPages { get { return _totalPagesNumber; } }

        public int currentPage { get { return _pageIndex; } }

        public SearchResultsViewModel(List<Category> categories, List<TrackDescription> trackDescriptions, int? pageNumber, List<int>? bookmarkedTracks) {
            foreach (var description in trackDescriptions) {
                _trackDescriptionItems.Add(new TrackDescriptionItem(description));
            }
            _searchDescription = generateSearchDescription(categories);
            _bookmarkedTracks = bookmarkedTracks;
            int pageSize = 10;
            _totalPagesNumber = PaginatedList<TrackDescriptionItem>.TotalPagesNumber(_trackDescriptionItems, pageSize);
            _pageIndex = pageNumber ?? 1;
            _pageIndex = _pageIndex <= 0 ? 1 : _pageIndex > _totalPagesNumber ? _totalPagesNumber : _pageIndex;
            _paginatedTrackDescriptionItems = PaginatedList<TrackDescriptionItem>.Create(_trackDescriptionItems, _pageIndex, pageSize);
        }

        private string generateSearchDescription(List<Category> categories) {
            string descriptionString = string.Empty;
            categories = categories.OrderBy(c => c.categoryGroupId).ToList();
            //var categoriesGroup1 = categories.Where(c => c.id == 1).ToList();
            //var categoriesGroup2 = categories.Where(c => c.id == 2).ToList();
            //var categoriesGroup3 = categories.Where(c => c.id == 3).ToList();


            //if(categoriesGroup1.Count() == 0 && categoriesGroup3.Count() == 0) {
            //    descriptionString += "Tracks for ";
            //    for(int i=0; i<categoriesGroup2.Count(); i++) {
            //        descriptionString += categoriesGroup2[i].name;
            //        if (i + 2 != categoriesGroup2.Count()) {
            //            descriptionString += " and ";
            //        }
            //        if (i + 1 != categoriesGroup2.Count()) {
            //            descriptionString += ", ";
            //        }
            //    }
            //}

            //for (int i = 0; i < categoriesGroup1.Count(); i++) {

            //}

            for (int i=0; i<categories.Count(); i++) {
                if(i+1 != categories.Count()) {
                    if (i+2 == categories.Count()) {
                        //second last
                        descriptionString += categories[i].name + " and ";
                        continue;
                    }
                    descriptionString += categories[i].name + ", ";
                    continue;
                }
                //last
                descriptionString += categories[i].name;
                descriptionString += " tracks";
            }

            return descriptionString;
        }
    }
}
