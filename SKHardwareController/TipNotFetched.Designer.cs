namespace SKHardwareController
{
    partial class TipNotFetched
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
            this.btnRetryNextPosition = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRetry
            // 
            this.btnRetry.Location = new System.Drawing.Point(12, 12);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(165, 34);
            this.btnRetry.TabIndex = 0;
            this.btnRetry.Text = "原位置重试";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnRetryNextPosition
            // 
            this.btnRetryNextPosition.Location = new System.Drawing.Point(12, 52);
            this.btnRetryNextPosition.Name = "btnRetryNextPosition";
            this.btnRetryNextPosition.Size = new System.Drawing.Size(165, 34);
            this.btnRetryNextPosition.TabIndex = 1;
            this.btnRetryNextPosition.Text = "下一位置重试";
            this.btnRetryNextPosition.UseVisualStyleBackColor = true;
            this.btnRetryNextPosition.Click += new System.EventHandler(this.btnRetryNextPosition_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(12, 92);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(165, 34);
            this.btnAbort.TabIndex = 2;
            this.btnAbort.Text = "放弃";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // TipNotFetched
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(191, 138);
            this.ControlBox = false;
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnRetryNextPosition);
            this.Controls.Add(this.btnRetry);
            this.Name = "TipNotFetched";
            this.Text = "未取到枪头";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRetry;
        private System.Windows.Forms.Button btnRetryNextPosition;
        private System.Windows.Forms.Button btnAbort;
    }
}