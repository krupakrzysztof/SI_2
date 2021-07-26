using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knn
{
    class Iris
    {
        public double sepal_length;
        public double sepal_width;
        public double petal_length;
        public double petal_width;
        /// <summary>
        /// Nazwa klasy
        /// </summary>
        // jest to poprzedzone znakiem @ ponieważ class to słowo kluczowe w C# i nie można do użyć jako nazwy zmiennej
        public string @class;

        /// <summary>
        /// Kontruktor isysa, który ustawi jego wartości
        /// </summary>
        /// <param name="sepal_length">Parametr 1</param>
        /// <param name="sepal_width">Parametr 2</param>
        /// <param name="petal_length">Parametr 3</param>
        /// <param name="petal_width">Parametr 4</param>
        /// <param name="class">Nazwa klasy</param>
        public Iris(double sepal_length, double sepal_width, double petal_length, double petal_width, string @class)
        {
            this.sepal_length = sepal_length;
            this.sepal_width = sepal_width;
            this.petal_length = petal_length;
            this.petal_width = petal_width;
            this.@class = @class;
        }
    }
}
