using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PNGExecutable
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opg = new OpenFileDialog();
            if (opg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = opg.FileName;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                var q = new Executable(textBox1.Text);
                q.Show();
                q.FormClosed += (r, t) => { Close(); };
                Hide();
            }
            else if (textBox2.Text != "")
            {
                var q = new FolderSearcher(textBox2.Text);
                q.Show();
                q.FormClosed += (r, t) => { Close(); };
                Hide();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog opg = new FolderBrowserDialog();
            if (opg.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = opg.SelectedPath;
                button2.Enabled = true;
            }
        }
    }
}
