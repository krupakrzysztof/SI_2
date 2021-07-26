using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_Grupowanie_1
{
    internal class Punkt
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
        /// Domyślny konstruktor klasy <see cref="Point" />
        /// </summary>
        /// <param name="x">Pierwsza wartość</param>
        /// <param name="y">Druga wartość</param>
        public Punkt(double x, double y)
        {
            X = x;
            Y = y;
        }


        /// <summary>
        /// Obliczenie odległości między grupami
        /// </summary>
        /// <param name="first">Pierwsza grupa</param>
        /// <param name="second">Druga grupa</param>
        /// <returns>Odległość</returns>
        public static double MetrykaEuklidesowa(List<Punkt> pierwszy, List<Punkt> drugi)
        {
            double odl = 0;
            // dla każdego elementu z pierwszej grupy
			foreach (var item in pierwszy)
			{
                // dla każdego elementu z drugiej grupy
				foreach (var item2 in drugi)
				{
                    // oblicz odległość między punktami wg wzoru euklidesa
                    // każdy z pierwszej grupy z każdym z drugiej grupy
                    odl += Math.Sqrt(Math.Pow(item.X - item2.X, 2) + Math.Pow(item.Y - item2.Y, 2));
				}
			}
            // podziel odległość przez ilość punktów z pierwszej grupie zsumowanych z ilością punktów w drugiej grupie
            odl /= pierwszy.Count * drugi.Count;
            return odl;
        }
    }

    class Program
    {
        /// <summary>
        /// Konwersja String to Double
        /// </summary>
        /// <param name="wartosc"></param>
        /// <returns></returns>
        static double StringToDouble(string wartosc)
        {
            double parsedValue;
            //usuwanie spacji
            wartosc = wartosc.Trim();
            //w przypadku niepowodzenia ukaze sie informacja
            if (!double.TryParse(wartosc.Replace(',', '.'), out parsedValue) && !double.TryParse(wartosc.Replace('.', ','), out parsedValue))
                throw new Exception("Nie mozna skonwertować string do double");

            return parsedValue;
        }

        /// <summary>
        /// Odczytanie danych z pliku
        /// </summary>
        /// <param name="filename">Nazwa pliku do odczytanie</param>
        /// <returns></returns>
        private static List<List<Punkt>> WczytajPunkty(string filename)
        {
            List<List<Punkt>> wynik = new List<List<Punkt>>();

            // odczytanie wszystkich linii z pliku oraz podzielenie ich po spacji oraz usunięcie pustych elementów
            foreach (var wiersz in File.ReadAllLines(filename))
            {
                var wartosci = wiersz.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Punkt punkt = new Punkt(StringToDouble(wartosci[0]), StringToDouble(wartosci[1]));
                List<Punkt> listaPunktow = new List<Punkt>();
                listaPunktow.Add(punkt);
                wynik.Add(listaPunktow);

            }

            return wynik;
        }

        static void Main(string[] args)
        {
            string filename = @"spirala.txt";

            // odczytanie danych z pliku
            List<List<Punkt>> grupy = WczytajPunkty(filename);

            do
            {
                Tuple<double, List<Punkt>, List<Punkt>> tuple = null;
                // odgleglosc miedzy grupami
                //double dystans = double.MaxValue;

                // pierwsza najblizsza grupa
                //List<Punkt> pierwszaNajblizsza = null;

                // druga najblizsza grupa
                //List<Punkt> drugaNajblizsza = null;

                for (int i = 0; i < grupy.Count; i++)
                {
                    for (int j = i + 1; j < grupy.Count; j++)
                    {
                        // obliczenie odleglosci miedzy dwiema grupami
                        var temp = Punkt.MetrykaEuklidesowa(grupy[i], grupy[j]);

                        if (tuple == null)
                        {
                            tuple = new Tuple<double, List<Punkt>, List<Punkt>>(temp, grupy[i], grupy[j]);
                        }

                        // jezeli obliczona odleglosc jest mniejsza od zapamietanej
                        if (temp < tuple.Item1)
                        {
                            tuple = new Tuple<double, List<Punkt>, List<Punkt>>(temp, grupy[i], grupy[j]);
                            //zapisz nowa najmniejsza odleglosc
                            //dystans = temp;
                            ////oraz grupy miedzy ktorymi ona byla
                            //pierwszaNajblizsza = grupy[i];
                            //drugaNajblizsza = grupy[j];
                        }
                    }
                }

                //// do pierwszej najbliższej grupy dodaj wszystkie wartości z drugiej
                //pierwszaNajblizsza.AddRange(drugaNajblizsza);
                //// usuń drugą grupę z kolekcji grup
                //grupy.Remove(drugaNajblizsza);

                tuple.Item2.AddRange(tuple.Item3);
                grupy.Remove(tuple.Item3);
                //powtarzaj dopóki liczba grup jest większa od 3 - wartosc z polecenia
            } while (grupy.Count > 3);

            // licznik nr grupy
            //int numer = 1;

            //foreach (List<Punkt> grupa in grupy)
            //{
            //    //wyswietlenie numeru grupy
            //    Console.WriteLine($"Grupa: {numer}");
            //    foreach (Punkt punkt in grupa)
            //    {
            //        //wyswietlenie zawartosci grupy
            //        Console.WriteLine("\t" + punkt.X + "\t" + punkt.Y);
            //    }
            //    //zwiekszenie licznika
            //    numer++;
            //}

            for (int i = 0; i < grupy.Count; i++)
            {
                Console.WriteLine("Grupa: {0}", i + 1);
                for (int j = 0; j < grupy[i].Count; j++)
                {
                    Console.WriteLine("\t{0}\t{1}", grupy[i][j].X, grupy[i][j].Y);
                }
            }

            Console.ReadKey();
        }
    }
}

