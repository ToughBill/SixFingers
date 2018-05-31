using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WTPipetting.StageControls
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }


        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x112;
            const int SC_CLOSE = 0xf060;
            if(m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                this.Visible = false;
                return;
            }
            base.WndProc(ref m);
        }
        public void AddLog(string txt)
        {
            txtLog.Text += txt;
            txtLog.Text += "\r\n";
        }
    }
}
