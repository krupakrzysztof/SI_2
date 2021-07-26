using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Knn_Pawel
{
    class Program
    {
        /// <summary>
        /// Ilość najbliższych próbek branych pod uwagę
        /// </summary>
        private static int K { get; set; } = 3;

        /// <summary>
        /// Metoda służaca za normalizację irysów
        /// </summary>
        /// <param name="list">Lista irysów do znormalizowania</param>
        /// <returns></returns>
        private static IEnumerable<Iris> Normalize(List<Iris> list)
        {
            // wyciągnięcie minimalnej wartości pierwszego atrybutu
            double item1Min = list.Min(x => x.Item1);
            // to samo co dla 1 itp.
            double item2Min = list.Min(x => x.Item2);
            double item3Min = list.Min(x => x.Item3);
            double item4Min = list.Min(x => x.Item4);
            // wyciągnięcie maksymalnej wartości pierwszego atrybutu
            double item1Max = list.Max(x => x.Item1);
            // to samo co dla 1 itp.
            double item2Max = list.Max(x => x.Item2);
            double item3Max = list.Max(x => x.Item3);
            double item4Max = list.Max(x => x.Item4);

            // obliczenie znormalizowanych wartości dla każdego elementu z listy
            foreach (var item in list)
            {
                // zwrócenie irysa z obliczonymi wartościami
                // metoda kończy działanie dopiero gdy zwrócone zostaną wszystkie elementy
                yield return new Iris(
                    (item.Item1 - item1Min) / (item1Max - item1Min),
                    (item.Item2 - item2Min) / (item2Max - item2Min),
                    (item.Item3 - item3Min) / (item3Max - item3Min),
                    (item.Item4 - item4Min) / (item4Max - item4Min),
                    item.Desc);
            }
        }

        /// <summary>
        /// Metoda obliczająca odległość euklidesową
        /// Funkcja przyjmuje 2 parametry typu Iris oraz zwraca typ double
        /// </summary>
        private static Func<Iris, Iris, double> Euclidean { get; } = new Func<Iris, Iris, double>((x, y) => Math.Sqrt(
            // tego nie muszę tłumaczyć jest to po prostu wzór
            Math.Pow(x.Item1 - y.Item1, 2) +
            Math.Pow(x.Item2 - y.Item2, 2) +
            Math.Pow(x.Item3 - y.Item3, 2) +
            Math.Pow(x.Item4 - y.Item4, 2))
        );

        /// <summary>
        /// Metoda obliczająca odległość manhatańską
        /// Funkcja przyjume 2 parametry typu Iris oraz zwraca typ double
        /// </summary>
        // tak samo jak przy euklidesowej, jest to po prostu wzór zapisany w innym sposób
        private static Func<Iris, Iris, double> Manhattan { get; } = new Func<Iris, Iris, double>((x, y) => Math.Abs(x.Item1 - y.Item1) + Math.Abs(x.Item2 - y.Item2) + Math.Abs(x.Item3 - y.Item3) + Math.Abs(x.Item4 - y.Item4));

        /// <summary>
        /// Fukncja testująca Knn
        /// </summary>
        /// <param name="iris">Testowany irys</param>
        /// <param name="list">Lista próbek wzorcowych</param>
        /// <param name="metric">Sposób liczenia metryki</param>
        /// <returns>Odpowiedź knn</returns>
        private static bool TestKnn(Iris iris, List<Iris> list, Func<Iris, Iris, double> metric)
        {
            // słownik przechowywujący irysa dla którego jest liczona odległość oraz odległość
            Dictionary<Iris, double> metrics = new Dictionary<Iris, double>();
            foreach (var item in list)
            {
                // dodanie do słownika irysa oraz obliczonej odległości
                metrics.Add(item, metric(iris, item));
            }
            // posortowanie słownika po odległości rosnąco i rzutowanie wyniku na słownika tego samego typu
            metrics = metrics.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // utworzenie słownika z informacją o ilości wystąpień danej klasy
            Dictionary<string, int> values = new Dictionary<string, int>();
            for (int i = 0; i < K; i++)
            {
                // sprawdzenie czy w słowniku obecna jest już nazwa klasy
                if (values.ContainsKey(metrics.ElementAt(i).Key.Desc))
                {
                    // jeżeli jest to zwiększenie wartości (ilości wystąpień)
                    values[metrics.ElementAt(i).Key.Desc]++;
                }
                else
                {
                    // jeżeli nie ma to dodaje nazwę klasy i do ilości wystąpień przypisuje 1 (jak coś wystąpiło to znaczy że jest 1 raz)
                    values.Add(metrics.ElementAt(i).Key.Desc, 1);
                }
            }

            // posortowanie słownika wystąpień po wartości (ilość wystapień) malejąco i rzutowanie wyniku sortowania na słownik
            values = values.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // jeżeli element który wystąpił najczęściej jest równy nazwie klasy testowanego irysa to zwróci true
            return values.ElementAt(0).Key == iris.Desc;
        }

        static void Main(string[] args)
        {
            // lista odczytanych i znormalizowanych irysów (Linq w praktyce :))
            // odczytuje wszystkie linie z pliku File.ReadAllLines("iris.txt")
            // wybieram z linii 1 linię .Select(x =>
            // dzielę linie to tabulatorze i usuwam puste wpisy x.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries)
            // tworzę nowego irysa x => new Iris
            // na wyniku uruchamiam metodę do normalizacji Normalize()
            List<Iris> irises = Normalize(File.ReadAllLines("iris.txt").Select(x => new Iris(x.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries))).ToList()).ToList();

            // ilość prawidłowych odpowiedzi knn
            int sucessed = 0;
            // dla każdego odczytanego irysa
            foreach (Iris iris in irises)
            {
                // utworzenie kopii listy irysów
                var copy = new List<Iris>(irises);
                // usunięcie testowanego irysa z kopii listy
                copy.Remove(iris);
                // sprawdzenie czy test knn zwróci prawdę
                if (TestKnn(iris, copy, Euclidean))
                {
                    // jeżeli tak to zwiekszenie ilości prawidłowych odpowiedzi
                    sucessed++;
                }
            }

            // wyświetlenie procentowej skuteczności odpowiedzi knn
            Console.WriteLine($"Skuteczność: {sucessed / (double)irises.Count * 100}%");

            // tu można dodać coś żeby program się na chwilę zatrzymał np:
            //Console.ReadLine();
        }
    }

    internal class Iris
    {
        /// <summary>
        /// Pierwsza wartość z pliku
        /// </summary>
        public double Item1 { get; set; }

        /// <summary>
        /// Druga wartość z pliku
        /// </summary>
        public double Item2 { get; set; }

        /// <summary>
        /// Trzecia wartość z pliku
        /// </summary>
        public double Item3 { get; set; }
        
        /// <summary>
        /// Czwarta wartość z pliku
        /// </summary>
        public double Item4 { get; set; }

        /// <summary>
        /// Piąta wartość z pliku
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Konstruktor nowego irysa
        /// </summary>
        /// <param name="values">Tablica wartości irysa</param>
        public Iris(string[] values)
        {
            // rzutowanie pierwszej wartości tablicy na double
            Item1 = double.Parse(values[0], CultureInfo.InvariantCulture);
            Item2 = double.Parse(values[1], CultureInfo.InvariantCulture);
            Item3 = double.Parse(values[2], CultureInfo.InvariantCulture);
            Item4 = double.Parse(values[3], CultureInfo.InvariantCulture);
            Desc = values[4];
        }

        /// <summary>
        /// Konstruktor nowego irysa
        /// </summary>
        /// <param name="item1">Wartość pierwsza</param>
        /// <param name="item2">Wartość druga</param>
        /// <param name="item3">Wartość trzecia</param>
        /// <param name="item4">Wartość czwarta</param>
        /// <param name="desc">Wartość piąta</param>
        public Iris(double item1, double item2, double item3, double item4, string desc)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Desc = desc;
        }
    }
}
