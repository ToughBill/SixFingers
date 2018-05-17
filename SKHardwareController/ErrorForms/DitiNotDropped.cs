using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKHardwareController
{
    public partial class DitiNotDropped : Form
    {
        public DitiNotDropped()
        {
            InitializeComponent();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            Exit(DitiNotDroppedAction.retry);
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            Exit(DitiNotDroppedAction.abort);
        }
        private void Exit(DitiNotDroppedAction nextActionOfDitiNotDropped)
        {
            UserSelection = nextActionOfDitiNotDropped;
            this.Close();
        }


        public DitiNotDroppedAction UserSelection { get; set; }
    }


    public enum DitiNotDroppedAction
    {
        retry,
        abort
    }
}
