using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DocumentSelector
{
    public partial class ImageForm : Form
    {
        public ImageForm()
        {
            InitializeComponent();
        }
        private Bitmap bmp = null;
        public void setImage(string filepath)
        {
            try
            {
                this.textBox1.Text = filepath;
                bmp = new Bitmap(filepath);
                Point ptLoction = new Point(bmp.Size);
                if (ptLoction.X > this.pictureBox1.Size.Width || ptLoction.Y > this.pictureBox1.Size.Height)
                {
                    //图像框的停靠方式   
                    //pcbPic.Dock = DockStyle.Fill;   
                    //图像充滿图像框，並且图像維持比例   
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    //图像在图像框置中   
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                }

                //LoadAsync：非同步转入图像   
                this.pictureBox1.LoadAsync(filepath);
            }
            catch (Exception ex)
            {
            }
        }

        private void ImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bmp != null) bmp.Dispose();
            this.pictureBox1.Image = null;
        }

        private void ImageForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
