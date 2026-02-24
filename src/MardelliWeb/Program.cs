using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MardelliWeb.Data;
using MardelliWeb.Components;
using MardelliWeb;
using MardelliWeb.Core;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Für Azure: Ordner "Data" anlegen, falls Connection String z. B. "Data Source=Data/mardelli.db" ist
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(conn) && conn.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
{
    var path = conn.Replace("Data Source=", "").Trim();
    var dir = Path.GetDirectoryName(path);
    if (!string.IsNullOrEmpty(dir))
        Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, dir));
}

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(conn) && (conn.Contains("Data Source=") || conn.EndsWith(".db")))
        options.UseSqlite(conn.Contains("Data Source=") ? conn : $"Data Source={conn}");
    else
        options.UseSqlServer(conn ?? "Server=(localdb)\\mssqllocaldb;Database=MardelliDictionary;Trusted_Connection=True;");
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("de"), new CultureInfo("tr") };
    options.DefaultRequestCulture = new RequestCulture("de");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider { QueryStringKey = "culture" });
});

builder.Services.AddScoped<DictionaryService>();
builder.Services.AddScoped<MediaService>();

var app = builder.Build();

app.UseRequestLocalization();
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPost("/api/media/upload", async (
    HttpContext ctx,
    IFormCollection form,
    MediaService mediaService,
    DictionaryService dictionaryService) =>
{
    var user = ctx.User;
    if (user?.Identity?.IsAuthenticated != true)
        return Results.Redirect("/Account/Login");
    var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
        return Results.Redirect("/Account/Login");

    var title = form["Title"].ToString().Trim();
    if (string.IsNullOrEmpty(title))
        return Results.Redirect("/media?error=" + Uri.EscapeDataString("Bitte einen Titel eingeben."));

    var typeStr = form["Type"].ToString();
    var type = MediaType.Text;
    if (string.Equals(typeStr, "Video", StringComparison.OrdinalIgnoreCase) || typeStr == "0")
        type = MediaType.Video;
    var desc = form["Description"].ToString().Trim();
    var regionIdStr = form["RegionId"].ToString();
    int.TryParse(regionIdStr, out var regionId);

    var file = form.Files.GetFile("File");
    Stream? stream = null;
    string? fileName = null;
    if (file != null && file.Length > 0)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var contentType = file.ContentType?.ToLowerInvariant() ?? "";

        if (type == MediaType.Text)
        {
            var textExts = new[] { ".txt", ".md", ".pdf" };
            var isTextContent = contentType.StartsWith("text/") || contentType == "application/pdf";
            if (!textExts.Contains(ext) && !isTextContent)
                return Results.Redirect("/media?error=" + Uri.EscapeDataString("Bei Typ „Text“ sind nur Textdateien (.txt, .md, .pdf) erlaubt."));
        }
        else
        {
            var videoExts = new[] { ".mp4", ".webm", ".mov", ".avi", ".mkv", ".m4v" };
            var isVideoContent = contentType.StartsWith("video/");
            if (!videoExts.Contains(ext) && !isVideoContent)
                return Results.Redirect("/media?error=" + Uri.EscapeDataString("Bei Typ „Video“ sind nur Videodateien erlaubt."));
        }

        stream = file.OpenReadStream();
        fileName = file.FileName;
    }

    try
    {
        var item = new MediaItem
        {
            Type = type,
            Title = title,
            Description = string.IsNullOrWhiteSpace(desc) ? null : desc,
            RegionId = regionId > 0 ? regionId : null
        };
        await mediaService.AddAsync(item, userId, stream, fileName);
        if (stream != null) await stream.DisposeAsync();
    }
    catch
    {
        if (stream != null) await stream.DisposeAsync();
        return Results.Redirect("/media?error=" + Uri.EscapeDataString("Fehler beim Hochladen."));
    }

    return Results.Redirect("/media?success=1");
})
.RequireAuthorization();

app.MapPost("/api/account/login", async (
    HttpContext ctx,
    IFormCollection form,
    SignInManager<ApplicationUser> signInManager) =>
{
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var rememberMe = form["RememberMe"].ToString().Equals("on", StringComparison.OrdinalIgnoreCase) || form["RememberMe"].ToString() == "true";
    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        return Results.Redirect("/Account/Login?error=" + Uri.EscapeDataString("E-Mail und Passwort eingeben."));
    var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
    if (result.Succeeded)
        return Results.Redirect("/");
    return Results.Redirect("/Account/Login?error=" + Uri.EscapeDataString("E-Mail oder Passwort falsch."));
}).AllowAnonymous();

app.MapPost("/api/account/register", async (
    HttpContext ctx,
    IFormCollection form,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) =>
{
    var email = form["Email"].ToString().Trim();
    var password = form["Password"].ToString();
    var confirmPassword = form["ConfirmPassword"].ToString();
    if (string.IsNullOrEmpty(email))
        return Results.Redirect("/Account/Register?error=" + Uri.EscapeDataString("E-Mail eingeben."));
    if (string.IsNullOrEmpty(password))
        return Results.Redirect("/Account/Register?error=" + Uri.EscapeDataString("Bitte Passwort eingeben."));
    if (password != confirmPassword)
        return Results.Redirect("/Account/Register?error=" + Uri.EscapeDataString("Passwörter stimmen nicht überein."));
    var user = new ApplicationUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);
    if (result.Succeeded)
    {
        await signInManager.SignInAsync(user, isPersistent: false);
        return Results.Redirect("/");
    }
    var err = result.Errors.FirstOrDefault()?.Description ?? "Registrierung fehlgeschlagen.";
    return Results.Redirect("/Account/Register?error=" + Uri.EscapeDataString(err));
}).AllowAnonymous();

app.MapGet("/api/admin/delete-vocab/{id:int}", async (int id, HttpContext ctx, DictionaryService dictionaryService) =>
{
    if (!ctx.User.IsInRole("Admin"))
        return Results.Redirect("/dictionary");
    await dictionaryService.DeleteVocabularyAsync(id);
    return Results.Redirect("/dictionary");
}).RequireAuthorization();

app.MapGet("/api/admin/delete-media/{id:int}", async (int id, HttpContext ctx, MediaService mediaService) =>
{
    if (!ctx.User.IsInRole("Admin"))
        return Results.Redirect("/materials");
    await mediaService.DeleteMediaAsync(id);
    return Results.Redirect("/materials");
}).RequireAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .DisableAntiforgery();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var logger = scope.ServiceProvider.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();

    db.Database.EnsureCreated();
    DbInitializer.SeedAsync(db).GetAwaiter().GetResult();
    DbInitializer.EnsureAdminRoleAsync(roleManager, userManager, config, logger).GetAwaiter().GetResult();
}

app.Run();
