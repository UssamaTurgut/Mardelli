namespace MardelliWeb.Core;

/// <summary>
/// Hochgeladenes Material (Video oder Text), in dem Mardelli vorkommt oder dokumentiert wird.
/// Später für RAG/Sprachbot nutzbar.
/// </summary>
public class MediaItem
{
    public int Id { get; set; }
    public MediaType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Dateipfad oder Blob-URL (z. B. /uploads/videos/xyz.mp4).</summary>
    public string FilePath { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public long? FileSizeBytes { get; set; }

    /// <summary>Für Texte: Inhalt (für Volltextsuche/RAG).</summary>
    public string? TextContent { get; set; }

    public int? RegionId { get; set; }
    public Region? Region { get; set; }

    public string UploaderId { get; set; } = string.Empty;
    /// <summary>Nur zur Anzeige (z. B. Admin-Freigabe), wird nicht in der DB gespeichert.</summary>
    public string? UploaderEmail { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Freigabe: nur Approved erscheint in Materialien.</summary>
    public EntryStatus Status { get; set; } = EntryStatus.Pending;
}
