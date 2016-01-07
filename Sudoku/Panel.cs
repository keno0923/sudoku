using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Sudoku
{
    class Panel
    {
        List<Cell> Cells;
        public Cell getCell(Point p) { return Cells[getIndex(p)]; }
        public int getIndex(Point p) { return p.X + p.Y * 9 - 10; }
        public IFormMessageBridge mesBridge { get; set; }

        private void InitializeCells()
        {
            Cells = new List<Cell>();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    Cells.Add(new Cell(j + 1, i + 1));
        }

        public Panel()
        {
            InitializeCells();
        }

        public void LoadFile(String _fn)
        {
            InitializeCells();

            using (StreamReader sr = new StreamReader(_fn))
            {
                int j = 0;
                while (!sr.EndOfStream)
                {
                    j++;
                    String str = sr.ReadLine();
                    for (int i = 0; i < 9; i++)
                    {
                        if (str[i] == 'X') continue;
                        int value = str[i] - '0';
                        SetValue(getCell(new Point(i + 1, j)), value);
                    }
                }
            }

        }

        public bool SetValueCandidates(Cell c, IEnumerable<Cell> expCells, IEnumerable<int> values)
        {
            HashSet<Cell> cs = new HashSet<Cell>(expCells);
            cs.Add(c);

            bool isModified = false;
            foreach (int v in values)
            {
                if (c.flag[v] != ValueFlag.Unknown)
                {
                    throw new ApplicationException(String.Format(
                       @"Cannot Set Candidate {0} to Cell {1}", v, c.index));
                }

                if (cs.Select(x => x.X).Distinct().Count() == 1)
                {
                    foreach (Cell tgtCell in Cells.Where(x => x.X == c.X && x.flag[v] == ValueFlag.Unknown)
                        .Except(cs))
                    {
                        tgtCell.setFalseValue(v);
                        isModified = true;
                    }
                }
                if (cs.Select(x => x.Y).Distinct().Count() == 1)
                {
                    foreach (Cell tgtCell in Cells.Where(x => x.Y == c.Y && x.flag[v] == ValueFlag.Unknown)
                        .Except(cs))
                    {
                        tgtCell.setFalseValue(v);
                        isModified = true;
                    }
                }
                if (cs.Select(x => x.Group).Distinct().Count() == 1)
                {
                    foreach (Cell tgtCell in Cells.Where(x => x.Group == c.Group && x.flag[v] == ValueFlag.Unknown)
                        .Except(cs))
                    {
                        tgtCell.setFalseValue(v);
                        isModified = true;
                    }
                }

                foreach (var p in c.flag.Select((x, idx) => new { x, idx })
                    .Where(y => !values.Contains(y.idx) && y.x == ValueFlag.Unknown))
                {
                    isModified = true;
                    c.setFalseValue(p.idx);
                }
            }
            return isModified;
        }

        public bool SetValue(Cell c, int value)
        {
            if (!c.setTrueValue(value))
            {
                throw new ApplicationException(String.Format(@"Cannot Set Value {0} to Cell {1}",
                    value, c.index));
            }

            foreach (Cell co in Cells.Where(x => x.X == c.X && !x.Equals(c)))
            {
                co.setFalseValue(value);
            }
            foreach (Cell co in Cells.Where(x => x.Y == c.Y && !x.Equals(c)))
            {
                co.setFalseValue(value);
            }
            foreach (Cell co in Cells.Where(x => x.Group == c.Group && !x.Equals(c)))
            {
                co.setFalseValue(value);
            }
            return true;
        }

        private bool DecideByCellDirectly()
        {
            foreach (Cell c in Cells)
            {
                if (c.flag.Count(x => x == ValueFlag.Unknown) == 1)
                {
                    int j = c.flag.Select((x, i) => new { x, i }).Skip(1)
                        .Single(y => y.x == ValueFlag.Unknown).i;
#if DEBUG
                    MessageBox.Show(c.index.ToString() + @" To " + j.ToString(), @"直接判定");
#endif
                    SetValue(c, j);
                    mesBridge.SendCommandToForm(enumFormMessageCommand.RenewImage);
                    mesBridge.SendCommandToForm(enumFormMessageCommand.RenewImage);;
                    return true;
                }
            }
            return false;
        }


        enum CheckDirection { X, Y, G }


        /// <summary>
        /// 周囲の決定状況から特定のマスにしか入らない数字を探す
        /// </summary>
        /// <param name="_d">縦・横・グループ</param>
        /// <param name="depth">検索の深さ（未実装）</param>
        /// <returns></returns>
        private bool DecideByBackwardDirection(CheckDirection _d, int depth)
        {
            IEnumerable<Cell> cs;
            for (int i = 1; i <= 9; i++)
            {
                switch (_d)
                {
                    case CheckDirection.X: cs = Cells.Where(x => x.Y == i); break;
                    case CheckDirection.Y: cs = Cells.Where(x => x.X == i); break;
                    case CheckDirection.G: cs = Cells.Where(x => x.Group == i); break;
                    default: throw new NotImplementedException();
                }
                for (int j = 1; j <= 9; j++)
                {
                    if (cs.Any(x => x.flag[j] == ValueFlag.Decided))
                        continue;
                    IEnumerable<Cell> csj = cs.Where(x => x.flag[j] == ValueFlag.Unknown);
                    if (csj.Count() == depth)
                    {
                        Cell c = csj.Single();
#if DEBUG
                        MessageBox.Show(c.index.ToString() + @" To " + j.ToString(), @"自動確認");
#endif
                        SetValue(c, j);
                        mesBridge.SendCommandToForm(enumFormMessageCommand.RenewImage);;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Check()
        {
            while (true)
            {
                if (CheckByReservingProcess(CheckDirection.G)) continue;
                if (DecideByCellDirectly()) continue;
                if (DecideByBackwardDirection(CheckDirection.X, 1)) continue;
                if (DecideByBackwardDirection(CheckDirection.Y, 1)) continue;
                if (DecideByBackwardDirection(CheckDirection.G, 1)) continue;
                break;
            }
            if (Cells.All(x => x.flag.Any(y => y == ValueFlag.Decided)))
                mesBridge.SendMessageToForm(@"全てのセルの値が決まりました。");
            else
                mesBridge.SendMessageToForm(@"機械的に決められるセルはありません。");

        }

        private bool CheckByReservingProcess(CheckDirection _d)
        {
            IEnumerable<Cell> cs;
            for (int i = 1; i <= 9; i++)
            {
                switch (_d)
                {
                    case CheckDirection.X: cs = Cells.Where(x => x.Y == i); break;
                    case CheckDirection.Y: cs = Cells.Where(x => x.X == i); break;
                    case CheckDirection.G: cs = Cells.Where(x => x.Group == i); break;
                    default: throw new NotImplementedException();
                }
                int depth = 2;
                var cands = Enumerable.Range(1, 9);

                foreach (HashSet<int> pair in Utility.getCombinationChildFunc(
                    new HashSet<int>(), cands, depth))
                {
                    var s = cs.Where(x => x.flag[pair.First()] == ValueFlag.Unknown);
                    if (s.Count() != depth) continue;
                    bool isEqual = true;
                    foreach (int val in pair.Skip(1))
                    {
                        var t = cs.Where(x => x.flag[val] == ValueFlag.Unknown);
                        isEqual &= s.SequenceEqual(t);
                    }
                    if (isEqual)
                    {
                        bool isModified = false;
                        HashSet<Cell> elim = new HashSet<Cell>(cs.Except(s));
                        foreach (Cell c in s)
                        {
                            isModified |= SetValueCandidates(c, s.Except(Enumerable.Repeat(c, 1)), pair);
                        }

                        if (isModified)
                        {
#if DEBUG
                            MessageBox.Show(String.Format(@"Eliminate {0}&{1} by {2},{3}",
                                 pair.First(), pair.Last(), s.First().index.ToString(), s.Last().index.ToString()));
#endif
                            mesBridge.SendCommandToForm(enumFormMessageCommand.RenewImage);;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
