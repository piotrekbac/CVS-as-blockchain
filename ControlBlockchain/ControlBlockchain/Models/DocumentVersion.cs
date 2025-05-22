using System;
using System.Security.Cryptography;
using System.Text;

//Piotr Bacior 15 722 WSEI Kraków - Zadanie dodatkowe - Blockchain 

namespace BlockchainVersionControl.Models
{
    //Definujemy klasę DocumentVersion, która będzie reprezentować wersję dokumentu w blockchainie
    public class DocumentVersion
    {
        //Index - odpowiada za indeks bloku, numer wersji zaczynając od 0 dla pierwszej wersji 
        public int Index { get; set; }

        //Timestamp - odpowiada za datę i czas utworzenia wersji dokumentu
        public DateTime Timestamp { get; set; }

        //Diff - odpowiada za różnice między wersjami dokumentu, czyli zmiany wprowadzone w tej wersji względem poprzedniej
        public string Diff { get; set; }

        //PreviousHash - odpowiada za hash poprzedniego bloku, czyli poprzedniej wersji dokumentu
        public string PreviousHash { get; set; }

        //Hash - odpowiada za hash tej wersji dokumentu, czyli unikalny identyfikator tej wersji
        public string Hash { get; set; }

        //Definiujemy konstruktor klasy DocumentVersion, który przyjmuje indeks, datę i czas, różnice, hash poprzedniego bloku i oblicza hash tej wersji dokumentu
        //Konstruktor wywołujemy przy tworzeniu nowej wersji bloku, ustawia on wszystkie wyżej wymienione właściwości oraz wylicza hash za pomocą metody CalculateHash()
        public DocumentVersion(int index, DateTime timestamp, string diff, string previousHash)
        {
            //Ustawiamy numer wersji bądź też indeks bloku
            Index = index;

            //Ustawiamy datę i czas utworzenia wersji dokumentu
            Timestamp = timestamp;

            //Ustawiamy różnice między wersjami dokumentu, czyli zmiany wprowadzone w tej wersji względem poprzedniej
            Diff = diff;

            //Ustawiamy hash poprzedniego bloku, czyli poprzedniej wersji dokumentu
            PreviousHash = previousHash;

            //Obliczamy hash tej wersji dokumentu, czyli unikalny identyfikator tej wersji
            Hash = CalculateHash();
        }

        //Definiujemy metodę CalculateHash(), która oblicza hash tej wersji dokumentu na podstawie indeksu, daty i czasu, różnic i hasha poprzedniego bloku
        //Hash odpowiada za to, że każda nawet najmniejsza zmiana w dokumencie powoduje zmianę hasha, co sprawia, że każda wersja dokumentu jest unikalna i niezmienna
        //Metoda ta wykorzystuje algorytm SHA256 do obliczenia hasha, skutkuje to brakiem możliwości sfałszowania wersji dokumentu
        public string CalculateHash()
        {
            //Tworzymy obiekt SHA256, który będzie używany do obliczenia hasha 
            using (SHA256 sha256 = SHA256.Create())
            {
                //Tworzymy stringa, który będzie używany do obliczenia hasha, łącząc wszystkie właściwości klasy DocumentVersion w jeden string
                string rawData = Index + Timestamp.ToString("o") + Diff + PreviousHash;

                //Przekształcamy tekst rawData na bajty i wyliczamy hash za pomocą algorytmu SHA256
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                //Przekształcamy bajty na stringa w formacie hex (szestnastkowej) i zwracamy go jako hash tej wersji dokumentu
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    //każdy bajt jest konwertowany na dwucyfrowy zapis szesnastkowy i dodawany do stringa
                    builder.Append(b.ToString("x2"));
                }

                //Zwracamy obliczony hash jako string
                return builder.ToString();
            }
        }
    }
}