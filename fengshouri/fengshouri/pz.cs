using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace fengshouri
{
    public class PingZheng
    {
        public string pz_num { set; get; }
        public string pz_date { set; get; }
        public string pz_depart { set; get; }
        public string pz_kemu { set; get; }
        public decimal pz_jieru { set; get; }
        public decimal pz_daifang { set; get; }
        public string pz_zhaiyao { set; get; }
        public string pz_fzhs1 { set; get; }
        public string pz_fzkm1 { set; get; }
        public string pz_fzhs2 { set; get; }
        public string pz_fzkm2 { set; get; }
        public string pz_fzhs3 { set; get; }
        public string pz_fzkm3 { set; get; }
        public string pz_fzhs4 { set; get; }
        public string pz_fzkm4 { set; get; }
        public string pz_fzhs5 { set; get; }
        public string pz_fzkm5 { set; get; }
    };

    public class PZXML{

        public void get_pz_list(DataTable pzexcel)
        {
            pz_list = new List<PingZheng>();
            for (int row = 0; row < pzexcel.Rows.Count; row++)
            {
                PingZheng pz = new PingZheng();
                pz.pz_num = pzexcel.Rows[row][0].ToString().Trim();
                pz.pz_date = pzexcel.Rows[row][1].ToString().Trim();
                pz.pz_depart = pzexcel.Rows[row][2].ToString().Trim();
                pz.pz_kemu = pzexcel.Rows[row][3].ToString().Trim();
                try
                { pz.pz_jieru = Convert.ToDecimal(pzexcel.Rows[row][4].ToString()); }
                catch
                {
                    pz.pz_jieru = new Decimal(0.0);
                }

                try
                {
                    pz.pz_daifang = Convert.ToDecimal(pzexcel.Rows[row][5].ToString());
                }
                catch
                {
                    pz.pz_daifang = new Decimal(0.0);
                }
                
                pz.pz_zhaiyao = pzexcel.Rows[row][7].ToString().Trim();
                pz.pz_fzhs1 = pzexcel.Rows[row][8].ToString().Trim();
                pz.pz_fzkm1 = pzexcel.Rows[row][9].ToString().Trim();
                pz.pz_fzhs2 = pzexcel.Rows[row][10].ToString().Trim();
                pz.pz_fzkm2 = pzexcel.Rows[row][11].ToString().Trim();
                pz.pz_fzhs3 = pzexcel.Rows[row][12].ToString().Trim();
                pz.pz_fzkm3 = pzexcel.Rows[row][13].ToString().Trim();
                pz.pz_fzhs4 = pzexcel.Rows[row][14].ToString().Trim();
                pz.pz_fzkm4 = pzexcel.Rows[row][15].ToString().Trim();
                pz.pz_fzhs5 = pzexcel.Rows[row][16].ToString().Trim();
                pz.pz_fzkm5 = pzexcel.Rows[row][17].ToString().Trim();
                pz_list.Add(pz);
            }
        }

        public void generate_xml_string(String pzDate)
        {
            StringBuilder xmlResult = new StringBuilder("<?xml version=\"1.0\" encoding=\"gb2312\"?>\n");
            xmlResult.AppendLine("<ufinterface account=\"01\" billtype=\"gl\" filename=\"凭证.xml\" isexchange=\"Y\" proc=\"add\" receiver=\"1001\" replace=\"Y\" roottag=\"voucher\" sender=\"01\" subbilltype=\"\" operation=\"req\">");
            xmlResult.AppendFormat("<voucher ID=\"{0}\">\n", string.Format("{0:yyyyMMddHHmmssffffff}", DateTime.Now));

            /*voucher header*/
            xmlResult.AppendLine("<voucher_head>");

            xmlResult.AppendLine("<company>10</company>");
            xmlResult.AppendLine("<account_code />");
            xmlResult.AppendLine("<voucher_type>收款凭证</ voucher_type>");
            xmlResult.AppendFormat("<fiscal_year>{0}</fiscal_year>\n", pzDate.Split('-')[0]);
            xmlResult.AppendFormat("<accounting_period>{0}</accounting_period>\n", pzDate.Split('-')[1]);
            xmlResult.AppendLine("<voucher_id />");
            xmlResult.AppendLine("<attachment_number>1</attachment_number>");
            xmlResult.AppendFormat("<prepareddate>{0}</prepareddate>\n", pzDate);
            xmlResult.AppendLine("<enter>N</enter>");
            xmlResult.AppendLine("<cashier />");
            xmlResult.AppendLine("<signature>N</signature>");
            xmlResult.AppendLine("<checker />");
            xmlResult.AppendLine("<operator />");
            xmlResult.AppendFormat("<posting_date>{0}</posting_date>\n", pzDate);
            xmlResult.AppendLine("<revokeflag>N</ revokeflag>");
            xmlResult.AppendLine("<voucherkind>0</voucherkind>");
            xmlResult.AppendLine("<voucher_making_system>总账</voucher_making_system>");

            xmlResult.AppendLine("<memo1 />");
            xmlResult.AppendLine("<memo2 />");
            xmlResult.AppendLine("<reserve1 />");
            xmlResult.AppendLine("<reserve2 />");
            xmlResult.AppendLine("</voucher_head>");
            /*end voucher header*/


            /*voucher body*/
            xmlResult.Append("<voucher_body>");
            for (int row=0; row < pz_list.Count; row ++)
            {
                xmlResult.AppendLine("<entry>");
                xmlResult.AppendFormat("<entry_id>{0}</entry_id>\n", row + 1);
                xmlResult.AppendFormat("<account_code></account_code>\n", pz_list[row].pz_kemu);
                xmlResult.AppendFormat("<abstract>{0}</abstract>\n", pz_list[row].pz_zhaiyao);
                xmlResult.AppendLine("<settlement />");
                xmlResult.AppendLine("<document_id />");
                xmlResult.AppendLine("<document_date />");
                xmlResult.AppendLine("<currency>CNY</currency>");
                xmlResult.AppendLine("<unit_price />");
                xmlResult.AppendLine("<exchange_rate1>0</exchange_rate1>");
                xmlResult.AppendLine("<exchange_rate2 />");
                xmlResult.AppendLine("<debit_quantity>0.0000</debit_quantity>");
                xmlResult.AppendFormat("<primary_debit_amount>{0}</primary_debit_amount>\n", pz_list[row].pz_jieru);
                xmlResult.AppendLine("<secondary_debit_amount>0.0000</secondary_debit_amount>");
                xmlResult.AppendFormat("<secondary_debit_amount>{0}</secondary_debit_amount>\n", pz_list[row].pz_jieru);
                xmlResult.AppendLine("<credit_quantity>0.0000</credit_quantity>");
                xmlResult.AppendFormat("<primary_credit_amount>{0}</primary_credit_amount>\n", pz_list[row].pz_daifang);
                xmlResult.AppendLine("<secondary_credit_amount>0.0000</secondary_credit_amount>");
                xmlResult.AppendFormat("<natural_credit_currency>{0}</ natural_credit_currency>\n", pz_list[row].pz_daifang);
                xmlResult.AppendLine("<bill_type />");
                xmlResult.AppendLine("<bill_id />");
                xmlResult.AppendLine("<bill_date />");
                xmlResult.AppendLine("<auxiliary_accounting>");
                xmlResult.AppendFormat("<item name=\"{0}\">{1}</item>\n", pz_list[row].pz_fzhs1, pz_list[row].pz_fzkm1);
                xmlResult.AppendFormat("<item name=\"{0}\">{1}</item>\n", pz_list[row].pz_fzhs2, pz_list[row].pz_fzkm2);
                xmlResult.AppendFormat("<item name=\"{0}\">{1}</item>\n", pz_list[row].pz_fzhs3, pz_list[row].pz_fzkm3);
                xmlResult.AppendFormat("<item name=\"{0}\">{1}</item>\n", pz_list[row].pz_fzhs4, pz_list[row].pz_fzkm4);
                xmlResult.AppendFormat("<item name=\"{0}\">{1}</item>\n", pz_list[row].pz_fzhs5, pz_list[row].pz_fzkm5);
                xmlResult.AppendLine("</ auxiliary_accounting>");
                xmlResult.AppendLine("<detail />");
                xmlResult.AppendLine("<otheruserdata />");
                xmlResult.AppendLine("</entry>");
            }
            xmlResult.AppendLine("</voucher_body>");
            /*end voucher body*/

            /*end voucher*/
            xmlResult.AppendLine("</voucher>");
            xmlResult.AppendLine("</ufinterface>");

            pzxml = xmlResult;
        }

        public void write_xml_to_file()
        {
            try
            {
                FileStream fileStream = new FileStream("E:\\pingzheng.xml", FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine(pzxml);
                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception e){ MessageBox.Show(e.ToString()); return; };
            MessageBox.Show("凭证导入完毕");
        }

        public void handle_pingzheng(DataTable pzexcel, String pzDate)
        {
            get_pz_list(pzexcel);
            generate_xml_string(pzDate);
            write_xml_to_file();
        }

        public void debug_pz_list()
        {
            for(int i=0; i<pz_list.Count; i++)
            {
                PingZheng pz= pz_list[i];
                Console.WriteLine(String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", pz.pz_num, pz.pz_date, pz.pz_depart, pz.pz_kemu,
                    pz.pz_jieru, pz.pz_daifang, pz.pz_zhaiyao, pz.pz_fzhs1, pz.pz_fzkm1));
            }
        }

        public void debug_xml()
        {
            Console.Write(pzxml.ToString());
        }

        private List<PingZheng> pz_list;
        private StringBuilder pzxml;
    }
}