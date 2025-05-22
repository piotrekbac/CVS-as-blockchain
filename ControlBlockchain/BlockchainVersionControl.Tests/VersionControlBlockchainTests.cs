using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlockchainVersionControl.Core;
using BlockchainVersionControl.Models;
using System;
using System.IO;

//Piotr Bacior 15 722 WSEI Kraków - Zadanie dodatkowe - Blockchain

// Testy jednostkowe klasy VersionControlBlockchain - każdy test sprawdza inny aspekt działania blockchaina wersji dokumentu
namespace BlockchainVersionControl.Tests
{
    [TestClass]
    public class VersionControlBlockchainTests
    {
        //Definiujemy pole klasy VersionControlBlockchain, które będzie przechowywać instancję blockchaina
        private VersionControlBlockchain blockchain;

        [TestInitialize]
        //Definiujemy metodę Inicjalizuj, która jest wywoływana przed każdym testem
        public void Inicjalizuj() 
        {
            blockchain = new VersionControlBlockchain("Linia 1\nLinia 2");
        }

        //Definiujemy test, który odpowiada za sprawdzenie, czy blockchain został poprawnie zainicjalizowany i czy zawiera dokładnie jeden blok genesis
        [TestMethod]
        public void CzyTworzyBlokGenesis() 
        {
            Assert.AreEqual(1, blockchain.Chain.Count);
            Assert.AreEqual(0, blockchain.Chain[0].Index);
        }

        //Definiujemy test, który odpowiada za sprawdzenie, czy dodanie nowej wersji dokumentu do blockchaina działa poprawnie
        [TestMethod]
        public void CzyDodajeNowaWersje() 
        {
            blockchain.AddNewVersion("Linia 1\nLinia Zmieniona");
            Assert.AreEqual(2, blockchain.Chain.Count);
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy GetLatestVersionBlock zwraca ostatni blok w łańcuchu
        [TestMethod]
        public void CzyZwracaNajnowszyBlok() 
        {
            blockchain.AddNewVersion("Nowy tekst");
            var najnowszy = blockchain.GetLatestVersionBlock();
            Assert.AreEqual(1, najnowszy.Index);
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy GetDocumentVersion zwraca poprawną treść dokumentu w danej wersji 
        [TestMethod]
        public void CzyRekonstruujeWersjeDokumentu() 
        {
            blockchain.AddNewVersion("Linia 1\nLinia Zmieniona");
            var wynik = blockchain.GetDocumentVersion(1);
            StringAssert.Contains(wynik, "Linia Zmieniona");
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy GetDiffBetweenVersions zwraca poprawne różnice między wersjami
        [TestMethod]
        public void CzyWyznaczaPoprawnyDiff() 
        {
            blockchain.AddNewVersion("Linia 1\nNowa linia");
            var diff = blockchain.GetDiffBetweenVersions(0, 1);
            StringAssert.Contains(diff, "+ Nowa linia");
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy IsChainValid zwraca true dla niezmodyfikowanego łańcucha
        [TestMethod]
        public void CzySprawdzaPoprawnoscLancuchaGdyPoprawny() 
        {
            blockchain.AddNewVersion("Linia 1\nNowa linia");
            Assert.IsTrue(blockchain.IsChainValid());
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy IsChainValid zwraca false dla zmodyfikowanego łańcucha
        [TestMethod]
        public void CzyWykrywaManipulacjeZawartosci() 
        {
            blockchain.AddNewVersion("A");
            blockchain.Chain[1].Diff = "Zmanipulowana zawartosc";
            Assert.IsFalse(blockchain.IsChainValid());
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy SaveToFile i LoadFromFile działają poprawnie
        [TestMethod]
        public void CzyZapisujeIOdczytujePoprawnieLancuch() 
        {
            var sciezka = "test_chain.json";
            blockchain.AddNewVersion("Linia nowa");
            blockchain.SaveToFile(sciezka);
            var wczytany = VersionControlBlockchain.LoadFromFile(sciezka);
            Assert.AreEqual(blockchain.Chain.Count, wczytany.Chain.Count);
            File.Delete(sciezka);
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy LoadFromFile obsługuje pusty plik bez wyjątku
        [TestMethod]
        public void CzyWczytujePustyLubBlednyPlik() 
        {
            var sciezka = "empty.json";
            File.WriteAllText(sciezka, "");
            var wczytany = VersionControlBlockchain.LoadFromFile(sciezka);
            Assert.IsNotNull(wczytany);
            File.Delete(sciezka);
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy IsChainValid wykrywa niepoprawny hash poprzedniego bloku
        [TestMethod]
        public void CzyWykrywaNiepoprawnyHashPoprzedniegoBloku() 
        {
            blockchain.AddNewVersion("Nowa linia");
            var zmanipulowanyBlok = blockchain.Chain[1];
            zmanipulowanyBlok.PreviousHash = "NiepoprawnyHash";
            var czyPoprawny = blockchain.IsChainValid();
            Assert.IsFalse(czyPoprawny);
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy CalculateHash oblicza poprawny hash
        [TestMethod]
        public void IsChainValid_ReturnsFalse_WhenPreviousHashDoesNotMatch()
        {
            // Arrange
            var blockchain = new VersionControlBlockchain("Initial content");
            blockchain.AddNewVersion("First update");
            blockchain.AddNewVersion("Second update");

            // Manipulacja: zmieniamy PreviousHash drugiego bloku
            blockchain.Chain[2].PreviousHash = "tampered_hash";

            // Act
            bool isValid = blockchain.IsChainValid();

            // Assert
            Assert.IsFalse(isValid, "Łańcuch powinien być nieważny, gdy PreviousHash nie odpowiada Hash poprzedniego bloku.");
        }

        //Definiujemy test, który odpowiada za sprawdzenie czy CalculateHash oblicza poprawny hash
        [TestMethod]
        public void CalculateHash_ReturnsDifferentHash_WhenContentChanges()
        {
            // Arrange
            var timestamp = DateTime.Now;
            var block1 = new DocumentVersion(1, timestamp, "Diff1", "prevHash");
            var block2 = new DocumentVersion(1, timestamp, "Diff2", "prevHash");

            // Act
            var hash1 = block1.Hash;
            var hash2 = block2.Hash;

            // Assert
            Assert.AreNotEqual(hash1, hash2, "Zmiana zawartości bloku powinna prowadzić do zmiany jego hasha.");
        }

        
    }
}