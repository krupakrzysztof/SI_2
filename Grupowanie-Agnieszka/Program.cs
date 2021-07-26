using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Grupowanie_Agnieszka
{
    class Program
    {
        /// <summary>
        /// Obliczenie odległości między dwoma grupami
        /// </summary>
        /// <param name="grupa1"></param>
        /// <param name="grupa2"></param>
        /// <returns></returns>
        private static double Odleglosc(HashSet<Punkt> grupa1, HashSet<Punkt> grupa2)
        {
            // środkowa wartość współrzędnej X z pierwszej grupy
            double g1x = 0;
            // środkowa wartość współrzędnej Y z pierwszej grupy
            double g1y = 0;

            foreach (Punkt punkt in grupa1)
            {
                // zsumowanie wszystkich wartość X
                g1x += punkt.X;
                // oraz Y
                g1y += punkt.Y;
            }
            // podzielenie sum przez ilość próbek (średnia)
            g1x /= grupa1.Count;
            g1y /= grupa1.Count;

            // analogicznie dla drugiej grupy
            double g2x = 0;
            double g2y = 0;

            foreach (Punkt punkt in grupa2)
            {
                g2x += punkt.X;
                g2y += punkt.Y;
            }

            g2x /= grupa2.Count;
            g2y /= grupa2.Count;

            // wysyliczenie odległości ze wzoru
            // pierwiastek((środek pierwszej grupy - środek drugiej grupy)^2)
            return Math.Sqrt(((g1x - g2x) * (g1x - g2x)) + ((g1y - g2y) * (g1x - g2y)));
        }

        static void Main(string[] args)
        {
            // odczytanie wszystkich linii z pliku (najlepiej zmień ścieżke)
            string[] linie = File.ReadAllLines(@"C:\VS\SI_2\spirala.txt");
            // stworzenie listy grupy punktów
            List<HashSet<Punkt>> grupy = new List<HashSet<Punkt>>();
            // dla każdej linii
            foreach (string item in linie)
            {
                // podzielenie odczytanej linii po spacji oraz usunięcie pustych wartości
                string[] wartosci = item.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // zamiana wartości pierwszej na double
                double x = double.Parse(wartosci[0], CultureInfo.InvariantCulture);
                // analogicznie jak z pierwszą wartością
                double y = double.Parse(wartosci[1], CultureInfo.InvariantCulture);
                // dodanie punktu do listy grupy
                // tzn. stworzenie nowej grupy (HashSet) i dodanej jej do listy
                grupy.Add(new HashSet<Punkt>()
                {
                    // dodanie do grupy (HashSet) nowego punktu i zainicjowanie odczytanymi wartościami
                    new Punkt
                    {
                        X = x,
                        Y = y
                    }
                });
            }

            int sredniaWielkosc = grupy.Count / 10;

            // powtarzanie dopóki ilość grupy jest większa od 3
            while (true)
            {
                if (grupy.Average(x => x.Count) > sredniaWielkosc)
                {
                    break;
                }
                // pierwsza najbliższa grupa
                HashSet<Punkt> g1 = new HashSet<Punkt>();
                // druga najbliższa grupa
                HashSet<Punkt> g2 = new HashSet<Punkt>();
                // odległość między grupami
                double odleglosc = double.MaxValue;
                
                // wyszukiwanie pary najbliższych grup
                for (int i = 0; i < grupy.Count; i++)
                {
                    for (int j = i + 1; j < grupy.Count; j++)
                    {
                        // odliczenie odległości między grupami
                        double odl = Odleglosc(grupy[i], grupy[j]);
                        // jeżeli obliczona odległość jest mniejsza od zapamiętaj
                        if (odleglosc > odl)
                        {
                            // zapamiętanie grupy pierwszej
                            g1 = grupy[i];
                            // zapamiętanie grupy drugiej
                            g2 = grupy[j];
                            // zapamiętanie odległości między grupami
                            odleglosc = odl;
                        }
                    }
                }

                // połączenie grupy pierwszej z drugą
                g1.UnionWith(g2);
                // usunięcie grupy drugiej z listy, jej wartości są dodanej do pierwszej grupy więcj jest ona już nie potrzebna
                grupy.Remove(g2);

                for (int i = 0; i < grupy.Count; i++)
                {
                    Console.WriteLine($"Grupa: {i + 1}");
                    Console.WriteLine(grupy[i].Count);
                }
                Console.WriteLine(grupy.Count);
            }

            // wyświetlenie wartości punktów w grupach
            for (int i = 0; i < grupy.Count; i++)
            {
                Console.WriteLine($"Grupa {i + 1}");
                foreach (var item in grupy[i])
                {
                    // x [tabulator] y
                    Console.WriteLine($"{item.X}\t{item.Y}");
                }
            }

            Console.ReadLine();
        }
    }

    // klasa pomocnicza
    class Punkt
    {
        // pierwsza wartość z pliku
        public double X { get; set; }
        // druga wartość z pliku
        public double Y { get; set; }
    }
}
