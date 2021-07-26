using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Spirala
{
    class Punkt
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static Punkt Zero => new Punkt()
        {
            X = 0,
            Y = 0
        };

        public override string ToString() => $"({X}, {Y})";
    }

    class Program
    {
        private static double ObliczOdleglosc(List<Punkt> p, List<Punkt> d)
        {
            Punkt srodekPierwszejGrupy = Punkt.Zero;
            p.ForEach(x =>
            {
                srodekPierwszejGrupy.X += x.X;
                srodekPierwszejGrupy.Y += x.Y;
            });
            srodekPierwszejGrupy = PodzielPunkt(srodekPierwszejGrupy, p.Count);

            Punkt srodekDrugiejGrupy = Punkt.Zero;
            d.ForEach(x =>
            {
                srodekDrugiejGrupy.X += x.X;
                srodekDrugiejGrupy.Y += x.Y;
            });
            srodekDrugiejGrupy = PodzielPunkt(srodekDrugiejGrupy, d.Count);

            return Math.Sqrt(
                (srodekPierwszejGrupy.X - srodekDrugiejGrupy.X) * (srodekPierwszejGrupy.X - srodekDrugiejGrupy.X) +
                (srodekPierwszejGrupy.Y - srodekDrugiejGrupy.Y) * (srodekPierwszejGrupy.Y - srodekDrugiejGrupy.Y)
                );
        }

        private static Punkt PodzielPunkt(Punkt pierwszy, int ile)
        {
            return new Punkt()
            {
                X = pierwszy.X / ile,
                Y = pierwszy.Y / ile
            };
        }

        private static void Main(string[] args)
        {
            List<List<Punkt>> grupy2 = new List<List<Punkt>>();

            var textLines = File.ReadAllLines("spirala.txt");
            foreach (var item in textLines)
            {
                var values = item.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                grupy2.Add(new List<Punkt>()
                {
                    new Punkt()
                    {
                        X = double.Parse(values[0], CultureInfo.InvariantCulture),
                        Y = double.Parse(values[1], CultureInfo.InvariantCulture)
                    }
                });
            }

            while (grupy2.Count > 3)
            {
                double minOdleglosc = double.MaxValue;
                List<Punkt> pierwszy = null;
                List<Punkt> drugi = null;
                for (int i = 0; i < grupy2.Count; i++)
                {
                    for (int j = i + 1; j < grupy2.Count; j++)
                    {
                        var p = grupy2[i];
                        var d = grupy2[j];

                        double odl = ObliczOdleglosc(p, d);
                        if (minOdleglosc > odl)
                        {
                            minOdleglosc = odl;
                            pierwszy = p;
                            drugi = d;
                        }
                    }
                }
                pierwszy.AddRange(drugi);
                grupy2.Remove(drugi);
            }

            int licznik = 1;
            grupy2.ForEach(x =>
            {
                Console.WriteLine($"Grupa {licznik}");
                x.ForEach(n => Console.WriteLine(n));
                licznik++;
            });
        }
    }
}
