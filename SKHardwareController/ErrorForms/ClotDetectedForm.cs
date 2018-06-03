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
    public partial class ClotDetectedForm : Form
    {
        public ClotDetectedForm()
        {
            InitializeComponent();
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            Exit(ClotDetectedAction.ignore);
        }

        private void btnDispenseBackThenDrop_Click(object sender, EventArgs e)
        {
            Exit(ClotDetectedAction.dispenseBackThenDropDiti);
        }

        private void btnDropDiti_Click(object sender, EventArgs e)
        {
            Exit(ClotDetectedAction.dropDiti);
        }
        private void Exit(ClotDetectedAction nextActionOfClotDetect)
        {
            UserSelection = nextActionOfClotDetect;
            this.Close();
        }
        public ClotDetectedAction UserSelection { get; set; }
    }

    public enum ClotDetectedAction
    {
        ignore,
        dispenseBackThenDropDiti,
        dropDiti
    }
}
