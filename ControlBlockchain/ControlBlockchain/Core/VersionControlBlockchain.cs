using BlockchainVersionControl.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

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
            var formattedContent = string.Join('\n', content
                .Split('\n')
                .Select(line => "+ " + line));
            var genesisBlock = new DocumentVersion(0, DateTime.Now, formattedContent, "0");
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

            //Porównujemy dwie wersje dokumentu linia po linii i dodajemy różnice do StringBuildera
            for (int i = 0; i < newLines.Length; i++)
            {
                if (i >= oldLines.Length || oldLines[i] != newLines[i])
                    diff.AppendLine($"+ {newLines[i]}");
                else
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
            for (int i = 1; i < Chain.Count; i++)
            {
                var current = Chain[i];
                var previous = Chain[i - 1];

                //Sprawdzamy czy hash bieżącego bloku jest poprawny
                if (current.Hash != current.CalculateHash())
                {
                    return false;
                }

                //Sprawdzamy czy PreviousHash bieżącego bloku odpowiada hashowi poprzedniego bloku
                if (current.PreviousHash != previous.Hash)
                {
                    return false;
                }
            }
            return true;
        }


        //Definiujemy metodę SaveToFile(), która zapisuje łańcuch bloków do pliku w formacie JSON
        public void SaveToFile(string path)
        {
            //Serializujemy łańcuch bloków do formatu JSON, czyli przekształcamy obiekt łańcucha bloków na tekst w formacie JSON
            var json = JsonConvert.SerializeObject(Chain, Newtonsoft.Json.Formatting.Indented);

            //Zapisujemy tekst w formacie JSON do pliku o podanej ścieżce
            File.WriteAllText(path, json);
        }

        //Definiujemy metodę LoadFromFile(), która ładuje łańcuch bloków z pliku w formacie JSON
        public static VersionControlBlockchain LoadFromFile(string path)
        {
            var json = File.ReadAllText(path);
            var chain = JsonConvert.DeserializeObject<List<DocumentVersion>>(json) ?? new List<DocumentVersion>();
            var blockchain = new VersionControlBlockchain("");
            blockchain.Chain = chain;
            return blockchain;
        }
    }
}
