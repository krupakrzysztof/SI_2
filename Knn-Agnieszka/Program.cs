using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifierKNN
{
    class Program
    {
        internal static void Main(string[] args)
        {
            #region Files

            // plik z danymi źródłowymi
            var sourceFile = @"Files/iris.txt";
            // plik z typami atrybutów
            var attributesTypesFile = @"Files/iris-type.txt";

            #endregion

            #region LoadData - wczytanie próbek

            // wczytanie danych z pliku źródłowego
            var sourceData = FileHelper.GetData(sourceFile);
            Console.WriteLine("Dane systemu - dane źródłowe");
            ConsoleHelper.ShowData(sourceData);

            // wczytanie danych z pliku z typami atrybutów
            var attributesTypesData = FileHelper.GetData(attributesTypesFile);
            Console.WriteLine("Dane pliku z typami atrybutów");
            ConsoleHelper.ShowData(attributesTypesData);

            #endregion

            #region Klasy

            // posortowany zbiór z nazwami klas
            var classesNames = new SortedSet<string>();
            // wczytanie nazw klas z danych źródłowych
            foreach (var data in sourceData)
            {
                var className = data.DefaultIfEmpty("").LastOrDefault();
                if (!string.IsNullOrWhiteSpace(className) && !classesNames.Contains(className))
                    classesNames.Add(className);
            }
            Console.WriteLine();
            Console.WriteLine("Nazwy klas");
            ConsoleHelper.ShowData(classesNames);

            #endregion

            #region Normalizacja

            // dane w postaci liczb zmiennoprzecinkowych
            var doubleData = new double[sourceData.Length, 5];
            // znormalizowane dane
            var normalizedData = new double[sourceData.Length, 5];
            // licznik wierszy
            var rowCounter = 0;
            // iteracja po danych źródłowych
            foreach (var data in sourceData)
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        var d = Convert.ToDouble(data[i]);
                        doubleData[rowCounter, i] = d;
                        normalizedData[rowCounter, i] = d;
                    }
                    catch
                    {
                        throw new Exception("Nie powiodła się konwersja wartości do liczby o typie double");
                    }
                }
                ++rowCounter;
            }

            // normalizacja wartości wszystkich atrybutów wejściowych (wszystkich poza ostatnim)
            for (int i = 0; i < 4; i++)
            {
                // wyznaczenie min i max
                //var max = Double.NegativeInfinity;
                //var min = Double.PositiveInfinity;
                var max = 0.0;
                var min = 1.0;
                for (int j = 0; j < sourceData.Length; j++)
                {
                    var value = doubleData[j, i];
                    min = min > value ? value : min;
                    max = max < value ? value : max;
                }

                // ustawienie danych znormalizowanych na podstawie wartości minimalnych i maksymalnych
                for (int j = 0; j < sourceData.Length; j++)
                {
                    for (int k = i; k < i + 1; k++)
                    {
                        var normalizedValue = normalizedData[j, k];
                        normalizedData[j, k] = (normalizedValue - min) / (max - min);
                    }
                }
            }

            // Później dla różnych kombinacji wartości parametru k oraz metryk 
            // wyliczyć dokładność klasyfikacji algorytmem k-nn korzystając z metody 1 kontra reszta.

            var correct = 0;
            var toSort = new double[sourceData.Length, 2];

            for (int i = 0; i < sourceData.Length; i++)
            {
                var samplesTest = 0;
                for (int x = 0; x < sourceData.Length; x++)
                {
                    if (x == i)
                    {
                        toSort[x, 0] = 100000;
                    }
                    else
                    {
                        ++samplesTest;

                        toSort[x, 0] = CalculateHelper.EuclideanMetrics(normalizedData[i, 0], normalizedData[x, 0],
                            normalizedData[i, 1], normalizedData[x, 1],
                            normalizedData[i, 2], normalizedData[x, 2],
                            normalizedData[i, 3], normalizedData[x, 3]);
                        toSort[x, 1] = normalizedData[x, 4];
                    }

                    if (x == 149)
                    {
                        var k = Math.Floor(0.1 * samplesTest);
                        ConsoleHelper.ShowInfo(k);

                        var m = 150;
                        do
                        {
                            for (var n = 0; n < m - 1; n++)
                            {
                                if (toSort[n, 0] > toSort[n + 1, 0])
                                {
                                    var tmp0 = toSort[n, 0];
                                    var tmp1 = toSort[n, 1];
                                    toSort[n, 0] = toSort[n + 1, 0];
                                    toSort[n, 1] = toSort[n + 1, 1];
                                    toSort[n + 1, 0] = tmp0;
                                    toSort[n + 1, 1] = tmp1;
                                }
                            }
                            --m;
                        }
                        while (m > 1);

                        var s1 = 0;
                        var s2 = 0;
                        var s3 = 0;

                        for (var n = 0; n < k; n++)
                        {
                            switch (toSort[n, 1])
                            {
                                case 1:
                                    ++s1;
                                    break;
                                case 2:
                                    ++s2;
                                    break;
                                case 3:
                                    ++s3;
                                    break;
                            }
                        }

                        switch (normalizedData[i, 4])
                        {
                            case 1:
                                if (s1 > s2 && s1 > s3)
                                    ++correct;
                                break;
                            case 2:
                                if (s2 > s1 && s2 > s3)
                                    ++correct;
                                break;
                            case 3:
                                if (s3 > s2 && s3 > s1)
                                    ++correct;
                                break;
                        }
                    }
                }
            }

            ConsoleHelper.ShowInfo("Dane zostały znormalizowane");

            #endregion

            #region Dokładność

            // wyliczenie i przedstawienie dokładności klasyfikacji
            var accuracy = (correct / 150.0) * 100.0;
            ConsoleHelper.ShowInfo(string.Format("Dokładność klasyfikacji dla wszystkich przeprowadzonych eksperymentów: {0:F2}%", accuracy));

            #endregion


            Console.ReadKey();
        }
    }
}
