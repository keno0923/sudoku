using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class Utility
    {
        public static IEnumerable<HashSet<TData>> getCombinationChildFunc<TData>(
            HashSet<TData> arg, IEnumerable<TData> orgs, int _num)
        {
            List<HashSet<TData>> ret = new List<HashSet<TData>>();
            if (orgs.Count() == 0)
                return ret;
            if (_num == 1)
            {
                foreach (TData c in orgs)
                {
                    HashSet<TData> final = new HashSet<TData>(arg);
                    final.Add(c);
                    ret.Add(final);
                }
            }
            else
            {
                TData cNow = orgs.First();
                HashSet<TData> addedArg = new HashSet<TData>(arg);
                addedArg.Add(cNow);
                ret.AddRange(getCombinationChildFunc(addedArg, orgs.Skip(1), _num - 1));
                ret.AddRange(getCombinationChildFunc(arg, orgs.Skip(1), _num));
            }
            return ret;
        }
    }
}
