using Microsoft.EntityFrameworkCore;
using MardelliWeb.Core;
using MardelliWeb.Data;

namespace MardelliWeb;

public class MediaService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public MediaService(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<List<MediaItem>> GetItemsAsync(int? regionId, int skip = 0, int take = 50, bool approvedOnly = true, string? createdByUserId = null, CancellationToken ct = default)
    {
        var q = _db.MediaItems.Include(x => x.Region).OrderByDescending(x => x.CreatedAt).AsQueryable();
        if (approvedOnly)
            q = q.Where(x => x.Status == EntryStatus.Approved);
        if (regionId.HasValue)
            q = q.Where(x => x.RegionId == regionId.Value);
        if (!string.IsNullOrEmpty(createdByUserId))
            q = q.Where(x => x.UploaderId == createdByUserId);
        return await q.Skip(skip).Take(take).ToListAsync(ct);
    }

    public async Task<List<MediaItem>> GetPendingMediaAsync(CancellationToken ct = default) =>
        await _db.MediaItems
            .Include(x => x.Region)
            .Where(x => x.Status == EntryStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task SetMediaStatusAsync(int id, EntryStatus status, CancellationToken ct = default)
    {
        var m = await _db.MediaItems.FindAsync(new object[] { id }, ct);
        if (m != null) { m.Status = status; await _db.SaveChangesAsync(ct); }
    }

    public async Task<int> AddAsync(MediaItem item, string userId, Stream? fileStream, string? fileName, CancellationToken ct = default)
    {
        item.UploaderId = userId;
        item.CreatedAt = DateTime.UtcNow;
        item.Status = EntryStatus.Pending;
        if (fileStream != null && !string.IsNullOrEmpty(fileName))
        {
            var uploads = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath!, "uploads");
            var typeFolder = item.Type == MediaType.Video ? "videos" : "texts";
            var dir = Path.Combine(uploads, typeFolder);
            Directory.CreateDirectory(dir);
            var safeName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
            item.FileName = fileName;
            item.FilePath = $"/uploads/{typeFolder}/{safeName}";
            var fullPath = Path.Combine(dir, safeName);
            await using (var fs = File.Create(fullPath))
                await fileStream.CopyToAsync(fs, ct);
            item.FileSizeBytes = new FileInfo(fullPath).Length;
        }
        _db.MediaItems.Add(item);
        await _db.SaveChangesAsync(ct);
        return item.Id;
    }
}
