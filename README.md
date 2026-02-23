# Mardelli Wörterbuch / Mardelli Sözlük

Webapp zur Dokumentation des **Mardelli-Arabisch**-Dialekts aus der Region Mardin (Südosttürkei).  
Authentifizierte Nutzer können Vokabeln (Türkisch→Mardelli oder Deutsch→Mardelli) und Video-/Textmaterial einreichen. **Nur von Admins freigegebene** Einträge erscheinen im Wörterbuch und bei den Materialien. Die Oberfläche ist auf **Deutsch** und **Türkisch** umschaltbar.

## Wo liegt das Projekt?

**Projektordner:** `MardelliDictionary` (z. B. bei dir: **`C:\Users\ussam\MardelliDictionary`**).

- **Lösung:** `MardelliDictionary.sln`
- **Webapp:** `src/MardelliWeb/`
- **Domain-Modelle:** `src/MardelliWeb.Core/`
- **Datenbank & Identity:** `src/MardelliWeb.Data/`
- **Konfiguration:** `src/MardelliWeb/appsettings.json`
- **Datenbank & Azure:** siehe [docs/DATENBANK-UND-AZURE.md](docs/DATENBANK-UND-AZURE.md)
- **Design & Templates:** siehe [docs/DESIGN-TIPPS.md](docs/DESIGN-TIPPS.md)
- **Bilder einfügen (5 Plätze):** siehe [docs/BILDER-EINFUEGEN.md](docs/BILDER-EINFUEGEN.md)

## Tech-Stack

