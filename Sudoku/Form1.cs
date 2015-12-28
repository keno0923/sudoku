using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        readonly public int CellSize;

        List<Cell> Cells = new List<Cell>();
        Cell getCell(Point p) { return Cells[getIndex(p)]; }
        int getIndex(Point p) { return p.X + p.Y * 9 - 10; } 

        public Form1()
        {
            InitializeComponent();
            CellSize = Math.Min(pictureBox1.Width, pictureBox1.Height) / 9;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point idx = new Point((int)(e.X / CellSize) + 1, (int)(e.Y / CellSize) + 1);
            Cell c = getCell(idx);
            label1.Text = String.Format(@"({0},{1}), ({2},{3}, G{4})",
                e.X, e.Y, c.index.X, c.index.Y, c.Group);
            label2.Text = c.GetProsString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap back = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    Cells.Add(new Cell(j+1, i+1));

            using (Graphics g = Graphics.FromImage(back))
            {
                g.FillRectangle(Brushes.White, 0, 0, back.Width, back.Height);
                Pen pen = new Pen(Brushes.Black, 1);
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        g.DrawRectangle(pen, i * CellSize, j * CellSize, CellSize, CellSize);
                pen.Width = 5;
                for (int i = 0; i < 9; i += 3)
                    for (int j = 0; j < 9; j += 3)
                        g.DrawRectangle(pen, i * CellSize, j * CellSize, CellSize * 3, CellSize * 3);
            }
            pictureBox1.Image = back;
            RenewImage();
        }

        private void RenewImage()
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                    {
                        Cell c = getCell(new Point(i + 1, j + 1));
                        Brush b = (c.value == -1) ? Brushes.White : Brushes.LightGreen;
                        g.FillRectangle(b, i * CellSize + 1, j * CellSize + 1,
                            CellSize - 1, CellSize - 1);
                        String valStr = (c.value == -1) ? " " : c.value.ToString();
                        Font f = new Font(FontFamily.GenericSansSerif, 24);
                        SizeF sz = g.MeasureString(valStr, f);
                        g.DrawString(valStr.ToString(), f, Brushes.Black,
                            new PointF(i * CellSize + 1 + (CellSize - sz.Width) / 2,
                            j * CellSize + 1));
                        for( int k=1; k <=9; k++)
                        {
                            switch (c.pros[k])
                            {
                                case 0: b = Brushes.Black; break;
                                case 1: b = Brushes.LightPink; break;
                                case 2: b = Brushes.Blue; break;
                                default: throw new NotImplementedException();
                            }
                            g.FillRectangle(b, i * CellSize + (int)( CellSize / 9 * ( k - 0.5) ), (int)((j+0.8) * CellSize),
                                CellSize / 9, CellSize / 5);
                        }
                    }

                Pen pen = new Pen(Brushes.Black, 5);
                for (int i = 0; i < 9; i += 3)
                    for (int j = 0; j < 9; j += 3)
                        g.DrawRectangle(pen, i * CellSize, j * CellSize, CellSize * 3, CellSize * 3);
            }

            pictureBox1.Refresh();

        }

        public bool SetValue(Cell c, int value)
        {
            if (!c.setTrueValue(value))
            {
                MessageBox.Show("エラーが発生しました。");
                return false;
            }

            foreach (Cell co in Cells.Where(x => x.index.X == c.index.X && !x.Equals(c)))
            {
                co.setFalseValue(value);
            }
            foreach (Cell co in Cells.Where(x => x.index.Y == c.index.Y && !x.Equals(c)))
            {
                co.setFalseValue(value);
            }
            foreach (Cell co in Cells.Where(x => x.Group == c.Group && !x.Equals(c)))
            {
                co.setFalseValue(value);
            }
            return true;
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point idx = new Point((int)(e.X / CellSize) + 1, (int)(e.Y / CellSize) + 1);
            Cell c = getCell(idx);
            PickForm f = new PickForm(c.pros, c.value );
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                SetValue(c, f.SelectedIndex);
                RenewImage();
            }
        }
    }
}
