using System.Collections.Generic;
using System.Linq;

namespace Apriori
{
    public static class Dodatki
    {
        public static HashSet<string> ToHashSet(this List<string> list)
        {
            HashSet<string> h = new HashSet<string>();
            list.ForEach(x => h.Add(x));
            return h;
        }

        public static HashSet<string> ToHashSet(this string[] array)
        {
            return array.ToList().ToHashSet();
        }
    }
}
