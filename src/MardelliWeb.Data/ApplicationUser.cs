using Microsoft.AspNetCore.Identity;
using MardelliWeb.Core;

namespace MardelliWeb.Data;

/// <summary>
/// Erweiterter Identity-User für Mardelli-Web (z. B. Heimatregion).
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public int? HomeRegionId { get; set; }
    public Region? HomeRegion { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
