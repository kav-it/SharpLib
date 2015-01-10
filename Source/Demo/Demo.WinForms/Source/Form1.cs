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
            // var data = Mem.Fill(1024, 0xDC);
            var data = Rand.GetBuffer(16 * 1024);
            hexBox1.DataSource = new HexBoxBufferDataSource(data);
        }
    }
}
