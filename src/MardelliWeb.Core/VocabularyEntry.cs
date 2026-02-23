namespace MardelliWeb.Core;

/// <summary>
/// Ein Wörterbucheintrag: Quellwort (TR oder DE) → Mardelli-Arabisch.
/// </summary>
public class VocabularyEntry
{
    public int Id { get; set; }

    /// <summary>Quellsprache (Türkisch oder Deutsch).</summary>
    public SourceLanguage SourceLanguage { get; set; }

    /// <summary>Wort in der Quellsprache (z. B. "Seker", "Zucker").</summary>
    public string SourceWord { get; set; } = string.Empty;

    /// <summary>Übersetzung in Mardelli (arabische Schrift).</summary>
    public string MardelliWord { get; set; } = string.Empty;

    /// <summary>Mardelli in lateinischer Schrift (z. B. "Sickar"). Gleichwertig zu MardelliWord.</summary>
    public string? MardelliLatin { get; set; }

    /// <summary>Optionale phonetische Umschrift (falls abweichend von MardelliLatin).</summary>
    public string? Transliteration { get; set; }

    /// <summary>Freigabe: nur Approved erscheint im Wörterbuch.</summary>
    public EntryStatus Status { get; set; } = EntryStatus.Pending;

    /// <summary>Optionale Bedeutungserklärung oder Beispielsatz.</summary>
    public string? Notes { get; set; }

    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;

    /// <summary>User-ID des Beitragenden (Identity).</summary>
    public string ContributorId { get; set; } = string.Empty;
    /// <summary>Nur zur Anzeige (z. B. Admin-Freigabe), wird nicht in der DB gespeichert.</summary>
    public string? ContributorEmail { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
