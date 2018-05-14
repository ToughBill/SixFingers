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
    public partial class LiquidNotDetected : Form
    {
        public LiquidNotDetected(string title = "")
        {
            InitializeComponent();
            if(title != "")
                this.Text = title;
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            Exit(NextActionOfNoLiquid.retry);
        }

        private void btnAspirateAir_Click(object sender, EventArgs e)
        {
            Exit(NextActionOfNoLiquid.aspirateAir);
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            Exit(NextActionOfNoLiquid.abort);
        }
        private void btnSkipThisPipetting_Click(object sender, EventArgs e)
        {
            Exit(NextActionOfNoLiquid.skip);
        }
        private void Exit(NextActionOfNoLiquid nextActionOfNoLiquid)
        {
            UserSelection = nextActionOfNoLiquid;
            this.Close();
        }

        public NextActionOfNoLiquid UserSelection { get; set; }

       
    }

    public enum NextActionOfNoLiquid
    {
        retry,
        aspirateAir,
        skip,
        gotoZMax,
        abort
    }
}
