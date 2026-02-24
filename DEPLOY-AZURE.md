# Mardelli Wörterbuch – Schritt für Schritt auf Azure hosten

Du hast bereits einen Azure-Account. So bringst du die App in wenigen Schritten in die Cloud.

---

## Teil 1: App Service in Azure anlegen

### Schritt 1.1: Azure Portal öffnen

1. Gehe zu **https://portal.azure.com** und melde dich an.
2. Oben in der Suche „App Service“ eingeben und **App Services** auswählen.
3. Klicke auf **+ Create** (oder **+ Erstellen**).

### Schritt 1.2: Basis-Einstellungen

| Feld | Eingabe |
|------|--------|
| **Subscription** | Dein Abo (z. B. Free Trial oder Pay-As-You-Go) |
| **Resource Group** | Neu: z. B. `rg-mardelli` |
| **Name** | Eindeutiger Name, z. B. `mardelli-woerterbuch` (wird zu `mardelli-woerterbuch.azurewebsites.net`) |
| **Publish** | **Code** |
| **Runtime stack** | **.NET 8 (LTS)** |
| **Operating System** | **Windows** (einfacher für den Start) |
| **Region** | z. B. **West Europe** |

### Schritt 1.3: Plan (Tarif)

- Unter **App Service Plan** auf **Create new** klicken.
- Name z. B. `plan-mardelli`.
- **Pricing tier**:  
  - **Free (F1)** – zum Testen, App schläft nach Inaktivität.  
  - **Basic B1** – ca. 12–15 €/Monat, läuft dauerhaft, eigene Domain möglich.

**Create new** bestätigen, dann unten **Review + create** → **Create**. Warte, bis die Bereitstellung fertig ist (1–2 Minuten).

---

## Teil 2: App veröffentlichen (Publish aus Visual Studio / VS Code)

### Schritt 2.1: Projekt vorbereiten

1. Lösung in **Visual Studio 2022** oder **Rider** öffnen.
2. Im **Solution Explorer** mit Rechtsklick auf das Projekt **MardelliWeb** (nicht auf die Solution) klicken.
3. **Publish** (Veröffentlichen) wählen.

### Schritt 2.2: Ziel „Azure“ wählen

1. Ziel: **Azure** auswählen → **Next**.
2. Konkrete Ziel: **Azure App Service (Windows)** → **Next**.
3. Bei **Sign in** mit deinem Azure-Account anmelden, falls noch nicht geschehen.
4. In der Liste deinen **App Service** auswählen (z. B. `mardelli-woerterbuch`).
5. **Finish** klicken.

### Schritt 2.3: Publish-Profil speichern

- Das Publish-Profil wird unter `MardelliWeb/Properties/PublishProfiles/` gespeichert.
- Einmal **Publish** klicken – die App wird gebaut und nach Azure hochgeladen (kann 1–3 Minuten dauern).
- Am Ende öffnet sich oft automatisch die Website (`https://deine-app.azurewebsites.net`).

**Alternative ohne Visual Studio (Kommandozeile):**

```powershell
cd src\MardelliWeb
dotnet publish -c Release -o ./publish
```

Anschließend die Inhalte von `publish` z. B. per **Azure App Service – Deploy** (VS Code Extension) oder per **Zip Deploy** (siehe Azure-Dokumentation) hochladen.

---

## Teil 2b: Automatisches Deploy mit GitHub Actions (empfohlen)

Wenn du das Projekt in einem **GitHub-Repository** hast, kann bei jedem **Push auf `main`** (oder `master`) automatisch nach Azure deployed werden. So reicht ein `git push` – die App in Azure wird aktualisiert.

### Voraussetzungen

- Projekt ist auf **GitHub** (z. B. `https://github.com/DEIN-USER/MardelliDictionary`).
- **Azure Web App** ist bereits erstellt (Teil 1).
- Du hast einmalig per Publish oder manuell mindestens ein Deployment gemacht, damit die App in Azure existiert.

### Schritt 2b.1: Publish-Profil aus Azure holen

1. Im **Azure Portal** deine Web App öffnen (z. B. `mardelli-woerterbuch`).
2. Oben auf **Get publish profile** (oder **Download publish profile**) klicken – es lädt eine `.PublishSettings`-Datei herunter.
3. Die Datei mit einem Editor öffnen und **den kompletten Inhalt** (alles von `<publishData>` bis `</publishData>`) kopieren. Du brauchst ihn im nächsten Schritt.

### Schritt 2b.2: Geheimnis in GitHub anlegen

1. Auf **GitHub** dein Repository öffnen.
2. **Settings** → links **Secrets and variables** → **Actions**.
3. Auf **New repository secret** klicken.
4. **Name:** `AZURE_WEBAPP_PUBLISH_PROFILE` (genau so, Groß-/Kleinschreibung beachten).
5. **Value:** den kopierten Inhalt der Publish-Profil-Datei einfügen.
6. **Add secret** speichern.

### Schritt 2b.3: App-Namen im Workflow anpassen

Im Projekt liegt die Workflow-Datei **`.github/workflows/deploy-azure.yml`**.

1. Diese Datei öffnen.
2. In der Zeile **`AZURE_WEBAPP_NAME: mardelli-woerterbuch`** den Namen durch **den Namen deiner Azure Web App** ersetzen (so wie im Portal).
3. Speichern und mit dem Rest des Projekts committen und pushen.

