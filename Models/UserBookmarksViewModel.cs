using FindYourMusic.Models;

namespace FindYourMusic.Models {
    public class UserBookmarksViewModel {
        PaginatedList<Track> _paginatedBookmarkedTracks { get; set; }
        private int _totalPagesNumber { get; set; }
        private int _pageIndex { get; set; }
        private int _tracksDescribed { get; set; }

        public int tracksDescribed { get { return _tracksDescribed; } }

        public PaginatedList<Track> bookmarkedTracks { get {  return _paginatedBookmarkedTracks; } }

        public int totalPages { get { return _totalPagesNumber; } }

        public int currentPage { get { return _pageIndex; } }

        public UserBookmarksViewModel(int tracksDescribed, List<Track> bookmarkedTracks, int? pageNumber) {
            _tracksDescribed = tracksDescribed;
            
            int pageSize = 20;
            _totalPagesNumber = PaginatedList<Track>.TotalPagesNumber(bookmarkedTracks, pageSize);
            _pageIndex = pageNumber ?? 1;
            _pageIndex = _pageIndex <= 0 ? 1 : _pageIndex > _totalPagesNumber ? _totalPagesNumber : _pageIndex;

            _paginatedBookmarkedTracks = PaginatedList<Track>.Create(bookmarkedTracks, _pageIndex, pageSize);
        }
    }
}
