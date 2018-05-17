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
    public partial class TipNotFetched : Form
    {
        public TipNotFetched()
        {
            InitializeComponent();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            Exit(NextActionOfNoTip.abort);
        }

        private void Exit(NextActionOfNoTip nextActionOfNoTip)
        {
            UserSelection = nextActionOfNoTip;
            this.Close();
        }

        private void btnRetryNextPosition_Click(object sender, EventArgs e)
        {
            Exit(NextActionOfNoTip.retryNextPosition);
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            Exit(NextActionOfNoTip.retry);
        }

        public NextActionOfNoTip UserSelection { get; set; }
    }

    public enum NextActionOfNoTip
    {
        retry,
        retryNextPosition,
        abort
    }
}
