using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PNGExecutable
{
    public partial class Executable : Form
    {
        string Files = "";
        public static byte[] PNGStart = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0d };
        public static byte[] PNGStop =  { 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82, };

        List<PNGData> images = new List<PNGData>();
        public Executable(string file)
        {
            InitializeComponent();          
            Files = file;
            textBox1.Text = file;
            textBox1.Update();

            if (Path.GetExtension(Files) == ".rcc") label6.Visible = true;
        }

        private void LoadPNG()
        {
            if (!File.Exists(Files))
            {               
                MessageBox.Show("Файл не найден", "PNG Executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            byte[] bytes = File.ReadAllBytes(Files);
            textBox2.Text = Convert.ToString(Crc32.Compute(bytes), 16).ToUpper();
            textBox2.Update();

            textBox3.Text = ((long)bytes.Length).formatFileSize();
            textBox3.Update();

            var start = bytes.SearchBytePattern(PNGStart, 0);
            var stop = bytes.SearchBytePattern(PNGStop, 0);

            numericUpDown1.Value = Math.Min(start.Count, stop.Count);
            numericUpDown1.Update();

            if (Math.Max(start.Count, stop.Count) == 0)
            {
                MessageBox.Show("Данный файл не содержит в себе изображений", "PNG Executable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            flowLayoutPanel1.Controls.Clear();
            images.Clear();
            comboBox2.Items.Clear();

            for (int i = 0; i < Math.Min(start.Count, stop.Count); i++)
            {
                int m = 4;
                try
                {
                    for (; bytes[stop[i] + PNGStop.Length + m] == 0x00; m++) ;
                }
                catch
                {
                    m = 0;
                }

                if (m > 32) stop[i] += m - 2;

                var img = bytes.Skip(start[i]).Take((stop[i] + PNGStop.Length) - start[i]).ToArray();
                images.Add(new PNGData(i, start[i], (stop[i] + PNGStop.Length), img));
                var pb = new PreviewPNG(img.ToImage(), i.ToString())
                {
                    Size = ParseFromString(comboBox1.Text),
                    //ContextMenuStrip = contextMenuStrip1,
                };
                var ia = i;
                pb.DoubleClick += (r, t) =>
                {
                    var pp = new PNGViever(images[ia]);
                    pp.UpdatePNG += (u, p) =>
                    {
                        images[u].data = p;
                        pb.Image = p.ToImage();
                    };
                    pp.Show();
                };
                pb.ChangeSelected += (q, y) =>
                {
                    for (int ip = 0; ip < flowLayoutPanel1.Controls.Count; ip++)
                    {
                        if (flowLayoutPanel1.Controls[ip] is PreviewPNG png)
                        {
                            png.Selected = false;                           
                        }
                    }
                    comboBox2.Text = (q as PreviewPNG).NameFile;
                };
                flowLayoutPanel1.Controls.Add(pb);
                comboBox2.Items.Add(i.ToString());
                flowLayoutPanel1.Update();
            }
        }



        private void Executable_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < images.Count; i++)
                {
                    File.WriteAllBytes(images[i].index + ".png", images[i].data);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            byte[] bytes = File.ReadAllBytes(Files);

            for (int i = 0; i < images.Count; i++)
            {
                images[i].data.CopyTo(bytes, images[i].start);
            }
            File.WriteAllBytes(Files, bytes);
            MessageBox.Show("Файл успешно сохранен");
            LoadPNG();
        }

        private Size ParseFromString(string str)
        {
            return new Size(int.Parse(str.Split('x').First()), int.Parse(str.Split('x').Last()));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*for (int i = 0; i < flowLayoutPanel1.Controls.Count; i++)
            {
                flowLayoutPanel1.Controls[i].Size = ParseFromString(comboBox1.Text);
            }*/
            LoadPNG();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex >= flowLayoutPanel1.Controls.Count)
            {
                comboBox2.SelectedIndex = flowLayoutPanel1.Controls.Count - 1;
                return;
            }
            (flowLayoutPanel1.Controls[comboBox2.SelectedIndex] as PreviewPNG).ChangeSelection();
            flowLayoutPanel1.ScrollControlIntoView(flowLayoutPanel1.Controls[comboBox2.SelectedIndex]);
        }

        private void Executable_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter && comboBox2.SelectedIndex >= 0)
            {
                (flowLayoutPanel1.Controls[comboBox2.SelectedIndex] as PreviewPNG).OpenPNGViever();
            }
            if (e.KeyCode == Keys.Left)
            {
                comboBox2.SelectedIndex --;
            }
            else if (e.KeyCode == Keys.Right)
            {
                comboBox2.SelectedIndex++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<Mapper> maps = new List<Mapper>();
            for (int i = 0; i < images.Count; i++)
            {
                Mapper mp = new Mapper(images[i].index, images[i].start, images[i].stop, images[i].stop - images[i].start, Files);
                maps.Add(mp);
            }
            SaveFileDialog svf = new SaveFileDialog()
            {
                Title = "Сохранение CSV файлов мапперов",
                Filter = "CSV File|*.csv",
                FileName = "Mappers from " + Path.GetFileName(Files),
            };
            if(svf.ShowDialog() == DialogResult.OK)
            {
                maps.ToArray().Save(svf.FileName);
            }
        }

        private void сохранитьВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            comboBox1.SelectedIndex = 3; //LoadPNG() тут же по событию
            comboBox2.SelectedIndex = 0;
        }
    }
    public class PNGData
    {
        public int index;
        public int start;
        public int stop;
        public byte[] data;

        public PNGData(int index, int start, int stop, byte[] data)
        {
            this.index = index;
            this.start = start;
            this.stop = stop;
            this.data = data;
        }
    }
}
