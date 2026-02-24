# Azure + GitHub – In welcher Reihenfolge einrichten?

Folge den Schritten **von oben nach unten**. Alles in Azure zuerst, dann GitHub, dann verbinden.

---

## Phase 1: Alles in Azure

### 1. App Service anlegen

1. Gehe zu **https://portal.azure.com** und melde dich an.
2. Suche oben nach **„App Services“** und wähle **App Services**.
3. Klicke auf **+ Create** → **Web App** (nicht „Web App + Database“).
4. Fülle aus:
   - **Subscription:** dein Abo
   - **Resource Group:** neu, z. B. `rg-mardelli`
   - **Name:** z. B. `mardelli-woerterbuch` (wird deine URL: `mardelli-woerterbuch.azurewebsites.net`)
   - **Publish:** Code
   - **Runtime stack:** .NET 8 (LTS)
   - **Operating System:** Windows
   - **Region:** z. B. West Europe
5. Bei **App Service Plan** auf **Create new** → Name z. B. `plan-mardelli`, **Pricing tier** z. B. **Free F1** (oder Basic B1).
6. **Review + create** → **Create**. Warten, bis „Your deployment is complete“.

**Merken:** Den **Namen** deiner Web App (z. B. `mardelli-woerterbuch`) – den brauchst du gleich noch.

---

### 2. Connection String in Azure setzen

1. Im Portal: **App Services** → deine App (z. B. `mardelli-woerterbuch`) öffnen.
2. Links **Configuration** (Konfiguration) → **Application settings**.
3. Unter **Connection strings** auf **+ New connection string**.
4. **Name:** `DefaultConnection`  
   **Value:** `Data Source=Data/mardelli.db`  
   **Type:** Custom (oder leer lassen).
5. **OK** → oben **Save**. App startet neu – ist in Ordnung.

---

### 3. Publish-Profil herunterladen (für GitHub)

1. Im gleichen App-Service (Übersichtsseite deiner Web App).
2. Oben auf **Get publish profile** (oder **Download publish profile**).
3. Die Datei wird heruntergeladen (z. B. `mardelli-woerterbuch.PublishSettings`).
4. Diese Datei mit **Editor/Notepad** öffnen und **den gesamten Inhalt** kopieren (Strg+A, Strg+C).  
   Du brauchst ihn im nächsten Abschnitt in GitHub.

**Azure ist damit fertig eingerichtet.**

---

## Phase 2: Alles in GitHub

### 4. Projekt auf GitHub haben

- Wenn das Projekt **noch nicht** auf GitHub ist: Neues Repository auf github.com anlegen, dann lokal z. B.:
  ```bash
  git remote add origin https://github.com/DEIN-USERNAME/MardelliDictionary.git
  git push -u origin main
  ```
- Wenn das Projekt **schon** auf GitHub ist: Nichts tun, weiter zu Schritt 5.

---

### 5. Geheimnis (Secret) für Azure anlegen

1. Öffne dein **GitHub-Repository** im Browser.
2. Oben: **Settings** (Einstellungen).
3. Links: **Secrets and variables** → **Actions**.
4. **New repository secret**.
5. **Name (exakt so):** `AZURE_WEBAPP_PUBLISH_PROFILE`
6. **Secret:** Den **kompletten Inhalt** der Publish-Profil-Datei aus Schritt 3 einfügen (Strg+V).
7. **Add secret** klicken.

---

### 6. App-Namen im Workflow eintragen

1. Im **Projekt auf deinem PC** die Datei öffnen:  
   **`.github/workflows/deploy-azure.yml`**
2. Suche die Zeile:  
   `AZURE_WEBAPP_NAME: mardelli-woerterbuch`
3. Ersetze `mardelli-woerterbuch` durch **den exakten Namen deiner Azure Web App** (aus Schritt 1).
4. Speichern.

**Falls dein Haupt-Branch `master` heißt (nicht `main`):**  
In derselben Datei die Zeile `branches: [main]` zu `branches: [master]` ändern.

---

### 7. Änderungen pushen

1. Commit z. B.:
   ```bash
   git add .
   git commit -m "Azure Deploy Workflow und App-Namen gesetzt"
   git push origin main
   ```
   (Oder `master` statt `main`, wenn du `master` nutzt.)

2. Auf **GitHub** → Tab **Actions** gehen.  
   Dort sollte ein Lauf **„Deploy to Azure Web App“** starten und nach ein paar Minuten grün werden.

---

## Phase 3: Prüfen

### 8. App im Browser testen

1. Im **Azure Portal** → deine Web App → oben **Overview** → bei **URL** auf den Link klicken  
   (z. B. `https://mardelli-woerterbuch.azurewebsites.net`).
2. Seite sollte laden: Registrieren/Anmelden und Wörterbuch testen.

---

## Kurz: Reihenfolge

| Schritt | Wo      | Was |
|--------|---------|-----|
| 1      | Azure   | Web App erstellen (Name merken) |
| 2      | Azure   | Connection String `DefaultConnection` = `Data Source=Data/mardelli.db` |
| 3      | Azure   | Publish-Profil herunterladen, Inhalt kopieren |
| 4      | GitHub  | Repo haben / Code pushen |
| 5      | GitHub  | Secret `AZURE_WEBAPP_PUBLISH_PROFILE` anlegen (Inhalt aus Schritt 3) |
| 6      | Projekt | In `.github/workflows/deploy-azure.yml` den Azure-App-Namen eintragen |
| 7      | GitHub  | Commit + Push → Actions startet Deploy |
| 8      | Browser | Azure-URL öffnen und testen |

Ab dann: Bei jedem **Push auf main** (oder master) wird automatisch nach Azure deployed.

---

## Nach der ersten Registrierung auf Azure: Admin wird erst nach Neustart

Die Admin-Rolle wird **nur beim Start der App** an die E-Mails aus `AdminEmails` vergeben. Auf Azure war die Datenbank beim ersten Start noch leer – du hast dich danach registriert. Dein User existiert, hat aber beim Start noch keine Admin-Rolle bekommen.

**Lösung:** Azure Web App einmal neu starten.

1. **Azure Portal** → deine Web App → **Overview** → oben **Restart** → bestätigen.
2. 1–2 Minuten warten, dann erneut einloggen – „Freigabe (Admin)“ sollte erscheinen.

**Wichtig für Admin auf Azure:** Die Admin-E-Mail muss in Azure stehen, nicht nur in appsettings.json.  
Unter **Configuration** → **Application settings** → **+ New application setting**:

- **Name:** `AdminEmails` (genau so)
- **Value:** `ussama-turgut@outlook.com`

→ **OK** → oben **Save**. App startet neu, danach erneut einloggen.  
(Alternativ: `AdminEmails__0` = `ussama-turgut@outlook.com` – zwei Unterstriche.)
