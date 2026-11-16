\# Sajam knjiga



Informacioni sistem uprave sajma knjiga – projektni zadatak iz predmeta Osnove informacionih sistema i softverskog inženjerstva (2025/2026).



\## Tim



\- Marina Sekulic – Student 1

\- Maja Marinković – Student 2



\## Pokretanje



1\. Otvoriti `BookFair.slnx` u Visual Studio 2026 (.NET 10).

2\. Postaviti `ConsoleClient` kao startup projekat i pokrenuti aplikaciju.

3\. Podaci se čuvaju u folderu `Data` u korijenu rješenja. Ako folder ili fajlovi ne postoje, aplikacija ih sama kreira.



\## Struktura rješenja



```

BookFair/

├── Core/                  zajednički dio (modeli, serijalizacija, pristup podacima)

│   ├── Models/            klase entiteta

│   ├── Serialization/     ISerializable, Serializer

│   ├── Storage/           čitanje i upis tekstualnih fajlova

│   ├── DAO/               dodavanje, izmjena, brisanje i pretraga entiteta

│   └── Utils/             pomoćne metode za parsiranje

├── ConsoleClient/         konzolna aplikacija

│   ├── Views/             meniji za entitete, unos i ispis

│   └── Program.cs         učitavanje podataka i pokretanje glavnog menija

└── Data/                  fajlovi sa podacima (.txt, polja odvojena znakom |)

```



\## Podjela posla



\### Prva kontrolna tačka (model podataka, serijalizacija, funkcionalnosti 8.1–8.4)



\*\*Marina Sekulic\*\* (#posetilac, #kupovina, #izdavac)



\- početna arhitektura rješenja (projekti Core i ConsoleClient)

\- serijalizacija: `ISerializable`, `Serializer`, `Storage` (po uzoru na vježbe 3) i `CsvParser`

\- modeli: `Posetilac`, `Kupovina`, `Izdavac`, `MembershipStatus`

\- DAO klase: `PosetilacDAO`, `KupovinaDAO`, `IzdavacDAO`, `ListaZeljaDAO`

\- konzola: `ConsoleInput` (provjera unosa), `ConsoleOutput` (ispis), `PosetilacView` (sa listom želja), `KupovinaView`, `IzdavacView`, `MainView`, `Program.cs`



\*\*Maja Marinković\*\* (#autor, #knjiga, #adresa)



\- modeli: `Autor`, `Knjiga`, `Adresa`

\- klase za veze: `KnjigaAutor` (knjiga – autor) i `ListaZelja` (posjetilac – knjiga)

\- DAO klase: `AutorDAO`, `KnjigaDAO`, `AdresaDAO`, `KnjigaAutorDAO`

\- konzola: `AutorView`, `KnjigaView`, `AdresaView` i dodavanje menija Autori, Knjige i Adrese u glavni meni

