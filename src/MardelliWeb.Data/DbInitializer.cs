using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    /// Erstellt die Rolle "Admin" und weist alle in AdminEmails (appsettings.json) genannten E-Mail-Adressen dieser Rolle zu.
    /// Erste Admins: E-Mail in "AdminEmails" eintragen (z. B. ["deine@email.de"]), App starten, ggf. Nutzer muss sich einmal registriert haben.
    /// </summary>
    public static async Task EnsureAdminRoleAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        if (!await roleManager.RoleExistsAsync(AdminRoleName))
            await roleManager.CreateAsync(new IdentityRole(AdminRoleName));

        var emails = config.GetSection("AdminEmails").Get<string[]>();
        if (emails == null) return;
        foreach (var email in emails)
        {
            if (string.IsNullOrWhiteSpace(email)) continue;
            var user = await userManager.FindByEmailAsync(email.Trim());
            if (user != null && !await userManager.IsInRoleAsync(user, AdminRoleName))
                await userManager.AddToRoleAsync(user, AdminRoleName);
        }
    }
}
