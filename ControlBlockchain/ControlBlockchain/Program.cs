using BlockchainVersionControl.Core;
using System;
using System.IO;

// Piotr Bacior 15 722 WSEI Kraków - Zadanie dodatkowe - Blockchain

namespace BlockchainVersionControl
{
    class Program
    {
        static void Main(string[] args)
        {
            //Definiujemy stała filePath, która będzie przechowywać ścieżkę do pliku blockchain.json, w którym będziemy przechowywać nasz łańcuch bloków
            const string filePath = "blockchain.json";

            //Sprawdzamy teraz czy program został uruchomiony z argumentami, czyli czy użytkownik podał jakieś polecenia
            if (args.Length > 0)
            {
                //Teraz pobieramy polecenie i zmieniamy je na małe litery, aby nie było problemu z wielkością liter
                var command = args[0].ToLower();

                //Jeżeli plik blockchain.json nie istnieje, a użytkownik nie podał polecenia init, to wyświetlamy komunikat o błędzie i kończymy działanie programu
                if (!File.Exists(filePath) && command != "init")
                {
                    Console.WriteLine("Nie znaleziono pliku blockchain.json. Użyj polecenia 'init', aby rozpocząć.");
                    return;
                }

                //Jeżeli plik blockchain.json istnieje, to ładujemy go do zmiennej blockchain
                VersionControlBlockchain blockchain = null;

                //Jeżeli polecenie podane przez użytkownika to init, to tworzymy nowy łańcuch bloków, a jeżeli nie, to ładujemy istniejący łańcuch bloków z pliku blockchain.json
                if (command != "init")
                    //Ładujemy łańcuch bloków z pliku blockchain.json
                    blockchain = VersionControlBlockchain.LoadFromFile(filePath);

                //Teraz sprawdzamy jakie polecenie zostało podane przez użytkownika i wykonujemy odpowiednią akcję
                switch (command)
                {
                    //Jeżeli polecenie to init, to tworzymy nowy łańcuch bloków i zapisujemy go do pliku blockchain.json
                    case "init":

                        //Wypisujemy komunikat o tym, że łańcuch bloków został zainicjowany i prosimy użytkownika o podanie początkowej treści dokumentu
                        Console.WriteLine("Wprowadź początkową treść dokumentu:");

                        //Pobieramy początkową treść dokumentu od użytkownika
                        string initialContent = Console.ReadLine();

                        //Tworzymy nowy łańcuch bloków z początkową treścią dokumentu
                        blockchain = new VersionControlBlockchain(initialContent);

                        //Zapisujemy łańcuch bloków do pliku blockchain.json
                        blockchain.SaveToFile(filePath);

                        //Wypisujemy komunikat o tym, że łańcuch bloków został zainicjowany
                        Console.WriteLine("Zainicjowano łańcuch bloków.");
                        break;

                    //Jeżeli polecenie to show, to wyświetlamy historię wersji dokumentu
                    case "show":

                        //Iterujemy po wszystkich blokach w łańcuchu i wypisujemy ich indeks, datę i czas oraz hash
                        foreach (var block in blockchain.Chain)
                        {
                            //Wypisujemy indeks bloku, datę i czas oraz hash bloku
                            Console.WriteLine($"Wersja {block.Index} | Czas: {block.Timestamp} | Hash: {block.Hash.Substring(0, 10)}...");
                        }
                        break;

                    //Jeżeli polecenie to diff, to porównujemy dwie wersje dokumentu
                    case "diff":

                        //Sprawdzamy czy użytkownik podał dwa argumenty, czyli dwie wersje do porównania
                        if (args.Length < 3)
                        {
                            //Wypisujemy komunikat o użyciu polecenia dif
                            Console.WriteLine("Użycie: diff <z wersji> <do wersji>");
                            return;
                        }

                        //Definiujemy zmienne from i to, które będą przechowywać numery wersji do porównania
                        int from = int.Parse(args[1]);
                        int to = int.Parse(args[2]);

                        //Sprawdzamy czy numery wersji są poprawne, czyli czy są większe od 0 i mniejsze od liczby bloków w łańcuchu
                        Console.WriteLine(blockchain.GetDiffBetweenVersions(from, to));
                        break;

                    //Jeżeli polecenie to validate, to sprawdzamy integralność łańcucha bloków
                    case "validate":

                        //Sprawdzamy czy łańcuch bloków jest ważny, czyli czy każdy blok w łańcuchu jest poprawny i nie został zmieniony
                        Console.WriteLine("Łańcuch ważny: " + blockchain.IsChainValid());
                        break;

                    //Jeżeli polecenie jest nieznane, to wypisujemy komunikat o błędzie
                    default:
                        Console.WriteLine("Nieznane polecenie. Dostępne: init, show, diff, validate");
                        break;
                }

                //Kończymy działanie programu
                return;
            }

            //Tryb interaktywny (menu konsolowe)
            VersionControlBlockchain vcb;

            //Jeżeli plik blockchain.json istnieje, to ładujemy go do zmiennej vcb, a jeżeli nie, to tworzymy nowy łańcuch bloków
            if (File.Exists(filePath))
            {
                vcb = VersionControlBlockchain.LoadFromFile(filePath);
            }
            else
            {
                //Wypisujemy komunikat o tym, że plik blockchain.json nie istnieje i prosimy użytkownika o podanie początkowej treści dokumentu
                Console.WriteLine("Wprowadź początkową treść dokumentu:");
                string init = Console.ReadLine();

                //Tworzymy nowy łańcuch bloków z początkową treścią dokumentu
                vcb = new VersionControlBlockchain(init);
            }

            //Teraz tworzymy pętle while dla menu, działa aż do wybrania opcji "Zapisz i zakończ"
            while (true)
            {
                Console.WriteLine("Piotr Bacior 15 722 - WSEI Kraków");
                Console.WriteLine("\nWybierz dogodną dla siebie opcję:");
                Console.WriteLine("1. Dodaj nową wersję");
                Console.WriteLine("2. Pokaż historię");
                Console.WriteLine("3. Wyświetl wersję dokumentu");
                Console.WriteLine("4. Porównaj dwie wersje");
                Console.WriteLine("5. Sprawdź integralność łańcucha");
                Console.WriteLine("6. Zapisz i zakończ");
                Console.Write("Opcja: ");

                //Pobieramy opcję od użytkownika
                string option = Console.ReadLine();

                //Sprawdzamy jaką opcję wybrał użytkownik i wykonujemy odpowiednią akcję
                switch (option)
                {
                    case "1":
                        //Dodajemy nową wersję dokumentu, czyli tworzymy nowy blok w łańcuchu bloków, pytamy użytkownika o treść nowej wersji i dodajemy blok do łańcucha
                        Console.WriteLine("Wprowadź nową treść dokumentu:");
                        string newContent = Console.ReadLine();
                        vcb.AddNewVersion(newContent);
                        break;

                    case "2":
                        //Wyświetlamy historię wersji dokumentu, czyli iterujemy po wszystkich blokach w łańcuchu i wypisujemy ich indeks, datę i czas oraz hash
                        foreach (var block in vcb.Chain)
                        {
                            Console.WriteLine($"[{block.Index}] {block.Timestamp} | Hash: {block.Hash.Substring(0, 10)}...");
                        }
                        break;

                    case "3":
                        //Wyświetlamy wersję dokumentu, czyli pytamy użytkownika o numer wersji i wypisujemy treść dokumentu w tej wersji
                        Console.WriteLine("Podaj numer wersji:");
                        if (int.TryParse(Console.ReadLine(), out int ver))
                            Console.WriteLine(vcb.GetDocumentVersion(ver));
                        else
                            Console.WriteLine("Nieprawidłowy numer.");
                        break;

                    case "4":
                        //Porównujemy dwie wersje dokumentu, czyli pytamy użytkownika o dwa numery wersji i wypisujemy różnice między nimi
                        Console.WriteLine("Porównanie wersji. Wprowadź dwa indeksy:");
                        if (int.TryParse(Console.ReadLine(), out int fromVer) &&
                            int.TryParse(Console.ReadLine(), out int toVer))
                            Console.WriteLine(vcb.GetDiffBetweenVersions(fromVer, toVer));
                        else
                            Console.WriteLine("Nieprawidłowe numery wersji.");
                        break;

                    case "5":
                        //Sprawdzamy integralność łańcucha bloków, czyli sprawdzamy czy każdy blok w łańcuchu jest poprawny i nie został zmieniony
                        Console.WriteLine("Łańcuch ważny: " + vcb.IsChainValid());
                        break;

                    case "6":
                        //Zapisujemy łańcuch bloków do pliku blockchain.json i kończymy działanie programu
                        vcb.SaveToFile(filePath);
                        Console.WriteLine("Zapisano. Kończę.");
                        return;

                    default:
                        ////Jeżeli opcja jest nieznana, to wypisujemy komunikat o błędzie
                        Console.WriteLine("Nieznana opcja.");
                        break;
                }
            }
        }
    }
}