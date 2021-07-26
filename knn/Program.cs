using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace knn
{
    class Program
    {
        /// <summary>
        /// Zamiana tekstu na wartość liczbową
        /// </summary>
        /// <param name="s">Tekst do zamiany</param>
        /// <returns>Wartość liczbową z tekstu</returns>
        static double ParseToDouble(string s)
        {
            s = s.Trim();
            if (!double.TryParse(s.Replace(',', '.'), out double wynik) && !double.TryParse(s.Replace('.', ','), out wynik))
            {
                throw new Exception("Nie udało się skonwertować liczby do double");
            }

            return wynik;
        }

        /// <summary>
        /// Wczytanie listy irysów z pliku
        /// </summary>
        /// <param name="plik">Plik przechowujący listę irysów</param>
        /// <returns>Lista wczytanych irysów</returns>
        static List<Iris> wczytaj(string plik)
        {
            // deklaracja pustej listy irysów
            List<Iris> irises = new List<Iris>();

            // wczytanie wszystkich lini tekstu z przekazanego jako parametr pliku
            foreach (var item in File.ReadAllLines(plik))
            {
                // podzielenie linii tekstu po znaku \t (tabulator)
                var items = item.Split('\t');
                // wrzutowanie pierwszej wartości na double i analogicznie dla pozostałych wartości
                double sepal_length = ParseToDouble(items[0]);
                double sepal_width = ParseToDouble(items[1]);
                double petal_length = ParseToDouble(items[2]);
                double petal_width = ParseToDouble(items[3]);
                // utworzenie nowego obietku Iris ze wczytanych danych i podanie numery klasy (ostatnia wartość w linii)
                Iris iris = new Iris(sepal_length, sepal_width, petal_length, petal_width, items[4]);
                // dodanie nowego irysa do listy
                irises.Add(iris);
            }

            // zwrócenie wszystkich irysów
            return irises;
        }

        /// <summary>
        /// Metoda normalizuje irysy (wszystkie wartości pól mają wartości z przedziału [0:1])
        /// </summary>
        /// <param name="lista">Lista do znormalizowania</param>
        /// <returns>List znormalizownych irysów</returns>
        static List<Iris> normalizuj(List<Iris> lista)
        {
            // deklaracja pustej listy znormalizowanych irysów
            List<Iris> znormalizowane = new List<Iris>();

            // deklaracja zmiennych przechowujących minimalne i maksymalne wartości poszczególnych pól
            // wartości dla minilanych muszą być duże, a dla maksymalnych bardzo małe
            double min_sepal_length = 10000;
            double min_sepal_width = 10000;
            double min_petal_length = 10000;
            double min_petal_width = 10000;
            double max_sepal_length = -10000;
            double max_sepal_width = -10000;
            double max_petal_length = -10000;
            double max_petal_width = -10000;

            // szukanie wartości minimalnej i maksymalnej w przekazanej liście
            foreach (Iris item in lista)
            {
                // sprawdzenie czy wartości elemetu z listy jest mniejsza od wartości, która jest aktualnie w zmiennej jeżeli tak to zapisanie tej wartości w zmiennej
                if (item.sepal_length < min_sepal_length)
                {
                    min_sepal_length = item.sepal_length;
                }
                // sprawdzenie czy wartości elemetu z listy jest większa od wartości, która jest aktualnie w zmiennej jeżeli tak to zapisanie tej wartości w zmiennej
                if (item.sepal_length > max_sepal_length)
                {
                    max_sepal_length = item.sepal_length;
                }

                // analogicznie jak wyżej
                if (item.sepal_width < min_sepal_width)
                {
                    min_sepal_width = item.sepal_width;
                }
                if (item.sepal_width > max_sepal_width)
                {
                    max_sepal_width = item.sepal_width;
                }

                if (item.petal_length < min_petal_length)
                {
                    min_petal_length = item.petal_length;
                }
                if (item.petal_length > max_petal_length)
                {
                    max_petal_length = item.petal_length;
                }

                if (item.petal_width < min_petal_width)
                {
                    min_petal_width = item.petal_width;
                }
                if (item.petal_width > max_petal_width)
                {
                    max_petal_width = item.petal_width;
                }
            }

            // normalizacja wszystkich elementów z listy
            foreach (var item in lista)
            {
                // obliczenie wartości pierwszego parametru dla konkretnego irysa na podstawie wzoru zamieszczonego w poleceniu
                double sepal_length = (item.sepal_length - min_sepal_length) / (max_sepal_length - min_sepal_length);
                double sepal_width = (item.sepal_width - min_sepal_width) / (max_sepal_width - min_sepal_width);
                double petal_length = (item.petal_length - min_petal_length) / (max_petal_length - min_petal_length);
                double petal_width = (item.petal_width - min_petal_width) / (max_petal_width - min_petal_width);
                // utworzenie nowego irysa, który ma wartości znomalizowane, numer klasy nie podlega normalizacji dlatego jego wartości nie zmieniamy i przypisujemy taką samą jak była na początku
                Iris iris = new Iris(sepal_length, sepal_width, petal_length, petal_width, item.@class);
                // dodanie znormalizowanego irysa do listy znormalizowanych
                znormalizowane.Add(iris);
            }

            // zwrócenie listy znormalizowanych irysów
            return znormalizowane;
        }

        /// <summary>
        /// Obliczenie odległości między dwoma irysami metodą euklidesową
        /// </summary>
        /// <param name="iris1">Irys pierwszy</param>
        /// <param name="iris2">Irys drugi</param>
        /// <returns>Odległość między irysami</returns>
        static double metryka_euklidesowa(Iris iris1, Iris iris2)
        {
            // obliczenie wartości pierwszego parametru na podstawie wzoru znajdującego się w poleceniu
            double sepal_length = (iris2.sepal_length - iris1.sepal_length) * (iris2.sepal_length - iris1.sepal_length);
            double sepal_width = (iris2.sepal_width - iris1.sepal_width) * (iris2.sepal_width - iris1.sepal_width);
            double petal_length = (iris2.petal_length - iris1.petal_length) * (iris2.petal_length - iris1.petal_length);
            double petal_width = (iris2.petal_width - iris1.petal_width) * (iris2.petal_width - iris1.petal_width);
            // obliczenie pierwiastka z sumy powyższych parametrów (według wzoru)
            return Math.Sqrt(sepal_length + sepal_width + petal_length + petal_width);
        }

        /// <summary>
        /// Obliczenie odległości między dwoma irysami metodą manhattan
        /// </summary>
        /// <param name="iris1">Irys pierwszy</param>
        /// <param name="iris2">Irys drugi</param>
        /// <returns>Odległość między irysami</returns>
        static double metryka_manhattan(Iris iris1, Iris iris2)
        {
            // odliczenie wartości pierwszego parametru na podstawie wzoru znajdującego się w poleceniu
            double sepal_length = iris1.sepal_length - iris2.sepal_length;
            // wyznaczenie wartości bezwzględnej, jeżeli wartość jest większa od 0 to przypisz tą samą wartość w przeciwnym przypadku przypisza wartość ze zmienionym znakiem
            sepal_length = sepal_length > 0 ? sepal_length : -sepal_length;
            double sepal_width = iris1.sepal_width - iris2.sepal_width;
            sepal_width = sepal_width > 0 ? sepal_width : -sepal_width;
            double petal_length = iris1.petal_length - iris2.petal_length;
            petal_length = petal_length > 0 ? petal_length : -petal_length;
            double petal_width = iris1.petal_width - iris2.petal_width;
            petal_width = petal_width > 0 ? petal_width : -petal_width;
            // zwrócenie sumy parametrów - wzór z polecenia
            return sepal_length + sepal_width + petal_length + sepal_width;
        }

        /// <summary>
        /// Testowanie skuteczności KNN
        /// </summary>
        /// <param name="iris">Próbka testowana</param>
        /// <param name="kopia">Próbki testujące</param>
        /// <param name="k">Ilość wybieranych sąsiadów</param>
        /// <param name="wybor">Wskazanie metryki</param>
        /// <returns>Czy KNN odpowiedział prawidłowo</returns>
        static bool testuj(Iris iris, List<Iris> kopia, int k, string wybor)
        {
            // lista przechowująca odległości między punktem testowanym a kolejnymi punktami (tabelka)
            List<KeyValuePair<double, Iris>> metryki = new List<KeyValuePair<double, Iris>>();
            // obliczenie odległości dla wszystkich elementów
            foreach (var item in kopia)
            {
                // deklaracja zmiennej w której będzie obliczona odległość
                double odleglosc = 0;
                // jeżeli wybór jest równy 1
                if (wybor == "1")
                {
                    // oblicz odległość metodą euklidesową
                    odleglosc = metryka_euklidesowa(iris, item);
                }
                else
                {
                    // oblicz odległość metodą manhattan
                    odleglosc = metryka_manhattan(iris, item);
                }

                // dodanie do listy odległości i irysa testowanego
                metryki.Add(new KeyValuePair<double, Iris>(odleglosc, item));
            }

            // sortowanie odległości od najmniejszej, x to konkretny wiersz z listy, znak '=>' znacza, że sortowanie ma się odbyć po kluczu czyli odległości
            metryki = metryki.OrderBy(x => x.Key).ToList();

            // słownik do przechwywanie ilości wystąpień próbek testowanych
            // string -> nazwa klasy
            // int -> ilość wystąpień
            Dictionary<string, int> ilosc_wystapien = new Dictionary<string, int>();
            // uruchomienie testowania dla k najbliższych elementów
            for (int i = 0; i < k; i++)
            {
                // nazwa klasy w elemencie znajdującym się na i-tym elemencie listy z metrykami
                string nazwa_klasy = metryki.ElementAt(i).Value.@class;
                // jeżeli w słowników występuję klucz o nazwie klasy aktualnego elementu
                if (ilosc_wystapien.ContainsKey(nazwa_klasy))
                {
                    // zwiększenie wartości w słowniku
                    ilosc_wystapien[nazwa_klasy]++;
                }
                else
                {
                    // dodanie nowego elementu do słownika
                    // nazwa_klasy -> nazwa klucza w słowniku
                    // 1 -> ilość wystapień, 1 ponieważ wystąpiło pierwszy raz
                    ilosc_wystapien.Add(nazwa_klasy, 1);
                }
            }

            // deklaracja listy która przychowuje posortowane ilości wystąpień od największej wartości
            List<KeyValuePair<string, int>> ilosc_wystapien_lista = ilosc_wystapien.OrderByDescending(x => x.Value).ToList();

            // sprawdzenie czy pierwszy element z listy w kluczu (nazwa klasy) ma taką samą wartość jak testowany irys
            if (ilosc_wystapien_lista.ElementAt(0).Key == iris.@class)
            {
                // zwrócenie prawdy -> oznacza że KNN dopasował prawidłową nazwe klasy do testowanego irysa
                return true;
            }

            // zwrócenie fałszu -> KNN źle dopasował nazwę klasy dla testowanego irysa
            return false;
        }

        static void Main(string[] args)
        {
            // lista wczytanych irysów z pliku iris.txt
            List<Iris> irises = wczytaj("iris.txt");
            // utworzenie listy znormalizowanych irysów
            List<Iris> znormalizowaneirisy = normalizuj(irises);

            // deklaracja zmiennej przechowującej wybów jakie metryki użyć
            string wybor = string.Empty;
            do
            {
                Console.WriteLine("Wybierz metryke");
                Console.WriteLine("1. Euklidesowa");
                Console.WriteLine("2. Manhattan");
                wybor = Console.ReadLine();
                // powtarzaj tak długo aż użytkownik wybierze 1 lub 2
            } while (wybor != "1" && wybor != "2");

            // parametr k -> k najbliższych sąsiadów
            int k = 3;
            int ilosc_poprawnych = 0;
            // dla każdego elementu z listy znomalizowanych
            foreach (var item in znormalizowaneirisy)
            {
                // utwórzenie aktualego irysa w nowej zmiennej
                var aktualny_irys = item;
                // utworzenie kopii listy irysów
                List<Iris> kopia = new List<Iris>();
                // dla każdego elementu z listy znormalizowanych dodaj go do listy o nazwie kopia
                znormalizowaneirisy.ForEach(x => kopia.Add(x));
                // z listy kopia usuń aktualny element
                kopia.Remove(aktualny_irys);

                // uruchomienie testowania knn, który zwróci prawdę jeżeli została dopasowana klasa jak w testowanym irysie
                if (testuj(aktualny_irys, kopia, k, wybor))
                {
                    // zwiekszenie ilości poprawnych odpowiedzi
                    ilosc_poprawnych++;
                }
            }

            //wyświetlenie napisu Skuteczność: x%
            // ilosc_poprawnych jest rzutowana na tym double aby otrzymać wynik typu double w przeciwny wypadku wynik był by równy 0
            Console.WriteLine("Skuteczność: " + (double)ilosc_poprawnych / znormalizowaneirisy.Count * 100 + "%");

            // program zatrzymuję się do momentu aż użytkonik naciśnie ENTER
            Console.ReadLine();
        }
    }
}
