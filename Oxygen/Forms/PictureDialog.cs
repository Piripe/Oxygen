using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oxygen.Forms
{
    public partial class PictureDialog : Form
    {
        public PictureDialog(string ImageURL)
        {
            InitializeComponent();
            pictureBox1.ImageLocation = ImageURL;
        }
    }
}
