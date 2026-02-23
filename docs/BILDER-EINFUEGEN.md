# Wo du deine 5 Bilder einfügen sollst

Alle Bilder legst du in **`src/MardelliWeb/wwwroot/images/`**.  
Wenn der Ordner noch nicht existiert: anlegen, dann die Dateien dort ablegen.

In der App werden sie so eingebunden: **`/images/Dateiname.jpg`** (z. B. im Browser: `https://localhost:5001/images/hero.jpg`).

---

## Empfohlene Plätze für 5 Bilder

| # | Platz | Zweck | Dateiname (Vorschlag) | Wo in der App |
|---|--------|------|------------------------|----------------|
| **1** | **Startseite – Hero** | Großes Stimmungsbild oben (Mardin, Altstadt, Steinhäuser) | `hero.jpg` | Startseite (`/`), ganz oben, volle Breite |
| **2** | **Sidebar / Logo** | Kleines Logo oder Icon (z. B. Buch + Mardin-Silhouette) | `logo.png` oder `sidebar-logo.png` | Linke Sidebar, über oder neben „Mardelli Sözlük / Wörterbuch“ |
| **3** | **Startseite – Region** | Bild zur Region / Leute / Sprache (Mardin, Südosttürkei) | `mardin-region.jpg` | Startseite, unter dem Textblock, als Stimmungsbild |
| **4** | **Wörterbuch-Seite** | Header oder dezentes Hintergrundbild für die Wörterbuch-Seite | `dictionary-header.jpg` | Seite „Wörterbuch“ (`/dictionary`), oben unter dem Titel |
| **5** | **Materialien oder Footer** | Banner für Materialien oder Abschluss der Startseite | `materials-banner.jpg` | Entweder oben auf „Materialien“ (`/materials`) oder am Ende der Startseite |

---

## Kurz-Anleitung

1. Ordner anlegen: **`src/MardelliWeb/wwwroot/images/`**
2. Deine 5 Bilder dort speichern – am besten mit den Namen oben (dann musst du im Code nichts ändern).
3. Wenn du andere Dateinamen nutzt: In der Doku bzw. in den Razor-Dateien (Home.razor, NavMenu.razor, Dictionary.razor, Materials.razor) den Pfad anpassen, z. B. von `hero.jpg` auf `mein-bild.jpg`.

**Formate:** JPG oder PNG. Hero/Banner ruhig groß (z. B. 1200–1600 px breit), Logo klein (z. B. 200 px).

Wenn du die Bilder mit genau diesen Namen ablegst, sind die Platzhalter in der App schon vorbereitet – sie zeigen deine Bilder an, sobald die Dateien im Ordner liegen.
