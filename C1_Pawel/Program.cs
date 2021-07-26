using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C1_Pawel
{
    class Program
    {
        static string TablicaDoString<T>(T[][] tab)
        {
            string wynik = "";
            for (int i = 0; i < tab.Length; i++)
            {
                for (int j = 0; j < tab[i].Length; j++)
                {
                    wynik += tab[i][j].ToString() + " ";
                }
                wynik = wynik.Trim() + Environment.NewLine;
            }

            return wynik;
        }

        static double StringToDouble(string liczba)
        {
            liczba = liczba.Trim();
            if (!double.TryParse(liczba.Replace(',', '.'), out double wynik) && !double.TryParse(liczba.Replace('.', ','), out wynik))
            {
                throw new Exception("Nie udało się skonwertować liczby do double");
            }

            return wynik;
        }


        static int StringToInt(string liczba)
        {
            if (!int.TryParse(liczba.Trim(), out int wynik))
            {
                throw new Exception("Nie udało się skonwertować liczby do int");
            }

            return wynik;
        }

        static string[][] StringToTablica(string sciezkaDoPliku)
        {
            string trescPliku = System.IO.File.ReadAllText(sciezkaDoPliku); // wczytujemy treść pliku do zmiennej
            string[] wiersze = trescPliku.Trim().Split(new char[] { '\n' }); // treść pliku dzielimy wg znaku końca linii, dzięki czemu otrzymamy każdy wiersz w oddzielnej komórce tablicy
            string[][] wczytaneDane = new string[wiersze.Length][];   // Tworzymy zmienną, która będzie przechowywała wczytane dane. Tablica będzie miała tyle wierszy ile wierszy było z wczytanego poliku

            for (int i = 0; i < wiersze.Length; i++)
            {
                string wiersz = wiersze[i].Trim();     // przypisuję i-ty element tablicy do zmiennej wiersz
                string[] cyfry = wiersz.Split(new char[] { ' ' });   // dzielimy wiersz po znaku spacji, dzięki czemu otrzymamy tablicę cyfry, w której każda oddzielna komórka to czyfra z wiersza
                wczytaneDane[i] = new string[cyfry.Length];    // Do tablicy w której będą dane finalne dokładamy wiersz w postaci tablicy integerów tak długą jak długa jest tablica cyfry, czyli tyle ile było cyfr w jednym wierszu
                for (int j = 0; j < cyfry.Length; j++)
                {
                    string cyfra = cyfry[j].Trim(); // przypisuję j-tą cyfrę do zmiennej cyfra
                    wczytaneDane[i][j] = cyfra;
                }
            }
            return wczytaneDane;
        }

        // znalezienie różnych klas decyzyjnych w systemie, ostatnia wartość z linii
        // np 1.234 22.08 11.46 2 4 4 1.585 0 0 0 1 2 100 1213 0
        // klasą decyzyjną będzie 0
        private static HashSet<string> FindClasses(string[][] array)
        {
            // listą, która przyjmuje tylko unikalne wartości
            HashSet<string> result = new HashSet<string>();
            // rzutowanie tablicy na listę (List<string[]>) i dla każdej linii dodanie ostatniego elementu to HashSet<string>
            array.ToList().ForEach(line => result.Add(line.Last()));

            return result;
        }

        // policzenie wielkości klas, tzn ile jest linii które na końcu mają 0 albo 1 (w przypadku obróbki pliku autralian.txt)
        // do metody przekazywane są znalezione wczesniej kasy oraz wczytany plik
        private static Dictionary<string, int> SizeOfClasses(HashSet<string> classes, string[][] array)
        {
            // słownik, który w kluczu będzie przechowywał nazwe klasy a wartością będzie ilość wystąpień
            Dictionary<string, int> result = new Dictionary<string, int>();
            // rzutowanie klas na listę (List<string>) i dla każdego elementu dodanie wpisu do słownika
            // pierwszym parametrem jest nazwa klasy, a wartością liczba linii, w których ostatni element jest równy nazwie klasy
            classes.ToList().ForEach(c => result.Add(c, array.Count(line => line.Last() == c)));
            
            return result;
        }

        // znalezienie minimalnej i maxymalnej wartości poszczególnych atrybutów
        // do metody przykazywana jest tablica wczytanych wartości oraz tablica z typami atrybutów
        private static Dictionary<string, KeyValuePair<double, double>> FindMinAndMaxAttributeValues(string[][] array, string[][] types)
        {
            // słownik, którego kluczem będzie nazwa atrybutu, a wartością para KeyValuePair<double, double> do przechowywania minimalnej i maxymalnej wartości
            Dictionary<string, KeyValuePair<double, double>> result = new Dictionary<string, KeyValuePair<double, double>>();

            for (int i = 0; i < types.Length; i++)
            {
                // jeżeli atrybut jest typu numerycznego (literka 'n' po spacji w pliku australian-type.txt)
                if (types[i][1] == "n")
                {
                    // wybierz z danych wartość na miejscu 'i', konwertuj tą wartość na double i całość rzutuj na listę (typ List<double>)
                    var attributeValues = array.Select(line => StringToDouble(line[i])).ToList();
                    // do wyniku dodaj nazwę atrybutu oraz parę (double, double) w której pierwszą wartością jest minimalna wartość, a drugą maxymalna
                    // https://msdn.microsoft.com/en-us/query-bi/m/list-min
                    // https://msdn.microsoft.com/en-us/query-bi/m/list-max
                    result.Add(types[i][0], new KeyValuePair<double, double>(attributeValues.Min(), attributeValues.Max()));
                }
            }

            return result;
        }

        // znalezienie różnych wartości atrybutów
        // do metody przykazywana jest tablica wczytanych wartości oraz tablica z typami atrybutów
        private static Dictionary<string, HashSet<string>> AttributeDistinctValues(string[][] array, string[][] types)
        {
            // słowników, który w kluczu będzie przychowywał nazwę atrybutu, a w wartości HastSet<string> unikalnych wartości
            Dictionary<string, HashSet<string>> result = new Dictionary<string, HashSet<string>>();

            for (int i = 0; i < types.Length; i++)
            {
                // rzutowanie tablicy string[][] na listę List<string[]> i dla każdego elementu
                array.ToList().ForEach(line =>
                {
                    // jeżeli słownik zawiera klucz o nazwie atrybutu
                    if (result.ContainsKey(types[i][0]))
                    {
                        // dodaj wartość linii na indexie 'i' do hashseta
                        result[types[i][0]].Add(line[i]);
                    }
                    else
                    {
                        // jeżeli nazwy atrybutu nie ma w słowniku to dodaj nowy hastset, który zawiera 1 wartość (aktualna wartość linii)
                        result.Add(types[i][0], new HashSet<string>()
                        {
                            line[i]
                        });
                    }
                });
            }

            return result;
        }

        // obliczenie odchylenia standardowego
        // do metody przykazywana jest tablica wczytanych wartości oraz tablica z typami atrybutów
        private static Dictionary<string, double> StandardDeviation(string[][] array, string[][] types)
        {
            // słownik: klucz - nazwa atrybutu, wartość - wartość odchylenia
            Dictionary<string, double> result = new Dictionary<string, double>();

            for (int i = 0; i < types.Length; i++)
            {
                // jeżeli atrybut jest typu numerycznego
                if (types[i][1] == "n")
                {
                    // wybierz wartości linii na indexie 'i', rzutuj je na double i na końcu stwórz listę wartości (typ List<double>)
                    var attr = array.ToList().Select(line => StringToDouble(line[i])).ToList();
                    // do słownika dodaj nazwę atrybutu (klucz) oraz wartość odchylenia (wzór na odchylenie trochę zwinięty :) )
                    // https://stackoverflow.com/a/5336708
                    // attr.Sum - sumowanie wszystkich wartości według wzoru podanego w nawiasie
                    // attr.Average - wyliczenie średniej z listy
                    // attr.Count - ilość elementów w liście
                    result.Add(types[i][0], Math.Sqrt(attr.Sum(item => Math.Pow((item - attr.Average()), 2)) / (attr.Count() - 1)));
                }
            }

            return result;
        }

        static void Main(string[] args)
        {
            string nazwaPlikuZDanymi = @"australian.txt";
            string nazwaPlikuZTypamiAtrybutow = @"australian-type.txt";

            string[][] wczytaneDane = StringToTablica(nazwaPlikuZDanymi);
            string[][] atrType = StringToTablica(nazwaPlikuZTypamiAtrybutow);

            Console.WriteLine("Dane systemu");
            string wynik = TablicaDoString(wczytaneDane);
            Console.Write(wynik);

            Console.WriteLine("");
            Console.WriteLine("Dane pliku z typami");

            string wynikAtrType = TablicaDoString(atrType);
            Console.Write(wynikAtrType);

            /****************** Miejsce na rozwiązanie *********************************/

            var classes = FindClasses(wczytaneDane);
            Console.WriteLine("Existing classes in system");
            // rzutowanie hashseta na listę i na każdego elementu wypisanie na konsoli wartości (nazwa klasy)
            classes.ToList().ForEach(c => Console.WriteLine(c));
            Console.WriteLine();

            var sizeOfClasses = SizeOfClasses(classes, wczytaneDane);
            Console.WriteLine("Size of classes");
            // rzutowanie wielkości klasy na listę i dla każdego elementu wyświetlenie na konsoli tekstu: nazwa_klasy has liczebność_klasy members np.
            // 0 has 300 members
            // na komputerach uczelnianych nie będzie to działać
            // https://docs.microsoft.com/pl-pl/dotnet/csharp/language-reference/tokens/interpolated
            // aby to działało trzeba zrobić np coś takiego
            //sizeOfClasses.ToList().ForEach(c => Console.WriteLine(c.Key + " has + "c.Value" + " members"));
            sizeOfClasses.ToList().ForEach(c => Console.WriteLine($"{c.Key} has {c.Value} members"));
            Console.WriteLine();

            var minAndMaxAttritube = FindMinAndMaxAttributeValues(wczytaneDane, atrType);
            Console.WriteLine("Min and max attribute values");
            // rzutowanie słownika na listę i dla każdego elementu wyświetlenie tekstu
            minAndMaxAttritube.ToList().ForEach(val =>
            {
                // val.Key - nazwa atrybutu
                // val.Value.Key - minimalna wartość
                // val.Value.Value - maxymalna wartość atrybutu
                Console.WriteLine($"Attribute {val.Key} min value: {val.Value.Key}");
                Console.WriteLine($"Attribute {val.Key} max value: {val.Value.Value}");
            });
            Console.WriteLine();

            var attributeDistinctValues = AttributeDistinctValues(wczytaneDane, atrType);
            // rzutowanie słownika na listę i dla każdego elementu wyświetlenie tekstu na konsoli
            // attr.Key - nazwa atrybutu
            // attr.Value.Count() - ilość wartości w hashsecie
            attributeDistinctValues.ToList().ForEach(attr => Console.WriteLine($"Attribute {attr.Key} has {attr.Value.Count()} difference values"));
            Console.WriteLine();

            // rzutowanie słownika na listę i dla każdego elementu wyświetlenie tekstu na konsoli
            // attr.Key - nazwa atrybutu
            // string.Join(", ", attr.Value) - połączenie wszystkich wartości w hashsecie znakami ", "
            attributeDistinctValues.ToList().ForEach(attr => Console.WriteLine($"Values of attribute {attr.Key}: {Environment.NewLine}{string.Join(", ", attr.Value)}"));

            var standardDeviationOfSystem = StandardDeviation(wczytaneDane, atrType);
            // rzutowanie słownika na listę i dla każdego elementu wyświetlenie nazwy atrybutu oraz wartości odchylenia
            standardDeviationOfSystem.ToList().ForEach(deviation => Console.WriteLine($"Deviation of attribute {deviation.Key} is {deviation.Value}"));
            Console.WriteLine();

            // dla każdej klasy w systemie (policzone wcześniej jakie są klasy)
            foreach (var item in classes)
            {
                // policzenie odchylenia w klasie
                // wczytaneDane.Where(line => line.Last() == item) - wybranie tych linii, których nazwa klasy (ostatni element) jest równy aktualnej klasy
                // https://msdn.microsoft.com/en-us/library/bb534803(v=vs.110).aspx
                // ToArray() - rzutowanie wyniku na tablicę string[][]
                // https://msdn.microsoft.com/en-us/library/x303t819(v=vs.110).aspx
                var standardDeviationOfClass = StandardDeviation(wczytaneDane.Where(line => line.Last() == item).ToArray(), atrType);
                Console.WriteLine($"Deviation in class {item}");
                // dokładnie to samo co wyżej
                standardDeviationOfClass.ToList().ForEach(deviation => Console.WriteLine($"Deviation of attribute {deviation.Key} is {deviation.Value}"));
                Console.WriteLine();
            }





            /****************** Koniec miejsca na rozwiązanie ********************************/
            Console.ReadKey();
        }
    }
}
