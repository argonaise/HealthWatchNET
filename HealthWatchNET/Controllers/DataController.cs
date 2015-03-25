using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using System.Text;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace HealthWatchNET.Controllers
{
    public class StoredPreset
    {
        public decimal SP_NO = 0;
        public string TST_CD_ARR = null;
        public decimal TST_CD_COUNT = 0;
        public string PSET_NAME = null;

        public override string ToString()
        {
            return "SP_NO: " + SP_NO + " TST_CD_ARR: " + TST_CD_ARR + " TST_CD_COUNT: " + TST_CD_COUNT + " PSET_NAME: " + PSET_NAME;
        }
    }

    public class DataController : Controller
    {
        //
        // GET: /Data/
        Database db = new Database();

        private static NameValueCollection GetEncodedForm(System.IO.Stream stream, Encoding encoding)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.ASCII);
            return GetEncodedForm(reader.ReadToEnd(), encoding);
        }


        private static NameValueCollection GetEncodedForm(string urlEncoded, Encoding encoding)
        {
            NameValueCollection form = new NameValueCollection();
            string[] pairs = urlEncoded.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (string pair in pairs)
            {
                string[] pairItems = pair.Split("=".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                string name = HttpUtility.UrlDecode(pairItems[0], encoding);
                string value = (pairItems.Length > 1) ? HttpUtility.UrlDecode(pairItems[1], encoding) : null;
                form.Add(name, value);
            }
            return form;
        }

        public DataController()
        {
        }

        ~DataController()
        {
            db.Close();
        }

        public List<StoredPreset> GetStoredPresetList(string userid)
        {
            List<StoredPreset> ret = new List<StoredPreset>();

            string sql = "SELECT SP_NO, TST_CD_COUNT, PSET_NAME FROM STOREDPRESET where UPDATE_ID='" + userid + "' order by SP_NO";
            Debug.WriteLine(sql);

            DataTable data = GetDataFromQuery(sql);

            foreach (DataRow row in data.Rows)
            {
                StoredPreset sp = new StoredPreset();

                sp.SP_NO = (decimal)row["SP_NO"];
                //sp.TST_CD_ARR = (string)row["TST_CD_ARR"];
                sp.TST_CD_COUNT = (decimal)row["TST_CD_COUNT"];
                sp.PSET_NAME = (string)row["PSET_NAME"];

                ret.Add(sp);

                Debug.WriteLine(sp.ToString());
            }

            return ret;
        }

        public ActionResult Index()
        {
            return View();
        }

        //public DataRow OneRowFetch(string table, string field, string where = "", string other = "")
        //{
        //    string sql = "SELECT " + field + " FROM " + table;
        //    if (where != "")
        //    {
        //        sql += " WHERE " + where;
        //    }
        //    if (other != "")
        //    {
        //        sql += " " + other;
        //    }

        //    sql = "SELECT * FROM ( " + sql + " ) WHERE ROWNUM <= 1";
        //    Debug.WriteLine(sql);
        //    DataTable data = GetDataFromQuery(sql);
        //    DataRow ret = null;
        //    if (data.Rows.Count > 0)
        //    {
        //        ret = data.Rows[0];
        //    }
        //    return ret;
        //}

        public ActionResult TestPresetItem()
        {
            db.Open();

            string query = Request.QueryString["query"];
            List<string> where = new List<string>();

            int offset = Int32.Parse(Request.QueryString["offset"]);
            int to = Int32.Parse(Request.QueryString["to"]);

            Debug.WriteLine("**************************************************");
            Debug.WriteLine("TestPresetItem: query : {0} offset : {1} to : {2}", query, offset, to);

            string sql = "SELECT TST_CD FROM ( SELECT TST_CD, ROWNUM rnum FROM ( SELECT TST_CD FROM STOREDPRESET_DATA WHERE SP_NO=" + query + " ORDER BY SEQ ASC ) WHERE ROWNUM <= " + (offset + to) + " ) WHERE rnum > " + offset;
            //SELECT TST_CD FROM ( SELECT TST_CD, ROWNUM rnum FROM ( SELECT TST_CD FROM STOREDPRESET_DATA WHERE SP_NO='" + query + "' ORDER BY SEQ ASC ) WHERE ROWNUM <= " + (offset + to) + " ) WHERE rnum > " + offset;
            Debug.WriteLine(sql);

            OracleCommand cmd = new OracleCommand(sql, db.conn);
            OracleDataReader reader = cmd.ExecuteReader();

            InterviewItem items = new InterviewItem();
            Dictionary<string, string> intv_items = items.getDictionary();

            // TST_CD_ARR 순서대로..
            string TST_CD;
            bool bFirst = true;

            List<string> item = new List<string>();
            int idx, cnt = 0;
            string intv_cd;

            StringBuilder str = new StringBuilder();

            while( reader.Read() ) {

                cnt++;
                TST_CD = reader.GetString(0);

                // INTV|| 가 붙은 것들은 문진표 항목들
                idx = TST_CD.IndexOf(".");
                
                if ( idx > 0 ) {
                    // 문진표 항목은 메모리에서 불러오고..
                    if (bFirst == true) bFirst = false;
                    else str.Append(",\n");

                    //intv_cd = TST_CD.Substring(idx + 1);
                    intv_cd = TST_CD;
                    Debug.WriteLine("intv_cd: " + intv_cd + " / intv_name: " + intv_items[intv_cd]);

                    string type = TST_CD.Substring(0, idx);

                    if (type == "P") type = "(환자정보)";
                    if (type == "R") type = "(예약정보)";
                    if (type == "I3") type = "(문진표)";

                    item.Clear();
                    item.Add("\t\t\"TST_CD\" : \"" + intv_cd + "\"");
                    item.Add("\t\t\"TST_NAME\" : \"" + intv_items[intv_cd] + " " + type + "\"");
                    str.Append("\t{\n" + string.Join(",\n", item) + "\n\t}");
                }
                else {
                    // 테스트 항목은 HCSCODLT에서 불러온다..
                    Table tbl = db.Table("HCSCODLT");
                    Dictionary<string, object> row = tbl.SelectFetch("LCODE AS TST_CD, CODENAME AS TST_NM", "LCODE='" + TST_CD + "'");
                    if ( row != null ) {
                        if (bFirst == true) bFirst = false;
                        else str.Append(",\n");

                        item.Clear();
                        item.Add("\t\t\"TST_CD\" : \"" + row["TST_CD"].ToString().Replace("\"", "&quot;").Replace("\\", "") + "\"");
                        item.Add("\t\t\"TST_NAME\" : \"" + row["TST_NM"].ToString().Replace("\"", "&quot;").Replace("\\", "") + "\"");
                        str.Append("\t{\n" + string.Join(",\n", item) + "\n\t}");
                    }
                }
            }

            string content = "{ \"sql\": \"" + sql + "\"\n, \"count\": " + cnt + ", \"total\": " + cnt + ", \"data\": [\n" + str.ToString() + "\n] }";

            db.Close();

            return Content(content, "application/json", System.Text.Encoding.UTF8);
        }

        public ActionResult TestPackageItem()
        {
            string query = Request.QueryString["query"];
            List<string> where = new List<string>();

            string sort = Request.QueryString["sort"];
            string dir = Request.QueryString["dir"];

            int offset = Int32.Parse(Request.QueryString["offset"]);
            int to = Int32.Parse(Request.QueryString["to"]);

            string order = null;
            if (sort != null && sort != "")
            {
                order = "order by " + sort + " " + dir;
            }
            else
            {
                order = "order by b.STARDATE asc";
            }

            string sql = "SELECT * FROM ( SELECT c.*, ROWNUM rnum FROM ( SELECT a.LCODE AS TST_CD, a.CODENAME as TST_NAME FROM HCSCODLT a, HCSUPKGE b WHERE b.PKGCODE='" + query + "' and a.LCODE=b.EXAMCODE(+) and a.GRPCODE='C0001' and a.USEYN='Y' " + order + " ) c WHERE ROWNUM <= " + (offset + to) + " ) WHERE rnum > " + offset;
            Debug.WriteLine(sql);

            DataTable data = GetDataFromQuery(sql);

            string content = "{ \"sql\": \"" + sql + "\"\n, \"count\": " + data.Rows.Count + ", \"total\": " + data.Rows.Count + ", \"data\": [\n";

            bool bFirst = true;
            List<string> item = new List<string>();
            foreach (DataRow row in data.Rows)
            {
                if (bFirst == true) bFirst = false;
                else content += ",\n";

                item.Clear();
                foreach (DataColumn col in data.Columns)
                {
                    string field = col.ColumnName;
                    item.Add("\t\t\"" + field + "\" : \"" + row[field].ToString().Replace("\"", "&quot;").Replace("\\", "") + "\"");
                }

                content += "\t{\n" + string.Join(",\n", item) + "\n\t}";
                Debug.WriteLine(content);
            }

            content += "\n] }";

            return Content(content, "application/json", System.Text.Encoding.UTF8);
        }

        public ActionResult TestPackage()
        {
            string query = Request.QueryString["query"];
            List<string> where = new List<string>();

            string sort = Request.QueryString["sort"];
            string dir = Request.QueryString["dir"];

            int offset = Int32.Parse(Request.QueryString["offset"]);
            int to = Int32.Parse(Request.QueryString["to"]);

            if (query != null && query != "")
            {
                //where.Add("( PKGNM like '%" + query + "%' )");
                where.Add("( REGEXP_LIKE(PKGNM, '" + query + "', 'i') )");
            }

            string order = null;
            if (sort != null && sort != "")
            {
                order = "order by " + sort + " " + dir;
            }
            else
            {
                order = "order by EDATE desc";
            }

            string cond = null;
            if (where.Count > 0)
            {
                cond = "WHERE (" + string.Join(" and ", where.ToArray()) + ")";
            }

            //string sql = "SELECT * FROM ( SELECT a.NO, a.PKGCODE, a.PKGNAME, a.FDATE, b.PKGCOUNT, ROWNUM rnum FROM ( SELECT NO, PKGCODE, PKGNAME, FDATE FROM TESTPACKAGE " + cond + " " + order + " ) a, ( SELECT PKGCODE, count(*) PKGCOUNT GROUP BY PKGCODE ) b WHERE a.PKGCODE=b.PKGCODE AND a.ROWNUM <= " + (offset + to) + " ) WHERE rnum > " + offset;
            string sql = "SELECT * FROM ( SELECT c.*, ROWNUM rnum FROM ( SELECT a.PKGCODE, a.PKGNAME, a.EDATE, b.PKGCOUNT FROM ( SELECT PKGCODE, PKGNM as PKGNAME, EDATE FROM HCSUPKGM " + cond + " " + order + " ) a, ( SELECT PKGCODE, count(*) PKGCOUNT FROM HCSUPKGE GROUP BY PKGCODE ) b WHERE a.PKGCODE=b.PKGCODE ) c WHERE ROWNUM <= " + (offset + to) + " ) WHERE rnum > " + offset;
            Debug.WriteLine(sql);

            DataTable data = GetDataFromQuery(sql);

            string content = "{ \"sql\": \"" + sql + "\"\n, \"count\": " + data.Rows.Count + ", \"total\": " + data.Rows.Count + ", \"data\": [\n";

            bool bFirst = true;
            List<string> item = new List<string>();
            foreach (DataRow row in data.Rows)
            {
                if (bFirst == true) bFirst = false;
                else content += ",\n";

                item.Clear();
                foreach (DataColumn col in data.Columns)
                {
                    string field = col.ColumnName;
                    item.Add("\t\t\"" + field + "\" : \"" + row[field].ToString().Replace("\"", "&quot;").Replace("\\", "") + "\"");
                }

                content += "\t{\n" + string.Join(",\n", item) + "\n\t}";
                Debug.WriteLine(content);
            }

            content += "\n] }";

            return Content(content, "application/json", System.Text.Encoding.UTF8);
        }

        public ActionResult TestItem()
        {
            string query = Request.QueryString["query"];
            List<string> where = new List<string>();

            string sort = Request.QueryString["sort"];
            string dir = Request.QueryString["dir"];

            int offset = Int32.Parse( Request.QueryString["offset"] );
            int to = Int32.Parse( Request.QueryString["to"] );

            where.Add("GRPCODE='C0001' AND USEYN='Y'");
                        
            if (query != null && query != "")
            {
                //where.Add("( CODENAME like '%" + query + "%' )");
                where.Add("( REGEXP_LIKE(CODENAME, '" + query + "', 'i') )");
            }

            string order = null;
            if (sort != null && sort != "")
            {
                order = "order by " + sort + " " + dir;
            }
            else
            {
                //order = "order by SET_SDATE desc";
            }

            string cond = null;
            if (where.Count > 0)
            {
                cond = "WHERE (" + string.Join(" and ", where.ToArray()) + ")";
            }

            string sql = "SELECT * FROM ( SELECT a.*, ROWNUM rnum FROM ( SELECT LCODE AS TST_CD, CODENAME as TST_NAME FROM HCSCODLT " + cond + " " + order + " ) a WHERE ROWNUM <= " + (offset + to) + " ) WHERE rnum > " + offset;
            Debug.WriteLine(sql);

            DataTable data = GetDataFromQuery(sql);

            string content = "{ \"sql\": \"" + sql + "\"\n, \"count\": " + data.Rows.Count + ", \"total\": " + data.Rows.Count + ", \"data\": [\n";

            bool bFirst = true;
            List<string> item = new List<string>();
            foreach (DataRow row in data.Rows)
            {
                if (bFirst == true) bFirst = false;
                else content += ",\n";

                item.Clear();
                foreach (DataColumn col in data.Columns)
                {
                    string field = col.ColumnName;
                    item.Add("\t\t\"" + field + "\" : \"" + row[field].ToString().Replace("\"", "&quot;").Replace("\\", "") + "\"");
                }

                content += "\t{\n" + string.Join(",\n", item) + "\n\t}";
                Debug.WriteLine(content);
            }

            content += "\n] }";
            
            return Content( content, "application/json", System.Text.Encoding.UTF8 );
        }

        public ActionResult PrcDelPreset()
        {
            string sp_no = Request.QueryString["SP_NO"];
            string[] sp_no_arr = sp_no.Split(';');
            string sql = "";

            db.Open();

            try
            {
                for (int i = 0; i < sp_no_arr.Length; i++)
                {
                    if (sp_no_arr[i] != "")
                    {
                        OracleCommand cmd = db.conn.CreateCommand();
                        OracleTransaction tr;

                        // start transaction
                        tr = db.conn.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = tr;

                        try
                        {
                            sql = "DELETE FROM STOREDPRESET WHERE SP_NO='" + sp_no_arr[i] + "'";
                            Debug.WriteLine(sql);
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();

                            sql = "DELETE FROM STOREDPRESET_DATA WHERE SP_NO='" + sp_no_arr[i] + "'";
                            Debug.WriteLine(sql);
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();

                            tr.Commit();
                        }
                        catch (Exception e)
                        {
                            tr.Rollback();
                            Debug.WriteLine(e.ToString());
                            Debug.WriteLine(e.StackTrace);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                db.Close();
            }

            return RedirectToAction("TestSearch", "Home");
        }

        public ActionResult PrcSavePreset()
        {
            string sp_no_edit = Request.Form["sp_no"];

            string name = Request.Form["preset_name"];
            string tst_cd_arr = Request.Form["preset_test_code"];
            string tst_name_arr = Request.Form["preset_test_name"];
            string[] tst_cd_arr_split = Request.Form["preset_test_code"].Split(';');
            string[] tst_name_arr_split = Request.Form["preset_test_name"].Split('^');
            int tst_cd_cnt = tst_cd_arr_split.Length - 1;
            if (tst_cd_cnt < 0) tst_cd_cnt = 0;

            string sql = "";
            db.Open();

            try
            {
                OracleCommand cmd;

                // AUTO-INCREMENTED INDEX를 얻는다..
                Table tbl = db.Table();
                tbl.SelectQuery("SELECT STOREDPRESET_SEQ.NEXTVAL + 1 as SEQ FROM dual");
                Dictionary<string, object> row = tbl.Fetch();
                string sp_no = row["SEQ"].ToString();

                // 인덱스를 포함하여 detail data를 쓴다..
                string tst_cd, tst_name;
                int cnt = 0;
                for (int i = 0; i < tst_cd_arr_split.Length; i++)
                {
                    tst_cd = tst_cd_arr_split[i];
                    tst_name = tst_name_arr_split[i];
                    if (tst_cd != "")
                    {
                        cnt++;
                        sql = "INSERT INTO STOREDPRESET_DATA (SP_NO, TST_CD, SEQ, TST_NAME) VALUES (" + sp_no + ",'" + tst_cd + "'," + cnt + ", '" + tst_name + "')";
                        Debug.WriteLine(sql);
                        cmd = new OracleCommand(sql, db.conn);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        cmd = null;
                    }
                }

                // 인덱스를 포함하여 meta data를 쓴다..
                sql = "INSERT INTO STOREDPRESET (SP_NO, TST_CD_COUNT,PSET_NAME,UPDATE_DATE,UPDATE_ID) VALUES (" + sp_no + "," + cnt + ",'" + name + "',TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd") + "','YYYY.MM.DD'),'" + Session["userid"] + "')";
                Debug.WriteLine(sql);
                cmd = new OracleCommand(sql, db.conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                // 수정모드였던 경우 새로운 항목 저장 후 기존의 항목들은 삭제한다.
                if (sp_no_edit != "")
                {
                    Table tbl_meta = db.Table("STOREDPRESET");
                    Table tbl_data = db.Table("STOREDPRESET_DATA");

                    tbl_meta.Delete("SP_NO='" + sp_no_edit + "'");
                    tbl_data.Delete("SP_NO='" + sp_no_edit + "'");
                }
            }
            catch (Exception e)
            {
                Response.Write("<SCRIPT> alert('An error has occurred : " + e.StackTrace.ToString().Replace('\'', '`') + "'); </SCRIPT>");
                Response.End();
                Debug.WriteLine(e);
            }
            finally
            {
                db.Close();
            }

            return RedirectToAction("TestSearch", "Home");
        }

        public ActionResult AutoCompletePackageList()
        {
            string pkg_name = Request.QueryString["pkg_name"];
            string pkg_cd = Request.QueryString["pkg_cd"];
            List<string> cond = new List<string>();
            List<string> searchtext = new List<string>();

            if ( pkg_name != null && pkg_name.Trim() != "" ) {

                cond.Add(Database.GetParsedWhereRegex("PKGNM", pkg_name));

                searchtext.Add("패키지이름 [" + pkg_name + "]");
            }

            if (pkg_cd != null && pkg_cd.Trim() != "")
            {
                cond.Add("( REGEXP_LIKE(PKGCODE, '" + pkg_cd + "', 'i') )");
                searchtext.Add("패키지코드 [" + pkg_cd + "]");
            }

            string sql = "SELECT PKGCODE, PKGNM as PKGNAME, SDATE as FDATE FROM HCSUPKGM WHERE " + string.Join(" and ", cond.ToArray()) + " GROUP BY PKGCODE, PKGNM, SDATE ORDER BY PKGNAME";
            ViewBag.SearchText = string.Join(", ", searchtext.ToArray());
            Debug.WriteLine(sql);

            DataTable data = GetDataFromQuery(sql);

            return View(data);
        }

        public ActionResult AutoCompleteTestList()
        {
            string test_name = Request.QueryString["test_name"];
            string test_cd = Request.QueryString["test_cd"];
            List<string> cond = new List<string>();
            List<string> searchtext = new List<string>();

            if (test_name != null && test_name.Trim() != "")
            {
                cond.Add(Database.GetParsedWhereRegex("TST_NM", test_name));
                searchtext.Add("검사명 [" + test_name + "]");
            }

            if (test_cd != null && test_cd.Trim() != "")
            {
                cond.Add("( REGEXP_LIKE(TST_CD, '" + test_cd + "', 'i') )");
                searchtext.Add("검사코드 [" + test_cd + "]");
            }
            string sql = "SELECT TST_NM as TST_NAME, TST_CD FROM CODE2TABLE WHERE " + string.Join(" and ", cond.ToArray()) + " GROUP BY TST_NM, TST_CD ORDER BY TST_NAME";
            ViewBag.SearchText = string.Join(", ", searchtext.ToArray());
            ViewBag.Selector = Request.QueryString["selector"];
            ViewBag.Method = Request.QueryString["method"];
            Debug.WriteLine(sql);

            DataTable data = GetDataFromQuery(sql);

            return View(data);
        }

        public ActionResult AutoCompleteQuery()
        {
            string sql = "SELECT * FROM (SELECT SQ_DESC, SQ_QUERY FROM STOREDQUERY WHERE SQ_ID='" + Session["userid"] + "' order by SQ_DATE desc) WHERE ROWNUM < 10";
            Debug.WriteLine(sql);

            DataTable data = GetDataFromQuery(sql);

            return View(data);
        }

        public DataTable GetDataFromQuery(string query)
        {
            db.Open();
            DataTable data = new DataTable();
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = query;
                cmd.Connection = db.conn;
                OracleDataReader reader = cmd.ExecuteReader();
                data.Load(reader);
                reader.Dispose();
                reader = null;
                cmd.Dispose();
                cmd = null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            finally
            {
                db.Close();
            }
            return data;
        }

    }
}
