using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apriori
{
    class Program
    {
        static int Prog => 2;

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

        static string[][] StringToTablica(string sciezkaDoPliku)
        {
            string trescPliku = System.IO.File.ReadAllText(sciezkaDoPliku); // wczytujemy treœæ pliku do zmiennej
            string[] wiersze = trescPliku.Trim().Split(new char[] { '\n' }); // treœæ pliku dzielimy wg znaku koñca linii, dziêki czemu otrzymamy ka¿dy wiersz w oddzielnej komórce tablicy
            string[][] wczytaneDane = new string[wiersze.Length][];   // Tworzymy zmienn¹, która bêdzie przechowywa³a wczytane dane. Tablica bêdzie mia³a tyle wierszy ile wierszy by³o z wczytanego poliku

            for (int i = 0; i < wiersze.Length; i++)
            {
                string wiersz = wiersze[i].Trim();     // przypisujê i-ty element tablicy do zmiennej wiersz
                string[] cyfry = wiersz.Split(new char[] { ' ' });   // dzielimy wiersz po znaku spacji, dziêki czemu otrzymamy tablicê cyfry, w której ka¿da oddzielna komórka to czyfra z wiersza
                wczytaneDane[i] = new string[cyfry.Length];    // Do tablicy w której bêd¹ dane finalne dok³adamy wiersz w postaci tablicy integerów tak d³ug¹ jak d³uga jest tablica cyfry, czyli tyle ile by³o cyfr w jednym wierszu
                for (int j = 0; j < cyfry.Length; j++)
                {
                    string cyfra = cyfry[j].Trim(); // przypisujê j-t¹ cyfrê do zmiennej cyfra
                    wczytaneDane[i][j] = cyfra;
                }
            }
            return wczytaneDane;
        }

        private static List<List<string>> ZnajdzZbioryF(string[][] paragon, List<List<string>> zbiorC)
        {
            List<List<string>> zbiorF = new List<List<string>>();
            Dictionary<List<string>, int> ilosc = new Dictionary<List<string>, int>();

            foreach (var elementZbioru in zbiorC)
            {
                foreach (var elementParagonu in paragon)
                {
                    if (elementZbioru.ToHashSet().IsSubsetOf(elementParagonu))
                    {
                        var test = paragon.Where(x => x.Intersect(elementZbioru).Count() >= elementZbioru.Count).ToList();

                        if (!ilosc.ContainsKey(elementZbioru))
                        {
                            ilosc.Add(elementZbioru, test.Count);
                        }
                        break;
                    }
                }
            }

            foreach (var item in ilosc.Where(item => item.Value >= Prog))
            {
                zbiorF.Add(item.Key);
            }

            return zbiorF;
        }

        private static List<List<string>> ZnajdzZbioryC(int poziom, string[][] paragon, List<List<string>> zbiorC_1)
        {
            List<List<string>> zbiorC = new List<List<string>>();

            for (int i = 0; i < zbiorC_1.Count; i++)
            {
                for (int j = i + 1; j < zbiorC_1.Count; j++)
                {
                    List<string> wspolne = zbiorC_1[i].Intersect(zbiorC_1[j]).ToList();
                    if (wspolne.Count() == poziom - 2)
                    {
                        List<string> polaczone = zbiorC_1[i].Union(zbiorC_1[j]).OrderBy(x => x).ToList();
                        if (zbiorC.All(x => !x.SequenceEqual(polaczone)))
                        {
                            zbiorC.Add(polaczone);
                        }
                    }
                }
            }

            return zbiorC;
        }

        static void Main(string[] args)
        {
            string sciezkaDoParagonu = @"paragon-system.txt";


            string[][] paragon = StringToTablica(sciezkaDoParagonu);

            Console.WriteLine("Dane paragonu");
            string wynikParagon = TablicaDoString(paragon);
            Console.Write(wynikParagon);


            /****************** Miejsce na rozwi¹zanie *********************************/
            var aktualnyPoziom = 1;

            List<List<string>> c1 = new List<List<string>>();
            List<string> c1_tmp = new List<string>();
            foreach (var item in paragon)
            {
                foreach (var item2 in item)
                {
                    if (!c1_tmp.Contains(item2))
                    {
                        c1_tmp.Add(item2);
                    }
                }
            }

            c1_tmp.ForEach(item => c1.Add(new List<string>()
            {
                item
            }));

            Dictionary<List<List<string>>, int> zbioryC = new Dictionary<List<List<string>>, int>()
            {
                { c1, aktualnyPoziom }
            };

            Dictionary<List<List<string>>, int> zbioryF = new Dictionary<List<List<string>>, int>();

            do
            {
                var zbiorf = ZnajdzZbioryF(paragon, zbioryC.Last().Key);
                foreach (var item in zbiorf)
                {
                    Console.WriteLine(string.Join(" i ", item));
                }
                zbioryF.Add(zbiorf, aktualnyPoziom);

                aktualnyPoziom++;
                var zbiorC = ZnajdzZbioryC(aktualnyPoziom, paragon, zbiorf);
                foreach (var item in zbiorC)
                {
                    Console.WriteLine(string.Join(" i ", item));
                }
                zbioryC.Add(zbiorC, aktualnyPoziom);
            } while (zbioryF.Last().Key.Count >= 2);



            var r = new List<KeyValuePair<KeyValuePair<List<string>, string>, double>>();
            foreach (var item in new[] { 1, 2, 3, 4 })
            {
                foreach (var zbiorF in zbioryF)
                {
                    if (zbiorF.Value > 1)
                    {
                        foreach (var re in zbiorF.Key)
                        {
                            double l = paragon.Count(par => re.ToHashSet().IsSubsetOf(par));
                            double w = l / paragon.Count();
                            foreach (var st in re)
                            {
                                List<string> akt = new List<string>(re);
                                akt.Remove(st);
                                double u = l / paragon.Count(par => akt.ToHashSet().IsSubsetOf(par));
                                double ufn = w * u;
                                if (ufn >= item / 10)
                                {
                                    r.Add(new KeyValuePair<KeyValuePair<List<string>, string>, double>(new KeyValuePair<List<string>, string>(re, st), ufn));
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Regu³y dla: " + 0.1);
            foreach (var item in r.Where(re => re.Value >= 0.1))
            {
                Console.WriteLine(string.Join(" i ", item.Key.Key) + " to " + item.Key.Value + " : " + item.Value);
            }

            Console.WriteLine("Regu³y dla: " + 0.2);
            foreach (var item in r.Where(re => re.Value >= 0.2))
            {
                Console.WriteLine(string.Join(" i ", item.Key.Key) + " to " + item.Key.Value + " : " + item.Value);
            }

            Console.WriteLine("Regu³y dla: " + 0.3);
            foreach (var item in r.Where(re => re.Value >= 0.3))
            {
                Console.WriteLine(string.Join(" i ", item.Key.Key) + " to " + item.Key.Value + " : " + item.Value);
            }

            Console.WriteLine("Regu³y dla: " + 0.4);
            foreach (var item in r.Where(re => re.Value >= 0.4))
            {
                Console.WriteLine(string.Join(" i ", item.Key.Key) + " to " + item.Key.Value + " : " + item.Value);
            }

            /****************** Koniec miejsca na rozwi¹zanie ********************************/
            Console.ReadLine();
        }
    }
}
