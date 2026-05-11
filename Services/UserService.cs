using FindYourMusic.Data;
using FindYourMusic.Models;
using FindYourMusic.Models.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FindYourMusic.Services {
    public class UserService {
        private readonly DatabaseContext _dbContext;

        public UserService(DatabaseContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUser(string username, string password) {
            var user = new User { username = username, hashedPassword = password };
            user.hashedPassword = new PasswordHasher<User>().HashPassword(user, user.hashedPassword);

            _dbContext.User.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UsernameExists(string username) {
            var existingUser = await _dbContext.User.FirstOrDefaultAsync(u => u.username == username);
            if (existingUser == null) {
                return false;
            }
            return true;
        }

        public async Task<User?> GetExistingUser(string username) {
            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.username == username);
            return user;
        }

        public async Task<List<int>> GetUserTrackCategoryIds(int userId, int trackId) {
            var results = await _dbContext.UserTrackCategory
                        .Where(x => x.userId == userId && x.trackId == trackId)
                        .Select(x => x.categoryId)
                        .ToListAsync();
            return results;
        }

        public async Task<bool> UpdateUser(int userId, string newUsername) {
            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.id == userId);
            if (user == null) return false;
            user.username = newUsername;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetUserTrackCategories(int userId, int trackId) {
            var userCategories = await _dbContext.UserTrackCategory
                .Where(x => x.userId == userId && x.trackId == trackId)
                .Join(_dbContext.Category,
                x => x.categoryId,
                c => c.id,
                (x, c) => c)
                .Distinct()
                .ToListAsync();
            return userCategories;
        }

        public int tracksDescribedAmount(int userId) {
            var amount = _dbContext.UserTrackCategory.Where(x => x.userId == userId).GroupBy(x => x.trackId).Count();
            return amount;
        }

        public async Task<List<TrackDescription>> GetRecentTrackDescriptions(int userId) {
            var userDescriptions = await _dbContext.UserTrackCategory
                    .Where(x => x.userId == userId)
                    .OrderByDescending(x => x.date)
                    .ToListAsync();
            var recentDescriptions = userDescriptions
                .GroupBy(x => x.trackId)
                .Take(4)
                .Select(g => new TrackDescription {
                    track = _dbContext.Track.First(t => t.id == g.Key),
                    categories = _dbContext.Category
                        .Where(c => g.Select(x => x.categoryId).Contains(c.id))
                        .ToList()
                })
                .ToList();

            return recentDescriptions;
        }

        public async Task<List<TrackDescription>> GetAllTrackDescriptions(int userId) {
            var userDescriptions = await _dbContext.UserTrackCategory
                    .Where(x => x.userId == userId)
                    .OrderByDescending(x => x.date)
                    .ToListAsync();
            var userTrackDescriptions = userDescriptions
                .GroupBy(x => x.trackId)
                .Select(g => new TrackDescription {
                    track = _dbContext.Track.First(t => t.id == g.Key),
                    categories = _dbContext.Category
                        .Where(c => g.Select(x => x.categoryId).Contains(c.id))
                        .ToList()
                })
                .ToList();

            return userTrackDescriptions;
        }

        public async Task<List<Track>> GetAllBookmarks(int userId) {
            var recentBookmarks = await _dbContext.Bookmark
                    .Where(b => b.userId == userId)
                    .OrderByDescending(b => b.date)
                    .Join(_dbContext.Track, b => b.trackId, t => t.id, (b, t) => t)
                    .ToListAsync();

            return recentBookmarks;
        }

        public async Task<List<Track>> GetRecentBookmarks(int userId) {
            var recentBookmarks = await _dbContext.Bookmark
                    .Where(b => b.userId == userId)
                    .OrderByDescending(b => b.date)
                    .Join(_dbContext.Track, b => b.trackId, t => t.id, (b, t) => t)
                    .Take(4)
                    .ToListAsync();

            return recentBookmarks;
        }

        public async Task<List<int>> GetBookmarkedTracksFromIds(int userId, List<int> trackIds) {
            var bookmarkedTrackIds = await _dbContext.Bookmark
                .Where(b => b.userId == userId && trackIds.Contains(b.trackId))
                .Select(b => b.trackId)
                .ToListAsync();
            return bookmarkedTrackIds;
        }

        public async Task<List<CategoryUsage>> GetFavoriteCategories(int userId) {
            var favoriteCategories = await _dbContext.UserTrackCategory
                    .Where(x => x.userId == userId)
                    .GroupBy(x => x.categoryId)
                    .Select(g => new CategoryUsage {
                        id = g.Key,
                        name = _dbContext.Category.First(c => c.id == g.Key).name,
                        amount = g.Count()
                    })
                    .OrderByDescending(x => x.amount)
                    .Take(4)
                    .ToListAsync();

            return favoriteCategories;                
        }
    }
}
