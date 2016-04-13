namespace Import_DBF
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.propdelbtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.departCombo = new System.Windows.Forms.ComboBox();
            this.btnBaobiao = new System.Windows.Forms.Button();
            this.dateEnd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dateFrom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.propBtn = new System.Windows.Forms.Button();
            this.logList = new System.Windows.Forms.ListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dateText = new System.Windows.Forms.TextBox();
            this.dailydeleteBtn = new System.Windows.Forms.Button();
            this.dailyinputBtn = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(30, 333);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(0, 12);
            this.Label7.TabIndex = 16;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.propdelbtn);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.propBtn);
            this.groupBox3.Controls.Add(this.logList);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.dateText);
            this.groupBox3.Controls.Add(this.dailydeleteBtn);
            this.groupBox3.Controls.Add(this.dailyinputBtn);
            this.groupBox3.Location = new System.Drawing.Point(12, 25);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 523);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "日报导入系统";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(256, 68);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 22);
            this.button1.TabIndex = 9;
            this.button1.Text = "日期检查";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // propdelbtn
            // 
            this.propdelbtn.Location = new System.Drawing.Point(172, 97);
            this.propdelbtn.Name = "propdelbtn";
            this.propdelbtn.Size = new System.Drawing.Size(146, 23);
            this.propdelbtn.TabIndex = 8;
            this.propdelbtn.Text = "删除指定日attinfo数据";
            this.propdelbtn.UseVisualStyleBackColor = true;
            this.propdelbtn.Click += new System.EventHandler(this.propdelbtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.departCombo);
            this.groupBox1.Controls.Add(this.btnBaobiao);
            this.groupBox1.Controls.Add(this.dateEnd);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dateFrom);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(381, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(380, 94);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "报表数据查询";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(302, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(62, 19);
            this.button2.TabIndex = 8;
            this.button2.Text = "导出数据";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // departCombo
            // 
            this.departCombo.FormattingEnabled = true;
            this.departCombo.Location = new System.Drawing.Point(87, 20);
            this.departCombo.Name = "departCombo";
            this.departCombo.Size = new System.Drawing.Size(121, 20);
            this.departCombo.TabIndex = 7;
            // 
            // btnBaobiao
            // 
            this.btnBaobiao.Location = new System.Drawing.Point(302, 20);
            this.btnBaobiao.Name = "btnBaobiao";
            this.btnBaobiao.Size = new System.Drawing.Size(62, 22);
            this.btnBaobiao.TabIndex = 6;
            this.btnBaobiao.Text = "开始查询";
            this.btnBaobiao.UseVisualStyleBackColor = true;
            this.btnBaobiao.Click += new System.EventHandler(this.btnBaobiao_Click);
            // 
            // dateEnd
            // 
            this.dateEnd.Location = new System.Drawing.Point(196, 56);
            this.dateEnd.Name = "dateEnd";
            this.dateEnd.Size = new System.Drawing.Size(80, 21);
            this.dateEnd.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(173, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "到";
            // 
            // dateFrom
            // 
            this.dateFrom.Location = new System.Drawing.Point(87, 55);
            this.dateFrom.Name = "dateFrom";
            this.dateFrom.Size = new System.Drawing.Size(80, 21);
            this.dateFrom.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "查询日期:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "门店代号:";
            // 
            // propBtn
            // 
            this.propBtn.Location = new System.Drawing.Point(20, 97);
            this.propBtn.Name = "propBtn";
            this.propBtn.Size = new System.Drawing.Size(146, 23);
            this.propBtn.TabIndex = 6;
            this.propBtn.Text = "导入指定日attinfo数据";
            this.propBtn.UseVisualStyleBackColor = true;
            this.propBtn.Click += new System.EventHandler(this.propBtn_Click);
            // 
            // logList
            // 
            this.logList.FormattingEnabled = true;
            this.logList.ItemHeight = 12;
            this.logList.Location = new System.Drawing.Point(6, 120);
            this.logList.Name = "logList";
            this.logList.Size = new System.Drawing.Size(755, 400);
            this.logList.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 33);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 4;
            this.label8.Text = "导入日期";
            // 
            // dateText
            // 
            this.dateText.Location = new System.Drawing.Point(77, 30);
            this.dateText.Name = "dateText";
            this.dateText.Size = new System.Drawing.Size(206, 21);
            this.dateText.TabIndex = 3;
            // 
            // dailydeleteBtn
            // 
            this.dailydeleteBtn.Location = new System.Drawing.Point(144, 68);
            this.dailydeleteBtn.Name = "dailydeleteBtn";
            this.dailydeleteBtn.Size = new System.Drawing.Size(106, 23);
            this.dailydeleteBtn.TabIndex = 1;
            this.dailydeleteBtn.Text = "删除指定日数据";
            this.dailydeleteBtn.UseVisualStyleBackColor = true;
            this.dailydeleteBtn.Click += new System.EventHandler(this.dailydeleteBtn_Click);
            // 
            // dailyinputBtn
            // 
            this.dailyinputBtn.Location = new System.Drawing.Point(20, 68);
            this.dailyinputBtn.Name = "dailyinputBtn";
            this.dailyinputBtn.Size = new System.Drawing.Size(118, 23);
            this.dailyinputBtn.TabIndex = 0;
            this.dailyinputBtn.Text = "导入指定日数据";
            this.dailyinputBtn.UseVisualStyleBackColor = true;
            this.dailyinputBtn.Click += new System.EventHandler(this.dailyinputBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Label7);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox dateText;
        private System.Windows.Forms.Button dailydeleteBtn;
        private System.Windows.Forms.Button dailyinputBtn;
        private System.Windows.Forms.ListBox logList;
        private System.Windows.Forms.Button propBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button propdelbtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox dateFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox dateEnd;
        private System.Windows.Forms.Button btnBaobiao;
        private System.Windows.Forms.ComboBox departCombo;
        private System.Windows.Forms.Button button2;
    }
}