### Schritt 2b.4: Branch prüfen

Der Workflow startet bei **Push auf den Branch `main`**. Wenn dein Standard-Branch **`master`** heißt:

- In **`.github/workflows/deploy-azure.yml`** die Zeile  
  `branches: [main]`  
  in  
  `branches: [master]`  
  ändern.

### Schritt 2b.5: Ablauf

- Sobald du Änderungen auf `main` (oder `master`) pushst, startet unter **Actions** der Lauf **Deploy to Azure Web App**.
- Nach einigen Minuten ist die App in Azure aktualisiert.
- Einen Lauf kannst du auch manuell starten: **Actions** → **Deploy to Azure Web App** → **Run workflow**.

**Hinweis:** Connection String und andere Einstellungen bleiben in Azure (Configuration) unverändert – nur der Code wird bei jedem Deploy erneuert.

---

## Teil 3: Konfiguration in Azure (Connection String & Co.)

### Schritt 3.1: Einstellungen im Portal

1. Im **Azure Portal** zu **App Services** gehen und deine App (z. B. `mardelli-woerterbuch`) öffnen.
2. Links im Menü **Configuration** (oder **Konfiguration**) → **Application settings** (Anwendungseinstellungen) wählen.

### Schritt 3.2: Connection String für SQLite (Standard)

Damit die App die SQLite-Datei auf dem Azure-Server nutzt:

1. Auf **+ New connection string** klicken.
2. **Name:** `DefaultConnection`
3. **Value:** `Data Source=Data/mardelli.db`  
   (Unterordner `Data` ist auf Azure beschreibbar und überlebt Neustarts besser.)
4. **Type:** leer lassen oder „Custom“.
5. **OK** → oben **Save** (Speichern). Die App wird dabei neu gestartet.

### Schritt 3.3: Admin-E-Mails (optional)

Falls du in `appsettings.json` unter **AdminEmails** E-Mail-Adressen hast, kannst du die gleiche Liste in Azure setzen:

1. **+ New application setting**
2. Name: `AdminEmails__0` → Value: deine-admin@email.de  
   (Weitere: `AdminEmails__1`, `AdminEmails__2` usw., falls du mehrere nutzt.)
3. **Save**.

---

## Teil 4: Code-Anpassung für Azure (SQLite-Pfad)

Damit die Datenbank im Ordner `Data` liegt (empfohlen auf Azure):

1. In **appsettings.json** die Connection String so setzen, dass Azure überschreiben kann:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=mardelli.db"
  },
  ...
}
```

2. In Azure (Application settings) den **Connection String** wie in Teil 3.2 auf  
   `Data Source=Data/mardelli.db`  
   setzen.

3. Sicherstellen, dass der Ordner **Data** existiert: In **Program.cs** (oder beim Start der App) einmalig das Verzeichnis anlegen, z. B.:

```csharp
// Ggf. am Anfang von Program.cs nach var builder = ...
var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDir);
```

Falls du möchtest, kann ich dir die genaue Stelle in deiner **Program.cs** dafür vorschlagen.

---

## Teil 5: Prüfen und Testen

1. Im Portal unter deiner App **Overview** → **URL** öffnen (z. B. `https://mardelli-woerterbuch.azurewebsites.net`).
2. Registrieren / Anmelden testen.
3. Wörterbuch, Vokabel eintragen, Medien hochladen kurz durchklicken.

**Hinweis:** Beim **Free Tier (F1)** kann der erste Aufruf nach längerer Inaktivität 1–2 Minuten dauern („Aufwachen“ der App).

---

## Kurz-Checkliste

- [ ] App Service (Windows, .NET 8) im Azure Portal erstellt  
- [ ] MardelliWeb per **Publish** aus Visual Studio (oder Zip Deploy) veröffentlicht  
- [ ] Connection String in Azure: `DefaultConnection` = `Data Source=Data/mardelli.db`  
- [ ] Optional: `Data`-Ordner im Code beim Start anlegen  
- [ ] App-URL im Browser getestet (Login, Wörterbuch, ggf. Upload)  
- [ ] **Optional – GitHub Actions:** Secret `AZURE_WEBAPP_PUBLISH_PROFILE` in GitHub gesetzt, `AZURE_WEBAPP_NAME` in `.github/workflows/deploy-azure.yml` angepasst, Branch `main`/`master` ggf. angepasst

---

## Typische Probleme

| Problem | Mögliche Lösung |
|--------|------------------|
| 500-Fehler nach Deploy | In Azure: **Configuration** → **Application settings** prüfen; Logs unter **Log stream** oder **Monitoring** ansehen. |
| DB-Fehler / Tabelle fehlt | App nutzt `EnsureCreated()` – beim ersten Start wird die DB angelegt. Connection String muss auf denselben Pfad zeigen wie in Azure (z. B. `Data/mardelli.db`). |
| Uploads gehen verloren | Auf Free/Basic wird das lokale Dateisystem bei Neustarts teils zurückgesetzt. Wichtige Uploads später in **Azure Blob Storage** auslagern. |

Wenn du einen bestimmten Schritt genauer brauchst (z. B. nur Portal oder nur Publish), sag einfach, welchen Teil du machst (Portal, Visual Studio, Konfiguration).