- **.NET 8**, **C#**, **ASP.NET Core**
- **Blazor Server** (eine Codebasis, alles in C#)
- **Entity Framework Core** (SQLite für Entwicklung, SQL Server möglich)
- **ASP.NET Core Identity** (Registrierung, Login, Rolle „Admin“)
- **Lokalisierung**: DE/TR über Query-Parameter `?culture=de` / `?culture=tr`

## Freigabe-Workflow (Admin)

- **Normale Nutzer** (per E-Mail registriert): Können Vokabeln und Materialien **einreichen**. Diese erscheinen **nicht** sofort im Wörterbuch bzw. unter Materialien.
- **Admins**: Sehen unter **Freigabe (Admin)** alle ausstehenden Einträge und können sie **freigeben** oder **ablehnen**. Nur freigegebene Einträge sind öffentlich sichtbar.

## Admin festlegen

Admins werden über die **E-Mail-Adresse** in der Konfiguration festgelegt:

1. In **`src/MardelliWeb/appsettings.json`** den Abschnitt **`AdminEmails`** setzen (Array von E-Mail-Adressen):
   ```json
   "AdminEmails": [ "deine@email.de", "admin2@beispiel.de" ]
   ```
2. Der betreffende Nutzer muss sich **mindestens einmal registriert** haben (gleiche E-Mail wie in `AdminEmails`).
3. **App neu starten** – beim Start werden diese Nutzer automatisch der Rolle **Admin** zugeordnet.
4. Danach erscheint im Menü der Link **Freigabe (Admin)**; nur diese Nutzer können Einträge freigeben/ablehnen.

**Erster Admin:** Zuerst mit der gewünschten E-Mail registrieren, dann diese E-Mail in `AdminEmails` eintragen und die App neu starten.

## Schrift: Arabisch und Latein

- **Mardelli (arabische Schrift)** – Hauptfeld für die arabische Schreibweise.
- **Mardelli (lateinische Schrift)** – optionales Feld für die lateinische Schreibweise (z. B. „Sickar“).  
Beide werden im Wörterbuch angezeigt; die Suche berücksichtigt arabische und lateinische Einträge.

## Projektstruktur

- `src/MardelliWeb` – Blazor Server Webapp (Seiten, Layout, Services)
- `src/MardelliWeb.Core` – Domain-Modelle (VocabularyEntry, Region, MediaItem, EntryStatus, Enums)
- `src/MardelliWeb.Data` – DbContext, Identity (ApplicationUser), DbInitializer, Admin-Rolle

## Voraussetzungen

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installieren (falls noch nicht vorhanden).

## App starten und ausprobieren (Schritt für Schritt)

1. **Terminal/PowerShell öffnen** und in den Projektordner wechseln:
   ```bash
   cd C:\Users\ussam\MardelliDictionary
   ```

2. **App starten:**
   ```bash
   dotnet run --project src/MardelliWeb
   ```

3. **Im Browser öffnen:**  
   Die Konsole zeigt eine URL, z. B. `https://localhost:5001` oder `http://localhost:5000`. Diese Adresse im Browser aufrufen.

4. **Als Admin einrichten (einmalig):**
   - **Registrieren** (oben rechts) mit deiner E-Mail **ussama-turgut@outlook.com** und einem Passwort.
   - App im Terminal mit **Strg+C** beenden und wieder mit `dotnet run --project src/MardelliWeb` starten.  
   Dann wirst du automatisch als Admin erkannt (steht bereits in `appsettings.json`).

5. **Ausprobieren:**
   - **Anmelden** mit ussama-turgut@outlook.com.
   - Im Menü erscheint **Freigabe (Admin)** – dort kannst du eingereichte Vokabeln und Materialien freigeben oder ablehnen.
   - **Vokabel eintragen** oder **Medien hochladen** – deine eigenen Einträge kannst du als Admin direkt unter **Freigabe (Admin)** freigeben, dann erscheinen sie im **Wörterbuch** bzw. unter **Materialien**.
   - **Sprache** oben rechts auf DE oder TR umstellen.

## Lokal starten (Kurz)

```bash
cd C:\Users\ussam\MardelliDictionary
dotnet run --project src/MardelliWeb
```

Dann im Browser: **https://localhost:5001** (oder die in der Konsole angezeigte URL).  
Beim ersten Start wird die SQLite-Datenbank `mardelli.db` im Projektordner angelegt und die Beispiel-Regionen eingetragen.  
**Falls du bereits eine ältere `mardelli.db` hast** (vor Freigabe-/Latein-Update): Entweder `mardelli.db` löschen und neu starten, oder Migrations ausführen (siehe Konfiguration).

- **Sprache**: DE/TR oben rechts oder per URL `?culture=de` / `?culture=tr`
- **Registrieren** → Konto anlegen → **Vokabel eintragen** / **Medien hochladen** (Einträge warten auf Freigabe)
- **Wörterbuch** / **Materialien**: Zeigen nur **freigegebene** Einträge
- **Admins**: Nach Eintrag in `AdminEmails` und Neustart Menüpunkt **Freigabe (Admin)**

## Konfiguration

- **Datenbank**: In `src/MardelliWeb/appsettings.json` unter `ConnectionStrings:DefaultConnection`  
  - SQLite (Standard): `"Data Source=mardelli.db"`  
  - SQL Server: z. B. `"Server=(localdb)\\mssqllocaldb;Database=MardelliDictionary;Trusted_Connection=True;"`
- **Admins**: `AdminEmails` in `appsettings.json` (siehe Abschnitt „Admin festlegen“).
- Für **Produktion**: Migrations nutzen statt `EnsureCreated()`:
  ```bash
  dotnet ef migrations add Initial --project src/MardelliWeb.Data --startup-project src/MardelliWeb
  dotnet ef database update --project src/MardelliWeb.Data --startup-project src/MardelliWeb
  ```
  In `Program.cs` dann `db.Database.EnsureCreated()` durch `await db.Database.MigrateAsync()` ersetzen.

## Geplante Erweiterung: RAG + Sprachbot

Die Datenstruktur ist so angelegt, dass später ein **RAG-System** und ein **Sprachbot** integriert werden können (Wörterbuch + freigegebene Medien als Grundlage für Embeddings/Vektor-DB und LLM-Kontext).

## Lizenz

Projekt für die Dokumentation des Mardelli-Dialekts; Lizenz nach Bedarf festlegen.
