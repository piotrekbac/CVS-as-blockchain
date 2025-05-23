using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlockchainVersionControl.Core;
using BlockchainVersionControl.Models;
using System;
using System.IO;

//Piotr Bacior 15 722 WSEI Kraków - Zadanie dodatkowe - Blockchain

//Testy jednostkowe klasy VersionControlBlockchain - każdy test sprawdza inny aspekt działania blockchaina wersji dokumentu
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

        //Definiujemy test inicjalizacji: sprawdzamy, czy utworzony został blok początkowy (genesis)
        [TestMethod]
        public void CzyTworzyBlokPoczatkowy()
        {
            //Assert
            Assert.AreEqual(1, blockchain.Chain.Count);
            Assert.AreEqual(0, blockchain.Chain[0].Index);
        }

        //Definiujemy teraz test dodania nowej wersji dokumentu
        [TestMethod]
        public void CzyDodajeWersje()
        {
            //Act
            blockchain.AddNewVersion("Linia 1\nLinia Zmieniona");

            //Assert
            Assert.AreEqual(2, blockchain.Chain.Count);
        }

        //Definiujemy test zwracania ostatniego bloku w łańcuchu
        [TestMethod]
        public void CzyZwracaOstatniBlok()
        {
            //Arrange
            blockchain.AddNewVersion("Nowy tekst");

            //Act
            var najnowszy = blockchain.GetLatestVersionBlock();

            //Assert
            Assert.AreEqual(1, najnowszy.Index);
        }

        //Przechodzimu to zdefiniowania testu odtworzenia pełnej wersji dokumentu na podstawie wskazanej wersji
        [TestMethod]
        public void CzyRekonstruujeWersje()
        {
            //Arrange
            blockchain.AddNewVersion("Linia 1\nLinia Zmieniona");

            //Act
            var wynik = blockchain.GetDocumentVersion(1);

            //Assert
            StringAssert.Contains(wynik, "Linia Zmieniona");
        }

        //Definiujemy test generowania różnic pomiędzy dwiema wersjami
        [TestMethod]
        public void CzyGenerujeDiffMiedzyWersjami()
        {
            //Arrange
            blockchain.AddNewVersion("Linia 1\nNowa linia");

            //Act
            var diff = blockchain.GetDiffBetweenVersions(0, 1);

            //Assert
            StringAssert.Contains(diff, "+ Nowa linia");
        }

        //Definujemy test poprawności łańcucha w przypadku braku manipulacji
        [TestMethod]
        public void CzyLancuchJestPoprawny()
        {
            //Arrange
            blockchain.AddNewVersion("Linia 1\nNowa linia");

            //Assert
            Assert.IsTrue(blockchain.IsChainValid());
        }

        //Teraz definiujemy test wykrywania manipulacji zawartości bloku
        [TestMethod]
        public void CzyWykrywaZmianeZawartosci()
        {
            //Arrange
            blockchain.AddNewVersion("A");

            //Act
            blockchain.Chain[1].Diff = "Zmanipulowana zawartosc";

            //Assert
            Assert.IsFalse(blockchain.IsChainValid());
        }

        //Definiujemy test poprawności działania zapisu i odczytu z pliku
        [TestMethod]
        public void CzyZapisujeIOdczytujePoprawnie()
        {
            var sciezka = "test_chain.json";
            try
            {
                //Arrange
                blockchain.AddNewVersion("Linia nowa");
                blockchain.SaveToFile(sciezka);

                //Act
                var wczytany = VersionControlBlockchain.LoadFromFile(sciezka);

                //Assert
                Assert.AreEqual(blockchain.Chain.Count, wczytany.Chain.Count);
                Assert.AreEqual(blockchain.Chain[1].Hash, wczytany.Chain[1].Hash);
            }
            finally
            {
                if (File.Exists(sciezka))
                    File.Delete(sciezka);
            }
        }

        //Teraz definiujemy test odczytu pustego pliku JSON
        [TestMethod]
        public void CzyWczytujePustyPlik()
        {
            //Arrange
            var sciezka = "empty.json";
            File.WriteAllText(sciezka, "");

            //Act
            var wczytany = VersionControlBlockchain.LoadFromFile(sciezka);

            //Assert
            Assert.IsNotNull(wczytany);
            File.Delete(sciezka);
        }

        //Przechodzimy do zdefiniowania testu wykrywania błędnego poprzedniego hasha w łańcuchu bloków
        [TestMethod]
        public void CzyWykrywaBlednyPoprzedniHash()
        {
            //Arrange
            blockchain.AddNewVersion("Nowa linia");

            //Act
            var zmanipulowanyBlok = blockchain.Chain[1];
            zmanipulowanyBlok.PreviousHash = "NiepoprawnyHash";
            var czyPoprawny = blockchain.IsChainValid();

            //Assert
            Assert.IsFalse(czyPoprawny);
        }

        //Definiujemy test porównania hashy przy zmianie zawartości bloku
        [TestMethod]
        public void CzyZmienionyDiffZmieniaHash()
        {
            //Arrange
            var timestamp = DateTime.Now;
            var block1 = new DocumentVersion(1, timestamp, "Diff1", "prevHash");
            var block2 = new DocumentVersion(1, timestamp, "Diff2", "prevHash");

            //Assert
            Assert.AreNotEqual(block1.Hash, block2.Hash);
        }

        //Definiujemy test porównania hashy przy zmianie czasu utworzenia bloku
        [TestMethod]
        public void CzyZmienionyCzasZmieniaHash()
        {
            //Arrange
            var block1 = new DocumentVersion(1, DateTime.UtcNow, "Zawartosc", "poprzedniHash");
            System.Threading.Thread.Sleep(10);

            //Act
            var block2 = new DocumentVersion(1, DateTime.UtcNow, "Zawartosc", "poprzedniHash");

            //Assert
            Assert.AreNotEqual(block1.Hash, block2.Hash);
        }

        //Teraz definiujemy test zwracania wyjątku przy żądaniu wersji o nieistniejącym indeksie
        [TestMethod]
        public void CzyZglaszaWyjatekDlaZlegoIndeksu()
        {
            //Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => blockchain.GetDocumentVersion(999));
        }

        //Definiujemy test poprawności łańcucha przy braku danych w Diff
        [TestMethod]
        public void CzyWykrywaNullDiff()
        {
            //Arrange
            blockchain.AddNewVersion("Tekst");
            blockchain.Chain[1].Diff = null;

            //Assert
            Assert.IsFalse(blockchain.IsChainValid());
        }

        //Definiujemy test poprawności łańcucha przy braku danych w PreviousHash
        [TestMethod]
        public void CzyZwracaOstatniBlokNaPoczatku()
        {
            //Arrange i Act
            var ostatni = blockchain.GetLatestVersionBlock();

            //Assert
            Assert.AreEqual(0, ostatni.Index);
        }

    }
}
