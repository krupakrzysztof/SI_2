using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Grupowanie_Dawid
{
    class Program
    {
        /// <summary>
        /// Obliczenie ogległości między grupami punktów
        /// </summary>
        /// <param name="pierwsza">Pierwsza grupa</param>
        /// <param name="druga">Druga grupa</param>
        /// <returns>Ogległość między grupami</returns>
        private static double OdlegloscEuklidesowa(List<Punkt> pierwsza, List<Punkt> druga)
        {
            // wyznaczenie środkowej wartości pierwszej grupy
            Punkt srodekPierwszy = new Punkt()
            {
                // obliczenie średniej dla właściwości X
                X = pierwsza.Average(p => p.X),
                // obliczenie średniej dla właściwości Y
                Y = pierwsza.Average(p => p.Y)
            };

            // analogicznie jak w przypadku pierwszej grupy
            Punkt srodekDrugi = new Punkt()
            {
                X = druga.Average(p => p.X),
                Y = druga.Average(p => p.Y)
            };

            // obliczenie odległości
            // (pierwszy.X - drugi.X) * (pierwszy.X - drugi.X) + (pierwszy.Y - drugi.Y) * (pierwszy.Y - drugi.Y)
            return Math.Sqrt(Math.Pow(srodekPierwszy.X - srodekDrugi.X, 2) + Math.Pow(srodekPierwszy.Y - srodekDrugi.Y, 2));
        }

        static void Main(string[] args)
        {
            // wczytanie wszystkich linii z pliku spirala.txt i trochę linq :)
            List<List<Punkt>> grupy = File.ReadAllLines("spirala.txt").Select(x =>
            {
                // podzielenie wczytanej linii po znaku ' ' (spacja) oraz usunięcie pustych wpisów
                string[] s = x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // utworzenie listy punktów (lista z 1 elementem) będzie to potrzebne za chwilę
                return new List<Punkt>()
                {
                    new Punkt()
                    {
                        // parsowanie pierwszej wartości na double (inaczej to zamiana tekstu na liczbę)
                        X = double.Parse(s[0], CultureInfo.InvariantCulture),
                        // parsowanie drugiej wartości na double
                        Y = double.Parse(s[1], CultureInfo.InvariantCulture)
                    }
                };
                // rzutowanie wszystkiego na liste
            }).ToList();

            int koniec = grupy.Count / 5;

            // powtarzaj dopóki liczba grup jest większa do 3
            while (true)
            {
                if (grupy.Any(x => x.Count >= koniec))
                {
                    break;
                }
                // minimalna odległość między grupami, na start przypisana jest maxymalna wartość double (czyli przy pierwszym sprawdzeniu zawsze zostanie podmieniona)
                double min = double.MaxValue;
                // grupa, do której zostaną dodane wartości z innej grupy
                List<Punkt> pierwszy = null;
                // grupa, która zostanie usunięta
                List<Punkt> drugi = null;
                for (int i = 0; i < grupy.Count; i++)
                {
                    for (int j = i + 1; j < grupy.Count; j++)
                    {
                        // obliczenie odległości między dwoma grupami
                        double odl = OdlegloscEuklidesowa(grupy[i], grupy[j]);
                        // jeżeli obliczona odległość jest mniejsza od zapisanej
                        if (min > odl)
                        {
                            // zapamiętaj nową najmniejszą odległość
                            min = odl;
                            // zapamiętaj pierwszą grupę
                            pierwszy = grupy[i];
                            // zapamiętaj drugą grupę
                            drugi = grupy[j];
                        }
                    }
                }
                // do pierwszej grupy dodaj wszystkie wartości z drugiej
                pierwszy.AddRange(drugi);
                // usuń drugą grupę z listy grupy (jej wartości nadal pozostają lecz są już w innej grupie)
                grupy.Remove(drugi);

                Console.WriteLine($"Łączna ilość grupoy: {grupy.Count}");
                //foreach (var item in grupy)
                //{
                //    foreach (var item2 in item)
                //    {
                //        Console.WriteLine(item2.ToString());
                //    }
                //}
                Console.WriteLine();
                Console.WriteLine();
                //Console.WriteLine(grupy.Count);
            }

            // wyświetlenie na ekranie wartości grup
            for (int i = 0; i < grupy.Count; i++)
            {
                Console.WriteLine($"Grupa nr: {i + 1}");
                foreach (Punkt item in grupy[i])
                {
                    Console.WriteLine(item.ToString());
                }
            }

            Console.ReadLine();
        }
    }

    internal class Punkt
    {
        /// <summary>
        /// Pierwsza wartość punktu
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Druga wartość punktu
        /// </summary>
        public double Y { get; set; }

        public override string ToString() => $"X: {X}, Y: {Y}";
    }
}
