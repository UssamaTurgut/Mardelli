using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MardelliWeb.Core;

namespace MardelliWeb.Data;

public static class DbInitializer
{
    public const string AdminRoleName = "Admin";

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Regions.AnyAsync())
            return;
        await db.Regions.AddRangeAsync(
            new Region { Id = 1, NameTurkish = "Ömerli", NameGerman = "Ömerli", NameArabic = "ميديات" },
            new Region { Id = 2, NameTurkish = "Mardin Merkez", NameGerman = "Mardin Zentrum", NameArabic = "ماردين" },
            new Region { Id = 3, NameTurkish = "Midyat", NameGerman = "Midyat", NameArabic = "ميديات" },
            new Region { Id = 4, NameTurkish = "Nusaybin", NameGerman = "Nusaybin", NameArabic = "نصيبين" },
            new Region { Id = 5, NameTurkish = "Savur", NameGerman = "Savur", NameArabic = "ساور" },
            new Region { Id = 6, NameTurkish = "Diyarbakır / Almanya / Diğer", NameGerman = "Diyarbakır / Deutschland / Sonstige", NameArabic = null }
        );
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Erstellt die Rolle "Admin" und weist alle in AdminEmails genannten E-Mail-Adressen dieser Rolle zu.
    /// Liest aus: appsettings.json "AdminEmails": ["a@x.com"] ODER Azure/Env "AdminEmails" = "a@x.com,b@x.com" (kommagetrennt).
    /// </summary>
    public static async Task EnsureAdminRoleAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration config, ILogger? logger = null)
    {
        if (!await roleManager.RoleExistsAsync(AdminRoleName))
            await roleManager.CreateAsync(new IdentityRole(AdminRoleName));

        var emails = config.GetSection("AdminEmails").Get<string[]>();
        if (emails == null || emails.Length == 0)
        {
            var single = config["AdminEmails"]?.Trim();
            if (!string.IsNullOrEmpty(single))
                emails = single.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
        if (emails == null || emails.Length == 0)
        {
            logger?.LogWarning("EnsureAdminRole: Keine AdminEmails in Konfiguration gefunden (AdminEmails oder AdminEmails__0 in Azure setzen).");
            return;
        }

        foreach (var email in emails)
        {
            if (string.IsNullOrWhiteSpace(email)) continue;
            var trimmed = email.Trim();
            var user = await userManager.FindByEmailAsync(trimmed);
            if (user == null)
                user = await userManager.FindByNameAsync(trimmed);
            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, AdminRoleName))
                {
                    await userManager.AddToRoleAsync(user, AdminRoleName);
                    logger?.LogInformation("EnsureAdminRole: {Email} wurde zur Admin-Rolle hinzugefügt.", trimmed);
                }
            }
            else
                logger?.LogWarning("EnsureAdminRole: Kein User mit E-Mail/Name \"{Email}\" gefunden (zuerst registrieren).", trimmed);
        }
    }
}
