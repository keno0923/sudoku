using System;
using System.IO;
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

        FormPanelMessageBridge mesPanelBridge;
        Panel panel;

        public Form1()
        {
            InitializeComponent();
            panel = new Panel();
            mesPanelBridge = new FormPanelMessageBridge(this, panel);
            CellSize = Math.Min(pictureBox1.Width, pictureBox1.Height) / 9;
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point idx = new Point((int)(e.X / CellSize) + 1, (int)(e.Y / CellSize) + 1);
            Cell c = panel.getCell(idx);
            label1.Text = String.Format(@"({0},{1}), ({2},{3}, G{4})",
                e.X, e.Y, c.X, c.Y, c.Group);
            label2.Text = c.GetProsString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap back = new Bitmap(pictureBox1.Width, pictureBox1.Height);

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

        public void RenewImage()
        {
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                    {
                        Cell c = panel.getCell(new Point(i + 1, j + 1));
                        Brush b = (c.value == -1) ? Brushes.White : Brushes.LightGreen;
                        g.FillRectangle(b, i * CellSize + 1, j * CellSize + 1,
                            CellSize - 1, CellSize - 1);
                        String valStr = (c.value == -1) ? " " : c.value.ToString();
                        Font f = new Font(FontFamily.GenericSansSerif, 24);
                        SizeF sz = g.MeasureString(valStr, f);
                        g.DrawString(valStr.ToString(), f, Brushes.Black,
                            new PointF(i * CellSize + 1 + (CellSize - sz.Width) / 2,
                            j * CellSize + 1));
                        for (int k = 1; k <= 9; k++)
                        {
                            switch (c.flag[k])
                            {
                                case ValueFlag.Prohibited: b = Brushes.Black; break;
                                case ValueFlag.Unknown: b = Brushes.LightPink; break;
                                case ValueFlag.Decided: b = Brushes.Blue; break;
                                default: throw new NotImplementedException();
                            }
                            g.FillRectangle(b, i * CellSize + (int)(CellSize / 9 * (k - 0.5)), (int)((j + 0.8) * CellSize),
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


        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point idx = new Point((int)(e.X / CellSize) + 1, (int)(e.Y / CellSize) + 1);
            Cell c = panel.getCell(idx);
            PickForm f = new PickForm(c.flag, c.value);
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                panel.SetValue(c, f.SelectedIndex);
                RenewImage();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String FilePath = @"../data.txt";

            while( !File.Exists(FilePath))
            {
                FilePath = @"../" + FilePath;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(FilePath));
            ofd.FileName = Path.GetFileName(FilePath);
            if( ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            panel.LoadFile(ofd.FileName);
            RenewImage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel.Check();
        }

        public void RecieveMessage(String _mes)
        {
            MessageBox.Show(_mes);
        }

        public void RecieveMessage(String _mes, String _caption )
        {
            MessageBox.Show(_mes, _caption);
        }
    }
}
