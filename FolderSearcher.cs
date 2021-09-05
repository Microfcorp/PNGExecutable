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
    public partial class FolderSearcher : Form
    {
        string File = "";
        public FolderSearcher()
        {
            InitializeComponent();
        }
        public FolderSearcher(string Path) : this()
        {
            File = Path;
            textBox1.Text = Path;
        }

        void SearchDir(string pth)
        {
            foreach (var item in Directory.GetFiles(pth))
            {
                progressBar1.Value++;
                byte[] bytes = System.IO.File.ReadAllBytes(item);
                var start = bytes.SearchBytePattern(Executable.PNGStart, 0);
                var stop = bytes.SearchBytePattern(Executable.PNGStop, 0);

                if(Math.Min(start.Count, stop.Count) > 0)
                {
                    numericUpDown1.Value++;
                    var ico = Path.GetExtension(item) != ".png" ? Properties.Resources.png_clipart_computer_icons_binary_file_others_text_logo1 : Properties.Resources._4595139;
                    PreviewPNG preview = new PreviewPNG(ico, Path.GetFileName(item))
                    {
                        Size = new Size(84,84),
                    };
                    preview.DoubleClick += (r, t) =>
                    {
                        var wait = new Waiting();
                        wait.Show();
                        wait.Update();

                        var pp = new Executable(item);
                        pp.Load += (l, k) => 
                        {
                            wait.Close();                        
                        };
                        pp.Show();
                    };
                    preview.ChangeSelected += (q, y) =>
                    {
                        for (int ip = 0; ip < flowLayoutPanel1.Controls.Count; ip++)
                        {
                            if (flowLayoutPanel1.Controls[ip] is PreviewPNG png)
                            {
                                png.Selected = false;
                            }
                        }
                    };
                    flowLayoutPanel1.Controls.Add(preview);
                    flowLayoutPanel1.Update();
                    numericUpDown1.Update();
                }
            }
            foreach (var item in Directory.GetDirectories(pth))
            {
                SearchDir(item);
            }
        }

        void StartSearch()
        {
            var max = Directory.GetFiles(@File, "*.*", SearchOption.AllDirectories).Length;
            progressBar1.Maximum = max;
            SearchDir(File);
        }

        private void FolderSearcher_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            StartSearch();
        }
    }
}
