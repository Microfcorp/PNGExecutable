using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PNGExecutable
{
    public partial class PreviewPNG : UserControl
    {

        public new event EventHandler DoubleClick;

        public event EventHandler ChangeSelected;

        public Image Image
        {
            get => pictureBox1.Image;
            set => pictureBox1.Image = value;
        }

        public string NameFile
        {
            get => label1.Text;
            set => label1.Text = value;
        }
        public bool Selected 
        { 
            get => selected;
            set
            {                
                selected = value;
                if (value)
                {
                    BackColor = SystemColors.ActiveCaption;
                }
                else
                {
                    BackColor = SystemColors.Control;
                }             
            }
        }

        public PreviewPNG()
        {
            InitializeComponent();
            pictureBox1.DoubleClick += (o,e) => DoubleClick?.Invoke(o,e);
            label1.DoubleClick += (o,e) => DoubleClick?.Invoke(o,e);
            pictureBox1.ContextMenuStrip = ContextMenuStrip;
            label1.ContextMenuStrip = ContextMenuStrip;
            //this.DoubleClick += (o, e) => DoubleClick?.Invoke(o, e);
        }

        private bool selected;

        public PreviewPNG(Image img, string title) : this()
        {
            Image = img;
            NameFile = title;
        }

        private void PreviewPNG_Load(object sender, EventArgs e)
        {

        }

        private void PreviewPNG_Click(object sender, EventArgs e)
        {
            ChangeSelection();
        }

        public void OpenPNGViever()
        {
            DoubleClick?.Invoke(this, new EventArgs());
        }

        public void ChangeSelection(bool IsActive = true)
        {
            if(IsActive) ChangeSelected?.Invoke(this, new EventArgs());
            Selected = IsActive;
        }
    }
}
