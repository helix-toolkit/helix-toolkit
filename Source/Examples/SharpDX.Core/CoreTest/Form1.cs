using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreTest
{
    public partial class Form1 : Form
    {
        private RenderForm renderForm;
        private CoreTestApp app;
        public Form1()
        {
            InitializeComponent();       
            renderForm = new RenderForm()
            {
                TopLevel = false, 
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            renderForm.Dock = DockStyle.Fill;
            renderForm.FormBorderStyle = FormBorderStyle.None;
            renderForm.ShowIcon = false;
            app = new CoreTestApp(renderForm);
            splitContainer1.Panel2.Controls.Add(renderForm);
            Shown += Form1_Shown;
            FormClosing += Form1_FormClosing;
            splitContainer1.Panel2.Resize += Panel2_Resize;
            renderForm.Width = splitContainer1.Panel2.Width;
            renderForm.Height = splitContainer1.Panel2.Height;
        }

        private void Panel2_Resize(object sender, EventArgs e)
        {
            Debug.WriteLine("Panel resize");
            app.RequestResize();
            renderForm.Width = splitContainer1.Panel2.Width;
            renderForm.Height = splitContainer1.Panel2.Height;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            renderForm.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderForm.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            SceneUI.SomeTextFromOutside = textBox1.Text;
        }
    }
}
