# Ćwiczenie - implementacja systemu kontroli wersji z wykorzystaniem blockchain

- Krzysztof Molenda, 2025-03-26

## Wprowadzenie

Blockchain to rozproszona i zdecentralizowana struktura danych, która przechowuje rekordy zwane **blokami** w sposób chronologiczny i połączony. Każdy blok zawiera grupę **transakcji** oraz odniesienie do poprzedniego bloku, tworząc w ten sposób łańcuch bloków. Kluczową cechą blockchain'a jest jego **niezmienność** – po dodaniu bloku do łańcucha, bardzo trudno jest go zmienić lub usunąć bez naruszenia integralności całego łańcucha.

W tradycyjnych systemach kontroli wersji, historia zmian dokumentu jest zazwyczaj przechowywana w centralnej bazie danych. Wykorzystanie idei blockchain do kontroli wersji dokumentu może zapewnić większą transparentność, integralność i bezpieczeństwo historii zmian. W tym ćwiczeniu skupimy się na przechowywaniu różnic (ang. *diffs*) między kolejnymi wersjami dokumentu w blokach blockchain'a, co pozwala na oszczędność miejsca i efektywne śledzenie zmian.

**Referencje: koncepcja blockchain**

- **How the Bitcoin protocol actually works (by Michael Nielsen):** Dobrze wyjaśnione, dogłębne spojrzenie na podstawy technologii blockchain (na przykładzie Bitcoina). ([http://michaelnielsen.org/ddi/how-the-bitcoin-protocol-actually-works/](http://michaelnielsen.org/ddi/how-the-bitcoin-protocol-actually-works/))
- **Blockchain Explained (Investopedia):** Przyjazne dla początkujących wyjaśnienie podstawowych koncepcji blockchain (nie tylko dla bitcoin). ([https://www.investopedia.com/terms/b/blockchain.asp](https://www.investopedia.com/terms/b/blockchain.asp))
- **What is Blockchain Technology? A Step-by-Step Guide (Blockgeeks):** Kolejny przewodnik wprowadzający do technologii blockchain. ([https://www.linkedin.com/pulse/what-blockchain-technology-step-by-step-guide-beginners-flores/](https://www.linkedin.com/pulse/what-blockchain-technology-step-by-step-guide-beginners-flores/))


## Zadanie

Zaimplementuj w języku C# prosty system kontroli wersji dokumentu tekstowego, który wykorzystuje ideę blockchain do przechowywania historii zmian. System powinien przechowywać jedynie różnice (*diffs*) między kolejnymi wersjami dokumentu w blokach łańcucha.

**Szczegółowe wymagania:**

1. **Klasa `DocumentVersion`:**
    
    - Zdefiniuj klasę `DocumentVersion` reprezentującą blok w łańcuchu. Klasa powinna zawierać następujące właściwości:
        - `Index` (int): Indeks bloku (numer wersji, zaczynając od 0 dla pierwszej wersji).
        - `Timestamp` (DateTime): Czas utworzenia tej wersji.
        - `Diff` (string): Tekst przedstawiający różnicę między tą wersją a poprzednią (dla pierwszej wersji może być to pełna treść dokumentu).
        - `PreviousHash` (string): Hash bloku poprzedniej wersji.
        - `Hash` (string): Hash bieżącego bloku.
    - Zaimplementuj konstruktor, który przyjmuje `Index`, `Timestamp`, `Diff`, i `PreviousHash`. W konstruktorze wywołaj metodę `CalculateHash` w celu obliczenia i ustawienia wartości właściwości `Hash`.
    - Zaimplementuj metodę `CalculateHash`, która generuje hash SHA-256 na podstawie właściwości `Index`, `Timestamp`, `Diff`, i `PreviousHash`.

2. **Klasa `VersionControlBlockchain`:**
    
    - Zdefiniuj klasę `VersionControlBlockchain` zarządzającą łańcuchem wersji dokumentu. Klasa powinna zawierać:
        - `Chain` (`List<DocumentVersion>`): Lista przechowująca bloki wersji dokumentu.
    - Zaimplementuj konstruktor, który inicjalizuje `Chain` i tworzy pierwszą wersję dokumentu (blok genezy) poprzez wywołanie metody `CreateGenesisBlock`.
    - Zaimplementuj metodę `CreateGenesisBlock`, która tworzy i dodaje do łańcucha pierwszy blok. Pierwszy blok powinien zawierać pełną początkową treść dokumentu jako `Diff`, `Index` ustawiony na 0, bieżący znacznik czasu, a `PreviousHash` ustawiony na "0".
    - Zaimplementuj metodę `GetLatestVersionBlock`, która zwraca ostatni blok w łańcuchu.
    - Zaimplementuj metodę `AddNewVersion`, która przyjmuje jako argument aktualną treść dokumentu (string). Metoda powinna:
        - Pobrać treść poprzedniej wersji dokumentu (z ostatniego bloku w łańcuchu).
        - Obliczyć różnicę (diff) między aktualną treścią a treścią poprzedniej wersji. Możesz zaimplementować prosty algorytm porównujący wiersze lub skorzystać z zewnętrznej biblioteki do obliczania diffów (np. biblioteki do porównywania tekstu).
        - Utworzyć nowy obiekt `DocumentVersion` z odpowiednim indeksem (następnym w kolejności), bieżącym znacznikiem czasu, obliczonym diffem oraz hashem poprzedniego bloku.
        - Obliczyć hash nowego bloku i dodać go do `Chain`.
    - Zaimplementuj metodę `GetDocumentVersion`, która przyjmuje jako argument numer wersji (indeks) i zwraca treść dokumentu w tej wersji poprzez odtworzenie zmian od bloku genezy do żądanej wersji.
    - Zaimplementuj metodę `IsChainValid`, która sprawdza integralność łańcucha wersji dokumentu. Metoda powinna iterować przez wszystkie bloki (zaczynając od drugiego) i weryfikować, czy hash każdego bloku jest poprawny oraz czy `PreviousHash` bieżącego bloku odpowiada hashowi poprzedniego bloku.

3. **Klasa `Program`:**
    
    - W metodzie `Main` utwórz instancję klasy `VersionControlBlockchain`.
    - Dodaj kilka kolejnych wersji dokumentu, symulując zmiany w jego treści.
    - Wyświetl historię wersji dokumentu, w tym indeks, znacznik czasu i hash każdego bloku.
    - Przetestuj metodę `GetDocumentVersion`, pobierając treść dokumentu dla różnych numerów wersji.
    - Wywołaj metodę `IsChainValid` i wyświetl jej wynik.
    - (Opcjonalnie) Spróbuj zmodyfikować zawartość jednego z wcześniejszych bloków (np. diff) i ponownie sprawdź ważność łańcucha, aby zademonstrować niezmienność.

4. **Rozszerzenia**

	- Dodaj możliwość wyświetlania konkretnych zmian między dwiema wersjami.
	- Zaimplementuj interfejs CLI do komunikacji z systemem (komendy, przełączniki jako argumenty w powłoce systemu operacyjnego)
	- Zaimplementuj prosty interfejs użytkownika (konsolowego) do interakcji z systemem kontroli wersji.
	- Zrealizuj zapisywanie i odczytywanie łańcucha wersji do/z pliku.

5. **Unit testy**
	- Opracuj testy jednostkowe do kluczowych funkcjonalności systemu

## Wymagane umiejętności i cele dydaktyczne

**Wymagane umiejętności:**

- Podstawowa znajomość języka C#.
- Zrozumienie programowania obiektowego.
- Znajomość list i innych struktur danych.
- Zrozumienie algorytmu haszującego SHA-256.
- Umiejętność korzystania z bibliotek C#

**Cele nauczania:**

- Zrozumienie koncepcji blockchaina i jego zastosowań poza kryptowalutami.
- Zastosowanie idei niezmienności i łańcuchowego łączenia bloków do kontroli wersji dokumentów.
- Implementacja struktury danych blockchain w języku C#.
- Wykorzystanie algorytmu haszującego SHA-256 do zapewnienia integralności danych.
- Zaimplementowanie mechanizmu przechowywania różnic (diffs) między wersjami dokumentu.
- Zrozumienie procesu rekonstrukcji konkretnej wersji dokumentu z łańcucha zmian.

## Referencje

- **SHA-256 in .NET:**
    
    - **`SHA256` Class Documentation (Microsoft Learn):** Explains how to use the built-in class for calculating SHA-256 hashes in C#. ([https://learn.microsoft.com/pl-pl/dotnet/api/system.security.cryptography.sha256?view=net-7.0](https://learn.microsoft.com/pl-pl/dotnet/api/system.security.cryptography.sha256?view=net-7.0))
    - **How to compute SHA256 hash in C# (Stack Overflow):** A common question with helpful answers and code examples. ([https://stackoverflow.com/questions/3834726/how-to-compute-sha256-hash-in-c-sharp](https://stackoverflow.com/questions/3834726/how-to-compute-sha256-hash-in-c-sharp))

- **Calculating Diffs:**
    
    - **Diff Algorithm on Wikipedia:** Provides a theoretical understanding of diff algorithms. ([https://en.wikipedia.org/wiki/Diff_algorithm](https://en.wikipedia.org/wiki/Diff_algorithm))
    - **Simple Line-by-Line Diff Implementation in C# (Stack Overflow):** A basic approach to implementing a diff function. ([https://stackoverflow.com/questions/999201/comparison-of-two-text-files-in-c-sharp](https://stackoverflow.com/questions/999201/comparison-of-two-text-files-in-c-sharp) - Look for answers discussing line-by-line comparison)
    - **Using a Diff Library in C# (e.g., DiffPlex):** They would need to search for the NuGet package and its documentation (e.g., "DiffPlex GitHub").
