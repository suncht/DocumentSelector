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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "admin"&&this.textBox2.Text == "123")
            {
                
                Form1 form = new Form1();
                form.Show();
            }
            else 
            {
                MessageBox.Show("密码错误");
            }

             }
            
        }
    }

