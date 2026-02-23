# Design-Tipps: Templates & Bilder

Das Projekt nutzt jetzt ein überarbeitetes Standard-Design (Schrift **DM Sans**, Akzentfarbe Terracotta, Sidebar mit Verlauf). Wenn du noch professioneller wirken willst, hier konkrete Ideen.

---

## 1. Bezahlbare Blazor/ASP.NET-Templates

- **ThemeForest (Envato):** Suche nach „Blazor Admin Template“ oder „ASP.NET Core Dashboard“. Viele Admin-/Dashboard-Themes für 20–50 €, die du in dein Blazor-Projekt einbauen kannst (Layout + CSS + ggf. Komponenten).
- **WrapPixel / AdminMart:** Blazor- und Bootstrap-basierte Admin-Templates (teils kostenlos, teils Premium).
- **MudBlazor:** Kostenlose UI-Bibliothek für Blazor mit vielen Komponenten und Themes. Du könntest schrittweise Bootstrap durch MudBlazor ersetzen und ein fertiges Theme wählen.

**Empfehlung:** Wenn du wenig anfassen willst: ein **Bootstrap-basiertes Blazor- oder Admin-Template** von ThemeForest kaufen und nur Farben/Logo anpassen. Wenn du lieber mit Komponenten arbeitest: **MudBlazor** (kostenlos) ausprobieren.

---

## 2. Bilder und Illustrationen

- **Header/Hero:** Ein Bild von **Mardin** (Altstadt, Steinhäuser) oder eine dezente Illustration (Wörterbuch, Sprache, Kultur) macht die Startseite einprägsam. Quellen: eigene Fotos, **Unsplash** (kostenlos), **Pexels**.
- **Sidebar/Logo:** Ein kleines **Logo oder Icon** (z. B. Buch + arabische Schrift oder Mardin-Silhouette) statt nur Text. Kann ich als **autogeneriertes Bild** vorschlagen (z. B. „minimales Logo Mardelli Wörterbuch, Buch mit arabischen Zeichen“).
- **Hintergrund:** Auf der Startseite ein **sanftes Muster oder ein sehr dezent eingeblendetes Bild** (niedrige Opacity), um die weiße Fläche zu brechen.

Wenn du sagst, was du brauchst (z. B. „Logo für Sidebar“, „Hero-Bild Startseite“), kann ich dir **konkrete Beschreibungen für KI-Bildgeneratoren** (z. B. DALL·E, Midjourney, Stable Diffusion) formulieren oder ein **einfaches Platzhalter-Logo** als Idee beschreiben, das du später ersetzen kannst.

---

## 3. Was im Projekt schon geändert wurde

- **Navigation:** Alle Menülinks nutzen jetzt **absolute Pfade** (z. B. `/`, `/dictionary`). „Start“ führt wieder zuverlässig auf die Startseite.
- **Schrift:** **DM Sans** (Google Fonts) für einen modernen Look.
- **Farben:** Sidebar mit Verlauf (Dunkelblau), Akzentfarbe Terracotta (`#c05621`) für Buttons und Links.
- **Sidebar:** Klarere Hover- und Active-Zustände, Icons (Bootstrap Icons) sichtbar.
- **Inhalt:** Leicht abgesetzte Karten für Tabellen/Filter, weicher Hintergrund.

Zum Testen: App neu starten und im Browser prüfen. Wenn du einen bestimmten Stil (z. B. „noch ruhiger“, „mehr Farbe“, „nur Schwarz-Weiß“) willst, kann das CSS gezielt angepasst werden.
