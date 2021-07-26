using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kol_zal_aproks
{
    class Program
    {
        const int liczbaProbekWzorcowych = 200;
        const int liczbaAtrybutowWejsciowych = 3;


        static void Main(string[] args)
        {
            List<List<double>> probkiWzorcowe;
            List<List<double>> probkiTestowe;
            const int liczbaProbekTestowych = 4;

            // Gerowanie Probek, poczatek 
            Random rkg;
            rkg = new Random(1234);
            probkiWzorcowe = new List<List<double>>(liczbaProbekWzorcowych);
            for (int s = 0; s < liczbaProbekWzorcowych; s++)
            {
                List<double> probka;
                double t1, t2, t3;
                t1 = rkg.NextDouble() * 2 * Math.PI;
                t2 = rkg.NextDouble() * Math.PI - Math.PI / 2.0;
                t3 = rkg.NextDouble() * (Math.PI / 2.0);
                probka = new List<double> {
                    Math.Sin(t1)*Math.Cos(t2)*Math.Cos(t3),
                    Math.Cos(t1)*Math.Cos(t2)*Math.Cos(t3),
                    Math.Sin(t2)*Math.Cos(t3),
                    Math.Sin(t3)
                };
                //Console.WriteLine(Math.Sqrt(probka[0] * probka[0] + probka[1] * probka[1] + probka[2] * probka[2] + probka[3] * probka[3]));
                probkiWzorcowe.Add(probka);
            }
            probkiTestowe = new List<List<double>>(liczbaProbekTestowych);
            for (int s = 0; s < liczbaProbekTestowych; s++)
            {
                List<double> probka;
                double t1, t2, t3;
                t1 = rkg.NextDouble() * 2 * Math.PI;
                t2 = rkg.NextDouble() * Math.PI - Math.PI / 2.0;
                t3 = rkg.NextDouble() * (Math.PI / 2.0);
                probka = new List<double> {
                    Math.Sin(t1)*Math.Cos(t2)*Math.Cos(t3),
                    Math.Cos(t1)*Math.Cos(t2)*Math.Cos(t3),
                    Math.Sin(t2)*Math.Cos(t3) //, Math.Sin(t3)
                };
                //Console.WriteLine(Math.Sqrt(probka[0] * probka[0] + probka[1] * probka[1] + probka[2] * probka[2] + probka[3] * probka[3]));
                probkiTestowe.Add(probka);
            }
            // Gerowanie Probek, koniec

            // Testowanie napisanej funkcji
            for (int s = 0; s < liczbaProbekTestowych; s++)
            {
                List<double> probka = null;
                double wynik = Double.NaN;
                double tmp = Double.NaN;
                int parametr1;
                probka = probkiTestowe[s];
                parametr1 = 2 + (s % 3);

                wynik = dajWynik(probka, probkiWzorcowe, parametr1);

                tmp = Math.Sqrt(probka[0] * probka[0] + probka[1] * probka[1] + probka[2] * probka[2] + wynik * wynik);
                Console.WriteLine(String.Format("{0}, we: {1:N3} {2:N3} {3:N3}  wy: {4:N5}  tmp: {5:N5}",
                    s, probka[0], probka[1], probka[2], wynik, tmp));
            }
            Console.ReadLine();
            return;
        }

        /// <summary>
        /// Zadaniem jest napisanie tej funkcji, proszę nie modyfikować kodu znajdującego się powyżej.
        /// </summary>
        /// <param name="probkaTestowa"> pojedyncza probka testowa o rozmiarze liczbaAtrybutowWejsciowych</param>
        /// <param name="probkiWzorcowe"> kolekcja probek wzorcowych o rozmiarze liczbaProbekWzorcowych x (liczbaAtrybutowWejsciowych+1)</param>
        /// <param name="k"> parametr algorytmu </param>
        /// <returns> wynik działania napisanego algorytmu </returns>
        static double dajWynik(List<double> probkaTestowa, List<List<double>> probkiWzorcowe, int k)
        {
            var odleglosci = new Dictionary<double, List<double>>();
            foreach (var probka in probkiWzorcowe)
            {
                var odl = ObliczOdleglosc(probkaTestowa, probka);
                odleglosci.Add(odl, probka);
            }

            var najblizsze = odleglosci.OrderBy(x => x.Key).ToList();

            double licznik = 0;
            for (int i = 0; i < k; i++)
            {
                licznik += najblizsze[i].Value[probkaTestowa.Count] / k;
            }
            double mianownik = 0;
            for (int i = 0; i < k; i++)
            {
                mianownik += 1.0 / k;
            }

            var wynik = licznik / mianownik;


            // ...
            // powodzenia
            // ...
            return wynik;
        }

        static double ObliczOdleglosc(List<double> pierwsza, List<double> druga)
        {
            double wynik = 0;

            for (int i = 0; i < pierwsza.Count; i++)
            {
                wynik += Math.Pow(pierwsza[i] - druga[i], 2);
            }
            wynik = Math.Sqrt(wynik);

            return wynik;
        }
    }
}
