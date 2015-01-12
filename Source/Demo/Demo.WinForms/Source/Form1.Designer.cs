namespace Demo.Winforms
{
    partial class Form1
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
            this.hexBox1 = new SharpLib.WinForms.Controls.HexBox();
            this.memoControl1 = new SharpLib.WinForms.Controls.MemoControl();
            this.SuspendLayout();
            // 
            // hexBox1
            // 
            this.hexBox1.BackColor = System.Drawing.Color.White;
            this.hexBox1.Font = new System.Drawing.Font("Consolas", 10F);
            this.hexBox1.Location = new System.Drawing.Point(29, 51);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox1.Size = new System.Drawing.Size(267, 405);
            this.hexBox1.TabIndex = 0;
            // 
            // memoControl1
            // 
            this.memoControl1.BackColor = System.Drawing.Color.White;
            this.memoControl1.Font = new System.Drawing.Font("Courier New", 10F);
            this.memoControl1.Location = new System.Drawing.Point(378, 68);
            this.memoControl1.MinimumSize = new System.Drawing.Size(50, 50);
            this.memoControl1.Name = "memoControl1";
            this.memoControl1.Size = new System.Drawing.Size(341, 340);
            this.memoControl1.TabIndex = 1;
            this.memoControl1.Text = "memoControl1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1030, 611);
            this.Controls.Add(this.memoControl1);
            this.Controls.Add(this.hexBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SharpLib.WinForms.Controls.HexBox hexBox1;
        private SharpLib.WinForms.Controls.MemoControl memoControl1;



    }
}

