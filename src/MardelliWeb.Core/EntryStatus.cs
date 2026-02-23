namespace MardelliWeb.Core;

/// <summary>
/// Freigabe-Status: Nur von Admins freigegebene Einträge erscheinen im Wörterbuch/Materialien.
/// </summary>
public enum EntryStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
