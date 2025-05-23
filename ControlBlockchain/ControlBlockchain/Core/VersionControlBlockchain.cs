using BlockchainVersionControl.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

//Piotr Bacior 15 722 WSEI Kraków - Zadanie dodatkowe - Blockchain

namespace BlockchainVersionControl.Core
{
    //Definiumy klasę VersionControlBlockchain, która będzie reprezentować blockchain wersji dokument
    //Pozwala ona na dodawanie nowych wersji dokumentu, obliczanie różnic między wersjami, walidację łańcucha bloków oraz zapisywanie i ładowanie z pliku
    public class VersionControlBlockchain
    {
        //Definiujemy listę przechowującom wszystkie bloki w naszym blockchainie
        public List<DocumentVersion> Chain { get; set; }

        //Tworzymy konstruktor klasy VersionControlBlockchain, który przyjmuje treść dokumentu jako argument 
        public VersionControlBlockchain(string initialContent)
        {
            //Inicjalizujemy łańcuch bloków jako nową listę DocumentVersion
            Chain = new List<DocumentVersion>();

            //Tworzymy blok genesis, czyli pierwszy blok w łańcuchu, który nie ma poprzedniego bloku
            CreateGenesisBlock(initialContent);
        }

        //Tworzymy pierwszy blok - genesis block, który jest pierwszym blokiem w łańcuchu, zawiera treść dokumentu i nie ma poprzedniego bloku
        private void CreateGenesisBlock(string content)
        {
            //Ustawiamy indeks bloku na 0, datę i czas na teraz, różnice jako treść dokumentu, a hash poprzedniego bloku jako "0"
            var genesisBlock = new DocumentVersion(0, DateTime.Now, content, "0");

            //Dodajemy blok genesis do łańcucha bloków
            Chain.Add(genesisBlock);
        }

        //Zwracamy ostatni blok w łańcuchu, czyli najnowszą wersję dokumentu
        public DocumentVersion GetLatestVersionBlock() => Chain.Last();

        //Zwracamy treść dokumentu w danej wersji, czyli wszystkie różnice między wersjami do tej wersji
        private string ComputeDiff(string oldText, string newText)
        {
            //Obliczamy różnice między wersjami dokumentu, czyli zmiany wprowadzone w tej wersji względem poprzedniej
            //Używamy algorytmu diff, który porównuje dwie wersje dokumentu i zwraca różnice między nimi
            var oldLines = oldText.Split('\n');
            var newLines = newText.Split('\n');

            //Tworzymy StringBuilder, który będzie używany do przechowywania różnic między wersjami dokumentu
            StringBuilder diff = new();

            //Obliczamy maksymalną długość obu wersji dokumentu, czyli maksymalną liczbę linii w obu wersjach
            int maxLen = Math.Max(oldLines.Length, newLines.Length);

            //Porównujemy dwie wersje dokumentu linia po linii i dodajemy różnice do StringBuildera
            for (int i = 0; i < maxLen; i++)
            {
                if (i < oldLines.Length && (i >= newLines.Length || oldLines[i] != newLines[i]))
                    diff.AppendLine($"- {oldLines[i]}");
                if (i < newLines.Length && (i >= oldLines.Length || oldLines[i] != newLines[i]))
                    diff.AppendLine($"+ {newLines[i]}");
                if (i < oldLines.Length && i < newLines.Length && oldLines[i] == newLines[i])
                    diff.AppendLine($"  {newLines[i]}");
            }

            //Dodajemy różnice do StringBuildera, które są tylko w starej wersji dokumentu
            return diff.ToString();
        }

        //Dodajemy nową wersję dokumentu do łańcucha bloków, czyli tworzymy nowy blok z różnicami między wersjami
        public void AddNewVersion(string newContent)
        {
            //Odtwarzamy treść poprzedniej wersji dokumentu, czyli wszystkie różnice między wersjami do tej wersji
            string previousContent = GetDocumentVersion(Chain.Count - 1);

            //Obliczamy różnice między wersjami dokumentu, czyli zmiany wprowadzone w tej wersji względem poprzedniej
            string diff = ComputeDiff(previousContent, newContent);

            //Pobieramy teraz ostatni blok (ostatnią wersję) 
            var previousBlock = GetLatestVersionBlock();

            //Twprzymy nowy blok, który będzie zawierał różnice między wersjami dokumentu, czyli zmiany wprowadzone w tej wersji względem poprzedniej
            var newBlock = new DocumentVersion(previousBlock.Index + 1, DateTime.Now, diff, previousBlock.Hash);

            //Dodajemy nowy blok do łańcucha bloków (blockchain)
            Chain.Add(newBlock);
        }

