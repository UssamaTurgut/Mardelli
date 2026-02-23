using Microsoft.EntityFrameworkCore;
using MardelliWeb.Core;
using MardelliWeb.Data;

namespace MardelliWeb;

public class DictionaryService
{
    private readonly AppDbContext _db;

    public DictionaryService(AppDbContext db) => _db = db;

    public async Task<List<VocabularyEntry>> GetEntriesAsync(
        SourceLanguage? sourceLanguage,
        string? search,
        int? regionId,
        int skip = 0,
        int take = 50,
        CancellationToken ct = default)
    {
        var q = _db.VocabularyEntries
            .Include(x => x.Region)
            .Where(x => x.Status == EntryStatus.Approved)
            .OrderBy(x => x.SourceWord)
            .AsQueryable();

        if (sourceLanguage.HasValue)
            q = q.Where(x => x.SourceLanguage == sourceLanguage.Value);
        if (regionId.HasValue)
            q = q.Where(x => x.RegionId == regionId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            q = q.Where(x =>
                x.SourceWord.ToLower().Contains(s) ||
                (x.MardelliWord != null && x.MardelliWord.ToLower().Contains(s)) ||
                (x.MardelliLatin != null && x.MardelliLatin.ToLower().Contains(s)) ||
                (x.Transliteration != null && x.Transliteration.ToLower().Contains(s)));
        }

        return await q.Skip(skip).Take(take).ToListAsync(ct);
    }

    public async Task<List<VocabularyEntry>> GetPendingVocabularyAsync(CancellationToken ct = default) =>
        await _db.VocabularyEntries
            .Include(x => x.Region)
            .Where(x => x.Status == EntryStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task SetVocabularyStatusAsync(int id, EntryStatus status, CancellationToken ct = default)
    {
        var e = await _db.VocabularyEntries.FindAsync(new object[] { id }, ct);
        if (e != null) { e.Status = status; e.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(ct); }
    }

    public async Task<VocabularyEntry?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.VocabularyEntries.Include(x => x.Region).FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<int> AddAsync(VocabularyEntry entry, string userId, CancellationToken ct = default)
    {
        entry.ContributorId = userId;
        entry.CreatedAt = DateTime.UtcNow;
        entry.Status = EntryStatus.Pending;
        _db.VocabularyEntries.Add(entry);
        await _db.SaveChangesAsync(ct);
        return entry.Id;
    }

    public async Task<List<Region>> GetRegionsAsync(CancellationToken ct = default) =>
        await _db.Regions.OrderBy(x => x.NameTurkish).ToListAsync(ct);
}
