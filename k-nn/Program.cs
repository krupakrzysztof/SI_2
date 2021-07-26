using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace k_nn
{
    class Program
    {
        /// <summary>
        /// Ilość najbliższych sąsiadów do sprawdzenia
        /// </summary>
        private static readonly int K = 3;

        /// <summary>
        /// Normalizacja listy irysów
        /// </summary>
        /// <param name="irysy"></param>
        /// <returns></returns>
        static List<Irys> Normalizuj(List<Irys> irysy)
        {
            // wynikowa lista irysów znormalizowanych
            List<Irys> wynik = new List<Irys>();

            // minimalna wartość pierwszej wartości
            double item1Min = irysy.Min(x => x.Item1);
            double item2Min = irysy.Min(x => x.Item2);
            double item3Min = irysy.Min(x => x.Item3);
            double item4Min = irysy.Min(x => x.Item4);

            // maksymalna wartość pierwszej wartości
            double item1Max = irysy.Max(x => x.Item1);
            double item2Max = irysy.Max(x => x.Item2);
            double item3Max = irysy.Max(x => x.Item3);
            double item4Max = irysy.Max(x => x.Item4);

            // powtórzenie dla wszystkich elementów z listy
            foreach (Irys irys in irysy)
            {
                // dodanie nowego irysa wg wzoru z dokumentu (nie mam go teraz ale coś podobnego było wczesniej)
                wynik.Add(new Irys((irys.Item1 - item1Min) / (item1Max - item1Min),
                    (irys.Item2 - item2Min) / (item2Max - item2Min),
                    (irys.Item3 - item3Min) / (item3Max - item3Min),
                    (irys.Item4 - item4Min) / (item4Max - item4Min),
                    irys.Description));
            }

            return wynik;
        }

        static void Main(string[] args)
        {
            // linie odczytane z pliku
            string[] linie = File.ReadAllLines("iris.txt");
            List<Irys> irysy = new List<Irys>();
            foreach (string linia in linie)
            {
                // podzielenie linii po znaku tabulatora
                string[] liniaWartosci = linia.Split('\t');

                // zamiana tekstu na liczbe typu double na pierwszej wartości
                double.TryParse(liniaWartosci[0], NumberStyles.Number, CultureInfo.InvariantCulture, out double item1);
                double.TryParse(liniaWartosci[1], NumberStyles.Number, CultureInfo.InvariantCulture, out double item2);
                double.TryParse(liniaWartosci[2], NumberStyles.Number, CultureInfo.InvariantCulture, out double item3);
                double.TryParse(liniaWartosci[3], NumberStyles.Number, CultureInfo.InvariantCulture, out double item4);

                // dodanie nowego irysa z odczytanych wartości
                irysy.Add(new Irys(item1, item2, item3, item4, liniaWartosci[4]));
            }

            // przypisanie do listy irysów listy znormalizowanej (tzn makysmalna wartość dowolnego z atrybutów to 1)
            irysy = Normalizuj(irysy);

            // sposób liczenia metryki
            Func<Irys, Irys, double> metryka = Irys.MetrykaEuklidesowa;
            // ilość poprawnych odpowiedzi knna
            int poprawne = 0;

            foreach (Irys irys in irysy)
            {
                // utworzenie kopii listy irysów
                List<Irys> wzorcowe = new List<Irys>(irysy);
                // usunięcie z kopii aktualnie sprawdzanego irysa
                wzorcowe.Remove(irys);

                // sprawdzenie czy test przechodzi prawidłowo
                if (Irys.Testuj(irys, wzorcowe, K, metryka))
                {
                    // jeżeli tak to zwiększam ilość poprawnych odpowiedzi
                    poprawne++;
                }
            }

            // obliczam skuteczność procentową i wyświetlam ją na konsoli
            Console.WriteLine($"Skuteczność to: {(double)poprawne / irysy.Count * 100}%");
            // zatrzymuje działanie programu do momentu naciśnięcia klawisza enter
            Console.ReadLine();
        }
    }

    internal class Irys
    {
        /// <summary>
        /// Konstruktor nowego irysa
        /// </summary>
        /// <param name="item1">Wartość parametru pierwszego</param>
        /// <param name="item2"></param>
        /// <param name="item3"></param>
        /// <param name="item4"></param>
        /// <param name="description"></param>
        public Irys(double item1, double item2, double item3, double item4, string description)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Description = description;
        }

        /// <summary>
        /// Wartość pierwsza z pliku
        /// </summary>
        public double Item1 { get; set; }

        /// <summary>
        /// Wartość druga z pliku
        /// </summary>
        public double Item2 { get; set; }

        /// <summary>
        /// Wartość trzecia z pliku
        /// </summary>
        public double Item3 { get; set; }

        /// <summary>
        /// Wartość czwarta z pliku
        /// </summary>
        public double Item4 { get; set; }

        /// <summary>
        /// Wartość piąta z pliku
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Obliczenie odległości między irysami metodą euklidesową
        /// </summary>
        /// <param name="irys1"></param>
        /// <param name="irys2"></param>
        /// <returns></returns>
        public static double MetrykaEuklidesowa(Irys irys1, Irys irys2)
        {
            // coś takiego było wcześniej w dokumencie od nowaka, nie pobierałem teraz nic od niego ze strony więc mógł coś zmienić
            // jak coś to daj znać i poprawie :)
            double item1 = Math.Pow(irys2.Item1 - irys1.Item1, 2);
            double item2 = Math.Pow(irys2.Item2 - irys1.Item2, 2);
            double item3 = Math.Pow(irys2.Item3 - irys1.Item3, 2);
            double item4 = Math.Pow(irys2.Item4 - irys1.Item4, 2);
            return Math.Sqrt(item1 + item2 + item3 + item4);
        }

        /// <summary>
        /// Obliczenie odległości między irysami metodą mantahańską
        /// </summary>
        /// <param name="irys1"></param>
        /// <param name="irys2"></param>
        /// <returns></returns>
        public static double MetrykaManhatanowska(Irys irys1, Irys irys2)
        {
            // analogicznie jak przy metryce euklidesowej
            double item1 = Math.Abs(irys2.Item1 - irys1.Item1);
            double item2 = Math.Abs(irys2.Item2 - irys1.Item2);
            double item3 = Math.Abs(irys2.Item3 - irys1.Item3);
            double item4 = Math.Abs(irys2.Item4 - irys1.Item4);
            return item1 + item2 + item3 + item4;
        }

        /// <summary>
        /// Testowanie poprawności udzielanych odpowiedzi przez algorytm
        /// </summary>
        /// <param name="irys">Testowany irys</param>
        /// <param name="wzorcowe">List próbek wzorcowych</param>
        /// <param name="k">Ilość najbliższych sąsiadów branych pod uwagę</param>
        /// <param name="metryka">Sposób liczenia metryki</param>
        /// <returns></returns>
        public static bool Testuj(Irys irys, List<Irys> wzorcowe, int k, Func<Irys, Irys, double> metryka)
        {
            // lista irysów wraz z odległościami do testowanego irysa
            List<Tuple<Irys, double>> odleglosci = new List<Tuple<Irys, double>>();

            foreach (Irys wzorcowy in wzorcowe)
            {
                // dodanie do listy odległości nowego obiektu Tuple (struktura przyjmująca w tym przypadku dwie wartości Irys i double)
                // pierwszym parametrem jest irys dla którego była obliczana odległość i wartość odległości
                odleglosci.Add(new Tuple<Irys, double>(wzorcowy, metryka(irys, wzorcowy)));
            }

            // posortowanie listy odległości od najmniejszej
            odleglosci = odleglosci.OrderBy(x => x.Item2).ToList();

            // słownika przechowujący w kluczu klase, a w wartości ilość jej wystąpień
            Dictionary<string, int> wystapienia = new Dictionary<string, int>();

            for (int i = 0; i < k; i++)
            {
                // sprawdzenie czy w słowniku w kluczu występuje nazwa klasy
                if (wystapienia.ContainsKey(odleglosci[i].Item1.Description))
                {
                    // jeżeli tak to wziększam ilość jej wystąpień
                    wystapienia[odleglosci[i].Item1.Description]++;
                }
                else
                {
                    // jeżeli nie to dodaje nowy element z nazwą klasy i z ilością wystąpień 1 (jeżeli tu trawiłem to znaczy że mam taki element tzn wystąpił on)
                    wystapienia.Add(odleglosci[i].Item1.Description, 1);
                }
            }

            // sortowanie słownika wystąpień malejąco (od największej ilości wystąpień) i rzutowanie wyniku na słownik
            // wybranie do klucza słownika jego poprzedniego klucza x => x.Key
            // wybranie do wartości slownika jego poprzedniej wartości x => x.Value
            wystapienia = wystapienia.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            if (wystapienia.Any(x => x.Key != irys.Description))
            {
                return false;
            }

            // jeżeli ilość wystąpień jest większa od 1 i element na pierwszej i drugiej pozycji mają taką samą ilość wystąpień to znaczy że knn nie może jednoznacznie określić czy odpowiedział prawidłowo
            // więc zwaracam błąd :)
            if (wystapienia.Count > 1 && wystapienia.ElementAt(0).Value == wystapienia.ElementAt(1).Value)
            {
                return false;
            }
            // jeżeli element, który wystąpił najczęściej ma taką samą nazwe klasy jak testowany irys to znaczy że knn odpowiedział prawidłowo zwracam prawde
            if (wystapienia.ElementAt(0).Key == irys.Description)
            {
                return true;
            }

            // domyślna wartość gdybym nie trawił w poprzednie warunki
            return false;
        }
    }
}
