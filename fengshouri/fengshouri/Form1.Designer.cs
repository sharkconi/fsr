namespace fengshouri
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
            this.label1 = new System.Windows.Forms.Label();
            this.xmlButton = new System.Windows.Forms.Button();
            this.pzTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pzdateTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "凭证文件：";
            // 
            // xmlButton
            // 
            this.xmlButton.Location = new System.Drawing.Point(355, 59);
            this.xmlButton.Name = "xmlButton";
            this.xmlButton.Size = new System.Drawing.Size(75, 23);
            this.xmlButton.TabIndex = 1;
            this.xmlButton.Text = "生成XML";
            this.xmlButton.UseVisualStyleBackColor = true;
            this.xmlButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // pzTextBox
            // 
            this.pzTextBox.Location = new System.Drawing.Point(94, 61);
            this.pzTextBox.Name = "pzTextBox";
            this.pzTextBox.Size = new System.Drawing.Size(255, 20);
            this.pzTextBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "凭证日期";
            // 
            // pzdateTextBox
            // 
            this.pzdateTextBox.Location = new System.Drawing.Point(94, 38);
            this.pzdateTextBox.Name = "pzdateTextBox";
            this.pzdateTextBox.Size = new System.Drawing.Size(129, 20);
            this.pzdateTextBox.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 285);
            this.Controls.Add(this.pzdateTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pzTextBox);
            this.Controls.Add(this.xmlButton);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "丰收日财务日报系统";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button xmlButton;
        private System.Windows.Forms.TextBox pzTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox pzdateTextBox;
    }
}

