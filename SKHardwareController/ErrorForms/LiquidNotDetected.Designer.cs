namespace SKHardwareController
{
    partial class LiquidNotDetected
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
            this.btnSkipThisPipetting = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnAspAir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRetry
            // 
            this.btnRetry.Location = new System.Drawing.Point(12, 13);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(165, 34);
            this.btnRetry.TabIndex = 1;
            this.btnRetry.Text = "重试";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnSkipThisPipetting
            // 
            this.btnSkipThisPipetting.Location = new System.Drawing.Point(12, 53);
            this.btnSkipThisPipetting.Name = "btnSkipThisPipetting";
            this.btnSkipThisPipetting.Size = new System.Drawing.Size(165, 34);
            this.btnSkipThisPipetting.TabIndex = 2;
            this.btnSkipThisPipetting.Text = "跳过此次加样";
            this.btnSkipThisPipetting.UseVisualStyleBackColor = true;
            this.btnSkipThisPipetting.Click += new System.EventHandler(this.btnSkipThisPipetting_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(12, 133);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(165, 34);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "终止";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnAspAir
            // 
            this.btnAspAir.Location = new System.Drawing.Point(12, 93);
            this.btnAspAir.Name = "btnAspAir";
            this.btnAspAir.Size = new System.Drawing.Size(165, 34);
            this.btnAspAir.TabIndex = 4;
            this.btnAspAir.Text = "吸空气";
            this.btnAspAir.UseVisualStyleBackColor = true;
            this.btnAspAir.Click += new System.EventHandler(this.btnAspAir_Click);
            // 
            // LiquidNotDetected
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 181);
            this.ControlBox = false;
            this.Controls.Add(this.btnAspAir);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnSkipThisPipetting);
            this.Controls.Add(this.btnRetry);
            this.Name = "LiquidNotDetected";
            this.Text = "检测液面失败";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRetry;
        private System.Windows.Forms.Button btnSkipThisPipetting;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnAspAir;
    }
}