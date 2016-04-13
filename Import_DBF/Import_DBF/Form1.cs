using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Import_DBF
{

    public partial class Form1 : Form
    {
        private List<String> folders = new List<String>();
        private String base_path = "Y:\\cust\\pos710\\";
        Dictionary<String,String> shop_map = new Dictionary<String,String>();
        Dictionary<String, String> base_map = new Dictionary<String, String>();
        private String dbname = "banzhaoyun";
        Dictionary<String, String> sql_map = new Dictionary<String, String>();
        Dictionary<String, ribao_t> ribao_map = new Dictionary<String,ribao_t>();
        Dictionary<String, String> depart_pp_map = new Dictionary<String, String>();

        public Form1()
        {
            InitializeComponent();
            shop_map.Add("fsr", "101");
            shop_map.Add("shopfhrl", "102");
            shop_map.Add("yun", "103");
            shop_map.Add("yunlu", "104");
            shop_map.Add("shop", "105");
            //shop_map.Add("shop_hk", "106");   /*hk shop is closed 2015-08-24*/
            shop_map.Add("hzfhrl", "107");
            shop_map.Add("njfhrl", "108");
            shop_map.Add("bjfhrl", "109");
            shop_map.Add("bjyunlu", "110");
            shop_map.Add("fsgs", "113");
            //shop_map.Add("fsrold", "112");    /*fsrold is closed*/

            dateText.Text = DateTime.Now.ToString("yyyy-MM-dd");
            dateFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
            string[] depart_list = new string[]{"S02", "S06", "S07", "S14", "S23", "F02", "S30", "F09", "F03", "S03", "S20", "S27", "S28", "S33", "F07", "F04",
                            "S04", "S08", "S15", "S18", "S31", "S36", "S37", "S39", "S09", "S17", "S21", "S24", "S25", "S32", "S35", "F10", "S10", "S12", "S19", "S22", "S26", "S38", "S40", "G01",
                            "F01", "F06", "Y01", "Y02", "L01", "F08", "F05", "L02", "005", "006"};
            departCombo.Items.AddRange(depart_list);
            foreach(String name in depart_list)
            {
                if (name == "G01")
                    depart_pp_map[name] = "113";
                else if (name == "F05" || name == "L02")
                    depart_pp_map[name] = "110";
                else if (name == "F08")
                    depart_pp_map[name] = "109";
                else if (name == "F07")
                    depart_pp_map[name] = "108";
                else if (name == "F03" || name == "F09")
                    depart_pp_map[name] = "107";
                else if (name == "005" || name == "006")
                    depart_pp_map[name] = "105";
                else if (name == "L01")
                    depart_pp_map[name] = "104";
                else if (name == "Y01" || name == "Y02")
                    depart_pp_map[name] = "103";
                else if (name == "F01" || name == "F04" || name == "F06")
                    depart_pp_map[name] = "102";
                else
                    depart_pp_map[name] = "101";
            }

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            base_map.Add("database\\category.dbf", "category");
            base_map.Add("database\\itemtype.dbf", "itemtype");
            base_map.Add("database\\disc_hd.dbf", "disc_hd");
            base_map.Add("database\\scat.dbf", "scat");
            base_map.Add("database\\payment.dbf", "payment");
            base_map.Add("database\\menu_0\\att.dbf", "att");
            base_map.Add("database\\menu_0\\atthead.dbf", "atthead");
            base_map.Add("database\\menu_0\\coupon.dbf", "coupon");
            base_map.Add("database\\menu_0\\item.dbf", "item");
         //   base_map.Add("database\\menu_0\\table.dbf", "table");

            sql_map.Add("get_branch", "select PK_BranchId, DF_BranchName, DF_QY, DF_DataBaseName from TBL_SSB_BranchInfo");
            sql_map.Add("get_free_dis", "select a.outlet, -Sum(AMT) free_dis from (select * from T_ORDER where TYPEA=\'N\' and FREE > 0 and DATEA=\'{0}\') a group by a.OUTLET");
            sql_map.Add("get_credit_dis", "select OUTLET, SUM(amount) credit_dis from T_PAY where TRAN_TYPE =\'n\' and PAY_TYPE =\'MPTS\' and DATEA=\'{0}\' group by OUTLET");
            sql_map.Add("get_recep_dis", "select OUTLET, SUM(amount) recep_dis from T_PAY where TRAN_TYPE =\'n\' and PAY_TYPE =\'0015\' and DATEA =\'{0}\' group by OUTLET");
            sql_map.Add("get_card_dis", "select OUTLET, SUM(amount) card_dis from T_PAY where TRAN_TYPE =\'n\' and PAY_TYPE =\'0013\' and DATEA=\'{0}\' group by OUTLET");
            sql_map.Add("get_ticket_dis", "select outlet, SUM(amount) ticket_dis from T_PAY where PAY_TYPE like \'LQ%\' and DATEA=\'{0}\'  group by OUTLET");
            sql_map.Add("get_ticket_dis_2", "select REF_NUM, COUNT(ref_num), SUM(amount) from T_PAY where PAY_TYPE like \'LQ%\' and DATEA=\'{0}\' and OUTLET=\'{1}\' and sub_ref = \'00\' group by REF_NUM");

            sql_map.Add("get_member_card", "select PP, ITEM_CODE from case后菜肴分类 where xiaolei like '%[会员补办]卡%'");
            sql_map.Add("get_credit_gift", "select PP, ITEM_CODE from case后菜肴分类 where 菜肴名称 in(\'#苹果8G多媒体播(白)/个\',\'#苹果8G多媒体播(白)/个\',\'#田园茶具/套\',\'#炫彩胸针/枚\',\'#超酷打火机/个\',\'#精美挂口/枚\',\'#魅力手包/个\',\'#高仿真娃娃/只\',\'#精致手袋/只\',\'#消费卡/张\',\'#四件套/套\',\'#蚕丝被/套\',\'#珠宝盒/只\',\'#名牌香水/瓶\',\'#品牌电水壶/只\',\'#名牌MP3/部\',\'#名牌手机/部\',\'#五帝珍宝蛙/只\',\'#水晶胸针(大)/枚\',\'#水晶胸针(小)/枚\',\'#U盘挂件/个\',\'#欧姆龙血压计/个\',\'#双立人四件套/套\',\'#世博纪念币/套\',\'#儿童书包/个\',\'#儿童阳伞/把\',\'#滑板车/个\',\'#儿童拉杆书包/个\',\'#卡通拼图/套\',\'#乐高积木/套\',\'#电动玩具/个\',\'#折叠自行车/辆\',\'#定制礼品/个\',\'#名牌MP3/只\',\'#苹果8G多媒体播(黑)/部\',\'#苹果8G多媒体播(白)/部\',\'#田园茶具/例\',\'#炫彩胸针/例\',\'#超酷打火机/例\',\'#精美挂口/只\',\'#魅力手包/只\',\'#苹果8G多媒体播(黑)/个\',\'#消费卡500/张\',\'#蚕丝被/条\',\'#名牌手机/只\',\'#水晶胸针(大)/个\',\'#水晶胸针(小)/个\',\'#双利人道具/套\')");
            sql_map.Add("get_outsale_credit", "select PP, ITEM_CODE from case后菜肴分类 where xiaolei in (\'海鲜礼包\',\'商品销售\' ,\'月饼\',\'超市产品\',\'其他外卖产品\',\'粽子\', \'礼品兑换\', \'积分礼品兑换类\')");
            sql_map.Add("get_total", "select OUTLET, sum(ORDER_AMT), sum(SERV_AMT), sum(TIPS) , sum(rounding), SUM(PERSON), sum(ITEM_DISC), sum(CAT_DISC), sum(ORDER_DISC) from T_TRAN where TRAN_TYPE=\'N\' and in_DATE=\'{0}\' group by OUTLET");
            sql_map.Add("get_order_non_free", "select PP, ITEM_CODE, OUTLET, AMT from T_ORDER where DATEA=\'{0}\' and TYPEA=\'N\'");
            sql_map.Add("get_order_free", "select PP, ITEM_CODE, OUTLET, AMT from T_ORDER where DATEA=\'{0}\' and TYPEA=\'N\' and free > 0");
            /*sql_map.Add("get_member_card_sale", "select c.outlet, sum(amt) 会员卡销售额 from (SELECT  a.* ,b.菜肴名称,b.xiaolei,b.dalei FROM T_ORDER  a inner join case后菜肴分类 b on a.PP=b.pp and a.ITEM_CODE =b.ITEM_CODE  where TYPEA =\'n\' and xiaolei like \'%[会员补办]卡%\' and DATEA=\'{0}\') c group by c.OUTLET");
            sql_map.Add("get_member_card_free", "select c.outlet, sum(amt) 会员卡赠送额 from (SELECT  a.* ,b.菜肴名称,b.xiaolei,b.dalei FROM T_ORDER  a inner join case后菜肴分类 b on a.PP=b.pp and a.ITEM_CODE =b.ITEM_CODE  where  TYPEA =\'n\' and free>0 and xiaolei like \'%[会员补办]卡%\' and DATEA=\'{0}\') c group by c.OUTLET");
            sql_map.Add("get_total", "select OUTLET, sum(ORDER_AMT), sum(SERV_AMT), sum(TIPS) , sum(rounding), SUM(PERSON), sum(ITEM_DISC), sum(CAT_DISC), sum(ORDER_DISC) from T_TRAN where TRAN_TYPE=\'N\' and in_DATE=\'{0}\' group by OUTLET");
            sql_map.Add("get_credit_gift_sale", "select c.outlet, sum(amt) 积分兑换礼品销售金额 from (SELECT  a.*, b.菜肴名称, b.xiaolei, b.dalei FROM T_ORDER a inner join case后菜肴分类 b on a.PP=b.pp and a.ITEM_CODE =b.ITEM_CODE where  TYPEA =\'n\' and  菜肴名称 in(\'#苹果8G多媒体播(白)/个\',\'#苹果8G多媒体播(白)/个\',\'#田园茶具/套\',\'#炫彩胸针/枚\',\'#超酷打火机/个\',\'#精美挂口/枚\',\'#魅力手包/个\',\'#高仿真娃娃/只\',\'#精致手袋/只\',\'#消费卡/张\',\'#四件套/套\',\'#蚕丝被/套\',\'#珠宝盒/只\',\'#名牌香水/瓶\',\'#品牌电水壶/只\',\'#名牌MP3/部\',\'#名牌手机/部\',\'#五帝珍宝蛙/只\',\'#水晶胸针(大)/枚\',\'#水晶胸针(小)/枚\',\'#U盘挂件/个\',\'#欧姆龙血压计/个\',\'#双立人四件套/套\',\'#世博纪念币/套\',\'#儿童书包/个\',\'#儿童阳伞/把\',\'#滑板车/个\',\'#儿童拉杆书包/个\',\'#卡通拼图/套\',\'#乐高积木/套\',\'#电动玩具/个\',\'#折叠自行车/辆\',\'#定制礼品/个\',\'#名牌MP3/只\',\'#苹果8G多媒体播(黑)/部\',\'#苹果8G多媒体播(白)/部\',\'#田园茶具/例\',\'#炫彩胸针/例\',\'#超酷打火机/例\',\'#精美挂口/只\',\'#魅力手包/只\',\'#苹果8G多媒体播(黑)/个\',\'#消费卡500/张\',\'#蚕丝被/条\',\'#名牌手机/只\',\'#水晶胸针(大)/个\',\'#水晶胸针(小)/个\',\'#双利人道具/套\') and DATEA = \'{0}\') c group by c.OUTLET");
            sql_map.Add("get_credit_gift_free", "select c.outlet, sum(amt) 积分兑换礼品赠送金额 from (SELECT  a.*, b.菜肴名称, b.xiaolei, b.dalei FROM T_ORDER a inner join case后菜肴分类 b on a.PP=b.pp and a.ITEM_CODE =b.ITEM_CODE where  TYPEA =\'n\' and free>0 and  菜肴名称 in(\'#苹果8G多媒体播(白)/个\',\'#苹果8G多媒体播(白)/个\',\'#田园茶具/套\',\'#炫彩胸针/枚\',\'#超酷打火机/个\',\'#精美挂口/枚\',\'#魅力手包/个\',\'#高仿真娃娃/只\',\'#精致手袋/只\',\'#消费卡/张\',\'#四件套/套\',\'#蚕丝被/套\',\'#珠宝盒/只\',\'#名牌香水/瓶\',\'#品牌电水壶/只\',\'#名牌MP3/部\',\'#名牌手机/部\',\'#五帝珍宝蛙/只\',\'#水晶胸针(大)/枚\',\'#水晶胸针(小)/枚\',\'#U盘挂件/个\',\'#欧姆龙血压计/个\',\'#双立人四件套/套\',\'#世博纪念币/套\',\'#儿童书包/个\',\'#儿童阳伞/把\',\'#滑板车/个\',\'#儿童拉杆书包/个\',\'#卡通拼图/套\',\'#乐高积木/套\',\'#电动玩具/个\',\'#折叠自行车/辆\',\'#定制礼品/个\',\'#名牌MP3/只\',\'#苹果8G多媒体播(黑)/部\',\'#苹果8G多媒体播(白)/部\',\'#田园茶具/例\',\'#炫彩胸针/例\',\'#超酷打火机/例\',\'#精美挂口/只\',\'#魅力手包/只\',\'#苹果8G多媒体播(黑)/个\',\'#消费卡500/张\',\'#蚕丝被/条\',\'#名牌手机/只\',\'#水晶胸针(大)/个\',\'#水晶胸针(小)/个\',\'#双利人道具/套\') and DATEA = \'{0}\') c group by c.OUTLET");
            sql_map.Add("get_outsale_credit_sale", "select c.outlet, sum(amt) 外卖积分销售金额 from (SELECT  a.*, b.菜肴名称, b.xiaolei, b.dalei FROM T_ORDER  a inner join case后菜肴分类 b on a.PP=b.pp and a.ITEM_CODE =b.ITEM_CODE  where  TYPEA =\'n\' and  xiaolei in (\'海鲜礼包\',\'商品销售\' ,\'月饼\',\'超市产品\',\'其他外卖产品\',\'粽子\', \'礼品兑换\', \'积分礼品兑换类\') and DATEA = \'{0}\') c group by c.OUTLET");
            sql_map.Add("get_outsale_credit_free", "select c.outlet, sum(amt) 外卖积分赠送金额 from (SELECT a.*, b.菜肴名称, b.xiaolei, b.dalei FROM T_ORDER a inner join case后菜肴分类 b on a.PP=b.pp and a.ITEM_CODE =b.ITEM_CODE  where TYPEA =\'n\' and free>0 and  xiaolei in (\'海鲜礼包\', \'商品销售\', \'月饼\', \'超市产品\', \'其他外卖产品\', \'粽子\', \'礼品兑换\', \'积分礼品兑换类\') and DATEA = \'{0}\') c group by c.OUTLET");
             */ 
        }
        
        private void add_daily_folders(String path)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            try
            {
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    folders.Add(NextFolder.FullName);
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void initial_import_environment(String path)
        {
            /*Get subdir*/
            DirectoryInfo TheFolder = new DirectoryInfo(path);
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                DirectoryInfo twoFolder = new DirectoryInfo(NextFolder.FullName);
                foreach (DirectoryInfo secFolder in twoFolder.GetDirectories())
                {
                    //DirectoryInfo thirdFolder = new DirectoryInfo(secFolder.FullName);
                    //foreach (DirectoryInfo thrFolder in thirdFolder.GetDirectories())
                        folders.Add(secFolder.FullName);
                }
            }
        }

        private void Delete_base_table()
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            String sql;
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);

            sql_conn.Open();

            foreach (String s in base_map.Keys)
            {
                sql = "delete from [banzhaoyun].[dbo].[" + base_map[s] + "]";
                sql_cmd = new SqlCommand(sql);
                sql_cmd.Connection = sql_conn;
                sql_cmd.ExecuteNonQuery();
            }

            sql_conn.Close();
        }

        private void Delete_attinfo_table(String date)
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            String sql;
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);

            sql_conn.Open();
            sql = "delete from dbo.att_info where DATEA=\'" + date + "\'";
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.ExecuteNonQuery();

            sql_conn.Close();

        }
        private void Delete_daily_table(String date)
        {
            String[] keys = date.Split('-');
            String year = keys[0], month = keys[1], day = keys[2];

            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            String sql;
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);

            sql_conn.Open();
            sql = "delete from dbo.T_tran where year(IN_DATE)=" + year + " and MONTH (IN_DATE)=" + month + " and DAY(IN_DATE)=" + day;
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.ExecuteNonQuery();
            logList.Items.Insert(0, "删除 T_TRAN 成功");
            sql = "delete from dbo.T_pay where year(DATEA)=" + year + " and MONTH (DATEA)=" + month + " and DAY(DATEA)=" + day;
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.ExecuteNonQuery();
            logList.Items.Insert(0, "删除 T_PAY 成功");
            sql = "delete from dbo.T_log where year(DATE)=" + year + " and MONTH (DATE)=" + month + " and DAY(DATE)=" + day;
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.ExecuteNonQuery();
            logList.Items.Insert(0, "删除 T_LOG 成功");
            sql = "delete from dbo.T_att where year(DATEA)=" + year + " and MONTH (DATEA)=" + month + " and DAY(DATEA)=" + day;
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.ExecuteNonQuery();
            logList.Items.Insert(0, "删除 T_ATT 成功");

            sql = "delete from dbo.T_order where year(DATEA)=" + year + " and MONTH (DATEA)=" + month + " and DAY(DATEA)=" + day;
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.ExecuteNonQuery();
            logList.Items.Insert(0, "删除 T_ORDER 成功");

            sql = "delete from dbo.A_PAY where year(DATE)=" + year + " and MONTH (DATE)=" + month + " and DAY(DATE)=" + day;
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.ExecuteNonQuery();
            logList.Items.Insert(0, "删除 A_PAY 成功");

            sql = "delete from dbo.A_TRAN where year(IN_DATE)=" + year + " and MONTH (IN_DATE)=" + month + " and DAY(IN_DATE)=" + day;
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.ExecuteNonQuery();
            logList.Items.Insert(0, "删除 A_TRAN 成功");

            sql_conn.Close();

            MessageBox.Show("删除 " + date +　" 数据成功");
        }
        private void check_daily_dbf_date(String table_name, String code)
        {
            foreach (String path in folders)
            {
                System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection();
                try
                {
                    String p = path.Replace(" ", "");
                    String table = p + "\\" + table_name + ".DBF";
                    string connStr = @"Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;SourceDB=" + table + ";Exclusive=No;NULL=NO;Collate=Machine;BACKGROUNDFETCH=NO;DELETED=NO";
                    conn.ConnectionString = connStr;

                    if (!File.Exists(table))
                    {
                        MessageBox.Show(table + " is not exist");
                        continue;
                    }
                    conn.Open();
                    String sql = @"select * from " + table;

                    System.Data.Odbc.OdbcDataAdapter dbf_da = new System.Data.Odbc.OdbcDataAdapter(sql, conn);
                    DataTable dbf_dt = new DataTable();
                    dbf_dt.Clear();
                    dbf_da.Fill(dbf_dt);
                    conn.Close();

                    foreach (DataRow row in dbf_dt.Rows)
                    {
                        DateTime in_date = Convert.ToDateTime(row[11]);
                        DateTime bill_date = Convert.ToDateTime(row[13]);
                        if (in_date.ToString("yyyy-MM-dd") != dateText.Text || bill_date.ToString("yyyy-MM-dd") != dateText.Text)
                            logList.Items.Add(table + ": REF_NUM: " + row[0] + " IN_DATE: " + in_date.ToString("yyyy-MM-dd") + " BILL_DATE: " + bill_date.ToString("yyyy-MM-dd"));
                    }
                }
                catch (Exception ex)
                {
                }
                
                
            }
        }
        private void start_daily_import(String table_name, String code)
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();

            /*Connect to Sql server*/
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);
            sql_cmd = new SqlCommand("select * from dbo." + table_name);
            sql_cmd.Connection = sql_conn;
            da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
            //da.Fill(dt);
            da.FillSchema(dt, System.Data.SchemaType.Mapped);

            /*Read DBF file content*/
            foreach (String path in folders)
            {
                System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection();
                try
                {
                    String p = path.Replace(" ", "");
                    String table = p + "\\" + table_name + ".DBF";
                    string connStr = @"Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;SourceDB=" + table + ";Exclusive=No;NULL=NO;Collate=Machine;BACKGROUNDFETCH=NO;DELETED=NO";
                    conn.ConnectionString = connStr;

                    if (!File.Exists(table))
                    {
                        MessageBox.Show(table + " is not exist");
                        continue;
                    }
                    conn.Open();
                    String sql;
                    if (table.Contains("A_TRAN"))
                        sql = @"select ref_num, outlet, tran_type, in_date from " + table;
                    else
                        sql = @"select * from " + table;

                    System.Data.Odbc.OdbcDataAdapter dbf_da = new System.Data.Odbc.OdbcDataAdapter(sql, conn);
                    DataTable dbf_dt = new DataTable();
                    dbf_dt.Clear();
                    dbf_da.Fill(dbf_dt);
                    conn.Close();

                    int year = path.IndexOf('2');
                    DateTime date = Convert.ToDateTime(path.Substring(year, 10).Replace('\\', '-'));
                    int k = 0;
                    //System.Console.WriteLine(table);
                    for (int j = 0; j < dbf_dt.Rows.Count; j++)
                    {
                        if (table.Contains("A_PAY"))
                        {
                            DataRow newrow = dt.NewRow();
                            for (k = 0; k < dbf_dt.Columns.Count; k++)
                            {
                                newrow[k] = dbf_dt.Rows[j][k];
                            }
                            newrow[k] = date;
                            dt.Rows.Add(newrow);
                        }
                        else if (table.Contains("A_TRAN"))
                        {
                            DataRow newrow = dt.NewRow();
                            newrow[0] = dbf_dt.Rows[j][0];
                            newrow[3] = dbf_dt.Rows[j][1];
                            newrow[5] = dbf_dt.Rows[j][2];
                            newrow[11] = dbf_dt.Rows[j][3];

                            dt.Rows.Add(newrow);
                        }
                        else
                        {
                            DataRow newrow = dt.NewRow();
                            newrow[0] = code;
                            for (k = 0; k < dbf_dt.Columns.Count; k++)
                            {
                                newrow[k + 1] = dbf_dt.Rows[j][k];
                            }

                            if (table.Contains("T_ATT") || table.Contains("T_PAY"))
                                newrow[k + 1] = date;
                            dt.Rows.Add(newrow);
                        }
                    }
                    
                    sql_cmd.Connection.Open();
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    da.Update(dt);
                    sql_cmd.Connection.Close();
                    dt.Clear();
                    logList.Items.Insert(0, table);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    logList.Items.Insert(0, ex.ToString());
                    conn.Close();
                    sql_cmd.Connection.Close();
                    dt.Clear();
                }
            }
        }

        private void start_daily_import2(String table_name, String code)
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();

            /*Connect to Sql server*/
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=fsrBigData;Server=192.168.3.252;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);
            sql_cmd = new SqlCommand("select * from dbo." + table_name);
            sql_cmd.Connection = sql_conn;
            da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
            //da.Fill(dt);
            da.FillSchema(dt, System.Data.SchemaType.Mapped);

            /*Read DBF file content*/
            foreach (String path in folders)
            {
                System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection();
                try
                {
                    String p = path.Replace(" ", "");
                    String table = p + "\\" + table_name + ".DBF";
                    string connStr = @"Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;SourceDB=" + table + ";Exclusive=No;NULL=NO;Collate=Machine;BACKGROUNDFETCH=NO;DELETED=NO";
                    conn.ConnectionString = connStr;

                    if (!File.Exists(table))
                    {
                        MessageBox.Show(table + " is not exist");
                        continue;
                    }
                    conn.Open();
                    String sql = @"select * from " + table;

                    System.Data.Odbc.OdbcDataAdapter dbf_da = new System.Data.Odbc.OdbcDataAdapter(sql, conn);
                    DataTable dbf_dt = new DataTable();
                    dbf_dt.Clear();
                    dbf_da.Fill(dbf_dt);
                    conn.Close();

                    int year = path.IndexOf('2');
                    DateTime date = Convert.ToDateTime(path.Substring(year, 10).Replace('\\', '-'));
                    int k = 0;

                    for (int j = 0; j < dbf_dt.Rows.Count; j++)
                    {
                        if (table.Contains("A_PAY"))
                        {
                            DataRow newrow = dt.NewRow();
                            for (k = 0; k < dbf_dt.Columns.Count; k++)
                            {
                                newrow[k] = dbf_dt.Rows[j][k];
                            }
                            newrow[k] = date;
                            dt.Rows.Add(newrow);
                        }
                        else if (table.Contains("A_TRAN"))
                        {
                            DataRow newrow = dt.NewRow();
                            for (k = 0; k < dbf_dt.Columns.Count; k++)
                            {
                                newrow[k] = dbf_dt.Rows[j][k];
                            }
                            dt.Rows.Add(newrow);
                        }
                        else
                        {
                            DataRow newrow = dt.NewRow();
                            newrow[0] = code;
                            for (k = 0; k < dbf_dt.Columns.Count; k++)
                            {
                                newrow[k + 1] = dbf_dt.Rows[j][k];
                            }

                            if (table.Contains("T_ATT") || table.Contains("T_PAY"))
                                newrow[k + 1] = date;
                            dt.Rows.Add(newrow);
                        }
                    }

                    sql_cmd.Connection.Open();
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    da.Update(dt);
                    sql_cmd.Connection.Close();
                    dt.Clear();
                    logList.Items.Insert(0, table);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    logList.Items.Insert(0, ex.ToString());
                    conn.Close();
                    sql_cmd.Connection.Close();
                    dt.Clear();
                }
            }
        }
        private void start_base_import(String table, String table_name, String code)
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();

            /*Connect to Sql server*/
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);
            sql_cmd = new SqlCommand("select * from [banzhaoyun].[dbo].[" + table_name + "]");
            sql_cmd.Connection = sql_conn;
            da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
            da.Fill(dt);
            da.FillSchema(dt, System.Data.SchemaType.Mapped);

            /*Read DBF file content*/
            try
            {
                //MessageBox.Show(table + " " + table_name + " " + code);
                System.Data.Odbc.OdbcConnection conn = new System.Data.Odbc.OdbcConnection();
                //String table = "E:\\banzhaoyun\\base\\" + table_name + ".DBF";
                string connStr = @"Driver={Microsoft Visual FoxPro Driver};SourceType=DBF;SourceDB=" + table + ";Exclusive=No;NULL=NO;Collate=Machine;BACKGROUNDFETCH=NO;DELETED=NO";
                conn.ConnectionString = connStr;
                conn.Open();

                String sql = @"select * from " + table;
                System.Data.Odbc.OdbcDataAdapter dbf_da = new System.Data.Odbc.OdbcDataAdapter(sql, conn);
                DataTable dbf_dt = new DataTable();
                dbf_dt.Clear();
                dbf_da.Fill(dbf_dt);
                conn.Close();

                for (int j = 0; j < dbf_dt.Rows.Count; j++)
                {
                    DataRow newrow = dt.NewRow();
                    newrow[0] = code;
                    for (int k = 0; k < dbf_dt.Columns.Count; k++)
                        newrow[k + 1] = dbf_dt.Rows[j][k];
                    dt.Rows.Add(newrow);
                }
                System.Console.WriteLine(table);

                sql_cmd.Connection.Open();
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Update(dt);
                sql_cmd.Connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(table + " " + table_name);
            }
        }

        private void baseBtn_Click(object sender, EventArgs e)
        {
            foreach (String key in shop_map.Keys)
            {
                String folder_path = base_path + key;
                foreach (String s in base_map.Keys)
                {
                    String path = folder_path + "\\" + s;
                    //MessageBox.Show(path + " " + shop_map[key] + " " + base_map[s]);
                    start_base_import(path, base_map[s], shop_map[key]);
                }
            }
            MessageBox.Show("基础数据导入成功");
 /*  
            tableText.Text = "item";
            codeText.Text = "101";

            if (tableText.Text == "" || codeText.Text == "")
            {
                MessageBox.Show("请输入基础表相关字段");
                return;
            }
            start_base_import(tableText.Text, codeText.Text);
            MessageBox.Show(tableText.Text + ".dbf 更新完成");
            tableText.Text = "";
            codeText.Text = "";
*/
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            logList.Items.Clear();
        }

        private void import_daily_dbf()
        {
            logList.Items.Clear();
            foreach (String key in shop_map.Keys)
            {
                String folder_path = base_path + key + "\\" + dateText.Text.Replace('-', '\\');
                folders.Clear();
                add_daily_folders(folder_path);
                start_daily_import("T_PAY", shop_map[key]);
                start_daily_import("T_TRAN", shop_map[key]);
                start_daily_import("T_LOG", shop_map[key]);
                start_daily_import("T_ATT", shop_map[key]);
                start_daily_import("T_ORDER", shop_map[key]);

                /*start_daily_import2("T_PAY", shop_map[key]);
                start_daily_import2("T_TRAN", shop_map[key]);
                start_daily_import2("T_LOG", shop_map[key]);
                start_daily_import2("T_ATT", shop_map[key]);
                start_daily_import2("T_ORDER", shop_map[key]);*/
                start_daily_import("A_PAY", shop_map[key]);
                start_daily_import("A_TRAN", shop_map[key]);
            }
            import_daily_ribao();
            MessageBox.Show("所有门店导入完成");
        }
        private void check_dbf_date()
        {
            logList.Items.Clear();
            foreach (String key in shop_map.Keys)
            {
                String folder_path = base_path + key + "\\" + dateText.Text.Replace('-', '\\');
                folders.Clear();
                add_daily_folders(folder_path);
                check_daily_dbf_date("T_TRAN", shop_map[key]);
            }
            MessageBox.Show("门店日期检查完毕");
        }

        private void import_daily_ribao()
        {
            ribao_map.Clear();
            delete_ribao_table();

            DataTable total_dt = get_datatable(String.Format(sql_map["get_total"], dateText.Text));
            DataTable member_card_dt = get_datatable(String.Format(sql_map["get_member_card"], dateText.Text));
            DataTable credit_gift_dt = get_datatable(String.Format(sql_map["get_credit_gift"], dateText.Text));
            DataTable outsale_credit_dt = get_datatable(String.Format(sql_map["get_outsale_credit"], dateText.Text));

            for (int i = 0; i < total_dt.Rows.Count; i++)
            {
                ribao_t ribao = new ribao_t(Convert.ToInt32(total_dt.Rows[i][5]), Convert.ToDouble(total_dt.Rows[i][1]),
                        Convert.ToDouble(total_dt.Rows[i][2]), Convert.ToDouble(total_dt.Rows[i][3]), 
                        Convert.ToDouble(total_dt.Rows[i][4]), Convert.ToDouble(total_dt.Rows[i][6]),
                        Convert.ToDouble(total_dt.Rows[i][7]), Convert.ToDouble(total_dt.Rows[i][8]));
                ribao_map.Add(total_dt.Rows[i][0].ToString().Trim(), ribao);
            }

            DataTable branch_dt = get_datatable(String.Format(sql_map["get_branch"], dateText.Text));
            for (int i = 0; i < branch_dt.Rows.Count; i++) {
                foreach (string key in ribao_map.Keys)
                    if (branch_dt.Rows[i][0].ToString().Trim() == key)
                    {
                        ribao_map[key].branch = branch_dt.Rows[i][1].ToString().Trim();
                        ribao_map[key].area = branch_dt.Rows[i][2].ToString().Trim();
                        ribao_map[key].band = branch_dt.Rows[i][3].ToString().Trim();
                    }
            }
            DataTable free_dis_dt = get_datatable(String.Format(sql_map["get_free_dis"], dateText.Text));
            for (int i = 0; i < free_dis_dt.Rows.Count; i++)
                ribao_map[free_dis_dt.Rows[i][0].ToString()].free_dis = Convert.ToDouble(free_dis_dt.Rows[i][1]);

            DataTable credit_dis_dt = get_datatable(String.Format(sql_map["get_credit_dis"], dateText.Text));
            for (int i = 0; i < credit_dis_dt.Rows.Count; i++)
                ribao_map[credit_dis_dt.Rows[i][0].ToString()].credit_dis = Convert.ToDouble(credit_dis_dt.Rows[i][1]);

            DataTable recep_dis_dt = get_datatable(String.Format(sql_map["get_recep_dis"], dateText.Text));
            for (int i = 0; i < recep_dis_dt.Rows.Count; i++)
                ribao_map[recep_dis_dt.Rows[i][0].ToString()].recep_dis = Convert.ToDouble(recep_dis_dt.Rows[i][1]);

            DataTable card_dis_dt = get_datatable(String.Format(sql_map["get_card_dis"], dateText.Text));
            for (int i = 0; i < card_dis_dt.Rows.Count; i++)
                ribao_map[card_dis_dt.Rows[i][0].ToString()].card_dis = Convert.ToDouble(card_dis_dt.Rows[i][1]);

            DataTable ticket_dis_dt = get_datatable(String.Format(sql_map["get_ticket_dis"], dateText.Text));
            for (int i = 0; i < ticket_dis_dt.Rows.Count; i++)
            {
                ribao_map[ticket_dis_dt.Rows[i][0].ToString()].ticket_dis = Convert.ToDouble(ticket_dis_dt.Rows[i][1]);
                DataTable ticket_dis_dt_2 = get_datatable(String.Format(sql_map["get_ticket_dis_2"], dateText.Text, ticket_dis_dt.Rows[i][0].ToString()));
                for (int j = 0; j < ticket_dis_dt_2.Rows.Count; j++)
                {
                    if (Convert.ToInt16(ticket_dis_dt_2.Rows[j][1]) > 1)
                    {
                        ribao_map[ticket_dis_dt.Rows[i][0].ToString()].ticket_dis -= Convert.ToDouble(ticket_dis_dt_2.Rows[j][2]);
                    }
                }
            }


            DataTable member_card_sale_dt = get_datatable(String.Format(sql_map["get_order_non_free"], dateText.Text));
            for (int i = 0; i < member_card_sale_dt.Rows.Count; i++)
            {
                for (int j = 0; j < member_card_dt.Rows.Count; j++)
                {
                    if (member_card_dt.Rows[j][0].ToString().Trim() == member_card_sale_dt.Rows[i][0].ToString().Trim() &&
                        member_card_dt.Rows[j][1].ToString().Trim() == member_card_sale_dt.Rows[i][1].ToString().Trim())
                    {
                        ribao_map[member_card_sale_dt.Rows[i][2].ToString()].member_card_sale += Convert.ToDouble(member_card_sale_dt.Rows[i][3]);
                        break;
                    }
                }
            }

            DataTable member_card_free_dt = get_datatable(String.Format(sql_map["get_order_free"], dateText.Text));
            for (int i = 0; i < member_card_free_dt.Rows.Count; i++)
            {
                for (int j = 0; j < member_card_dt.Rows.Count; j++)
                {
                    if (member_card_dt.Rows[j][0].ToString().Trim() == member_card_free_dt.Rows[i][0].ToString().Trim() &&
                        member_card_dt.Rows[j][1].ToString().Trim() == member_card_free_dt.Rows[i][1].ToString().Trim())
                    {
                        ribao_map[member_card_free_dt.Rows[i][2].ToString()].member_card_free += Convert.ToDouble(member_card_free_dt.Rows[i][3]);
                        break;
                    }
                }
            }
            
            DataTable credit_gift_sale_dt = get_datatable(String.Format(sql_map["get_order_non_free"], dateText.Text));
            for (int i = 0; i < credit_gift_sale_dt.Rows.Count; i++)
            {
                for (int j = 0; j < credit_gift_dt.Rows.Count; j++)
                {
                    if (credit_gift_dt.Rows[j][0].ToString().Trim() == credit_gift_sale_dt.Rows[i][0].ToString().Trim() &&
                        credit_gift_dt.Rows[j][1].ToString().Trim() == credit_gift_sale_dt.Rows[i][1].ToString().Trim())
                    {
                        ribao_map[credit_gift_sale_dt.Rows[i][2].ToString()].credit_gift_sale += Convert.ToDouble(credit_gift_sale_dt.Rows[i][3]);
                        break;
                    }
                }
            }

            DataTable credit_gift_free_dt = get_datatable(String.Format(sql_map["get_order_free"], dateText.Text));
            for (int i = 0; i < credit_gift_free_dt.Rows.Count; i++)
            {
                for (int j = 0; j < credit_gift_dt.Rows.Count; j++)
                {
                    if (credit_gift_dt.Rows[j][0].ToString().Trim() == credit_gift_free_dt.Rows[i][0].ToString().Trim() &&
                        credit_gift_dt.Rows[j][1].ToString().Trim() == credit_gift_free_dt.Rows[i][1].ToString().Trim())
                    {
                        ribao_map[credit_gift_free_dt.Rows[i][2].ToString()].credit_gift_free += Convert.ToDouble(credit_gift_free_dt.Rows[i][3]);
                        break;
                    }
                }
            }
            
            DataTable outsale_credit_sale_dt = get_datatable(String.Format(sql_map["get_order_non_free"], dateText.Text));
            for (int i = 0; i < outsale_credit_sale_dt.Rows.Count; i++)
            {
                for (int j = 0; j < outsale_credit_dt.Rows.Count; j++)
                {
                    if (outsale_credit_dt.Rows[j][0].ToString().Trim() == outsale_credit_sale_dt.Rows[i][0].ToString().Trim() &&
                        outsale_credit_dt.Rows[j][1].ToString().Trim() == outsale_credit_sale_dt.Rows[i][1].ToString().Trim())
                    {
                        ribao_map[outsale_credit_sale_dt.Rows[i][2].ToString()].outsale_credit_sale += Convert.ToDouble(outsale_credit_sale_dt.Rows[i][3]);
                        break;
                    }
                }
            }
            
            DataTable outsale_credit_free_dt = get_datatable(String.Format(sql_map["get_order_free"], dateText.Text));
            for (int i = 0; i < outsale_credit_free_dt.Rows.Count; i++)
            {
                for (int j = 0; j < outsale_credit_dt.Rows.Count; j++)
                {
                    if (outsale_credit_dt.Rows[j][0].ToString().Trim() == outsale_credit_free_dt.Rows[i][0].ToString().Trim() &&
                        outsale_credit_dt.Rows[j][1].ToString().Trim() == outsale_credit_free_dt.Rows[i][1].ToString().Trim())
                    {
                        ribao_map[outsale_credit_free_dt.Rows[i][2].ToString()].outsale_credit_free += Convert.ToDouble(outsale_credit_free_dt.Rows[i][3]);
                        break;
                    }
                }
            }

            foreach (string key in ribao_map.Keys)
            {
                import_ribao_table(key, ribao_map[key]);
                /*MessageBox.Show(String.Format("{0}: {1} {2} {3} {4} {5} {6} {7}", key, ribao_map[key].branch,
                            ribao_map[key].person, ribao_map[key].get_meal_income(), ribao_map[key].get_member_card_income(),
                            ribao_map[key].item_dis, ribao_map[key].cat_dis, ribao_map[key].order_dis
                           ));*/
                
            }
            //MessageBox.Show("日报导入完成");
        }

        private void dailyinputBtn_Click(object sender, EventArgs e)
        {
            //import_daily_ribao();
            Thread t = new Thread(import_daily_dbf);
            t.Start();
        }

        private void delete_daily_dbf()
        {
            logList.Items.Clear();
            Delete_daily_table(dateText.Text);
            //delete_ribao_table();
        }
        private void dailydeleteBtn_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(delete_daily_dbf);
            t.Start();
        }

        private DataTable get_datatable(string sql)
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();

            /*Connect to Sql server*/
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=6000";
            sql_conn = new SqlConnection(strConnection);
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            try
            {
                sql_cmd.Connection.Open();
                da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
                da.Fill(dt);
                sql_cmd.Connection.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return dt;
        }

        private void delete_ribao_table()
        {
            string conn_str = "server=rds3vt9mpg9qj5biy748a.mysql.rds.aliyuncs.com;User Id=fsradmin;password=fsradmin;Database=fsr";
            MySqlConnection conn = new MySqlConnection(conn_str);
            
            string del_str = "delete from ribao where date=\'{0}\'";
            MySqlCommand cmd = new MySqlCommand(String.Format(del_str, dateText.Text), conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            del_str = "delete from ribao_dis where date=\'{0}\'";
            MySqlCommand cmd_dis = new MySqlCommand(String.Format(del_str, dateText.Text), conn);
            conn.Open();
            cmd_dis.ExecuteNonQuery();
            conn.Close();
        }
        private void import_ribao_table(string outlet, ribao_t r)
        {
            string conn_str = "server=rds3vt9mpg9qj5biy748a.mysql.rds.aliyuncs.com;User Id=fsradmin;password=fsradmin;Database=fsr";
            MySqlConnection conn = new MySqlConnection(conn_str);
            string insert_str = "insert into ribao(outlet, branch, brand, area, person, meal_income, member_income, outsale_income, credit_income, total_income, total_discount, date) values(\'{0}\', \'{1}\', \'{2}\', \'{3}\', {4}, {5}, {6}, {7}, {8}, {9}, {10}, \'{11}\')";
            MySqlCommand cmd = new MySqlCommand(String.Format(insert_str, outlet, r.branch, r.band, r.area, r.person, r.get_meal_income(), r.get_member_card_income(),
                    r.get_outsale_income(), r.get_credit_gift_income(), r.get_income(), r.get_discount(), dateText.Text), conn);
            //MessageBox.Show(cmd.CommandText);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            string sql_dis_str = "insert into ribao_dis(outlet, branch, brand, area, item_dis, cat_dis, order_dis, rounding_dis, free_dis, credit_dis, recep_dis, card_dis, ticket_dis, total_dis, date) values(\'{0}\', \'{1}\', \'{2}\', \'{3}\', {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, \'{14}\')";
            MySqlCommand cmd_dis = new MySqlCommand(String.Format(sql_dis_str, outlet, r.branch, r.band, r.area, r.item_dis, r.cat_dis + r.order_dis, r.order_dis, r.rounding_amt, r.free_dis, r.credit_dis,r.recep_dis, r.card_dis, r.ticket_dis, r.get_discount(), dateText.Text), conn);
            conn.Open();
            cmd_dis.ExecuteNonQuery();
            conn.Close();
            
            /*
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            SqlDataAdapter da;

           
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);
            string sql = String.Format("insert into dbo.ribao values(\'{0}\', \'{1}\', \'{2}\', \'{3}\', {4}, {5}, {6}, {7}, {8}, {9}, {10}, \'{11}\')",
                    outlet, r.branch, r.band, r.area, r.person, r.get_meal_income(), r.get_member_card_income(),
                    r.get_outsale_income(), r.get_credit_gift_income(), r.get_income(), r.get_discount(), dateText.Text);
            sql_cmd = new SqlCommand(sql);
            sql_cmd.Connection = sql_conn;
            sql_cmd.Connection.Open();
            sql_cmd.ExecuteNonQuery();
            sql_cmd.Connection.Close();
*/
            return;
        }

        private DataTable get_daily_t_order_tables()
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();

            /*Connect to Sql server*/
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);
            sql_cmd = new SqlCommand("select * from dbo.T_ORDER where DATEA=\'" + dateText.Text + "\' order by OUTLET");
            sql_cmd.Connection = sql_conn;
            sql_cmd.Connection.Open();
            da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
            da.Fill(dt);
            sql_cmd.Connection.Close();
            return dt;
        }

        private DataTable get_daily_att_tables()
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();

            /*Connect to Sql server*/
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);
            sql_cmd = new SqlCommand("select * from dbo.att");
            sql_cmd.Connection = sql_conn;
            sql_cmd.Connection.Open();
            da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
            da.Fill(dt);
            sql_cmd.Connection.Close();
            return dt;
        }

        private DataTable get_daily_t_att_tables(string outlet)
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            SqlDataAdapter da;
            DataTable dt = new DataTable();

            /*Connect to Sql server*/
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);
            sql_cmd = new SqlCommand("select * from dbo.t_att where OUTLET=\'" + outlet + "\' and DATEA=\'" + dateText.Text + "\'");
            sql_cmd.Connection = sql_conn;
            sql_cmd.Connection.Open();
            da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
            da.Fill(dt);
            sql_cmd.Connection.Close();
            return dt;
        }

        private void get_daily_prop_fields(DataTable att_dt, DataTable t_att_dt, string pp, string outlet, string ref_num, string sub_ref, string typea,
                                    string item_code, string item_idx, 
                                    string qty, string amt, string idisc_type, string idisc_qty, string item_disc, string cat_disc,
                                    string order_disc, string cancel, string free, string cost)
        {
            if (t_att_dt.Rows.Count == 0) { }
            //MessageBox.Show("NO item found: " + outlet + " " + item_idx);
            else
            {
                string msg = "item found: " + outlet + " " + item_idx + "\n";
                //for (int i = 0; i < t_att_dt.Rows.Count; i++)
                //d    msg = msg + " GROUPA: " + t_att_dt.Rows[i][4].ToString() + " CODE; " + t_att_dt.Rows[i][5].ToString() + "\n";
                

                string prop = "";
                string num = "";
                string count = "";
                for (int i = 0; i < t_att_dt.Rows.Count; i++)
                {
                    switch (t_att_dt.Rows[i][4].ToString())
                    {
                        case "00":
                            break;
                        case "01":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "01" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    num = num + att_dt.Rows[j][3].ToString().Trim();
                                    break;
                                }
                            }
                            break;
                        case "02":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "02" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    num = num + att_dt.Rows[j][3].ToString().Trim();
                                    break;
                                }
                            }
                            break;
                        case "03":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "03" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    count = att_dt.Rows[j][3].ToString().Trim();
                                    break;
                                }
                            }
                            break;
                        case "04":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "04" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    count = att_dt.Rows[j][3].ToString().Trim();
                                    break;
                                }
                            }
                            break;
                        case "0Z":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0Z" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    count = att_dt.Rows[j][3].ToString().Trim();
                                    break;
                                }
                            }
                            break;
                        case "0A":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0A" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0B":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0B" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0C":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0C" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0D":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0D" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0F":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0F" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "A0":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "A0" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "B0":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "B0" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "C0":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "C0" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "D0":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "D0" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "E0":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "E0" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0L":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0L" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0H":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0H" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0E":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0E" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0G":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0G" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0I":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0I" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0K":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0K" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0J":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0J" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0M":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0M" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0N":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0N" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    prop = prop + att_dt.Rows[j][3].ToString().Trim() + " ";
                                    break;
                                }
                            }
                            break;
                        case "0X":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0X" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    count = att_dt.Rows[j][3].ToString().Trim();
                                    break;
                                }
                            }
                            break;
                        case "0Y":
                            for (int j = 0; j < att_dt.Rows.Count; j++)
                            {
                                if (att_dt.Rows[j][1].ToString().Trim() == "0Y" && att_dt.Rows[j][2].ToString().Trim() == t_att_dt.Rows[i][5].ToString().Trim() && att_dt.Rows[j][0].ToString() == pp)
                                {
                                    count = att_dt.Rows[j][3].ToString().Trim();
                                    break;
                                }
                            }
                            break;
                        default:
                            MessageBox.Show("unhandled groupa type: " + t_att_dt.Rows[i][4].ToString());
                            break;
                    }
                }
                if (typea != "N")
                {
                    t_att_dt.Clear();
                    return;
                }
                 
                msg = msg + " 口味:" + prop + " 斤两; " + num + " 数量:" + count;
                logList.Items.Insert(0, msg);
               
                insert_daily_att_info(prop, num, count, pp, t_att_dt.Rows[0][1].ToString(), ref_num, sub_ref, typea, item_code, item_idx, 
                                            qty, amt, idisc_type, idisc_qty, item_disc, cat_disc, order_disc, cancel, free, cost, 
                                                    t_att_dt.Rows[0][6].ToString(), t_att_dt.Rows[0][7].ToString());
            }

            t_att_dt.Clear();
            return;
        }
        private void insert_daily_att_info(string flavor, string weight, string amount, string pp, string outlet, 
                                    string ref_num, string sub_ref, string typea, string item_code, string item_idx, 
                                    string qty, string amt, string idisc_type, string idisc_qty, string item_disc, string cat_disc,
                                    string order_disc, string cancel, string free, string cost, string att_amt, string datea)
        {
            SqlConnection sql_conn;
            SqlCommand sql_cmd;
            string sql;

            /*Connect to Sql server*/
            string strConnection = "user id=sa;password=wang419420;";
            strConnection += "initial catalog=" + dbname + ";Server=192.168.3.101\\foodsd;";
            strConnection += "Connect Timeout=30";
            sql_conn = new SqlConnection(strConnection);
            if (weight.Trim() == "")
                weight = "0.0";
            else
            {
                try
                {
                    int qty_num = Convert.ToInt32(qty);
                    double weight_num = Convert.ToDouble(weight);
                    weight_num = qty_num * weight_num;
                    weight = weight_num.ToString();
                }
                catch(Exception ex)
                {
                }
            }
            if (typea != "N")
                return;
            ref_num = outlet + ref_num + item_idx + String.Join("", datea.Split(' ')[0].Split('-')) + qty;
            sql = String.Format("insert into dbo.att_info values(\'{0}\',\'{1}\',\'{2}\',\'{3}\',\'{4}\',\'{5}\',\'{6}\',\'{7}\',\'{8}\',\'{9}\',\'{10}\',\'{11}\',\'{12}\',\'{13}\',\'{14}\',\'{15}\',\'{16}\',\'{17}\',\'{18}\', \'{19}\', \'{20}\', \'{21}\')", 
                                    pp, outlet, ref_num, sub_ref, typea, item_code, item_idx, qty, amt, idisc_type, idisc_qty, item_disc, cat_disc, order_disc, cancel, free, cost, flavor, weight, amount, att_amt, datea);
            sql_cmd = new SqlCommand();
            sql_cmd.Connection = sql_conn;
            sql_cmd.CommandText = sql;
            try
            {
                sql_cmd.Connection.Open();
                sql_cmd.ExecuteNonQuery();
                sql_cmd.Connection.Close();
            }
            catch
            {
                sql_cmd.Connection.Close();
            }
            return;
        }
        private void handle_daily_prop()
        {
            DataTable t_order_dt = get_daily_t_order_tables();
            DataTable att_dt = get_daily_att_tables();
            DataTable t_att_dt = new DataTable();
            DataTable t_att_outlet_dt = new DataTable();
            string outlet = "";

            for (int i = 0; i < t_order_dt.Rows.Count; i++ )
            {
                if (t_order_dt.Rows[i][1].ToString() != outlet)
                {
                    t_att_dt.Clear();
                    outlet = t_order_dt.Rows[i][1].ToString();
                    t_att_dt = get_daily_t_att_tables(outlet);
                    t_att_outlet_dt = t_att_dt.Clone();
                }
                if (outlet == "001") /*drop HK branch*/
                    continue;

                for (int j = 0; j < t_att_dt.Rows.Count; j++)
                {
                    if (t_order_dt.Rows[i][1].ToString() == t_att_dt.Rows[j][1].ToString() && t_order_dt.Rows[i][7].ToString() == t_att_dt.Rows[j][2].ToString()) //&& t_order_dt.Rows[4].ToString().Trim() == "N")
                    {
                        t_att_outlet_dt.Rows.Add(t_att_dt.Rows[j].ItemArray);
                        //Console.WriteLine(t_att_dt.Rows[j].ItemArray.ToString());
                    }
                }

                if (t_att_outlet_dt.Rows.Count == 0)
                {   
                    insert_daily_att_info("", "", "", t_order_dt.Rows[i][0].ToString(), t_order_dt.Rows[i][1].ToString(), t_order_dt.Rows[i][2].ToString(), t_order_dt.Rows[i][3].ToString(),
                        t_order_dt.Rows[i][4].ToString(), t_order_dt.Rows[i][6].ToString(), t_order_dt.Rows[i][7].ToString(),
                                            t_order_dt.Rows[i][15].ToString(), t_order_dt.Rows[i][16].ToString(), t_order_dt.Rows[i][18].ToString(), t_order_dt.Rows[i][19].ToString(),
                                        t_order_dt.Rows[i][20].ToString(), t_order_dt.Rows[i][21].ToString(), t_order_dt.Rows[i][22].ToString(),
                                        t_order_dt.Rows[i][24].ToString(), t_order_dt.Rows[i][25].ToString(), t_order_dt.Rows[i][26].ToString(),
                                                    "", t_order_dt.Rows[i][11].ToString());
                    continue;
                }
                else
                {
                    /*for ( i = 0; i< t_att_outlet_dt.Rows.Count; i++)
                    {
                        Console.WriteLine(t_att_outlet_dt.Rows[i][0] + " " + t_att_outlet_dt.Rows[i][1] + " " + t_att_outlet_dt.Rows[i][2] + " " + t_att_outlet_dt.Rows[i][3] + " " + t_att_outlet_dt.Rows[i][4]);
                    }*/
                    get_daily_prop_fields(att_dt, t_att_outlet_dt, t_order_dt.Rows[i][0].ToString(), t_order_dt.Rows[i][1].ToString(), t_order_dt.Rows[i][2].ToString(), t_order_dt.Rows[i][3].ToString(),
                                t_order_dt.Rows[i][4].ToString(), t_order_dt.Rows[i][6].ToString(), t_order_dt.Rows[i][7].ToString(),
                                t_order_dt.Rows[i][15].ToString(), t_order_dt.Rows[i][16].ToString(), t_order_dt.Rows[i][18].ToString(), t_order_dt.Rows[i][19].ToString(),
                                t_order_dt.Rows[i][20].ToString(), t_order_dt.Rows[i][21].ToString(), t_order_dt.Rows[i][22].ToString(),
                                t_order_dt.Rows[i][24].ToString(), t_order_dt.Rows[i][25].ToString(), t_order_dt.Rows[i][26].ToString());
                    
                }
                t_att_outlet_dt.Clear();
            }

            att_dt.Clear();
            t_order_dt.Clear();
                        
            MessageBox.Show("更新完成");
        }
        private void propBtn_Click(object sender, EventArgs e)
        {
            logList.Items.Clear();
            Thread t = new Thread(handle_daily_prop);
            t.Start();
        }

        private void baseBtnDelete_Click(object sender, EventArgs e)
        {
            Delete_base_table();
            MessageBox.Show("基础表删除成功");
        }

        private void propdelbtn_Click(object sender, EventArgs e)
        {
            logList.Items.Clear();
            Delete_attinfo_table(dateText.Text);
            MessageBox.Show("删除" + dateText.Text + " att_info表成功");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //import_daily_ribao();
            Thread t = new Thread(check_dbf_date);
            t.Start();
        }

        private void btnBaobiao_Click(object sender, EventArgs e)
        {
            int i = 0, j = 0;
            Decimal d = new Decimal(0), zkou=new Decimal(0);

            if (dateEnd.Text.Trim() == "")
                dateEnd.Text = dateFrom.Text;

            logList.Items.Clear();
            logList.Items.Add("门店" + departCombo.SelectedItem.ToString() + " (" + dateFrom.Text + "-" + dateEnd.Text + ") 报表数据");
            logList.Items.Add("---------------------------------------");
            logList.Items.Add("");

            sqlhelper sh = new sqlhelper();
            sh.sqlserver_set_connstr("user id=sa;password=wang419420; initial catalog=banzhaoyun; Server=192.168.3.101\\foodsd;Connect Timeout=30");

            Baobiaohelper bbh = new Baobiaohelper();

            string sql_cmd = string.Format(bbh.get_sql_command("mfei"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString(), depart_pp_map[departCombo.SelectedItem.ToString()]);
            DataTable dt = sh.sqlserver_execute_get_dt(sql_cmd);
            if (dt.Rows.Count != 0)
                d = Convert.ToDecimal(dt.Rows[0][1].ToString());

            sql_cmd = string.Format(bbh.get_sql_command("lrun"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString());
            dt = sh.sqlserver_execute_get_dt(sql_cmd);
            logList.Items.Add("食品原价：" + (Convert.ToDecimal(dt.Rows[0][3].ToString()) + d).ToString());
            logList.Items.Add("大单利润：" + dt.Rows[0][1].ToString());
            logList.Items.Add("销售总额：" + (Convert.ToDecimal(dt.Rows[0][2].ToString()) + d).ToString());

            sql_cmd = string.Format(bbh.get_sql_command("jdui"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString(), depart_pp_map[departCombo.SelectedItem.ToString()]);
            dt = sh.sqlserver_execute_get_dt(sql_cmd);
            if (dt.Rows.Count != 0)
                logList.Items.Add("积兑礼额：" + dt.Rows[0][1].ToString());
            else
                logList.Items.Add("积兑礼额：0");

            sql_cmd = string.Format(bbh.get_sql_command("zkou"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString(), depart_pp_map[departCombo.SelectedItem.ToString()]);
            dt = sh.sqlserver_execute_get_dt(sql_cmd);
            for (i = 0; i < dt.Rows.Count; i++)
                zkou += Convert.ToDecimal(dt.Rows[i][2].ToString());

            logList.Items.Add("折扣总额：" + (zkou-d).ToString());

            logList.Items.Add("");
            logList.Items.Add("================================");

            d = 0;
            logList.Items.Add("食品收款方式");
            sql_cmd = string.Format(bbh.get_sql_command("food_income"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString(), depart_pp_map[departCombo.SelectedItem.ToString()]);
            dt = sh.sqlserver_execute_get_dt(sql_cmd);
            for (i = 0; i < dt.Rows.Count; i++)
            {
                logList.Items.Add(dt.Rows[i][1].ToString() + ":\t" + dt.Rows[i][2].ToString());
                d += Convert.ToDecimal(dt.Rows[i][2].ToString());
            }
            logList.Items.Add("总额：\t" + d.ToString());

            logList.Items.Add("================================");
            d = 0;
            logList.Items.Add("充值及储值收款方式");
            sql_cmd = string.Format(bbh.get_sql_command("czhi_income"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString(), depart_pp_map[departCombo.SelectedItem.ToString()]);
            dt = sh.sqlserver_execute_get_dt(sql_cmd);
            for (i = 0; i < dt.Rows.Count; i++)
            {
                logList.Items.Add(dt.Rows[i][1].ToString() + ":\t" + dt.Rows[i][2].ToString());
                d += Convert.ToDecimal(dt.Rows[i][2].ToString());
            }
            logList.Items.Add("总额：\t" + d.ToString());

            logList.Items.Add("================================");
            d = 0;
            logList.Items.Add("挂账回收收款方式");
            sql_cmd = string.Format(bbh.get_sql_command("gzhang_income"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString(), depart_pp_map[departCombo.SelectedItem.ToString()]);
            dt = sh.sqlserver_execute_get_dt(sql_cmd);
            for (i = 0; i < dt.Rows.Count; i++)
            {
                logList.Items.Add(dt.Rows[i][1].ToString() + ":\t" + dt.Rows[i][2].ToString());
                d += Convert.ToDecimal(dt.Rows[i][2].ToString());
            }
            logList.Items.Add("总额：\t" + d.ToString());

            logList.Items.Add("================================");
            d = 0;
            logList.Items.Add("签单预付收款方式");
            sql_cmd = string.Format(bbh.get_sql_command("qdan_income"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString(), depart_pp_map[departCombo.SelectedItem.ToString()]);
            dt = sh.sqlserver_execute_get_dt(sql_cmd);
            for (i = 0; i < dt.Rows.Count; i++)
            {
                logList.Items.Add(dt.Rows[i][1].ToString() + ":\t" + dt.Rows[i][2].ToString());
                d += Convert.ToDecimal(dt.Rows[i][2].ToString());
            }
            logList.Items.Add("总额：\t" + d.ToString());

            logList.Items.Add("================================");
            d = 0;
            logList.Items.Add("定金收取/取消收款方式");
            sql_cmd = string.Format(bbh.get_sql_command("djin_income"), dateFrom.Text.Trim(), dateEnd.Text.Trim(), departCombo.SelectedItem.ToString(), depart_pp_map[departCombo.SelectedItem.ToString()]);
            dt = sh.sqlserver_execute_get_dt(sql_cmd);
            for (i = 0; i < dt.Rows.Count; i++)
            {
                logList.Items.Add(dt.Rows[i][1].ToString() + ":\t" + dt.Rows[i][2].ToString());
                d += Convert.ToDecimal(dt.Rows[i][2].ToString());
            }
            logList.Items.Add("总额：\t" + d.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("D:\\baobiao.txt");
            foreach (string line in logList.Items)
                sw.WriteLine(line);
            sw.Close();
            MessageBox.Show("数据导入到: D:\\baobiao.txt");
        }
    }

    public class ribao_t
    {
        public ribao_t(int person, double order_amt, double server_amt, double tips_amt, double rounding_amt,
                    double item_dis, double order_dis, double cat_dis)
        {
            this.person = person;
            this.order_amt = order_amt;
            this.server_amt = server_amt;
            this.tips_amt = tips_amt;
            this.rounding_amt = -rounding_amt;
            this.item_dis = item_dis;
            this.cat_dis = cat_dis;
            this.order_dis = order_dis;
        }

        public double get_member_card_income() { return this.member_card_sale - this.member_card_free; }
        public double get_credit_gift_income() { return this.credit_gift_sale - this.credit_gift_free; }
        public double get_outsale_income() { return this.outsale_credit_sale - this.outsale_credit_free - this.get_credit_gift_income(); }
        public double get_meal_income() { return this.get_income() - this.get_member_card_income() - this.get_outsale_income() - this.get_credit_gift_income(); }
        public double get_income() { return this.order_amt + this.server_amt + this.tips_amt + this.free_dis; }
        public double get_discount()
        {
            return this.card_dis + this.credit_dis + this.free_dis + this.recep_dis + this.ticket_dis
                    + this.rounding_amt + this.item_dis + this.cat_dis + this.order_dis;
        }

        public int person = 0;
        public string branch = "";
        public string band = "";
        public string area = "";
        public double order_amt;
        public double server_amt;
        public double tips_amt;
        public double rounding_amt;

        public double free_dis;
        public double credit_dis;
        public double recep_dis;
        public double card_dis;
        public double ticket_dis;
        public double item_dis;
        public double cat_dis;
        public double order_dis;

        public double member_card_sale = 0;
        public double member_card_free = 0;
        public double outsale_credit_sale = 0;
        public double outsale_credit_free = 0;
        public double credit_gift_sale = 0;
        public double credit_gift_free = 0;
    };

    class sqlhelper
    {
        public void sqlserver_execute_non_query(string sql_str)
        {
            //string sqlserver_conn_str = "user id=sa;password=wang419420; initial catalog=banzhaoyun; Server=192.168.3.101\\foodsd;Connect Timeout=30";

            SqlConnection sql_conn = new SqlConnection(this.conn_str);
            SqlCommand sql_cmd = new SqlCommand(sql_str);
            sql_cmd.Connection = sql_conn;

            sql_cmd.Connection.Open();
            sql_cmd.ExecuteNonQuery();
            sql_cmd.Connection.Close();
            sql_cmd.Connection.Dispose();
        }

        public DataTable sqlserver_execute_get_dt(string sql_str)
        {
            //string sqlserver_conn_str = "user id=sa;password=wang419420; initial catalog=banzhaoyun; Server=192.168.3.101\\foodsd;Connect Timeout=30";
            SqlConnection sql_conn = new SqlConnection(this.conn_str);
            SqlCommand sql_cmd = new SqlCommand(sql_str);
            sql_cmd.CommandTimeout = 300;
            DataTable dt = new DataTable();
            sql_cmd.Connection = sql_conn;

            sql_cmd.Connection.Open();
            SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
            da.Fill(dt);

            da.Dispose();
            sql_cmd.Connection.Close();
            sql_cmd.Connection.Dispose();

            return dt;
        }
        public void sqlserver_set_connstr(string str) { conn_str = str; }

        private string conn_str;
    }

    class Baobiaohelper
    {

        public Baobiaohelper()
        {
            this.sqlcmd_map.Add("food_income", "SELECT b.OUTLET,C.DESC1 as 付款名称, SUM(B.AMOUNT) as 付款金额 FROM T_TRAN A INNER JOIN T_PAY B ON A.PP=B.PP AND A.OUTLET=B.OUTLET AND A.REF_NUM=B.REF_NUM INNER JOIN payment2 C on B.PAY_TYPE=C.CODE AND B.PP=C.PP WHERE A.IN_DATE BETWEEN \'{0}\' AND \'{1}\' AND B.DATEA BETWEEN \'{0}\' AND \'{1}\' AND A.TRAN_TYPE=\'N\' AND B.TRAN_TYPE=\'N\' AND A.OUTLET=\'{2}\' AND B.OUTLET=\'{2}\' AND A.PP=\'{3}\' AND B.PP=\'{3}\' GROUP BY C.DESC1,PAY_TYPE,b.OUTLET");
            this.sqlcmd_map.Add("gzhang_income", "SELECT b.OUTLET,C.DESC1 as 付款名称,SUM(B.AMOUNT) as 付款金额 FROM A_TRAN A INNER JOIN A_PAY B ON  A.OUTLET=B.OUTLET AND A.REF_NUM=B.REF_NUM INNER JOIN payment2 C on B.PAY_TYPE=C.CODE WHERE A.IN_DATE BETWEEN \'{0}\' AND \'{1}\' 	AND B.DATE BETWEEN \'{0}' AND \'{1}\' AND A.TRAN_TYPE=\'N\' AND B.TRAN_TYPE=\'N\' AND A.OUTLET=\'{2}\' AND B.OUTLET=\'{2}\' AND A.REF_NUM LIKE \'%H%\' 	AND B.REF_NUM LIKE \'%H%\' And c.PP=\'{3}\' GROUP BY C.DESC1,PAY_TYPE,b.OUTLET");
            this.sqlcmd_map.Add("czhi_income", "SELECT b.OUTLET,C.DESC1 as 付款名称,SUM(B.AMOUNT) as 付款金额 FROM A_TRAN A INNER JOIN A_PAY B ON  A.OUTLET=B.OUTLET AND A.REF_NUM=B.REF_NUM INNER JOIN payment2 C on B.PAY_TYPE=C.CODE WHERE A.IN_DATE BETWEEN \'{0}\' AND \'{1}\' AND B.DATE BETWEEN \'{0}\' AND \'{1}\' AND A.TRAN_TYPE=\'N\' AND B.TRAN_TYPE=\'N\' AND A.OUTLET=\'{2}\' AND B.OUTLET=\'{2}\' AND A.REF_NUM LIKE \'%N%\' AND B.REF_NUM LIKE \'%N%\' and c.PP=\'{3}\' GROUP BY C.DESC1,PAY_TYPE,b.OUTLET");
            this.sqlcmd_map.Add("djin_income", "SELECT b.OUTLET,C.DESC1 as 付款名称,SUM(B.AMOUNT) as 付款金额 FROM A_TRAN A INNER JOIN A_PAY B ON  A.OUTLET=B.OUTLET AND A.REF_NUM=B.REF_NUM INNER JOIN payment2 C on B.PAY_TYPE=C.CODE WHERE A.IN_DATE BETWEEN \'{0}\' AND \'{1}\'	AND B.DATE BETWEEN \'{0}\' AND \'{1}\' AND A.TRAN_TYPE=\'N\' AND B.TRAN_TYPE=\'N\' AND A.OUTLET=\'{2}\' AND B.OUTLET=\'{2}\' AND A.REF_NUM LIKE \'%D%\'	AND B.REF_NUM LIKE \'%D%\' and c.PP=\'{3}\' GROUP BY C.DESC1,PAY_TYPE,b.OUTLET");
            this.sqlcmd_map.Add("qdan_income", "SELECT b.OUTLET,C.DESC1 as 付款名称,SUM(B.AMOUNT) as 付款金额 FROM A_TRAN A INNER JOIN A_PAY B ON  A.OUTLET=B.OUTLET AND A.REF_NUM=B.REF_NUM INNER JOIN payment2 C on B.PAY_TYPE=C.CODE WHERE A.IN_DATE BETWEEN \'{0}\' AND \'{1}\'	AND B.DATE BETWEEN \'{0}\' AND \'{1}\' AND A.TRAN_TYPE=\'N\' AND B.TRAN_TYPE=\'N\' AND A.OUTLET=\'{2}\' AND B.OUTLET=\'{2}\' AND A.REF_NUM LIKE \'%G%\' AND B.REF_NUM LIKE \'%G%\' and c.PP=\'{3}\' GROUP BY C.DESC1,PAY_TYPE,b.OUTLET");
            this.sqlcmd_map.Add("jdui", "SELECT outlet,SUM(ITEM_DISC) FROM T_ORDER WHERE DATEA BETWEEN \'{0}\' AND \'{1}\' AND PP=\'{3}\' and OUTLET=\'{2}\' AND TYPEA=\'N\' group by outlet");
            this.sqlcmd_map.Add("zkou", "SELECT OUTLET,b.DESC1,-SUM(AMT2) FROM T_LOG a inner join disc_hdHC b on a.pp=b.PP and a.REMARK3=b.CODE WHERE TYPEA=\'N\' and a.PP=\'{3}\' AND b.PP=\'{3}\' AND OUTLET=\'{2}\' AND DATE BETWEEN \'{0}\' AND \'{1}\' AND LOG_TYPE IN (\'DORP\',\'DORA\',\'DCTP\') GROUP BY b.DESC1,OUTLET");
            this.sqlcmd_map.Add("mfei", "SELECT OUTLET,-SUM(AMT) FROM T_ORDER WHERE DATEA BETWEEN \'{0}\' AND \'{1}\' AND TYPEA=\'N\' and PP=\'{3}\'	and outlet=\'{2}\' AND REASON IN (\'51\',\'52\') GROUP BY OUTLET");
            this.sqlcmd_map.Add("lrun", "SELECT OUTLET,SUM(AMT-QTY*COST) as 大单利润, SUM(AMT) as 销售总额, SUM(QTY*COST) as 食品原价 FROM T_ORDER WHERE SETMEAL<=\'0\' AND DATEA BETWEEN \'{0}\' AND \'{1}\'	AND OUTLET=\'{2}\' AND TYPEA=\'N\' GROUP BY OUTLET");
        }
        public String get_sql_command(string key) { return sqlcmd_map[key]; }

        private Dictionary<String, String> sqlcmd_map = new Dictionary<String, String>();
    }
}
