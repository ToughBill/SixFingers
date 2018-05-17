namespace SKHardwareController
{
    partial class DitiNotDropped
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRetry = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRetry
            // 
            this.btnRetry.Location = new System.Drawing.Point(12, 12);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(143, 34);
            this.btnRetry.TabIndex = 0;
            this.btnRetry.Text = "重试";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(13, 52);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(143, 34);
            this.btnAbort.TabIndex = 1;
            this.btnAbort.Text = "放弃";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // DitiNotDropped
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(168, 94);
            this.ControlBox = false;
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnRetry);
            this.Name = "DitiNotDropped";
            this.Text = "枪头未能丢弃";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRetry;
        private System.Windows.Forms.Button btnAbort;
    }
}