using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PNGExecutable
{
    public partial class PNGViever : Form
    {
        private readonly int index;
        private readonly int stopC;
        private readonly byte[] data;

        public int StartC { get; }

        public delegate void UpdatesPNG(int index, byte[] data);
        public event UpdatesPNG UpdatePNG;

        public PNGViever(PNGData data)
        {
            InitializeComponent();

            label2.Text = data.index.ToString();
            label7.Text = data.start.ToString();
            label8.Text = data.stop.ToString();
            label9.Text = (data.stop - data.start).ToString();
            label10.Text = ((long)(data.stop - data.start)).formatFileSize();
            pictureBox1.Image = data.data.ToImage();
            label15.Text = pictureBox1.Image.Width.ToString();
            label16.Text = pictureBox1.Image.Height.ToString();
            this.index = data.index;
            this.StartC = data.start;
            stopC = data.stop;
            this.data = data.data;
            label18.Text = pictureBox1.Image.AverageBrightnes().ToString();          
        }

        private void PNGViever_Load(object sender, EventArgs e)
        {
            IsNightTheme = data.ToImage().IsHighBrightnes();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog svf = new SaveFileDialog();
            svf.Filter = "PNG File|*.png";
            if(svf.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(svf.FileName, data);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog svf = new OpenFileDialog();
            svf.Filter = "PNG File|*.png";
            if (svf.ShowDialog() == DialogResult.OK)
            {
                var newf = File.ReadAllBytes(svf.FileName);
                if(newf.Length > (stopC - StartC))
                {
                    MessageBox.Show("Загружаемое изображение должно весить меньше или равным исходному");
                    return;
                }                
                label12.Text = Math.Abs(newf.Length - (stopC - StartC)).ToString();

                for (int i = 0; i < data.Length; i++)
                    data[i] = 0x00;

                /*newf.Take(newf.Length - Executable.PNGStop.Length).ToArray().CopyTo(data, 0);


                /*for (int i = 1; i < Math.Abs(newf.Length - (stopC - StartC)) - Executable.PNGStop.Length; i++)
                    data[newf.Length + i] = 0x00;*/

                /*Executable.PNGStop.CopyTo(data, data.Length - Executable.PNGStop.Length);*/

                newf.CopyTo(data, 0);

                pictureBox1.Image = data.ToImage();
                label15.Text = pictureBox1.Image.Width.ToString();
                label16.Text = pictureBox1.Image.Height.ToString();
                pictureBox2.Enabled = false;
                button2.Text += " (Измененное)";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdatePNG?.Invoke(index, data);
            Close();
        }

        private void CopyToClipboard(object sender, EventArgs e)
        {
            var c = sender as Control;
            Clipboard.SetText(c.Text);
            MessageBox.Show("Скопировано в буфер обмена");
        }

        public bool IsNightTheme
        {
            get
            {
                return pictureBox2.Name == "Night";
            }
            set
            {
                if (value)
                {
                    pictureBox2.Name = "Night";
                    pictureBox2.Image = Properties.Resources.месяц;
                    pictureBox1.Image = data.ToImage().ToNight();
                }
                else
                {
                    pictureBox2.Name = "Sun";
                    pictureBox2.Image = Properties.Resources.солнце;
                    pictureBox1.Image = data.ToImage();
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            IsNightTheme = !IsNightTheme;
        }

        private void PNGViever_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
        }
    }
}
