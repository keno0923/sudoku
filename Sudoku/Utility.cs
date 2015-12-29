using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class Utility
    {
        public static IEnumerable<HashSet<Cell>> getCombinationChildFunc(
            HashSet<Cell> arg, IEnumerable<Cell> orgs, int _num)
        {
            List<HashSet<Cell>> ret = new List<HashSet<Cell>>();
            if (orgs.Count() == 0)
                return ret;
            if (_num == 1)
            {
                foreach (Cell c in orgs)
                {
                    HashSet<Cell> final = new HashSet<Cell>(arg);
                    final.Add(c);
                    ret.Add(final);
                }
            }
            else
            {
                Cell cNow = orgs.First();
                HashSet<Cell> addedArg = new HashSet<Cell>(arg);
                addedArg.Add(cNow);
                ret.AddRange(getCombinationChildFunc(addedArg, orgs.Skip(1), _num - 1));
                ret.AddRange(getCombinationChildFunc(arg, orgs.Skip(1), _num));
            }
            return ret;
        }
    }
}
