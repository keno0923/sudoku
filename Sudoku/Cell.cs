using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sudoku
{
    public enum ValueFlag
    {
        Prohibited,
        Unknown,
        Decided
    }

    public class Cell
    {
        readonly public Point index;
        public int X { get { return index.X; } }
        public int Y { get { return index.Y; } }
        readonly public int Group;
        public ValueFlag[] flag;  //  0で入らない、1で分からない、2で入ってる
        public int value
        {
            get; protected set;
        }

        public Cell(int x,int y)
        {
            index = new Point(x, y);
            flag = new List<ValueFlag>(new ValueFlag[10]).Select(t=> ValueFlag.Unknown).ToArray();
            value = -1;
            Group = ((y-1) / 3) * 3 + ( (x-1) / 3 ) + 1; 
        }

        public String GetProsString()
        {
            return flag.Aggregate<ValueFlag, String>(@"",
                (ret, val) =>
                {
                    switch (val)
                    {
                        case ValueFlag.Prohibited: return ret + "F";
                        case ValueFlag.Unknown: return ret + "?";
                        case ValueFlag.Decided: return ret + "T";
                        default: throw new NotImplementedException();
                    }
                }
                ).Substring(1);
        }

        public bool setTrueValue(int val)
        {
            if (val < 1 || val >= 10) return false;
            if (flag[val] == 0) return false;
            value = val;
            flag = flag.Select(x => ValueFlag.Prohibited).ToArray();
            flag[val] = ValueFlag.Decided;
            return true;
        }

        public bool setFalseValue(int val)
        {
            if (val < 1 || val >= 10) return false;
            if (flag[val] == ValueFlag.Decided) return false;
            flag[val] = ValueFlag.Prohibited;
            return true;
        }

    }
}
