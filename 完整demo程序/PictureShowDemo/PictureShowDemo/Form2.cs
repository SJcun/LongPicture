using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PictureShowDemo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void AddControlsToPanel(Control c, Panel panel)
        {
            c.Dock = DockStyle.Fill;
            panel.Controls.Add(c);

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            UCShowLongPicture longPicture = new UCShowLongPicture(MainPanel.Width, MainPanel.Height);
            longPicture.SetImageParameter(@"C:\Users\admin\Pictures\Saved Pictures\test.jpg", 400, 300);
            AddControlsToPanel(longPicture, MainPanel);
        }
    }
}
