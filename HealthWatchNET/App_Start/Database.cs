using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Oracle.DataAccess.Client;
using System.Collections.Specialized;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;

namespace HealthWatchNET
{
    public class Database
    {
        public OracleConnection conn = null;
        public bool IsOpen = false;

        private static NameValueCollection GetEncodedForm(System.IO.Stream stream, Encoding encoding)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.ASCII);
            return GetEncodedForm(reader.ReadToEnd(), encoding);
        }

        public static string GetParsedWhereRegex(string field, string test_str)
        {
            test_str = test_str.Trim();

            if (test_str.IndexOf(',') > 0)
            {
                string[] names = test_str.Split(',');
                List<string> where_arr = new List<string>();
                string name_trim;
                foreach (string name in names)
                {
                    name_trim = name.Trim();
                    if (name_trim.Length > 0)
                    {
                        name_trim = Regex.Escape(name_trim);
                        if (name_trim[0] == '-')
                        {
                            name_trim = name_trim.Substring(1);
                            where_arr.Add("( NOT REGEXP_LIKE(" + field + ", '" + name_trim + "', 'i') )");
                        }
                        else
                        {
                            where_arr.Add("( REGEXP_LIKE(" + field + ", '" + name_trim + "', 'i') )");
                        }
                    }
                }
                return String.Join(" AND ", where_arr.ToArray());
            }
            else return "( REGEXP_LIKE(" + field + ", '" + Regex.Escape(test_str.Trim()) + "', 'i') )";

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

        public void Open()
        {
            if (conn == null) {
                string connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Entities"].ConnectionString;
                conn = new OracleConnection(connectionString);
            }

            switch (conn.State)
            {
                case ConnectionState.Closed:
                    conn.Open();
                    IsOpen = true;
                    break;
                case ConnectionState.Broken:
                case ConnectionState.Connecting:
                case ConnectionState.Executing:
                case ConnectionState.Fetching:
                case ConnectionState.Open:
                default:
                    break;
            }
        }

        public void Close()
        {
            if (conn != null)
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                    IsOpen = false;
                }
            }
        }

        public Database() { this.Open(); }

        public Table Table()
        {
            Table ret;
            ret = new Table(ref conn);
            return ret;
        }

        public Table Table(string name = "")
        {
            Table ret;
            ret = new Table(name, ref conn);
            return ret;
        }
    }

    public class Table
    {
        OracleConnection conn;
        OracleCommand cmd;
        public string tablename;
        public string lastsql;
        OracleDataReader reader;
        private Dictionary<string, string> rows = new Dictionary<string, string>();
        string quote_replace = "\\''";

        public Table(ref OracleConnection conn)
        {
            this.conn = conn;
        }

        public Table(string name, ref OracleConnection conn)
        {
            this.conn = conn;
            tablename = name;
        }

        public void NonQuery(string sql)
        {
            lastsql = sql;

            //if (this.conn == null || this.conn.State == ConnectionState.Closed) this.Open();

            try
            {
                cmd = new OracleCommand(sql, this.conn);
                cmd.ExecuteNonQuery();
                //this.Close();
            }
            catch (OracleException e)
            {
                Debug.WriteLine("* Error Message : " + e.Message);
                Debug.WriteLine("* Error executing SQL : " + lastsql);
                Debug.WriteLine(e.StackTrace);
            }
        }

        public OracleDataReader Query(string sql)
        {
            lastsql = sql;

            //if (this.conn == null || this.conn.State == ConnectionState.Closed) this.Open();

            try
            {
                cmd = new OracleCommand(sql, this.conn);
                return cmd.ExecuteReader();
            }
            catch (OracleException e)
            {
                Exception ex = new Exception( "* Oracle returns error : " + e.Message + "\r\n" + "* Error executing SQL : " + lastsql + "\r\n" + e.StackTrace );
                throw ex;
            }
        }

        public void SelectQuery(string sql)
        {
            reader = Query(sql);
        }

        public void Select(string field, string where = "", string spec = "")
        {
            string sql = "SELECT " + field + " FROM " + tablename;
            if (where != "" && where != null)
            {
                sql += " WHERE ( " + where + " )";
            }
            sql += " " + spec;

            reader = Query(sql);
        }

        public Dictionary<string, object> SelectFetch(string field, string where = "", string spec = "")
        {
            string sql = "SELECT * FROM ( SELECT " + field + " FROM " + tablename;
            if (where != "")
            {
                sql += " WHERE ( " + where + " )";
            }
            sql += " " + spec;
            sql += " ) WHERE ROWNUM=1";

            reader = Query(sql);

            if (reader == null) return null;

            Dictionary<string, object> ret = Fetch();

            reader.Close();

            return ret;
        }

        public Dictionary<string, object> Fetch()
        {
            if (reader == null || reader.IsClosed == true)
            {
                return null;
            }

            Dictionary<string, object> ret = new Dictionary<string, object>();

            object val = "";
            if (reader.HasRows == true)
            {
                if (reader.Read() == true)
                {
                    for (int lp = 0; lp < reader.FieldCount; lp++)
                    {
                        val = reader.GetValue(lp);
                        if (val == DBNull.Value) val = "";
                        
                        ret.Add(reader.GetName(lp), val.ToString());
                    }

                    return ret;
                }
                else
                {
                    reader.Close();
                    return null;
                }
            }
            else
            {
                reader.Close();
                return null;
            }
        }

        public bool FetchTo(out Dictionary<string, object> ret)
        {
            ret = Fetch();
            if (ret == null) return false;
            else return true;
        }

        public void Insert(string values, string fields)
        {
            string sql = "INSERT INTO " + tablename + " ( " + fields + " ) VALUES ( " + values + " )";

            NonQuery(sql);
        }

        public void Update(string update, string where)
        {
            string sql = "UPDATE " + tablename + " SET " + update + " WHERE " + where;

            NonQuery(sql);
        }

        public long NumRows(string where = "")
        {
            string sql = "SELECT count(*) FROM " + tablename;
            if (where != "")
            {
                sql += " WHERE ( " + where + " )";
            }
            cmd = new OracleCommand(sql, this.conn);
            object s = cmd.ExecuteScalar();

            if (s == null)
            {
                return 0;
            }
            else
            {
                return long.Parse(s.ToString());
            }
        }

        public void Delete(string where)
        {
            string sql = "DELETE FROM " + tablename + " WHERE " + where;

            NonQuery(sql);
        }

        public void rowinit()
        {
            rows.Clear();
        }

        public void rowdata(string field, string value)
        {
            if (value.Length >= 3 && value.Substring(0, 3) == "FN:")
            {
                rows.Add(field, value.Trim());
            }
            else
            {
                rows.Add(field, value.Trim().Replace("'", quote_replace));
            }
        }

        public string rowfields() { return String.Join(", ", rows.Keys.ToArray()); }
        public string rowvalues()
        {
            List<string> a = new List<string>();
            foreach (string key in rows.Keys)
            {
                if (rows[key].Length >= 3 && rows[key].Substring(0, 3) == "FN:")
                {
                    a.Add(rows[key].Substring(3));
                }
                else
                {
                    a.Add("'" + rows[key] + "'");
                }
            }
            return String.Join(", ", a.ToArray());
        }

        public string rowupdate()
        {
            List<string> a = new List<string>();
            foreach (string key in rows.Keys)
            {
                if (rows[key].Length >= 3 && rows[key].Substring(0, 3) == "FN:")
                {
                    a.Add(key + "=" + rows[key].Substring(3));
                }
                else
                {
                    a.Add(key + "='" + rows[key] + "'");
                }
            }
            return String.Join(", ", a.ToArray());
        }
    }

    public class LogManager
    {
        Table tblLog, tblUser;
        HttpRequestBase req;
        HttpSessionStateBase session;
        Dictionary<string, object> user = null;

        public LogManager(Database db, HttpRequestBase req, HttpSessionStateBase sess)
        {
            tblLog = db.Table("LOGMANAGE");
            tblUser = db.Table("USERMANAGE");
            this.req = req;
            this.session = sess;
        }

        public void SetUserInfo(string id)
        {
            if (user == null)
            {
                user = tblUser.SelectFetch("*", "UM_ID='" + id + "'");
            }
        }

        public string GetName(string id)
        {
            SetUserInfo(id);

            if (user != null && user.ContainsKey("UM_NAME") == true && user["UM_NAME"].ToString() != "")
            {
                return user["UM_NAME"].ToString();
            }
            else
            {
                return "<NONE>";
            }
        }

        public string GetOccu(string id)
        {
            SetUserInfo(id);

            if (user != null && user.ContainsKey("UM_OCCU") == true && user["UM_OCCU"].ToString() != "")
            {
                return user["UM_OCCU"].ToString();
            }
            else
            {
                return "<NONE>";
            }
        }

        public void Log(String action, String content, String id = "")
        {
            if (id == null || id == "")
            {
                if ((string)session["userid"] != "" && (string)session["userid"] != null)
                {
                    id = (string)session["userid"];
                }
                else
                {
                    id = "<NONE>";
                }
            }

            Dictionary<string,object> lm_no = tblLog.SelectFetch("max(LM_NO)+1 LM_NO_MAX");
            String lm_no_max = "1";
            if (lm_no.ContainsKey("LM_NO_MAX") == true && lm_no["LM_NO_MAX"].ToString() != "")
            {
                lm_no_max = lm_no["LM_NO_MAX"].ToString();
            }
            tblLog.rowinit();
            tblLog.rowdata("LM_NO", lm_no_max);
            tblLog.rowdata("LM_DATE", "FN:TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', 'yyyy/mm/dd hh24:mi:ss')");
            tblLog.rowdata("LM_ID", id);
            tblLog.rowdata("LM_NAME", GetName(id));
            tblLog.rowdata("LM_OCCU", GetOccu(id));
            tblLog.rowdata("LM_ACTION", action);

            if (content.Length > 4000)
            {
                // 4천자 제한, 오류방지..
                content = content.Substring(0, 3900);
            }

            tblLog.rowdata("LM_LOG", content);
            string ip = req.ServerVariables["REMOTE_ADDR"];
            tblLog.rowdata("LM_IP", ip);
            tblLog.Insert(tblLog.rowvalues(), tblLog.rowfields());
        }
    }
}
