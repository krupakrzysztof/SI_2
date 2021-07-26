using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c1
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
            string trescPliku = File.ReadAllText(sciezkaDoPliku);
            string[] wiersze = trescPliku.Trim().Split(new char[] { '\n' });
            string[][] wczytaneDane = new string[wiersze.Length][];

            for (int i = 0; i < wiersze.Length; i++)
            {
                string wiersz = wiersze[i].Trim();
                string[] cyfry = wiersz.Split(new char[] { ' ' });
                wczytaneDane[i] = new string[cyfry.Length];
                for (int j = 0; j < cyfry.Length; j++)
                {
                    string cyfra = cyfry[j].Trim();
                    wczytaneDane[i][j] = cyfra;
                }
            }
            return wczytaneDane;
        }

        /// <summary>
        /// Wyszukanie minimalnej wartości poszczegolnych atrybutów numerycznych
        /// </summary>
        /// <param name="wczytaneDane">Tablica wczytanych danych</param>
        /// <param name="atrType">Tablica z typami atrybutów</param>
        /// <returns>Minimalne wartości atrybutów</returns>
        static Dictionary<string, double> MinimalnaWartoscAtrybutow(string[][] wczytaneDane, string[][] atrType)
        {
            // utworzenie słownika do przechowywania nazwy atrybutu i minimalnej wartości
            Dictionary<string, double> wartosci = new Dictionary<string, double>();

            // dla każdej linii z wczytanych danych
            foreach (var item in wczytaneDane)
            {
                // dla każdego elementu z linii
                for (int i = 0; i < item.Length - 1; i++)
                {
                    // sprawdzenie czy element w linii na indexie 'i' jest typu numerycznego
                    if (atrType[i][1] == "n")
                    {
                        // sprawdzenie czy w słowniku występuje wpis w klucz z nazwą atrybutu
                        if (wartosci.ContainsKey(atrType[i][0]))
                        {
                            // jeżeli wartość w kluczu z nazwą artybutu jest większa od aktualnie sprawdzanej wartości
                            if (wartosci[atrType[i][0]] > StringToDouble(item[i]))
                            {
                                // przypisz w kluczu z nazwą atrybutu aktualnie sprawdzaną wartość (mniejsza od poprzedniej)
                                wartosci[atrType[i][0]] = StringToDouble(item[i]);
                            }
                        }
                        // jeżeli w słowniku nie ma klucza o nazwie atrybutu
                        else
                        {
                            // dodaj do słownika nazwę atrybutu i aktualną wartość sprawdzanego elementu
                            wartosci.Add(atrType[i][0], StringToDouble(item[i]));
                        }
                    }
                }
            }

            return wartosci;
        }

        /// <summary>
        /// Wyszukanie maxymalnej wartości poszczególnych atrybutów numerycznych
        /// </summary>
        /// <param name="wczytaneDane">Tablica wczytanych danych</param>
        /// <param name="atrType">Tablica z typami atrybutów</param>
        /// <returns>Maxymalne wartości atrybutów</returns>
        static Dictionary<string, double> MaxymalnaWartoscAtrybutow(string[][] wczytaneDane, string[][] atrType)
        {
            // utworzenie słownika do przechowywania nazwy atrybutu i maxymalnej wartości
            Dictionary<string, double> wartosci = new Dictionary<string, double>();

            // dla każdej linii z wczytanych danych
            foreach (var item in wczytaneDane)
            {
                // dla każdego elementu z linii
                for (int i = 0; i < item.Length - 1; i++)
                {
                    // sprawdzenie czy element w linii na indexie 'i' jest typu numerycznego
                    if (atrType[i][1] == "n")
                    {
                        // sprawdzenie czy w słowniku występuje wpis w klucz z nazwą atrybutu
                        if (wartosci.ContainsKey(atrType[i][0]))
                        {
                            // jeżeli wartość w kluczu z nazwą artybutu jest mniejsza od aktualnie sprawdzanej wartości
                            if (wartosci[atrType[i][0]] < StringToDouble(item[i]))
                            {
                                // przypisz w kluczu z nazwą atrybutu aktualnie sprawdzaną wartość (większa od poprzedniej)
                                wartosci[atrType[i][0]] = StringToDouble(item[i]);
                            }
                        }
                        // jeżeli w słowniku nie ma klucza o nazwie atrybutu
                        else
                        {
                            // dodaj do słownika nazwę atrybutu i aktualną wartość sprawdzanego elementu
                            wartosci.Add(atrType[i][0], StringToDouble(item[i]));
                        }
                    }
                }
            }

            return wartosci;
        }

        /// <summary>
        /// Wyszukanie unikalnych wartości atrybutów
        /// </summary>
        /// <param name="wczytaneDane">Tablica wczytanych danych</param>
        /// <param name="atrType">Tablica z typami atrybutów</param>
        /// <returns>Lista unikalnych wartości atrybutów</returns>
        static Dictionary<string, List<string>> RozneAtrybuty(string[][] wczytaneDane, string[][] atrType)
        {
            // stworzenie słownika o kluczu typu string - nazwa atrybutu i wartości typu List<string> - lista wartości typu string
            Dictionary<string, List<string>> atrybuty = new Dictionary<string, List<string>>();

            // dla każdej linii z wczytanych danych
            foreach (var item in wczytaneDane)
            {
                // dla każdego elementu z linii
                for (int i = 0; i < item.Length - 1; i++)
                {
                    // sprawdzenie czy słownik nie zawiera klucza z nazwą atrybutu
                    if (!atrybuty.ContainsKey(atrType[i][0]))
                    {
                        // dodanie do słownika klucza z nazwą atrybutu i nowej listy do której dodany zostaje wartość atrybutu
                        atrybuty.Add(atrType[i][0], new List<string>()
                        {
                            item[i]
                        });
                    }
                    // jeżeli słownik zawiera w kluczu nazwę atrybutu
                    else
                    {
                        // sprawdzenie czy wartość (lista) w kluczu z nazwą atrybutu nie zawiera aktualnej wartości
                        if (!atrybuty[atrType[i][0]].Contains(item[i]))
                        {
                            // dodanie do listy wartości aktualnej wartości atrybutu
                            atrybuty[atrType[i][0]].Add(item[i]);
                        }
                    }
                }
            }

            return atrybuty;
        }

        /// <summary>
        /// Liczenie odchylenia standardowego dla listy wartości
        /// </summary>
        /// <param name="values">Lista wartości</param>
        /// <returns>Odchylenie standardowe z wartości</returns>
        // based on https://stackoverflow.com/a/5336708
        static double Odchylenie(List<double> values)
        {
            double ret = 0;

            if (values.Count > 1)
            {
                double avg = values.Average();

                double sum = values.Sum(x => Math.Pow((x - avg), 2));

                ret = Math.Sqrt(sum / (values.Count - 1));
            }

            return ret;
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

            // 3 a pierwszy -
            // utworzenie listy HashSet (przetrzymuje tylko wartości unikalne) typu string do przechwywania klas
            HashSet<string> klasy = new HashSet<string>();

            // dla każdej linii z wczytanego plik
            foreach (var item in wczytaneDane)
            {
                // dodaj do listy ostatni element z wiersza (nazwa klasy)
                // dodaje tylko wartości unikalne czyli np. 1 nie zostanie dodane dwa razy
                klasy.Add(item.Last());
            }

            Console.WriteLine("Klasy w systemie");
            // wyświetlenie wszystkich wartości (nazw klas) na konsoli
            foreach (var item in klasy)
            {
                Console.WriteLine(item);
            }


            // 3 a drugi -
            // utworzenie słownika w który Key jest typu string a Value jest typu int
            // będą tu przechwywane nazwa klasy i ilość wystąpień
            Dictionary<string, int> wielkosci = new Dictionary<string, int>();
            // dla każdej linii danych ze wczytanych dancyh
            foreach (var linia in wczytaneDane)
            {
                // jest słownik zawiera klucz o nazwie klasy
                // linia[linia.Length - 1]] zwraca ostatni element z tej tablicy
                if (wielkosci.ContainsKey(linia[linia.Length - 1]))
                {
                    // zwiększ wartość Value (typ int) o 1 w klucz linia[linia.Length - 1]]
                    wielkosci[linia[linia.Length - 1]]++;
                }
                else
                {
                    // dodanie do słownika klucza o nazwie klasy i wartości 1 - pierwsze wystąpienie danej klasy
                    wielkosci.Add(linia[linia.Length - 1], 1);
                }
            }
            Console.WriteLine("Liczebności klas decyzyjnych w systemie");
            foreach (var item in wielkosci)
            {
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            }


            // 3 a trzeci - min
            var minAtr = MinimalnaWartoscAtrybutow(wczytaneDane, atrType);
            Console.WriteLine("Minimalne wartości atrybutów");
            foreach (var item in minAtr)
            {
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            }


            // 3 a trzeci - max
            var maxAtr = MaxymalnaWartoscAtrybutow(wczytaneDane, atrType);
            Console.WriteLine("Maxymalne wartości atrybutów");
            foreach (var item in maxAtr)
            {
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            }


            // 3 b pierwszy -
            var ilosciRoznychWartosciAtrybutow = RozneAtrybuty(wczytaneDane, atrType);
            Console.WriteLine("Ilosci różnych wartości atrybutów");
            foreach (var item in ilosciRoznychWartosciAtrybutow)
            {
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            }


            // 3 b drugi -
            Console.WriteLine("Wartości różnych atrybutów");
            foreach (var item in ilosciRoznychWartosciAtrybutow)
            {
                Console.WriteLine(item.Key);
                foreach (var item2 in item.Value)
                {
                    Console.WriteLine(item2);
                }
            }


            // 3 b trzeci - dla systemu
            Console.WriteLine("Odchylenie w systemie");
            // stworzenie słownika który będzie przechowywał w kluczu nazwe atrybutu a w kluczu listę wartości typu double
            var odchylenieListaWartosci = new Dictionary<string, List<double>>();
            // dla każdej lini z wczytanych danych
            foreach (var item in wczytaneDane)
            {
                // dla każdej elementu w linii
                for (int i = 0; i < item.Length - 1; i++)
                {
                    // sprawdzenie czy element jest typu "n" w tablicy z typami atrybutów
                    if (atrType[i][1] == "n")
                    {
                        // jeżeli słownik zawiera klucz o nazwie atrybutu
                        if (odchylenieListaWartosci.ContainsKey(atrType[i][0]))
                        {
                            // dodaj do słownika w kluczu z nazwą atrybutu wartość elementu przekonwertowanego na doubla
                            odchylenieListaWartosci[atrType[i][0]].Add(StringToDouble(item[i]));
                        }
                        else
                        {
                            // do słownika dodaj nowy wpis o kluczu z nazwą atrybutu i nową listę z 1 elementem -> wartością elementu przekonwertowanego na doubla
                            odchylenieListaWartosci.Add(atrType[i][0], new List<double>()
                            {
                                StringToDouble(item[i])
                            });
                        }
                    }
                }
            }

            // słownik który przechowuje w kluczu nazwę atrybutu a w wartości policzone odchylenie dla danego atrybutu
            Dictionary<string, double> odchyleniePoliczone = new Dictionary<string, double>();
            foreach (var item in odchylenieListaWartosci)
            {
                // dodanie do słownika klucza o nazwie atrybutu i wartości, którą jest policzone odchylenie
                odchyleniePoliczone.Add(item.Key, Odchylenie(item.Value));
            }

            // dla każdego elementu wypisanie na konsoli 'Nazwa_Atrybutu - Wartosc_Odchylenia'
            foreach (var item in odchyleniePoliczone)
            {
                Console.WriteLine("{0} - {1}", item.Key, item.Value);
            }


            // 3 b trzeci - dla klas
            // utworzenie słownika, który w kluczu będzie przechowywał nazwę klasy a w wartości 2 słownik - klucz = nazwa atrybutu, wartość = lista wartości
            var odchylenieKlasWartosci = new Dictionary<string, Dictionary<string, List<double>>>();
            // dla każdej lini z wczytanych danych
            foreach (var item in wczytaneDane)
            {
                // dla każdej elementu w linii
                for (int i = 0; i < item.Length - 1; i++)
                {
                    // sprawdzenie czy element jest typu "n" w tablicy z typami atrybutów
                    if (atrType[i][1] == "n")
                    {
                        // od tego miejsca zaczyna się taka magia, że ja pierdole xD - ale działa :D
                        // sprawdzenie czy słownik zawiera klucz o nazwie klasy
                        if (odchylenieKlasWartosci.ContainsKey(item.Last()))
                        {
                            // wybranie elementu z nazwą klasy i sprawdzenie czy 2 słownika zawiera w kluczu nazwę atrybutu
                            if (odchylenieKlasWartosci[item.Last()].ContainsKey(atrType[i][0]))
                            {
                                // dodanie do słownika w kluczu z nazwą klasy elementu do słownika podrzędnego (wartość słownika głownego) w kluczu z nazwą atrybutu wartości z wiersza przekonwertowanej na doubla
                                odchylenieKlasWartosci[item.Last()][atrType[i][0]].Add(StringToDouble(item[i]));
                            }
                            else
                            {
                                // dodanenie do słownika w kluczu z nazwą klasy nowego elementu (2 słownik) nazwy atrybutu i nowej listy typu double z wartością atrybutu przekonwertowaną na doubla
                                odchylenieKlasWartosci[item.Last()].Add(atrType[i][0], new List<double>()
                                {
                                    StringToDouble(item[i])
                                });
                            }
                        }
                        // jeżeli słownik nie zawiera nazwy klasy w kluczu to
                        else
                        {
                            // dodaj nowy wpis do słownika słównego w kluczu nazwy klasu i wartości nowego słownika (podrzędny) elementów: nazwa atrybutu i listy wartości tych atrybutów
                            odchylenieKlasWartosci.Add(item.Last(), new Dictionary<string, List<double>>()
                            {
                                {
                                    atrType[i][0], new List<double>()
                                    {
                                        // wartość atrybutu przekonwertowana na double
                                        StringToDouble(item[i])
                                    }
                                }
                            });
                        }
                    }
                }
            }
            // utworzenie nowego słownika w kluczu typu string - nazwa klasy oraz wartości słownika z kluczen typu string - nazwa atrybutu i wartości typu double - wartość odchylenia
            var odchylenieKlasPoliczone = new Dictionary<string, Dictionary<string, double>>();
            // dla każdego elementu ze słownika z wartościami atrybutów w klasach
            foreach (var item in odchylenieKlasWartosci)
            {
                // dla każdego elementu ze słownika podrzędnego
                foreach (var item2 in item.Value)
                {
                    // jeżeli słownika zawiera klucz z nazwą klasy
                    if (odchylenieKlasPoliczone.ContainsKey(item.Key))
                    {
                        // dodaj do słownika w kluczu z nazwą klasy nowy element słownika (podrzędnego) nazwę atrybutu i policzone odchylenie
                        odchylenieKlasPoliczone[item.Key].Add(item2.Key, Odchylenie(item2.Value));
                    }
                    // jeżeli słownika nie zawiera w klczu nazwy klasy
                    else
                    {
                        // dodaj do niego wpis o kluczu równym nazwie klasy i wartości typu słownik o kluczu typu string - nazwa atrybutu i wartości typu double - policzona wartośc odchylenia
                        odchylenieKlasPoliczone.Add(item.Key, new Dictionary<string, double>()
                        {
                            { item2.Key, Odchylenie(item2.Value) }
                        });
                    }
                }
            }
            // dla każdego elementu ze słownika
            foreach (var item in odchylenieKlasPoliczone)
            {
                // tekst na konsoli
                Console.WriteLine("Odchylenie w klasie: " + item.Key);
                // dla każdego elementu ze słownika podrzędnego
                foreach (var item2 in item.Value)
                {
                    // wypisz na konsoli nazwe atryubut i wartość jego odchylenia
                    // item2.Key - nazwa atrybutu
                    // item2.Value - wartość odchylenia
                    Console.WriteLine("{0} - {1}", item2.Key, item2.Value);
                }
            }


            /****************** Koniec miejsca na rozwiązanie ********************************/
            Console.ReadKey();
        }
    }
}