        //Otwieramy wersję dokumentu w danej wersji, czyli wszystkie różnice między wersjami do tej wersji
        public string GetDocumentVersion(int versionIndex)
        {
            //Sprawdzamy, czy podany indeks wersji jest poprawny, czyli czy nie jest mniejszy od 0 i nie jest większy od liczby bloków w łańcuchu
            if (versionIndex < 0 || versionIndex >= Chain.Count)

                //Jeżeli indeks jest niepoprawny, to rzucamy wyjątek ArgumentOutOfRangeException, czyli indeks jest poza zakresem
                throw new ArgumentOutOfRangeException(nameof(versionIndex), "Podany indeks wersji nie istnieje.");

            //Deklarujemy zmienną document jako StringBuilder, która będzie używana do przechowywania treści dokumentu w danej wersji
            StringBuilder document = new();

            //Iterujemy przez wszystkie bloki w łańcuchu do danej wersji (versionIndex) i dodajemy różnice między wersjami dokumentu do StringBuildera
            foreach (var block in Chain.Take(versionIndex + 1))
            {
                //Każdy blok zawiera różnice między wersjami dokumentu, czyli zmiany wprowadzone w tej wersji względem poprzedniej
                foreach (var line in block.Diff.Split('\n'))
                {
                    //Teraz dodajemy tylko te linie, które zaczynają się od "+ " lub "  ", czyli różnice między wersjami dokumentu
                    if (line.StartsWith("+ ") || line.StartsWith("  "))

                        //Dodajemy różnice do StringBuildera, czyli zmiany wprowadzone w tej wersji względem poprzedniej
                        document.AppendLine(line[2..]);
                }
            }

            //Zwracamy treść dokumentu w danej wersji, czyli wszystkie różnice między wersjami do tej wersji
            return document.ToString();
        }

        //Zwracamy teraz różnicę między dwiema wersjami dokumentu, czyli zmiany wprowadzone w tej wersji względem poprzedniej
        public string GetDiffBetweenVersions(int fromVersion, int toVersion)
        {
            //W zmiennej fromContent przechowujemy treść dokumentu w danej wersji, czyli treść wersji początkowej 
            string fromContent = GetDocumentVersion(fromVersion);

            //W zmiennej toContent przechowujemy treść dokumentu w danej wersji, czyli treść wersji końcowej
            string toContent = GetDocumentVersion(toVersion);

            ////Obliczamy różnice między wersjami dokumentu, czyli zmiany wprowadzone w tej wersji względem poprzedniej
            return ComputeDiff(fromContent, toContent);
        }

        //Teraz sprawdzamy, czy integralność łańcucha bloków jest zachowana, czyli czy każdy blok w łańcuchu jest poprawny i nie został zmieniony
        public bool IsChainValid()
        {
            //Sprawdzamy, czy łańcuch bloków jest pusty, czyli nie zawiera żadnych bloków
            for (int i = 1; i < Chain.Count; i++)
            {
                var current = Chain[i];
                var previous = Chain[i - 1];

                //Sprawdzamy teraz czy hash bloku jest poprawny, czyli czy hash bloku jest zgodny z hashem obliczonym na podstawie jego właściwości na nowo
                if (current.Hash != current.CalculateHash()) return false;

                //Sprawdzamy teraz czy hash poprzedniego bloku jest zgodny z zapisanym hashem poprzedniego bloku PreviousHash
                if (current.PreviousHash != previous.Hash) return false;
            }

            //Jeżeli wszystkie bloki są poprawne i nie zostały zmienione, to zwracamy true, czyli łańcuch bloków jest poprawny
            return true;
        }

        //Definiujemy metodę SaveToFile(), która zapisuje łańcuch bloków do pliku w formacie JSON
        public void SaveToFile(string path)
        {
            //Serializujemy łańcuch bloków do formatu JSON, czyli przekształcamy obiekt łańcucha bloków na tekst w formacie JSON
            var json = JsonConvert.SerializeObject(Chain, Formatting.Indented);

            //Zapisujemy tekst w formacie JSON do pliku o podanej ścieżce
            File.WriteAllText(path, json);
        }

        //Definiujemy metodę LoadFromFile(), która ładuje łańcuch bloków z pliku w formacie JSON
        public static VersionControlBlockchain LoadFromFile(string path)
        {
            //Wczytujemy tekst z pliku o podanej ścieżce
            var json = File.ReadAllText(path);

            //Odtwarzamy łańcuch bloków z tekstu w formacie JSON, czyli przekształcamy tekst w formacie JSON na obiekt łańcucha bloków
            var chain = JsonConvert.DeserializeObject<List<DocumentVersion>>(json);

            //Tworzymy pusty łańcuch bloków, który będzie zawierał odtworzony łańcuch bloków z pliku
            var blockchain = new VersionControlBlockchain("");

            //Ustawiamy łańcuch bloków na odtworzony łańcuch bloków z pliku
            blockchain.Chain = chain;

            //Zwracamy odtworzony łańcuch bloków, czyli obiekt klasy VersionControlBlockchain
            return blockchain;
        }
    }
}