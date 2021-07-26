using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace grupowanie
{
    class Program
    {
        /// <summary>
        /// Ilość grup, przy której program kończy łączenie
        /// </summary>
        private static int IloscKonca { get; } = 3;

        static void Main(string[] args)
        {
            // lista grup punktów (lista lista)
            List<HashSet<Punkt>> punkty = new List<HashSet<Punkt>>();

            // odczytanie wszystkich linii z pliku
            string[] linie = File.ReadAllLines("spirala.txt");
            foreach (string linia in linie)
            {
                // podzielenie linii po znaku spacji i usunięcie pustych komórek tabeli
                string[] liniaWartosci = linia.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // jeżeli liczba wartość jest różna od 2 to pomijam grupe
                if (liniaWartosci.Length != 2)
                {
                    continue;
                }
                // do grup dodaje nową liste zawierającą jeden punkt
                punkty.Add(new HashSet<Punkt>()
                {
                    // dodanie do listy nowego elementu
                    new Punkt()
                    {
                        // przypisanie wartości pierwszej liczby z pliku
                        X = double.Parse(liniaWartosci[0].Replace(",", "."), CultureInfo.InvariantCulture),
                        // przypisanie wartości drugiej liczby z pliku
                        Y = double.Parse(liniaWartosci[1].Replace(",", "."), CultureInfo.InvariantCulture)
                    }
                });
            }

            double polowa = punkty.Count / 2d;

            // powtarzam dopóki liczba grup jest większa od liczby kończącej działanie aplikacji
            while (true)
            {
                if (punkty.Any(x => x.Count > polowa))
                {
                    break;
                }
                // struktura przechowująca 3 wartości 
                // Item1 - double
                // Item2 - HashSet<Punkt>
                // Item3 - HashSet<Punkt>
                Tuple<double, HashSet<Punkt>, HashSet<Punkt>> min = new Tuple<double, HashSet<Punkt>, HashSet<Punkt>>(double.MaxValue, null, null);
                // pętla obliczająca odległość między wszystkimi grupami (każdy z każdym)
                for (int i = 0; i < punkty.Count; i++)
                {
                    for (int j = i + 1; j < punkty.Count; j++)
                    {
                        // odliczenie odległości między grupami
                        double odleglosc = Punkt.Odlegosc(punkty[i], punkty[j]);
                        // jeżeli obliczona odległość jest mniej od poprzedniej najmniejszej do należy ją zapamiętać wraz z grupami dla których ona wystąpiła
                        if (odleglosc < min.Item1)
                        {
                            // zapamiętanie
                            min = new Tuple<double, HashSet<Punkt>, HashSet<Punkt>>(odleglosc, punkty[i], punkty[j]);
                        }
                    }
                }
                // połączenie grup dla których odległość jest najmniejsza
                min.Item2.UnionWith(min.Item3);
                // usunięcie grupy, której wartości zostały linijkę wyżej połączone (dodane do innej grupy). pozbycie się zbuplikowanych wartości
                punkty.Remove(min.Item3);
            }

            // wyświetlenie zawartości grup na konsoli
            for (int i = 0; i < punkty.Count; i++)
            {
                Console.WriteLine($"Grupa numer {i + 1}");
                foreach (Punkt punkt in punkty[i])
                {
                    Console.WriteLine($"{punkt.X};{punkt.Y}");
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }

    internal class Punkt
    {
        // to jest nie potrzebne
        public Punkt()
        {

        }

        // to też jest nie potrzebne
        public Punkt(double z, double y)
        {
            X = X;
            Y = y;
        }

        /// <summary>
        /// Wartość pierwszej współrzędnej punktu
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Wartość drugiej współrzędnej punktu
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Obliczenie odległości między dwoma grupami punktów
        /// </summary>
        /// <param name="grupa1"></param>
        /// <param name="grupa2"></param>
        /// <returns></returns>
        public static double Odlegosc(HashSet<Punkt> grupa1, HashSet<Punkt> grupa2)
        {
            // wartość środkowa pierwszej grupy
            Punkt srodek1 = new Punkt()
            {
                // do X przypisuje średnią wartość X z pierwszej grupy
                X = grupa1.Average(x => x.X),
                Y = grupa1.Average(x => x.Y)
            };
            Punkt srodek2 = new Punkt()
            {
                X = grupa2.Average(x => x.X),
                Y = grupa2.Average(x => x.Y)
            };
            // Math.Pow - potęga
            // Math.Sqrt - pierwiastek
            // to było generalnie gdzieś w jego dokumencie jak liczyć
            return Math.Sqrt(Math.Pow(srodek1.X - srodek2.X, 2) + Math.Pow(srodek1.Y - srodek2.Y, 2));
        }
    }
}
