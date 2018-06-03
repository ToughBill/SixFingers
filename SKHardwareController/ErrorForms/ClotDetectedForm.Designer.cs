namespace SKHardwareController
{
    partial class ClotDetectedForm
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
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnDispenseBackThenDrop = new System.Windows.Forms.Button();
            this.btnDropDiti = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(12, 12);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(165, 42);
            this.btnIgnore.TabIndex = 0;
            this.btnIgnore.Text = "忽略";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnDispenseBackThenDrop
            // 
            this.btnDispenseBackThenDrop.Location = new System.Drawing.Point(12, 60);
            this.btnDispenseBackThenDrop.Name = "btnDispenseBackThenDrop";
            this.btnDispenseBackThenDrop.Size = new System.Drawing.Size(165, 42);
            this.btnDispenseBackThenDrop.TabIndex = 1;
            this.btnDispenseBackThenDrop.Text = "打回，丢弃枪头";
            this.btnDispenseBackThenDrop.UseVisualStyleBackColor = true;
            this.btnDispenseBackThenDrop.Click += new System.EventHandler(this.btnDispenseBackThenDrop_Click);
            // 
            // btnDropDiti
            // 
            this.btnDropDiti.Location = new System.Drawing.Point(12, 108);
            this.btnDropDiti.Name = "btnDropDiti";
            this.btnDropDiti.Size = new System.Drawing.Size(165, 42);
            this.btnDropDiti.TabIndex = 2;
            this.btnDropDiti.Text = "直接丢弃枪头";
            this.btnDropDiti.UseVisualStyleBackColor = true;
            this.btnDropDiti.Click += new System.EventHandler(this.btnDropDiti_Click);
            // 
            // ClotHappened
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(189, 160);
            this.ControlBox = false;
            this.Controls.Add(this.btnDropDiti);
            this.Controls.Add(this.btnDispenseBackThenDrop);
            this.Controls.Add(this.btnIgnore);
            this.Name = "ClotHappened";
            this.Text = "检测到凝块";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Button btnDispenseBackThenDrop;
        private System.Windows.Forms.Button btnDropDiti;
    }
}