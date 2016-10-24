using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace fengshouri
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            pzdateTextBox.Text = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
        }

        private DataTable GetExcelToDataTableBySheet(string path, string sheet_name)
        {
            string connStr = "Provider = Microsoft.ACE.OLEDB.12.0; " + "Data Source = " + path + "; " + "; Extended Properties =\"Excel 12.0;HDR=YES;IMEX=1\"";
            OleDbConnection conn = new OleDbConnection(connStr);

            conn.Open();
            DataSet ds = new DataSet();
            string sql = "Select * FROM [" + sheet_name + "$]";
            OleDbDataAdapter odda = new OleDbDataAdapter(sql, conn);
            odda.Fill(ds, sheet_name);
            conn.Close();
            return ds.Tables[0];

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pzdateTextBox.Text == "")
            {
                MessageBox.Show("请输入凭证文件日期");
                return;
            }

            if (pzTextBox.Text == "")
            {
                MessageBox.Show("请输入凭证文件路径");
                return;
            }
            DataTable pzexcel = GetExcelToDataTableBySheet(pzTextBox.Text.Trim(), "Sheet1");
            PZXML pzxml = new PZXML();
            pzxml.handle_pingzheng(pzexcel, pzdateTextBox.Text.Trim());
        }
    }
}
