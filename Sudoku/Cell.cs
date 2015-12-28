using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sudoku
{
    public class Cell
    {
        readonly public Point index;
        readonly public int Group;
        public int[] pros;  //  0で入らない、1で分からない、2で入ってる
        public int value
        {
            get; protected set;
        }

        public Cell(int x,int y)
        {
            index = new Point(x, y);
            pros = new List<int>(new int[10]).Select(t=>1).ToArray();
            value = -1;
            Group = ((y-1) / 3) * 3 + ( (x-1) / 3 ) + 1; 
        }

        public String GetProsString()
        {
            return pros.Aggregate<int, String>(@"",
                (ret, val) =>
                {
                    switch (val)
                    {
                        case 0: return ret + "F";
                        case 1: return ret + "?";
                        case 2: return ret + "T";
                    }
                    return ret;
                }
                ).Substring(1);
        }

        public bool setTrueValue(int val)
        {
            if (val < 1 || val >= 8) return false;
            if (pros[val] == 0) return false;
            value = val;
            pros = pros.Select(x => 0).ToArray();
            pros[val] = 2;
            return true;
        }

        public bool setFalseValue(int val)
        {
            if (val < 1 || val >= 8) return false;
            if (pros[val] == 2) return false;
            pros[val] = 0;
            return true;
        }

    }
}
