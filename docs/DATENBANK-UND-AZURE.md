# Wo liegen die Daten? Datenbank & Azure

## Aktuelles System

- **Datenbank:** **SQLite**
- **Datei:** `mardelli.db` – liegt im **Projektordner** der Web-App, also:
  - **Lokal:** `C:\Users\ussam\MardelliDictionary\src\MardelliWeb\mardelli.db`  
    (wird beim ersten Start automatisch angelegt)
- **Konfiguration:** In `src/MardelliWeb/appsettings.json` unter  
  `ConnectionStrings.DefaultConnection` → `"Data Source=mardelli.db"`

SQLite ist eine **dateibasierte** Datenbank: eine einzige Datei, keine separate Datenbank-Installation nötig. Ideal für Entwicklung und kleine bis mittlere Projekte.

---

## Azure: kostengünstig und sinnvoll

Für **Produktion** in Azure gibt es zwei pragmatische Wege:

### Option A: Azure App Service + Azure SQL Database (empfohlen)

- **App:** Blazor-App auf **Azure App Service** (Windows oder Linux).
- **Datenbank:** **Azure SQL Database** (gehosteter SQL Server).
- **Vorteile:** Skalierbar, Backups, keine eigene Server-Pflege.
- **Kosten (ca.):**
  - App Service: z. B. **B1** (~12 €/Monat) oder **F1** (Free Tier zum Testen).
  - Azure SQL: **Basic** (~5 €/Monat) oder **Serverless** (nutzungsbasiert, oft nur wenige Euro).

**Schritte (Kurz):**

1. In Azure: **App Service** + **Azure SQL Server** + **Azure SQL Database** anlegen.
2. In `appsettings.json` (bzw. App Service → Konfiguration) die Connection String auf Azure SQL umstellen, z. B.:
   ```text
   Server=tcp:<dein-server>.database.windows.net,1433;Initial Catalog=MardelliDictionary;User ID=<user>;Password=<pass>;Encrypt=True;
   ```
3. EF Core Migrations ausführen (siehe README), damit die Tabellen in Azure SQL angelegt werden.
4. In `Program.cs` für Produktion `EnsureCreated()` durch `MigrateAsync()` ersetzen (steht so schon im README).

### Option B: App Service + SQLite auf Netzlaufwerk (eher Experiment)

- SQLite-Datei auf einer **Azure Files**-Freigabe ablegen und in der App auf diesen Pfad zeigen.
- **Nachteil:** Weniger robust bei mehreren Instanzen und bei Neustarts; nicht für „richtige“ Produktion empfohlen.

**Fazit:** Für ein sauberes, kostengünstiges Setup: **Option A (App Service + Azure SQL)**.

---

## Kostenüberblick (grobe Richtwerte)

| Komponente        | Günstige Wahl | Ca. Kosten      |
|------------------|---------------|-----------------|
| App Service      | F1 (Free)     | 0 €             |
|                  | B1            | ~12 €/Monat     |
| Azure SQL        | Basic / Small | ~5 €/Monat      |
|                  | Serverless    | nutzungsbasiert |

Mit **F1 + Azure SQL Basic/Serverless** kommst du mit wenigen Euro pro Monat aus; für mehr Nutzer dann B1 + etwas größere DB.

---

## Was du konkret anpassen musst

1. **Connection String** in Azure (App Service → Konfiguration / Verbindungszeichenfolgen) auf deine Azure-SQL-Instanz setzen.
2. **Migrations** einmalig ausführen (lokal oder in der Build-Pipeline), damit die Tabellen in Azure SQL existieren.
3. **AdminEmails** und ggf. andere Einstellungen in den App-Einstellungen (Umgebungsvariablen) in Azure setzen.

Wenn du willst, können wir als Nächstes Schritt für Schritt die Azure-Ressourcen (App Service + SQL) und die Umstellung der Connection String durchgehen.
