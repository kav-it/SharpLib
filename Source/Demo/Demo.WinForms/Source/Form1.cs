using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpLib;
using SharpLib.WinForms.Controls;

namespace Demo.Winforms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitOpenGL();
            InitHexBox();
        }

        private void InitOpenGL()
        {
            
        }

        private void InitHexBox()
        {
            hexBox1.DataSource = new HexBoxBufferDataSource(Rand.GetBuffer(1024));
        }
    }
}
