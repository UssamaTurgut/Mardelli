namespace MardelliWeb.Core;

/// <summary>
/// Ort/Region in und um Mardin (Südosttürkei), aus der ein Beitrag stammt.
/// </summary>
public class Region
{
    public int Id { get; set; }
    public string NameTurkish { get; set; } = string.Empty;
    public string NameGerman { get; set; } = string.Empty;
    public string? NameArabic { get; set; }

    public ICollection<VocabularyEntry> VocabularyEntries { get; set; } = new List<VocabularyEntry>();
}
