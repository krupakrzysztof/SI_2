using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Grupowanie_Pawel
{
    class Program
    {
        /// <summary>
        /// Obliczenie odległości między dwoma grupami
        /// </summary>
        /// <param name="first">Pierwsza grupa</param>
        /// <param name="second">Druga grupa</param>
        /// <returns>Obliczona odległość</returns>
        private static double Euclidean(HashSet<Point> first, HashSet<Point> second)
        {
            // zwrócenie pierwiastka
            return Math.Sqrt(
                // potęga średniej wartość współrzędnej X z pierwszej grupy - średnia wartość punktu X z drugiej grupy
                Math.Pow(first.Average(x => x.X) - second.Average(x => x.X), 2) +
                // potęga średniej wartości współrzędnej Y z pierwszej grupy first.Average(x => x.Y) - średnia wartość Y z drugiej grupy second.Average(x => x.Y)
                // do potęgi 2 ..., 2)
                Math.Pow(first.Average(x => x.Y) - second.Average(x => x.Y), 2));
        }

        static void Main(string[] args)
        {
            // odczytanie danych z pliku File.ReadAllLines("spirala.txt")
            // utworzenie z każdej linii nowego HashSeta z odczytanym punktem .Select(x =>
            List<HashSet<Point>> groups = File.ReadAllLines("spirala.txt").Select(x =>
            {
                return new HashSet<Point>
                {
                    // dodanie wartości do nowo tworzonego HashSeta
                    new Point
                    {
                        // podzielenie linii po spacji i usunięcie pustych elementów tablicy x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        // zparsowanie pierwszej wartości na doubla i przypisanie jej do właściwości X
                        X = double.Parse(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture),
                        Y = double.Parse(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1], CultureInfo.InvariantCulture)
                    }
                };
            }).ToList();
            // powtarzanie dopóki liczba grup jest większa od 3
            while (groups.Count > 3)
            {
                // obliczenie odległości między grupą pierwsza i drugą
                // zapamiętanie tych wartości oraz grup (całych HashSetów)
                Tuple<HashSet<Point>, HashSet<Point>, double> closest = new Tuple<HashSet<Point>, HashSet<Point>, double>(groups[0], groups[1], Euclidean(groups[0], groups[1]));
                // obliczanie odległości między grupami typu: każdy z każdym (zaczynam od 1 bo dla wartości 0 już mam obliczone i zapamiętane)
                for (int i = 1; i < groups.Count; i++)
                {
                    // zaczyna się od i + 1 aby nie sprawdzać odległości między grupą samą z sobą (kompletnie bez sensu by coś takiego było)
                    for (int j = i + 1; j < groups.Count; j++)
                    {
                        // obliczenie odległości między grupami
                        double distance = Euclidean(groups[i], groups[j]);
                        // sprawdzenie czy obliczona odległość jest mniejsza od zapamiętanej
                        if (distance < closest.Item3)
                        {
                            // zapamiętanie nowych najbliższych sobie grup oraz odległości między nimi
                            closest = new Tuple<HashSet<Point>, HashSet<Point>, double>(groups[i], groups[j], distance);
                        }
                    }
                }

                // połączenie pierwszej z zapamiętanych grup z drugą
                closest.Item1.UnionWith(closest.Item2);
                // usunięcie drugiej grup z listy grup (jej punkty są już w pierwszej grupie w wyniku łączenia)
                groups.Remove(closest.Item2);
            }

            // wyświetlenie zawartości grup
            for (int i = 0; i < groups.Count; i++)
            {
                Console.WriteLine($"Group number {i + 1}");
                foreach (var item in groups[i])
                {
                    Console.WriteLine(item);
                }
            }

            // i tu też możesz dodać coś żeby program nie kończył się od razu np.
            //Console.ReadLine();
        }
    }

    internal class Point
    {
        /// <summary>
        /// Pierwsza wartość z pliku
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Druga wartość z pliku
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Nadpisanie metody aby w każdej części programu zwracać zawsze ten sam styl podczas wyświetlania na konsoli
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{X}\t{Y}";
    }
}
