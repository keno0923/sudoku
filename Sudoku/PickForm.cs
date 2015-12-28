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
    public partial class PickForm : Form
    {
        public int SelectedIndex
        {
            get {
                int val;
                if (int.TryParse(comboBox1.Text, out val))
                    return val;
                else return -1;
            }
        }

        public PickForm(ValueFlag[] b, int val)
        {
            InitializeComponent();
            comboBox1.Items.Add("None");
            for (int i = 1; i <= 9; i++)
                if (b[i] != ValueFlag.Prohibited)
                    comboBox1.Items.Add(i);
            if( val < 1 || 9 <= val)
                comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
