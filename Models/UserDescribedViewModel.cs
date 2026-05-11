namespace FindYourMusic.Models {
    public class UserDescribedViewModel {
        private int _tracksDescribed { get; set; }
        private List<TrackDescriptionItem> _trackDescriptionItems { get; set; } = [];
        private PaginatedList<TrackDescriptionItem> _paginatedTrackDescriptionItems { get; set; }
        private int _totalPagesNumber;
        private int _pageIndex;

        public int tracksDescribed { get { 
            return _tracksDescribed;
        } }

        public PaginatedList<TrackDescriptionItem> trackDescriptionItems { get {
            return _paginatedTrackDescriptionItems;
        } }

        public int totalPages { get { return _totalPagesNumber; } }

        public int currentPage { get { return _pageIndex; } }

        public UserDescribedViewModel(int tracksDescribed, List<TrackDescription> trackDescriptions, int? pageNumber) { 
            _tracksDescribed = tracksDescribed;
            foreach (var description in trackDescriptions) {
                _trackDescriptionItems.Add(new TrackDescriptionItem(description));
            }

            int pageSize = 20;
            _totalPagesNumber = PaginatedList<TrackDescriptionItem>.TotalPagesNumber(_trackDescriptionItems, pageSize);
            _pageIndex = pageNumber ?? 1;
            _pageIndex = _pageIndex <= 0 ? 1 : _pageIndex > _totalPagesNumber ? _totalPagesNumber : _pageIndex;

            _paginatedTrackDescriptionItems = PaginatedList<TrackDescriptionItem>.Create(_trackDescriptionItems, _pageIndex, pageSize);
        }
    }
}
