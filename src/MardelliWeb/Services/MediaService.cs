using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MardelliWeb.Core;
using MardelliWeb.Data;

namespace MardelliWeb;

public class MediaService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;

    public MediaService(AppDbContext db, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _env = env;
        _userManager = userManager;
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

    public async Task<List<MediaItem>> GetPendingMediaAsync(CancellationToken ct = default)
    {
        var list = await _db.MediaItems
            .Include(x => x.Region)
            .Where(x => x.Status == EntryStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);
        foreach (var item in list)
        {
            if (!string.IsNullOrEmpty(item.UploaderId))
            {
                var user = await _userManager.FindByIdAsync(item.UploaderId);
                item.UploaderEmail = user?.Email ?? user?.UserName ?? item.UploaderId;
            }
        }
        return list;
    }

    public async Task SetMediaStatusAsync(int id, EntryStatus status, CancellationToken ct = default)
    {
        var m = await _db.MediaItems.FindAsync(new object[] { id }, ct);
        if (m != null) { m.Status = status; await _db.SaveChangesAsync(ct); }
    }

    public async Task<bool> DeleteMediaAsync(int id, CancellationToken ct = default)
    {
        var m = await _db.MediaItems.FindAsync(new object[] { id }, ct);
        if (m == null) return false;
        if (!string.IsNullOrEmpty(m.FilePath))
        {
            var fullPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath!, m.FilePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                try { File.Delete(fullPath); } catch { /* ignore */ }
            }
        }
        _db.MediaItems.Remove(m);
        await _db.SaveChangesAsync(ct);
        return true;
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
