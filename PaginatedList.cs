using Microsoft.EntityFrameworkCore;

namespace FindYourMusic {
    public class PaginatedList<T> : List<T> {
        public int PageIndex { get; set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize) {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize) {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        public static int TotalPagesNumber<T>(List<T> items, int pageSize) {
            var totalPagesNumber = 1;
            var itemsCount = items.Count();

            while (itemsCount > pageSize) {
                totalPagesNumber++;
                itemsCount -= pageSize;
            }
            return totalPagesNumber;
        }
    }
}
