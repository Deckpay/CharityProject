# Értékmentő – Digitális jótékonysági platform

> Digitális jótékonysági platformunkkal az **okoseszközzel rendelkező**, nehéz sorsú rétegek számára teremtünk közvetlen utat az életkörülményeik javításához.

---

## 👥 Csapat tagjai

| Név | Szerepkör |
|---|---|
| **Kemény Benedek** | Fejlesztő |
| **Papp Bence** | Fejlesztő |

---

## 📌 Projekt címe

**Értékmentő** – Digitális jótékonysági platform

---

## 📖 Projekt rövid ismertetése

Az **Értékmentő** egy webalapú jótékonysági platform, amelynek célja közvetlen kapcsolat teremtése az adományozók és az igénylők között. A platform lehetővé teszi, hogy feleslegessé vált, de még jó állapotú tárgyakat (bútorok, elektronikai eszközök, ruhák, könyvek stb.) ingyenesen felajánljanak azoknak, akiknek valóban szükségük van rájuk.

A rendszer külön szerepkörrel kezeli az **adományozókat** és az **igénylőket**, területi szűrést biztosít, valamint kategóriánként szabályozható igénylési limiteket alkalmaz, hogy az esélyegyenlőség fennmaradjon.

### 🛠️ Alkalmazott technológiák

| Technológia | Verzió | Felhasználási terület |
|---|---|---|
| C# / ASP.NET Core Web API | .NET 8 | Backend logika, REST API |
| Blazor Server | .NET 8 | Webes felhasználói felület |
| Entity Framework Core | 8.x | ORM – adatbázis-kezelés |
| MS SQL Server / T-SQL | 2019+ | Relációs adatbázis |
| SignalR | ASP.NET Core | Valós idejű chat kommunikáció |
| JWT (JSON Web Token) | Bearer Auth | Felhasználó-hitelesítés |
| BCrypt.Net | 4.x | Jelszó hash-elés |
| Bootstrap 5 | 5.x | Reszponzív UI keretrendszer |

### 🏗️ Architektúra

Az alkalmazás a **Clean Architecture** elveit követi, öt fő rétegre bontva:
- `Domain` – üzleti entitások és szabályok
- `Application` – alkalmazáslogika, service-ek
- `Infrastructure` – adatbázis, külső szolgáltatások
- `API` – REST API végpontok
- `WEB` – Blazor Server frontend

---

## 🧪 Tesztfelhasználók

A rendszer gyors kipróbálásához az alábbi előre létrehozott tesztfelhasználók használhatók:

### 👤 Adományozó

| Mező | Érték |
|---|---|
| **Felhasználónév** | Felado |
| **Email** | userSender@teszt.com |
| **Jelszó** | 123123 |

**Funkciók:**
- Adományok feltöltése
- Saját felajánlások kezelése
- Kapcsolattartás az igénylőkkel

---

### 🤝 Igénylő

| Mező | Érték |
|---|---|
| **Felhasználónév** | Igenylo |
| **Email** | userRequester@teszt.com |
| **Jelszó** | 123123 |

**Funkciók:**
- Adományok böngészése
- Jelentkezés tárgyakra
- Kommunikáció az adományozókkal

---

### 🛡️ Admin

| Mező | Érték |
|---|---|
| **Felhasználónév** | Admin |
| **Email** | useradmin@teszt.com |
| **Jelszó** | 123123 |

**Funkciók:**
- Felhasználók kezelése
- Tartalmak moderálása
- Rendszer felügyelet

---

## 🔨 Munkamegosztás

| Feladat | Kemény Benedek | Papp Bence |
|---|---|---|
| Backend (API, Domain, Infrastructure) | ✅ | ✅ |
| Frontend (Blazor WEB) | ✅ | ✅ |
| Adatbázis tervezés | ✅ | ✅ |
| Tesztelés, dokumentáció | ✅ | ✅ |

---

## 📁 Szerkezete

```
📁 Dokumentumok/
    ├── Nyilatkozatok/
    ├── Dokumentacio/
    ├── Adatbazis/
    └── Tesztek/
📁 Solutions/
    ├── API/
    ├── Application/
    ├── Domain/
    ├── Infrastructure/
    ├── WEB/
    └── ErtekmentoProjekt.slnx
📁 Published/
📄 README.md
```

---

*Budapest, 2026*
