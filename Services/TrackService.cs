using FindYourMusic.Data;
using FindYourMusic.Models;
using Microsoft.EntityFrameworkCore;

namespace FindYourMusic.Services {
    public class TrackService {
        private readonly DatabaseContext _dbContext;
        
        public TrackService(DatabaseContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<Track>? GetTrackById(int id) {
            var track = await _dbContext.Track.FirstOrDefaultAsync(t => t.id == id);
            return track;
        }

        public async Task<Track>? GetTrack(string title, string artist) {
            var track = await _dbContext.Track.FirstOrDefaultAsync(t => t.name == title && t.artist == artist);
            return track;
        }

        public async Task<List<TrackResult>> GetTracksAddedByUser(string query) {
            var results = await _dbContext.Track.Where(t => (t.name.Contains(query) || t.artist.Contains(query)) && t.userAdded == true).Select(t => new TrackResult { name = t.name, artist = t.artist }).ToListAsync();
            return results;
        }

        public bool IsBookmarked(int userId, int trackId) {
            var bookmarked = _dbContext.Bookmark.FirstOrDefault(b => b.userId == userId && b.trackId == trackId);
            if (bookmarked == null) {
                return false;
            }
            return true;
        }

        public bool TrackExists(string title, string artist) {
            var track = _dbContext.Track.FirstOrDefault(t => t.name == title && t.artist == artist);
            if (track == null) {
                return false;
            }
            return true;
        }

        public async Task<List<Track>> GetRecentlyDescribedTracks() {
            var trackIds = await _dbContext.UserTrackCategory
                .GroupBy(x => x.trackId)
                .Select(g => new {
                    TrackId = g.Key,
                    LatestDate = g.Max(x => x.date)
                })
                .OrderByDescending(x => x.LatestDate)
                .Take(5)
                .Select(x => x.TrackId)
                .ToListAsync();
            var tracks = await _dbContext.Track.Where(t => trackIds.Contains(t.id)).ToListAsync();
            var orderedTracks = trackIds.Select(id => tracks.First(t => t.id == id)).ToList();
            return orderedTracks;
        }

        public async Task<List<Track>> GetRecommendedTracks(int userId, List<CategoryUsage> favoriteCategories, List<Track> bookmarkedTracks) {
            var userDescribedTracks = await _dbContext.UserTrackCategory
                .Where(x => x.userId == userId)
                .Select(x => x.trackId)
                .Distinct()
                .ToListAsync();
            var bookmarkedTrackIds = bookmarkedTracks.Select(t => t.id);
            var allTrackCategories = await _dbContext.UserTrackCategory
                .Where(x => !userDescribedTracks.Contains(x.trackId) && !bookmarkedTrackIds.Contains(x.trackId))
                .GroupBy(x => new { x.trackId, x.categoryId })
                .Select(g => new {
                    trackId = g.Key.trackId,
                    categoryId = g.Key.categoryId,
                    count = g.Count()
                })
                .ToListAsync();

            var similarityInTracks = allTrackCategories
                .GroupBy(x => x.trackId)
                .Select(g => {
                    int score = g.Sum(x => {
                        var match = favoriteCategories
                            .FirstOrDefault(c => c.id == x.categoryId);

                        if (match == null) return 0;

                        return match.amount * x.count;
                    });

                    return new { trackId = g.Key, score };
                })
                .Where(x => x.score > 0)
                .OrderByDescending(x => x.score)
                .Take(5)
                .ToList();

            var trackIds = similarityInTracks.Select(x => x.trackId);
            var recommendedTracks = await _dbContext.Track.Where(t => trackIds.Contains(t.id)).ToListAsync();

            return recommendedTracks;
        }

        //all given categories for the track
        public async Task<List<CategoryUsage>> GetAllUsedCategories(int trackId) {
            var allUsedCategories = await _dbContext.UserTrackCategory
                    .Where(x => x.trackId == trackId)
                    .GroupBy(x => x.categoryId)
                    .Select(g => new CategoryUsage {
                        id = g.Key,
                        name = _dbContext.Category.First(c => c.id == g.Key).name,
                        amount = g.Count()
                    })
                    .OrderByDescending(x => x.amount)
                    .ToListAsync();
            return allUsedCategories;
        }

        //similar tracks (3)
        public async Task<List<Track>> GetSimilarTracks(int trackId, List<CategoryUsage> allUsedCategories) {
            var trackTop4Categories = allUsedCategories.Take(4).ToList();
            var allTrackCategories = await _dbContext.UserTrackCategory
                .Where(x => x.trackId != trackId)
                .GroupBy(x => new { x.trackId, x.categoryId })
                .Select(g => new {
                    trackId = g.Key.trackId,
                    categoryId = g.Key.categoryId,
                    count = g.Count()
                })
                .ToListAsync();
            var similarityInTracks = allTrackCategories
                .GroupBy(x => x.trackId)
                .Select(g => {
                    int score = g.Sum(x => {
                        var match = trackTop4Categories
                            .FirstOrDefault(c => c.id == x.categoryId);

                        if (match == null) return 0;

                        return match.amount * x.count;
                    });

                    return new { trackId = g.Key, score };
                })
                .Where(x => x.score > 0)
                .OrderByDescending(x => x.score)
                .Take(4)
                .ToList();

            var trackIds = similarityInTracks.Select(x => x.trackId);
            var similarTracks = await _dbContext.Track.Where(t => trackIds.Contains(t.id)).ToListAsync();

            return similarTracks;
        }

        public async Task<List<TrackDescription>> GetTracksThatFitCategories(List<int> selectedCategories) {
            var results = await _dbContext.UserTrackCategory
                .Where(x => selectedCategories.Contains(x.categoryId))
                .GroupBy(x => x.trackId)
                .Select(g => new {
                    trackId = g.Key,
                    categoryIds = g.Select(x => x.categoryId).Distinct(),
                    matches = g.Select(x => x.categoryId).Distinct().Count(),
                    usage = g.Count()
                })
                .OrderByDescending(x => x.matches)
                .ThenByDescending(x => x.usage)
                .ToListAsync();

            var allCategoryIds = results
                .SelectMany(x => x.categoryIds)
                .Distinct()
                .ToList();
            var categories = await _dbContext.Category
                .Where(c => allCategoryIds.Contains(c.id))
                .ToDictionaryAsync(c => c.id);

            var trackIds = results.Select(x => x.trackId);
            var fittingTracks = await _dbContext.Track
                .Where(t => trackIds.Contains(t.id))
                .ToDictionaryAsync(t => t.id);

            var finalResult = results
                .Select(r => new TrackDescription {
                    track = fittingTracks[r.trackId],
                    categories = r.categoryIds.Select(id => categories[id]).ToList()
                })
                .ToList();

            return finalResult;
        }
    }
}
