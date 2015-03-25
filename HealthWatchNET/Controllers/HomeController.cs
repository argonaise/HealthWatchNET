using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using MvcPaging;
using System.Security.Cryptography;

namespace HealthWatchNET.Controllers
{
    public enum ExportFormat
    {
        ExportHtml,
        ExportCsv
    };

    public class HomeController : Controller
    {
        //
        // GET: /Home/

        private DataController dc;
        private Database db = new Database();
        public InterviewItem iv_obj;
        private LogManager lm;
        string quote_replace = "\\''";

        public HomeController()
        {
        }

        ~HomeController()
        {
            if (db != null)
            {
                db.Close();
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrcLogin()
        {
            lm = new LogManager(db, Request, Session);

            String t_local = Request.QueryString["t"];

            Table tbl = db.Table("USERMANAGE");
            Dictionary<string, object> data = tbl.SelectFetch("UM_ID, UM_PW, UM_NAME, UM_TYPE, TO_CHAR(UM_DATE_BEGIN, 'yyyy-mm-dd') UM_DATE_BEGIN, TO_CHAR(UM_DATE_END, 'yyyy-mm-dd') UM_DATE_END, UM_TIME_BEGIN_H, UM_TIME_BEGIN_M, UM_TIME_END_H, UM_TIME_END_M", "UM_ID='" + Request.Form["id"] + "'");

            string errScript = "";

            if (data == null)
            {
                errScript += "<script>";
                errScript += "alert('없는 ID입니다. 다시 확인 후 입력해 주세요');";
                errScript += "location.href = '/';";
                errScript += "</script>";

                lm.Log("LoginError/NO_ID", "NO_ID=[" + Request.Form["id"] + "] PASS=[" + Request.Form["pw"] + "]");

                return Content(errScript);
            }
            else
            {
                if (Request.Form["pw"] == data["UM_PW"].ToString())
                {
                    if (data["UM_TYPE"].ToString() == "외부연구원")
                    {
                        // 시작 날짜
                        string dateBeginStr = data["UM_DATE_BEGIN"].ToString() + " " + data["UM_TIME_BEGIN_H"].ToString() + ":" + data["UM_TIME_BEGIN_M"].ToString();
                        DateTime dateBegin = DateTime.ParseExact(dateBeginStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                        // 종료 날짜
                        string dateEndStr = data["UM_DATE_END"].ToString() + " " + data["UM_TIME_END_H"].ToString() + ":" + data["UM_TIME_END_M"].ToString();
                        DateTime dateEnd = DateTime.ParseExact(dateEndStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                        // 날짜가 범위 안에 있지 않으면 오류내고 중단
                        if (dateBegin > DateTime.Now || dateEnd < DateTime.Now)
                        {
                            errScript += "<script>";
                            errScript += "alert('사용 기한이 만료되었습니다.\\n사용 기한 : " + dateBeginStr + "~" + dateEndStr + "\\n관리자에게 권한을 요청하세요.');";
                            errScript += "location.href = '/';";
                            errScript += "</script>";

                            lm.Log("LoginError/EXPIRED_ID", "EXPIRED_ID=[" + Request.Form["id"] + "] PERIOD=[" + dateBeginStr + "~" + dateEndStr + "]");

                            return Content(errScript);
                        }
                        else
                        {
                            Response.Cookies["expiredate"].Value = dateBeginStr + "~" + dateEndStr;
                            Response.Cookies["expiredate"].Expires = DateTime.Now.AddDays(15);
                            Session["expiredate"] = dateBeginStr + "~" + dateEndStr;
                        }
                    }

                    Response.Cookies["userid"].Value = data["UM_ID"].ToString();
                    Response.Cookies["userid"].Expires = DateTime.Now.AddDays(15);
                    Session["userid"] = data["UM_ID"].ToString();

                    Response.Cookies["name"].Value = data["UM_NAME"].ToString();
                    Response.Cookies["name"].Expires = DateTime.Now.AddDays(15);
                    Session["name"] = data["UM_NAME"].ToString();

                    Response.Cookies["permtype"].Value = data["UM_TYPE"].ToString();
                    Response.Cookies["permtype"].Expires = DateTime.Now.AddDays(15);
                    Session["permtype"] = data["UM_TYPE"].ToString();

                    lm.Log("LoginOk", "ID=[" + Request.Form["id"] + "]");

                    db.Close();

                    return RedirectToAction("TestSearch");
                }
                else
                {
                    errScript += "<script>";
                    errScript += "alert('비밀번호가 일치하지 않습니다. 다시 확인 후 입력해 주세요.');";
                    errScript += "location.href = '/';";
                    errScript += "</script>";

                    lm.Log("LoginError/PASS_NOT_MATCH", "ID=[" + Request.Form["id"] + "] PASS_NOT_MATCH=[" + Request.Form["pw"] + "]");

                    db.Close();

                    return Content(errScript);
                }
            }

        }

        public ActionResult PrcLogout()
        {
            lm = new LogManager(db, Request, Session);

            String t_local = Request.QueryString["t"];

            lm.Log("LogoutOk", "ID=[" + Session["userid"] + "]");
            
            Response.Cookies["userid"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["name"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["permtype"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["expiredate"].Expires = DateTime.Now.AddDays(-1);

            Session.Remove("userid");
            Session.Remove("name");
            Session.Remove("permtype");
            Session.Remove("expiredate");

            return RedirectToAction("Index", new { t = t_local });
        }

        public ActionResult TestItem()
        {
            return View();
        }

        public ActionResult TestPackage()
        {
            return View();
        }

        public ActionResult TestSearch()
        {
            return View();
        }

        // View Model for TestPresetMake
        public class ViewModelForTestPresetMake
        {
            public DataTable InterviewItem { get; set; }
            public DataTable PatientItem { get; set; }
            public string sp_no, preset_name;
            public Dictionary<string, string> tests;
        }

        public ActionResult TestPresetMake()
        {
            DataTable m = InterviewItem.getInterviewItems();
            ViewModelForTestPresetMake objModel = new ViewModelForTestPresetMake() { InterviewItem = m, PatientItem = null };

            if (Request.QueryString["SP_NO"] != null)
            {
                string sp_no = Request.QueryString["SP_NO"];
                Table t1 = db.Table("STOREDPRESET");
                Dictionary<string, object> d = t1.SelectFetch("PSET_NAME", "SP_NO='" + sp_no + "'");
                objModel.sp_no = sp_no;
                objModel.preset_name = d["PSET_NAME"].ToString();

                objModel.tests = new Dictionary<string, string>();

                Table t2 = db.Table("STOREDPRESET_DATA");
                t2.Select("TST_CD, TST_NAME", "SP_NO='" + sp_no + "'", "ORDER BY SEQ");
                Dictionary<string,object> tmp;
                while ((tmp = t2.Fetch()) != null)
                {
                    objModel.tests.Add(tmp["TST_CD"].ToString(),tmp["TST_NAME"].ToString());
                }
            }

            return View(objModel);
        }

        public ActionResult QueryMake()
        {
            List<string> condProf = new List<string>();
            List<string> condExam = new List<string>();
            List<string> condTest = new List<string>();
            List<string> condString = new List<string>();

            Dictionary<string, string> ieq = new Dictionary<string, string>() {
                {"gt", ">" }, {"gteq", ">=" }, {"eq", "=" }, {"lteq", "<=" }, {"lt", "<" }
            };
            Dictionary<string, string> ieq_reverse = new Dictionary<string, string>() {
                {"gt", "<" }, {"gteq", "<=" }, {"eq", "=" }, {"lteq", ">=" }, {"lt", ">" }
            };

            Dictionary<string, object> row;
            Table t = db.Table("SELECTEDITEM");
            row = t.SelectFetch("count(*) as CNT", "USER_ID='" + Session["userid"] + "' and PSET_NAME='^^'");

            if (row == null || row["CNT"].Equals("0") == true)
            {
                return Content("<SCRIPT> alert('선택된 검사 항목이 없습니다'); </SCRIPT>");
            }

            if (Request.Form["p_date_1"] == "")
            {
                return Content("<SCRIPT> alert('수진일자 검색범위 시작시점을 입력해 주세요'); </SCRIPT>");
            }

            if (Request.Form["p_date_2"] == "")
            {
                return Content("<SCRIPT> alert('수진일자 검색범위 종료시점을 입력해 주세요'); </SCRIPT>");
            }

            DateTime d1, d2;

            try
            {
                d1 = DateTime.ParseExact(Request.Form["p_date_1"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (FormatException fe)
            {
                return Content("<SCRIPT> alert('수진일자 검색범위 시작시점 형식을 YYYY-MM-DD 형식에 맞추어 입력해 주세요'); </SCRIPT>");
            }

            try
            {
                d2 = DateTime.ParseExact(Request.Form["p_date_2"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (FormatException fe)
            {
                return Content("<SCRIPT> alert('수진일자 검색범위 종료시점 형식을 YYYY-MM-DD 형식에 맞추어 입력해 주세요'); </SCRIPT>");
            }

            TimeSpan ts = d2 - d1;
            if (ts.Days > 365 * 3)
            {
                return Content("<SCRIPT> alert('수진일자 검색범위는 3년을 넘을 수 없습니다'); </SCRIPT>");
            }

            string cond_date = "BETWEEN TO_DATE('" + Request.Form["p_date_1"] + "','yyyy-mm-dd') AND TO_DATE('" + Request.Form["p_date_2"] + "', 'yyyy-mm-dd')";
            string tmp_orddate = "( HCSURSVT.ORDDATE " + cond_date + " )";

            if (Request.Form["sw_name"] == "on")
            {
                //Key : 'p_name_op' | Value : 'equal'
                //Key : 'p_name' | Value : '김영호'
                if (Request.Form["p_name"] == "")
                {
                    return Content("<SCRIPT> alert('환자 성명을 입력해 주세요'); </SCRIPT>");
                }

                if (Request.Form["p_name_op"] == "equal")
                {
                    condProf.Add("( HCSUPATB.PATNAME='" + Request.Form["p_name"].Replace("'", quote_replace) + "' )");
                    condString.Add("환자명=" + Request.Form["p_name"]);
                }
                else
                {
                    condProf.Add("( REGEXP_LIKE( HCSUPATB.PATNAME, '" + Request.Form["p_name"].Replace("'", quote_replace) + "', 'i' ) )");
                    condString.Add("환자명에 " + Request.Form["p_name"] + " 포함");
                }
            }

            if (Request.Form["sw_age"] == "on")
            {
                if (Request.Form["p_age_1"] == "")
                {
                    return Content("<SCRIPT> alert('환자 연령을 입력해 주세요'); </SCRIPT>");
                }

                if (Request.Form["p_age_between"] != "" && Request.Form["p_age_2"] == "")
                {
                    return Content("<SCRIPT> alert('환자 연령(2번째)를 입력해 주세요'); </SCRIPT>");
                }

                // 나이 기준으로 생년월일 range를 만들어 검색
                // 만 46세 이면 현재(2014)-46 = 1968-01-01을 기준으로 하면 됨

                string tmp = "( ";
                int op1_age;
                int this_year = DateTime.Now.Year;

                if (Int32.TryParse(Request.Form["p_age_1"], out op1_age) == false)
                {
                    return Content("<SCRIPT> alert('환자 연령(1번째)을 숫자로 입력해 주세요'); </SCRIPT>");
                }

                string op1 = (this_year - op1_age) + "-01-01";

                tmp += "HCSUPATB.BIRTHDAY " + ieq_reverse[Request.Form["p_age_op_1"]] + " date '" + op1 + "'";
                condString.Add("연령 " + ieq_reverse[Request.Form["p_age_op_1"]] + " " + op1_age);

                if (Request.Form["p_age_between"] != "")
                {
                    int op2_age;

                    if (Int32.TryParse(Request.Form["p_age_2"], out op2_age) == false)
                    {
                        return Content("<SCRIPT> alert('환자 연령(2번째)을 숫자로 입력해 주세요'); </SCRIPT>");
                    }

                    string op2 = (this_year - op2_age) + "-01-01";

                    tmp += Request.Form["p_age_between"] + " HCSUPATB.BIRTHDAY " + ieq_reverse[Request.Form["p_age_op_2"]] + " " + " date '" + op2 + "'";
                    condString.Add(Request.Form["p_age_between"] + " 연령 " + ieq_reverse[Request.Form["p_age_op_2"]] + " " + op2_age);
                }

                tmp += " )";

                condProf.Add(tmp);
            }

            if (Request.Form["sw_sex"] == "on")
            {
                if (Request.Form["p_sex"] == "")
                {
                    return Content("<SCRIPT> alert('환자 성별을 입력해 주세요'); </SCRIPT>");
                }
                else
                {
                    condProf.Add("( HCSUPATB.SEX='" + Request.Form["p_sex"].Replace("'", quote_replace) + "' )");
                    condString.Add("성별=" + Request.Form["p_sex"]);
                }
            }

            if (Request.Form["sw_patno"] == "on")
            {
                if (Request.Form["p_patno_arr"] == null || Request.Form["p_patno_arr"] == "")
                {
                    return Content("<SCRIPT> alert('환자 번호를 입력해 주세요'); </SCRIPT>");
                }
                else
                {
                    string[] patno_split = Request.Form["p_patno_arr"].Split('\n');
                    List<string> patno = new List<string>();
                    for (int i = 0; i < patno_split.Length; i++)
                    {
                        patno.Add("'" + patno_split[i].Trim() + "'");
                    }
                    string patno_str = String.Join(", ", patno);
                    condProf.Add("( HCSUPATB.PT_NO in (" + patno_str + ") )");
                    condString.Add("환자번호=(" + patno_str + ")");
                }
            }

            if (Request.Form["sw_rsvno"] == "on")
            {
                //Key : 'p_rsvno_op' | Value : 'include'
                //Key : 'p_rsvno' | Value : '2412523'
                if (Request.Form["p_rsvno"] == "")
                {
                    return Content("<SCRIPT> alert('예약번호를 입력해 주세요'); </SCRIPT>");
                }

                if (Request.Form["p_rsvno_op"] == "equal")
                {
                    condProf.Add("( HCSURSVT.RSVNO='" + Request.Form["p_rsvno"].Replace("'", quote_replace) + "' )");
                    condString.Add("예약번호=" + Request.Form["p_rsvno"]);
                }
                else
                {
                    condProf.Add("(REGEXP_LIKE(RSVNO, '" + Request.Form["p_rsvno"].Replace("'", quote_replace) + "', 'i'))");
                    condString.Add("예약번호 " + Request.Form["p_rsvno"] + " 포함");
                }
            }

            if (Request.Form["sw_pkg"] == "on")
            {
                if (Request.Form["p_pkg_cd"] == "")
                {
                    return Content("<SCRIPT> alert('패키지 코드를 입력해 주세요'); </SCRIPT>");
                }

                string pkg_cd = Request.Form["p_pkg_cd"].Replace("'", quote_replace);

                if (pkg_cd != null && pkg_cd.Trim() != "")
                {
                    condProf.Add("HCSURSVT.PKGCODE='"+pkg_cd+"'");
                    condString.Add("패키지코드에 '" + pkg_cd + "' 포함");
                }

            }

            //----------------------------------------------------------------------------------------------------------------------------

            //if (Request.Form["sw_test1"] == "on")
            //{
                if (Request.Form["p_test1_name"] == "" && Request.Form["p_test1_cd"] == "")
                {
                    return Content("<SCRIPT> alert('검사명 혹은 검사 코드를 입력해 주세요'); </SCRIPT>");
                }

                string tst_cd = "";
                if (Request.Form["p_test1_cd"] != null && Request.Form["p_test1_cd"] != "")
                {
                    tst_cd = Request.Form["p_test1_cd"].Replace("'", quote_replace);
                }
                string tst_nm = "";
                if (Request.Form["p_test1_name"] != null && Request.Form["p_test1_name"] != "")
                {
                    tst_nm = Request.Form["p_test1_name"].Replace("'", quote_replace);
                }

                // sql1은 union으로 연결된 테이블이어야 함
                // sql2와도 union으로 연결되어야 하기 때문
                // prof 테이블은 outer join으로 이 table들의 union에 연결됨

                // tst_cd, tst_nm을 넘겨 주면 테이블에 해당하는 tst_cd를 찾아 sql을 만들어 union으로 연결해 주는 관리자가 필요함
                // PANEL2CODE는 아직 신경쓰지 않아도 됨
                // CODE2TABLE에 의존하여 코드를 검색함

                Dictionary<string, List<string>> codes = new Dictionary<string, List<string>>();
                string condStringTmp = "";

                Table tbl = db.Table("CODE2TABLE");
                if (tst_cd != "")
                {
                    tbl.Select("TABLENAME, TST_CD", "TST_CD='" + tst_cd + "'");
                    while (tbl.FetchTo(out row))
                    {
                        string tablename = row["TABLENAME"].ToString();
                        string code = row["TST_CD"].ToString();

                        if (codes.ContainsKey(tablename) == false) codes[tablename] = new List<string>();
                        codes[tablename].Add(code);
                    }

                    condStringTmp = "검사코드 " + tst_cd + " 가 ";
                }

                if (tst_nm != "")
                {
                    tbl.Select("TABLENAME, TST_CD", Database.GetParsedWhereRegex("TST_NM", tst_nm), "GROUP BY TABLENAME, TST_CD");
                    
                    Debug.WriteLine(tbl.lastsql);
                    while (tbl.FetchTo(out row))
                    {
                        string tablename = row["TABLENAME"].ToString();
                        string code = row["TST_CD"].ToString();

                        if (codes.ContainsKey(tablename) == false) codes[tablename] = new List<string>();
                        codes[tablename].Add(code);
                    }

                    condStringTmp = "검사명 " + tst_nm + " 이 ";
                }

                if (Request.Form["p_test1_val_op"] == "NOT NULL")
                {
                    foreach (KeyValuePair<string,List<string>> t2 in codes) {
                        string tablename = t2.Key;
                        string tst_cd_in = "'" + String.Join("', '", t2.Value.ToArray()) + "'";

                        if (tablename == "HCSUREST")
                        {
                            string sql = "( SELECT PT_NO, ORDDATE, RSVNO, EXAMCODE as CODE, HD_TST_RSLT as EXVALUE ";
                            sql += "FROM HCSUREST WHERE ( ORDDATE " + cond_date + " ) AND ( EXAMCODE in (" + tst_cd_in + ") ) ";
                            sql += "AND ( HD_TST_RSLT is NOT NULL and HD_TST_RSLT<>'' ) )";
                            condExam.Add(sql);
                        }

                        if (tablename == "HCSUXW0T")
                        {
                            string sql = "( SELECT PT_NO, ORDDATE, RSVNO, TST_CD as CODE, TST_RSLT as EXVALUE ";
                            sql += "FROM HCSUXW0T WHERE ( ORDDATE " + cond_date + " ) AND ( TST_CD in (" + tst_cd_in + ") ) ";
                            sql += "AND ( TST_RSLT is NOT NULL and TST_RSLT<>'' ) )";
                            condExam.Add(sql);
                        }

                        if (tablename == "HCSURE0T")
                        {
                            string sql = "( SELECT PT_NO, ORDDATE, RSVNO, TST_CD as CODE, RSLT1 || RSLT2 || READ_SUM as EXVALUE ";
                            sql += "FROM HCSURE0T WHERE ( ORDDATE " + cond_date + " ) AND ( TST_CD in (" + tst_cd_in + ") ) ";
                            sql += "AND ( RSLT1 is NOT NULL OR READ_SUM is NOT NULL ) )";
                            condExam.Add(sql);
                        }
                    }

                    condString.Add(condStringTmp + " NULL이 아님");
                }

                if (Request.Form["p_test1_val_op"] == "EXACT")
                {
                    if (Request.Form["p_test1_val_exact"] == "")
                    {
                        return Content("<SCRIPT> alert('(입력값 검색)검색할 검사 결과를 입력해 주세요'); </SCRIPT>");
                    }
                    else
                    {
                        string val_exact = Request.Form["p_test1_val_exact"].Replace("'", quote_replace);

                        foreach (KeyValuePair<string, List<string>> t2 in codes)
                        {
                            string tablename = t2.Key;
                            string tst_cd_in = "'" + String.Join("', '", t2.Value.ToArray()) + "'";

                            if (tablename == "HCSUREST")
                            {
                                string sql = "( SELECT PT_NO, ORDDATE, RSVNO, EXAMCODE as CODE, HD_TST_RSLT as EXVALUE ";
                                sql += "FROM HCSUREST WHERE ( ORDDATE " + cond_date + " ) AND ( EXAMCODE in (" + tst_cd_in + ") ) ";
                                sql += "AND ( HD_TST_RSLT='" + val_exact + "' ) )";
                                condExam.Add(sql);
                            }

                            if (tablename == "HCSUXW0T")
                            {
                                string sql = "( SELECT PT_NO, ORDDATE, RSVNO, TST_CD as CODE, TST_RSLT as EXVALUE ";
                                sql += "FROM HCSUXW0T WHERE ( ORDDATE " + cond_date + " ) AND ( TST_CD in (" + tst_cd_in + ") ) ";
                                sql += "AND ( TST_RSLT='" + val_exact + "' ) )";
                                condExam.Add(sql);
                            }

                            if (tablename == "HCSURE0T")
                            {
                                string sql = "( SELECT PT_NO, ORDDATE, RSVNO, TST_CD as CODE, RSLT1 || RSLT2 || READ_SUM as EXVALUE ";
                                sql += "FROM HCSURE0T WHERE ( ORDDATE " + cond_date + " ) AND ( TST_CD in (" + tst_cd_in + ") ) ";
                                sql += "AND ( RSLT1='" + val_exact + "' OR RSLT2='" + val_exact + "' OR READ_SUM='" + val_exact + "' ) )";
                                condExam.Add(sql);
                            }
                        }

                        condString.Add(condStringTmp + "=" + val_exact);
                    }
                }

                if (Request.Form["p_test1_val_op"] == "LIKE")
                {
                    if (Request.Form["p_test1_val_like"] == "")
                    {
                        return Content("<SCRIPT> alert('(일부 포함)검색할 검사 결과를 입력해 주세요'); </SCRIPT>");
                    }
                    else
                    {
                        string val_like = Request.Form["p_test1_val_like"].Replace("'", quote_replace);

                        foreach (KeyValuePair<string, List<string>> t2 in codes)
                        {
                            string tablename = t2.Key;
                            string tst_cd_in = "'" + String.Join("', '", t2.Value.ToArray()) + "'";

                            if (tablename == "HCSUREST")
                            {
                                string sql = "( SELECT PT_NO, ORDDATE, RSVNO, EXAMCODE as CODE, HD_TST_RSLT as EXVALUE ";
                                sql += "FROM HCSUREST WHERE ( ORDDATE " + cond_date + " ) AND ( EXAMCODE in (" + tst_cd_in + ") ) ";
                                sql += "AND ( REGEXP_LIKE(HD_TST_RSLT, '" + val_like + "', 'i') )";
                                condExam.Add(sql);
                            }

                            if (tablename == "HCSUXW0T")
                            {
                                string sql = "( SELECT PT_NO, ORDDATE, RSVNO, TST_CD as CODE, TST_RSLT as EXVALUE ";
                                sql += "FROM HCSUXW0T WHERE ( ORDDATE " + cond_date + " ) AND ( TST_CD in (" + tst_cd_in + ") ) ";
                                sql += "AND ( REGEXP_LIKE(TST_RSLT, '" + val_like + "', 'i') )";
                                condExam.Add(sql);
                            }

                            if (tablename == "HCSURE0T")
                            {
                                string sql = "( SELECT PT_NO, ORDDATE, RSVNO, TST_CD as CODE, RSLT1 || RSLT2 || READ_SUM as EXVALUE ";
                                sql += "FROM HCSURE0T WHERE ( ORDDATE " + cond_date + " ) AND ( TST_CD in (" + tst_cd_in + ") ) ";
                                sql += "AND ( REGEXP_LIKE(RSLT1, '" + val_like + "', 'i') OR REGEXP_LIKE(RSLT2, '" + val_like + "', 'i') OR REGEXP_LIKE(READ_SUM, '" + val_like + "', 'i') )";
                                condExam.Add(sql);
                            }
                        }

                        condString.Add(condStringTmp + " 결과가 " + val_like + " 포함");
                    }
                }

            //}

            //if (Request.Form["sw_test2"] == "on")
            //{
            //    if (Request.Form["p_test2_name"] == "")
            //    {
            //        return Content("<SCRIPT> alert('검사명을 입력해 주세요'); </SCRIPT>");
            //    }

            //    if (Request.Form["p_test2_cd"] == "")
            //    {
            //        return Content("<SCRIPT> alert('검사 코드를 입력해 주세요'); </SCRIPT>");
            //    }

            //    string tst_cd = Request.Form["p_test2_cd"].Replace("'", quote_replace);

            //    if (Request.Form["p_test2_val_op"] == "NOT NULL")
            //    {
            //        string sql1 = "";

            //        // 테스트 코드에 대한 데이터가 어느 테이블에 있는지 구함..
            //        string tablename = TestCodeManager.GetCodeTable(db, tst_cd);
            //        if (tablename != null)
            //        {
            //            sql1 += "HCSURSVT.RSVNO in ( ";
            //            // 테이블에 맞게 sql을 생성..
            //            // 조건 - NOT NULL
            //            if (tablename == "HCSURE0T")
            //                sql1 += "SELECT RSVNO FROM HCSURE0T WHERE ORDDATE " + cond_date + " and TST_CD='" + tst_cd + "' and ( RSLT1 is NOT NULL OR RSLT2 is NOT NULL OR READ_SUM is NOT NULL )";
            //            if (tablename == "HCSUREST")
            //                sql1 += "SELECT RSVNO FROM HCSUREST WHERE ORDDATE " + cond_date + " and EXAMCODE='" + tst_cd + "' and ( HD_TST_RSLT is NOT NULL )";
            //            if (tablename == "HCSUXW0T")
            //                sql1 += "SELECT RSVNO FROM HCSUXW0T WHERE ORDDATE " + cond_date + " and TST_CD='" + tst_cd + "' and ( TST_RSLT is NOT NULL )";
            //            sql1 += " )";
            //        }

            //        condTest.Add(sql1);
            //        condString.Add("검사코드 " + tst_cd + " 가 NULL이 아님");
            //        tables.Add("HCSURSVT");
            //    }

            //    if (Request.Form["p_test2_val_op"] == "EXACT")
            //    {
            //        if (Request.Form["p_test2_val_exact"] == "")
            //        {
            //            return Content("<SCRIPT> alert('(입력값 검색)검색할 검사 결과를 입력해 주세요'); </SCRIPT>");
            //        }
            //        else
            //        {
            //            string val_exact = Request.Form["p_test2_val_exact"].Replace("'", quote_replace);
            //            string sql1 = "";

            //            string tablename = TestCodeManager.GetCodeTable(db, tst_cd);
            //            if (tablename != null)
            //            {
            //                sql1 += "HCSURSVT.RSVNO in ( ";
            //                // 테이블에 맞게 sql을 생성..
            //                // 조건 - 값이 정확하게 일치할때 (EXACT)
            //                if (tablename == "HCSURE0T")
            //                    sql1 += "SELECT RSVNO FROM HCSURE0T WHERE ORDDATE " + cond_date + " and TST_CD='" + tst_cd + "' and ( RSLT1='" + val_exact + "' OR RSLT2='" + val_exact + "' OR READ_SUM='" + val_exact + "' )";
            //                if (tablename == "HCSUREST")
            //                    sql1 += "SELECT RSVNO FROM HCSUREST WHERE ORDDATE " + cond_date + " and EXAMCODE='" + tst_cd + "' and ( HD_TST_RSLT='" + val_exact + "' )";
            //                if (tablename == "HCSUXW0T")
            //                    sql1 += "SELECT RSVNO FROM HCSUXW0T WHERE ORDDATE " + cond_date + " and TST_CD='" + tst_cd + "' and ( TST_RSLT='" + val_exact + "' )";
            //                sql1 += " )";
            //            }

            //            condTest.Add(sql1);
            //            condString.Add("검사코드 " + tst_cd + "=" + val_exact);
            //            tables.Add("HCSURSVT");
            //        }
            //    }

            //    if (Request.Form["p_test2_val_op"] == "LIKE")
            //    {
            //        if (Request.Form["p_test2_val_like"] == "")
            //        {
            //            return Content("<SCRIPT> alert('(일부 포함)검색할 검사 결과를 입력해 주세요'); </SCRIPT>");
            //        }
            //        else
            //        {
            //            string val_like = Request.Form["p_test2_val_like"].Replace("'", quote_replace);
            //            string sql1 = "";

            //            string tablename = TestCodeManager.GetCodeTable(db, tst_cd);
            //            if (tablename != null)
            //            {
            //                sql1 += "HCSURSVT.RSVNO in ( ";
            //                // 테이블에 맞게 sql을 생성..
            //                // 조건 - 값이 검색어를 포함하는 경우 (LIKE)
            //                if (tablename == "HCSURE0T")
            //                    sql1 += "SELECT RSVNO FROM HCSURE0T WHERE ORDDATE " + cond_date + " and TST_CD='" + tst_cd + "' and ( REGEXP_LIKE(RSLT1, '" + val_like + "', 'i') OR REGEXP_LIKE(RSLT2, '" + val_like + "', 'i') OR REGEXP_LIKE(READ_SUM, '" + val_like + "', 'i') )";
            //                if (tablename == "HCSUREST")
            //                    sql1 += "SELECT RSVNO FROM HCSUREST WHERE ORDDATE " + cond_date + " and EXAMCODE='" + tst_cd + "' and ( REGEXP_LIKE(HD_TST_RSLT, '" + val_like + "', 'i') )";
            //                if (tablename == "HCSUXW0T")
            //                    sql1 += "SELECT RSVNO FROM HCSUXW0T WHERE ORDDATE " + cond_date + " and TST_CD='" + tst_cd + "' and ( REGEXP_LIKE(TST_RSLT, '" + val_like + "', 'i') )";
            //                sql1 += " )";
            //            }

            //            condTest.Add(sql1);
            //            condString.Add("검사코드 " + tst_cd + " 결과가 " + val_like + " 포함");
            //            tables.Add("HCSURSVT");
            //        }
            //    }
            //}

            if (condProf.Count + condExam.Count == 0)
            {
                return Content("<SCRIPT> alert('최소 하나의 검색 조건을 입력하세요'); </SCRIPT>");
            }
            
            //-----------------------------------------------------------------------------------------------------------
            // MAIN SQL 만들기 시작
            //-----------------------------------------------------------------------------------------------------------

            // 회원정보 부분...
            string sqlProf = "";
            sqlProf += "( SELECT HCSUPATB.PATNAME as PATNAME, HCSUPATB.ENGNAME as ENGNAME, HCSUPATB.BIRTHDAY as DOB, HCSUPATB.SEX as SEX, HCSURSVT.PKGCODE as PKGCODE, ";
            sqlProf += "HCSUPATB.PT_NO as P_PT_NO, HCSURSVT.ORDDATE as P_ORDDATE, HCSURSVT.RSVNO as P_RSVNO ";
            sqlProf += "FROM HCSUPATB, HCSURSVT WHERE ( HCSUPATB.PT_NO=HCSURSVT.PT_NO ) AND ( HCSURSVT.ORDDATE " + cond_date + " ) ";

            string condSql = String.Join( " AND ", condProf.ToArray());
            if (condProf.Count != 0) condSql = " AND " + condSql;

            sqlProf += condSql + " ) PROF";

            // 테스트 query 부분..
            string sqlExam = "( " + String.Join(" UNION ALL ", condExam.ToArray()) + " ) EXAM";

            // 세 가지를 조합한 outer query
            // Test result query를 기반으로 profile query를 함..

            string sqlMain = "SELECT '" + Session.SessionID.ToString() + "' as SESSID, PATNAME, ENGNAME, DOB, SEX, PKGCODE, PT_NO, ORDDATE, RSVNO, CODE, EXVALUE FROM ";
            sqlMain += sqlProf + ", ";
            sqlMain += sqlExam + " WHERE PROF.P_PT_NO=EXAM.PT_NO(+) AND PROF.P_RSVNO=EXAM.RSVNO(+) AND PROF.P_ORDDATE=EXAM.ORDDATE(+) GROUP BY PATNAME, ENGNAME, DOB, SEX, PKGCODE, PT_NO, ORDDATE, RSVNO, CODE, EXVALUE ORDER BY RSVNO ";
            sqlMain += "/*" + cond_date + "*/";

            string js = "<SCRIPT>\n";
            js += "parent.PutExecScript(\"" + sqlMain.Replace("\"", "&quot;") + "\",\"" + Request.Form["selected_sp_no"] + "\");\n";
            js += "parent.PutExecString(\"" + String.Join(", ", condString.ToArray()).Replace("\"", "&quot;") + "\");\n";
            js += "parent.PutExecCond(\"" + cond_date + "\");\n";
            js += "</SCRIPT>\n";

            db.Close();

            return Content(js);
        }

        public Dictionary<string, string> GetPresetFields()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            Dictionary<string, object> row;

            Table t = db.Table("SELECTEDITEM");
            t.Select("TEST_CD, TEST_NM", "USER_ID='" + Session["userid"] + "' and PSET_NAME='^^'", "order by SEQ");
            while (t.FetchTo(out row) == true)
            {
                if (ret.ContainsKey(row["TEST_CD"].ToString()) == false)
                {
                    ret.Add((string)row["TEST_CD"], (string)row["TEST_NM"]);
                }
            }

            return ret;
        }
        
        public ActionResult QueryExec()
        {
            try
            {
                lm = new LogManager(db, Request, Session);

                Dictionary<string, object> row;
                Table t = db.Table("SELECTEDITEM");
                row = t.SelectFetch("count(*) as CNT", "USER_ID='" + Session["userid"] + "' and PSET_NAME='^^'");

                if (row == null || row["CNT"].Equals("0") == true)
                {
                    return Content("<SCRIPT> alert('검사 항목을 선택해 주세요.'); </SCRIPT>");
                }

                if (Request.Form["query"] == "")
                {
                    return Content("<SCRIPT> alert('검색 조건을 설정해 주세요'); window.close(); </SCRIPT>");
                }

                Match match = Regex.Match(Request.Form["query"], "(DELETE|UPDATE)", RegexOptions.IgnoreCase);
                if (match.Success == true)
                {
                    return Content("<SCRIPT> alert('DELETE 또는 UPDATE 구문은 사용할 수 없습니다'); window.close(); </SCRIPT>");
                }

                ExportFormat ef;
                string exportFormatStr = "";

                if (Request.Form["export_rows"] == "100" || Request.Form["export_rows"] == "500" || Request.Form["export_rows"] == "1000")
                {
                    ef = ExportFormat.ExportHtml;
                    exportFormatStr = "출력형식 : HTML, [" + Request.Form["export_rows"] + "]건 출력";
                }
                else
                {
                    ef = ExportFormat.ExportCsv;
                    exportFormatStr = "출력형식 : CSV, 조건에 해당하는 전체 자료";
                }

                if (Session["userid"] == null || (string)Session["userid"] == "")
                {
                    lm.Log("QueryError", "세션 만료, 다시 로그인 필요");

                    Response.Write("<script> alert('세션이 만료되었습니다. 다시 로그인해 주세요'); window.close(); </script>");
                    Response.End();
                    return null;
                }

                AddQueryHistory(Request.Form, Session["userid"].ToString());

                iv_obj = new InterviewItem();

                Dictionary<string, string> presetFields = new Dictionary<string, string>();

                StringBuilder o = new StringBuilder();

                // header start

                presetFields = GetPresetFields();
                List<string> tst_cd_list = new List<string>();
                int nfieldCount = 0;

                if (ef == ExportFormat.ExportHtml)
                {
                    StringBuilder o2 = new StringBuilder();


                    string key = "";
                    foreach (KeyValuePair<string, string> p in presetFields)
                    {
                        nfieldCount++;
                        key = p.Key;
                        if (key.Substring(0, 2) == "R." || key.Substring(0, 2) == "P.")
                        {
                            key = key.Substring(2, key.Length - 2);
                        }
                        if (key.Substring(0, 3) == "I3.")
                        {
                            key = key.Substring(3, key.Length - 3);
                        }

                        o2.Append("<td>" + key + "<br />" + p.Value + "</td>\n");
                        tst_cd_list.Add(p.Key);
                    }

                    // HTML header (Sample 100 Export)
                    o.Append("<html><head>\n");
                    o.Append("<title>Healthwatch.NET[QUERY RESULT]</title>\n");
                    o.Append("<style> table { table-layout:fixed; width:" + ((nfieldCount * 200) + 60) + "px } td { width:200px; overflow:hidden; text-overflow:ellipsis; word-break:break-all; line-break:strict; padding:5px; } </style>");
                    o.Append("</head>\n");
                    o.Append("<body><script src='/Script/ZeroClipboard.js'></script><div>\n");
                    o.Append("\n");
                    o.Append("<br />");
                    o.Append("<input type='button' value='Copy Data Table to Clipboard' style='width:200px; height:50px;' id='copy-button' data-clipboard-target='div_copyrange' />");
                    o.Append("<input type='button' value='Close Window' style='width:200px; height:50px;' onclick='window.close()' />");
                    o.Append("<br /><br />");

                    o.Append("<div id='div_copyrange'><table border='1' cellpadding='0' cellspacing='0'>\n");
                    o.Append("<tr align='center'>\n");

                    o.Append("<td width='60'>번호</td>\n");
                    //o.Append("<td>예약번호</td>\n");
                    //o.Append("<td>성명</td>\n");

                    o.Append(o2.ToString());

                    o.Append("</tr>\n");

                    Response.Write(o.ToString());
                    o.Clear();
                }
                else
                {
                    // CSV header (Whole Record Export)
                    string now = DateTime.Now.ToString("yyyyMMddHHmmss");
                    Response.AddHeader("Content-Disposition", "attachment;filename=HealthWatch_" + now + ".csv");
                    Response.Charset = "euckr";

                    o.Append("\"번호\",");
                    //o.Append("\"예약번호\",");
                    //o.Append("\"성명\",");

                    string key = "";
                    foreach (KeyValuePair<string, string> p in presetFields)
                    {
                        key = p.Key;
                        if (key.Substring(0, 2) == "R." || key.Substring(0, 2) == "P.")
                        {
                            key = key.Substring(2, key.Length - 2);
                        }
                        if (key.Substring(0, 3) == "I3.")
                        {
                            key = key.Substring(3, key.Length - 3);
                        }

                        o.Append("\"" + p.Value + "(" + p.Key + ")\",");
                        tst_cd_list.Add(p.Key);
                    }

                    o.Append("\r\n");

                    // Convert to Euc-kr
                    Response.BinaryWrite(ToEuckrByte(o.ToString()));
                    o.Clear();
                }

                // body start

                // loop call based on rsvno query
                string sql;
                if (ef == ExportFormat.ExportHtml)
                {
                    // 나중에 export_rows 만큼만 loop를 돌려서 리스트 개수를 맞추어 주어야 함..
                    int nRow = nfieldCount * Int32.Parse(Request.Form["export_rows"]);
                    sql = "SELECT * FROM ( " + Request.Form["query"] + " ) WHERE ROWNUM <= " + nRow;
                    Debug.WriteLine(sql);
                }
                else
                {
                    sql = Request.Form["query"];
                }

                Table tbl = db.Table();

                //string TEMPTABLE = "TBL_TMPSQL_" + Session["userid"] + "_" + Session.SessionID.Substring(10,10); // Session ID is 24 char long..
                string TEMPTABLE = "TBL_TMPSQL_QUERY";

                // 결과를 저장할 임시 테이블 만들기
                // PATNAME, ENGNAME, DOB, SEX, PKGCODE, PT_NO, ORDDATE, RSVNO, CODE, EXVALUE
                string t_sql = "DECLARE\n";
                t_sql += "V_TMP_TABLE_CNT INTEGER := 0;\n";
                t_sql += "V_SQL VARCHAR(500) := '';\n";
                t_sql += "BEGIN\n";
                t_sql += "SELECT COUNT(TABLE_NAME) INTO V_TMP_TABLE_CNT FROM USER_TABLES WHERE TABLE_NAME = '" + TEMPTABLE + "';\n";
                t_sql += "IF V_TMP_TABLE_CNT = 0 THEN\n";
                t_sql += "V_SQL := '';\n";
                t_sql += "V_SQL := V_SQL || ' CREATE GLOBAL TEMPORARY TABLE " + TEMPTABLE + " ';\n";
                t_sql += "V_SQL := V_SQL || ' ( ';\n";
                t_sql += "V_SQL := V_SQL || ' SESSID     VARCHAR(30), ';\n";
                t_sql += "V_SQL := V_SQL || ' PATNAME    VARCHAR2(40), ';\n";
                t_sql += "V_SQL := V_SQL || ' ENGNAME    VARCHAR2(40), ';\n";
                t_sql += "V_SQL := V_SQL || ' DOB        DATE, ';\n";
                t_sql += "V_SQL := V_SQL || ' SEX        VARCHAR2(10), ';\n";
                t_sql += "V_SQL := V_SQL || ' PKGCODE    VARCHAR2(30), ';\n";
                t_sql += "V_SQL := V_SQL || ' PT_NO      VARCHAR2(10), ';\n";
                t_sql += "V_SQL := V_SQL || ' ORDDATE    DATE, ';\n";
                t_sql += "V_SQL := V_SQL || ' RSVNO      VARCHAR2(20), ';\n";
                t_sql += "V_SQL := V_SQL || ' CODE       VARCHAR2(30), ';\n";
                t_sql += "V_SQL := V_SQL || ' EXVALUE    VARCHAR2(2000) ';\n";
                t_sql += "V_SQL := V_SQL || ' ) ';\n";
                t_sql += "V_SQL := V_SQL || ' ON COMMIT PRESERVE ROWS ';\n";
                t_sql += "EXECUTE IMMEDIATE V_SQL;\n";
                t_sql += "ELSE\n";
                t_sql += "V_SQL := 'DELETE FROM " + TEMPTABLE + " WHERE SESSID=''" + Session.SessionID.ToString() + "''';\n";
                t_sql += "EXECUTE IMMEDIATE V_SQL;\n";
                t_sql += "END IF;\n";
                t_sql += "END;\n";

                Debug.WriteLine(t_sql);

                int pos_start = sql.IndexOf("/*") + 2;
                int pos_end = sql.IndexOf("*/");
                string orddate_cond = sql.Substring(pos_start, pos_end - pos_start);
                //Debug.WriteLine(orddate_cond);

                tbl.Query(t_sql);
                Debug.WriteLine(tbl.lastsql);
                tbl.Query("INSERT INTO " + TEMPTABLE + " " + sql);
                Debug.WriteLine(tbl.lastsql);

                // MAIN QUERY
                // UNION JOIN WITH (tablename), with ORDDATE

                // Get field list, tables to query
                string field = "", tablename = "";
                List<string> keys = new List<string>();
                Dictionary<string, List<string>> codetable = new Dictionary<string, List<string>>();
                string cond_date = Request.Form["query_cond"];

                foreach (KeyValuePair<string, string> kvp in presetFields)
                {
                    field = kvp.Key;

                    if (field.Substring(0, 2) == "R." || field.Substring(0, 2) == "P." || field.Substring(0, 3) == "I3.") continue;
                    if (keys.Contains(field) == false) keys.Add(field);
                }

                string query = "SELECT TST_CD, TABLENAME FROM CODE2TABLE WHERE TST_CD in ('" + String.Join("', '", keys) + "') GROUP BY TST_CD, TABLENAME";
                Debug.WriteLine(query);
                tbl.SelectQuery(query);
                while (tbl.FetchTo(out row) == true)
                {
                    tablename = row["TABLENAME"].ToString();
                    field = row["TST_CD"].ToString();
                    if (codetable.ContainsKey(tablename) == false) codetable.Add(tablename, new List<string>());
                    codetable[tablename].Add(field);
                }

                // get unions of table which contains RSVNO of temporary TEMPTABLE table
                List<string> examSqlList = new List<string>();

                //// HCSUXW0T
                //if ( codetable.ContainsKey("HCSUXW0T") == true && codetable["HCSUXW0T"].Count > 0 )
                //{
                //    string sqlxw0t = "SELECT a.PT_NO, a.ORDDATE, a.RSVNO, b.TST_CD as RSTCODE, b.TST_RSLT as RSTVALUE FROM " + TEMPTABLE + " a, HCSUXW0T b WHERE ";
                //    sqlxw0t += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE )";
                //    //sqlxw0t += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE ) AND ";
                //    //sqlxw0t += "( b.TST_CD in ( '" + String.Join("', '", codetable["HCSUXW0T"]) + "' ) )";
                //    examSqlList.Add(sqlxw0t);
                //}
                //// HCSURE0T
                //if (codetable.ContainsKey("HCSURE0T") == true && codetable["HCSURE0T"].Count > 0)
                //{
                //    string sqlre0t = "SELECT a.PT_NO, a.ORDDATE, a.RSVNO, b.TST_CD as RSTCODE, TO_CLOB(b.RSLT1) || b.RSLT2 || b.READ_SUM as RSTVALUE FROM " + TEMPTABLE + " a, HCSURE0T b WHERE ";
                //    sqlre0t += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE )";
                //    //sqlre0t += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE ) AND ";
                //    //sqlre0t += "( b.TST_CD in ( '" + String.Join("', '", codetable["HCSURE0T"]) + "' ) )";
                //    examSqlList.Add(sqlre0t);
                //}
                //// HCSUREST
                //if (codetable.ContainsKey("HCSUREST") == true && codetable["HCSUREST"].Count > 0)
                //{
                //    string sqlrest = "SELECT a.PT_NO, a.ORDDATE, a.RSVNO, b.TST_CD as RSTCODE, b.HD_TST_RSLT as RSTVALUE FROM " + TEMPTABLE + " a, HCSUREST b WHERE ";
                //    sqlrest += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE )";
                //    //sqlrest += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE ) AND ";
                //    //sqlrest += "( b.TST_CD in ( '" + String.Join("', '", codetable["HCSUREST"]) + "' ) )";
                //    examSqlList.Add(sqlrest);
                //}

                //string examSql = "SELECT * FROM ( " + String.Join(" ) UNION ( ", examSqlList.ToArray()) + " ) ORDER BY RSVNO ASC";

                // HCSUXW0T
                if (codetable.ContainsKey("HCSUXW0T") == true && codetable["HCSUXW0T"].Count > 0)
                {
                    string sqlxw0t = "SELECT a.PT_NO, a.ORDDATE, a.RSVNO, b.TST_CD as RSTCODE, b.TST_RSLT as RSTVALUE FROM " + TEMPTABLE + " a, HCSUXW0T b WHERE ";
                    sqlxw0t += "( a.ORDDATE=b.ORDDATE AND b.ORDDATE " + orddate_cond + " AND a.PT_NO=b.PT_NO AND a.RSVNO=b.RSVNO )";
                    //sqlxw0t += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE ) AND ";
                    //sqlxw0t += "( b.TST_CD in ( '" + String.Join("', '", codetable["HCSUXW0T"]) + "' ) )";
                    examSqlList.Add(sqlxw0t);
                }

                // HCSURE0T
                if (codetable.ContainsKey("HCSURE0T") == true && codetable["HCSURE0T"].Count > 0)
                {
                    string sqlre0t = "SELECT a.PT_NO, a.ORDDATE, a.RSVNO, d.TST_CD as RSTCODE, SUBSTR(d.RSLT1, 1, 1800) || SUBSTR(d.RSLT2, 1, 300) || SUBSTR(d.READ_SUM, 1, 1800) as RSTVALUE FROM " + TEMPTABLE + " a, HCSURE0T d WHERE ";
                    sqlre0t += "( a.ORDDATE=d.ORDDATE AND a.ORDDATE " + orddate_cond + " AND a.PT_NO=d.PT_NO AND a.RSVNO=d.RSVNO )";
                    //sqlre0t += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE ) AND ";
                    //sqlre0t += "( b.TST_CD in ( '" + String.Join("', '", codetable["HCSURE0T"]) + "' ) )";
                    examSqlList.Add(sqlre0t);
                }

                // HCSUREST
                if (codetable.ContainsKey("HCSUREST") == true && codetable["HCSUREST"].Count > 0)
                {
                    string sqlrest = "SELECT a.PT_NO, a.ORDDATE, a.RSVNO, f.EXAMCODE as RSTCODE, f.HD_TST_RSLT as RSTVALUE FROM " + TEMPTABLE + " a, HCSUREST f WHERE ";
                    sqlrest += "( a.ORDDATE=f.ORDDATE AND a.ORDDATE " + orddate_cond + " AND a.PT_NO=f.PT_NO AND a.RSVNO=f.RSVNO )";
                    //sqlrest += "( a.PT_NO(+)=b.PT_NO AND a.RSVNO(+)=b.RSVNO AND a.ORDDATE(+)=b.ORDDATE ) AND ";
                    //sqlrest += "( b.TST_CD in ( '" + String.Join("', '", codetable["HCSUREST"]) + "' ) )";
                    examSqlList.Add(sqlrest);
                }

                //string examSql = "SELECT * FROM ( " + String.Join(" ) UNION ( ", examSqlList.ToArray()) + " ) ORDER BY RSVNO ASC";
                string examSql = "SELECT * FROM ( " + String.Join(" ) UNION ( ", examSqlList.ToArray()) + " )";
                Debug.WriteLine(examSql);
                tbl.SelectQuery(examSql);

                // get rows for an rsvno

                int cnt = 1;
                int max = Int32.Parse(Request.Form["export_rows"]);

                Dictionary<string, string> recordObj = new Dictionary<string, string>();
                string cur_rsvno = "", past_rsvno = "", code = "", value = "";

                while (tbl.FetchTo(out row) == true)
                {
                    cur_rsvno = row["RSVNO"].ToString();
                    code = row["RSTCODE"].ToString();
                    value = row["RSTVALUE"].ToString();

                    if (cur_rsvno.Equals(past_rsvno) == false && past_rsvno != "")
                    {
                        if (ef == ExportFormat.ExportHtml)
                        {
                            Response.Write(QueryExecGetLine(cnt++, past_rsvno, recordObj, tst_cd_list, ef));
                            if (max > 0 && cnt >= max)
                            {
                                recordObj.Clear();
                                break;
                            }
                        }
                        else
                        {
                            // Convert to Euc-kr
                            Response.BinaryWrite(ToEuckrByte(QueryExecGetLine(cnt++, past_rsvno, recordObj, tst_cd_list, ef)));
                        }

                        recordObj.Clear();
                    }

                    if (recordObj.ContainsKey(code) == false) recordObj.Add(code, value);
                    past_rsvno = cur_rsvno;
                }

                if (recordObj.Count > 0)
                {
                    // Clear 
                    if (ef == ExportFormat.ExportHtml)
                    {
                        Response.Write(QueryExecGetLine(cnt++, past_rsvno, recordObj, tst_cd_list, ef));
                    }
                    else
                    {
                        // Convert to Euc-kr
                        Response.BinaryWrite(ToEuckrByte(QueryExecGetLine(cnt++, past_rsvno, recordObj, tst_cd_list, ef)));
                    }
                }

                // footer start

                if (ef == ExportFormat.ExportHtml)
                {
                    Response.Write("</table></div>\n");

                    o.Append("</div>\n");
                    o.Append("<SCRIPT>");
                    o.Append("	var clip = new ZeroClipboard( document.getElementById(\"copy-button\"), {");
                    o.Append("	  moviePath: \"/Script/ZeroClipboard.swf\"");
                    o.Append("	} );");

                    o.Append("	clip.setHandCursor(true);");

                    o.Append("	clip.on( 'dataRequested', function ( client, args ) {");
                    o.Append("	  clip.setText( document.getElementById('div_copyrange').outerHTML );");
                    o.Append("	} );");

                    o.Append("	clip.on( 'complete', function(client, args) {");
                    o.Append("	  alert( \"내용이 클립보드로 복사되었습니다. 엑셀에 붙여넣으세요.\" );");
                    o.Append("	} );");
                    o.Append("</SCRIPT>");
                    o.Append("</body></html>\n");

                    Response.Write(o.ToString());

                    o.Clear();
                }
                else
                {
                    Response.Write("\r\n");
                }

                lm.Log("QueryOk", "Query 출력 완료(" + exportFormatStr + ") 총 record 수 [" + (cnt - 1) + "]건");
                lm.Log("Query", "Query SQL: " + Request.Form["query"]);
            }
            catch (OracleException ex)
            {
                Response.Write("Error in executing SQL : ");
                Response.Write(Request.Form["query"]);
                Response.Write("<br />");
                Response.Write("<br />");
                Response.Write(ex.Message);
            }
            catch (Exception ex)
            {
                Response.Write("Error in executing program : ");
                Response.Write(ex.Message.Replace("\r\n", "<br />\r\n"));
                Response.Write(ex.StackTrace.Replace("\r\n", "<br />\r\n"));
            }
            finally
            {
                db.Close();
            }

            return null;
        }

        private string QueryExecGetLine(int line_count, string rsvno, Dictionary<string,string> data, List<string> tst_cd_list, ExportFormat ef)
        {
            StringBuilder ret = new StringBuilder();

            string tst_cd = "'" + string.Join("', '", tst_cd_list) + "'";
            int tst_cd_cnt = tst_cd_list.Count;

            if (ef == ExportFormat.ExportHtml)
            {
                ret.Append("<tr>\n");
            }

            // Preparing data for a line

            // HCSUPATB
            Table tbl = db.Table("(Select * from HCSUPATB where PT_NO='" + rsvno.Substring(0, 8) + "') res");
            Dictionary<string,object> row = tbl.SelectFetch("trunc(months_between(sysdate,BIRTHDAY)/12) AGE, TO_CHAR(BIRTHDAY, 'YYYY-MM-DD') DATE_OF_BIRTH, res.*");
            foreach(KeyValuePair<string,object> idx in row)
            {
                string key = idx.Key;
                if (data.ContainsKey(key) == false)
                {
                    //if (key == "PT_NO" || key == "RSVNO" || key == "PATNAME" || key == "ENGNAME")
                    //{
                    //    data.Add("P." + key, Encryption.Encrypt(row[key].ToString()));
                    //}
                    //else
                    //{
                    //    data.Add("P." + key, row[key].ToString());
                    //}
                    data.Add("P." + key, row[key].ToString());
                }
            }
            tbl = null;
            row.Clear();

            // HCSURSVT
            tbl = db.Table("HCSURSVT");
            row = tbl.SelectFetch("*", "RSVNO='" + rsvno + "'");
            foreach (KeyValuePair<string, object> idx in row)
            {
                string key = idx.Key;
                if (data.ContainsKey(key) == false)
                {
                    if (key == "PT_NO" || key == "RSVNO" || key == "PATNAME" || key == "ENGNAME")
                    {
                        //pairs.Add("R." + key, Encryption.Encrypt(row[key].ToString()));
                        data.Add("R." + key, row[key].ToString());
                    }
                    else
                    {
                        data.Add("R." + key, row[key].ToString());
                    }
                }
            }
            tbl = null;
            row.Clear();

            // HCSUOCRT
            tbl = db.Table("HCSUOCRT");
            row = tbl.SelectFetch("*", "RSVNO='" + rsvno + "' AND ( OCRTTYPE='1' OR OCRTTYPE IS NULL )");
            Debug.WriteLine(tbl.lastsql);

            if (row != null)
            {
                string intv_ver = row["VER_NO"].ToString();
                if (intv_ver == null || intv_ver == "") intv_ver = "2nd";
                else intv_ver = "3rd";

                row.Clear();
                tbl.Select("*", "RSVNO='" + rsvno + "'");
                Debug.WriteLine(tbl.lastsql);

                Dictionary<int, Dictionary<string, object>> intv_data = new Dictionary<int, Dictionary<string, object>>();
                while (tbl.FetchTo(out row) == true)
                {
                    int page = int.Parse(row["PAGE"].ToString());
                    intv_data[page] = row;
                }

                List<string> item_code_arr = new List<string>();
                foreach (string field in tst_cd_list)
                {
                    if (field.IndexOf("I3.") > -1)
                    {
                        item_code_arr.Add(field.Replace("I3.", ""));
                    }
                }

                string iv_sex = data["P.SEX"];

                Dictionary<string, string> decoded_iv_data = iv_obj.decode_iv_item(intv_ver, intv_data, item_code_arr, iv_sex);
                foreach (KeyValuePair<string, string> val in decoded_iv_data)
                {
                    if (data.ContainsKey(val.Key) == false)
                    {
                        data.Add("I3." + val.Key, val.Value);
                    }
                }
            }

            switch (ef)
            {
                case ExportFormat.ExportHtml:
                    ret.Append("<td align='center' width='60'>" + line_count + "</td>\n");
                    //ret.Append("<td width=\"100\">" + rsvno + "</td>\n");
                    //ret.Append("<td width=\"100\">" + pairs["P.PATNAME"] + "</td>\n");
                    break;
                case ExportFormat.ExportCsv:
                    ret.Append("\"" + line_count + "\",");
                    //ret.Append("\"" + rsvno + "\",");
                    //ret.Append("\"" + pairs["P.PATNAME"] + "\",");
                    break;
            }

            foreach (string field in tst_cd_list)
            {
                switch (ef)
                {
                    case ExportFormat.ExportHtml:
                        if (data.ContainsKey(field) == true)
                        {
                            ret.Append("<td>" + data[field].Replace("\r\n", "<br />\r\n") + "</td>\n");
                        }
                        else
                        {
                            ret.Append("<td>&nbsp;</td>\n");
                        }
                        break;
                    case ExportFormat.ExportCsv:
                        if (data.ContainsKey(field) == true)
                        {
                            ret.Append("\"" + data[field].Replace("\"", "\\\"") + "\",");
                        }
                        else
                        {
                            ret.Append("\"\",");
                        }
                        break;
                }
            }

            if (ef == ExportFormat.ExportHtml)
            {
                ret.Append("</tr>\n");
            }
            else
            {
                ret.Append("\r\n");
            }

            return ret.ToString();
        }

        public byte[] ToEuckrByte(string utf8str)
        {
            int euckrCodepage = 51949;
            System.Text.Encoding euckr = System.Text.Encoding.GetEncoding(euckrCodepage);

            return euckr.GetBytes(utf8str);
        }

        public ActionResult UserManager()
        {
            Table tbl = db.Table("USERMANAGE");

            UserManagerModel u = new UserManagerModel();
            u.list = new List<Dictionary<string, object>>();

            tbl.Select("*", "", "order by um_name");
            Dictionary<string, object> t;
            while (tbl.FetchTo(out t))
            {
                u.list.Add(t);
            }

            if (Request.QueryString["UM_ID"] != null)
            {
                u.id = Request.QueryString["UM_ID"];
                u.editData = tbl.SelectFetch("*", "UM_ID='" + u.id + "'");
            }
            else
            {
                u.editData = new Dictionary<string, object>();
            }

            return View(u);
        }

        public ActionResult IdDupCheck()
        {
            Table tbl = db.Table("USERMANAGE");
            Dictionary<string, object> data = tbl.SelectFetch("UM_ID", "UM_ID='" + Request.QueryString["UM_ID"].ToString() + "'");

            string result = "";

            if (data != null)
            {
                result += "<script>";
                result += "alert('ID가 이미 사용중입니다. 다른 ID를 입력해 주세요');";
                result += "parent.dupCheckOk(false);";
                result += "</script>";
            }
            else
            {
                result += "<script>";
                result += "alert('ID를 사용하실 수 있습니다');";
                result += "parent.dupCheckOk(true);";
                result += "</script>";
            }

            return Content(result);
        }

        public ActionResult PrcRemoveId()
        {
            Table tbl = db.Table("USERMANAGE");

            tbl.Delete("UM_ID='" + Request.QueryString["UM_ID"] + "'");

            string result = "";
            result += "<script>";
            result += "alert('ID를 삭제하였습니다');";
            result += "location.href = '/Home/UserManager';";
            result += "</script>";

            lm = new LogManager(db, Request, Session);
            lm.Log("IDDeleteOk", "사용자 삭제 : ID=[" + Request.QueryString["UM_ID"] + "]");

            return Content(result);
        }

        public ActionResult PrcUserCreate()
        {
            Table tbl = db.Table("USERMANAGE");

            tbl.rowinit();
            tbl.rowdata("UM_PW", Request.Form["UM_PW"]);
            tbl.rowdata("UM_NAME", Request.Form["UM_NAME"]);
            tbl.rowdata("UM_TEL", Request.Form["UM_TEL"]);
            tbl.rowdata("UM_OCCU", Request.Form["UM_OCCU"]);
            tbl.rowdata("UM_POS", Request.Form["UM_POS"]);
            tbl.rowdata("UM_TYPE", Request.Form["UM_TYPE"]);
            tbl.rowdata("UM_DATE_BEGIN", "FN:TO_DATE('" + Request.Form["UM_DATE_BEGIN"] + "','YYYY.MM.DD')");
            tbl.rowdata("UM_DATE_END", "FN:TO_DATE('" + Request.Form["UM_DATE_END"] + "','YYYY.MM.DD')");
            tbl.rowdata("UM_TIME_BEGIN_H", Request.Form["UM_TIME_BEGIN_H"]);
            tbl.rowdata("UM_TIME_BEGIN_M", Request.Form["UM_TIME_BEGIN_M"]);
            tbl.rowdata("UM_TIME_END_H", Request.Form["UM_TIME_END_H"]);
            tbl.rowdata("UM_TIME_END_M", Request.Form["UM_TIME_END_M"]);

            string userid = Session["userid"].ToString();

            tbl.rowdata("UM_UPDATE_DATE", "FN:TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd") + "','YYYY.MM.DD')");
            tbl.rowdata("UM_UPDATE_ID", userid);

            lm = new LogManager(db, Request, Session);

            if (Request.Form["UM_ID"] != null && Request.Form["UM_ID"] != "")
            {
                tbl.rowdata("UM_ID", Request.Form["UM_ID"]);
                tbl.Insert(tbl.rowvalues(), tbl.rowfields());

                lm.Log("IDCreateOk", "사용자 추가 : ID=[" + Request.Form["UM_ID"] + "] NAME=[" + Request.Form["UM_NAME"] + "] TYPE=[" + Request.Form["UM_TYPE"] + "] OCCU=[" + Request.Form["UM_OCCU"] + "]");
            }
            else
            {
                tbl.Update(tbl.rowupdate(), "UM_ID='" + Request.QueryString["UM_ID"] + "'");

                lm.Log("IDUpdateOk", "사용자 정보 수정 : ID=[" + Request.QueryString["UM_ID"] + "] NAME=[" + Request.Form["UM_NAME"] + "] TYPE=[" + Request.Form["UM_TYPE"] + "] OCCU=[" + Request.Form["UM_OCCU"] + "]");
            }

            return RedirectToAction("UserManager", "Home");
        }

        public ActionResult LogManager(int? page, String type)
        {
            int curPage = (page.HasValue == true) ? page.Value : 1;
            int pageSize = 15;
            int offset = (curPage - 1) * pageSize;

            List<string> cond = new List<string>();

            if (type != null)
            {
                if (type == "logon") cond.Add("LM_ACTION in ('LoginOk', 'LogoutOk', 'LoginError/NO_ID', 'LoginError/PASS_NOT_MATCH', 'LoginError/EXPIRED_ID')");
                if (type == "user") cond.Add("LM_ACTION in ('IDCreateOk', 'IDUpdateOk', 'IDDeleteOk')");
                if (type == "query") cond.Add("LM_ACTION in ('QueryOk', 'Query')");
                if (type == "update") cond.Add("LM_ACTION in ('UpdateOk', 'UpdateError')");
            }

            ViewData["name"] = Request.QueryString["LM_NAME"];
            ViewData["occu"] = Request.QueryString["LM_OCCU"];
            ViewData["date_begin"] = Request.QueryString["LM_DATE_BEGIN"];
            ViewData["date_end"] = Request.QueryString["LM_DATE_END"];
            if (ViewData["name"] != null && (string)ViewData["name"] != "")
            {
                cond.Add("(LM_NAME like '%" + ViewData["name"] + "%')");
            }
            if (ViewData["occu"] != null && (string)ViewData["occu"] != "")
            {
                cond.Add("(LM_OCCU like '%" + ViewData["occu"] + "%')");
            }
            if (ViewData["date_begin"] != null && ViewData["date_end"] != null && (string)ViewData["date_begin"] != "" && (string)ViewData["date_end"] != "")
            {
                cond.Add("(LM_DATE >= TO_DATE('" + ViewData["date_begin"] + " 00:00:00', 'yyyy-mm-dd HH24:MI:SS') and LM_DATE <= TO_DATE('" + ViewData["date_end"] + " 23:59:00', 'yyyy-mm-dd HH24:MI:SS'))");
            }

            string where = "";
            if (cond.Count > 0)
            {
                where = String.Join(" and ", cond.ToArray());
            }

            Table tblRow = db.Table("LOGMANAGE");
            ViewData["TotalItemCount"] = (int)tblRow.NumRows(where);

            if (where != "") where = "WHERE " + where;

            Table tbl;
            tbl = db.Table();
            tbl.SelectQuery("select * from ( select /*+ FIRST_ROWS(" + pageSize + ") */ a.*, ROWNUM rnum FROM ( SELECT * FROM LOGMANAGE " + where + " order by lm_no desc ) a WHERE ROWNUM <= " + (offset + pageSize) + " ) WHERE rnum  > " + offset);

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            //tbl = db.Table("( select /*+ FIRST_ROWS(" + pageSize + ") */ a.*, ROWNUM rnum FROM ( SELECT * FROM LOGMANAGE order by lm_no desc ) a WHERE ROWNUM < " + (offset + pageSize) + " )");
            //tbl.Select("*", "WHERE rnum  >= " + offset);

            Dictionary<string, object> t;
            while (tbl.FetchTo(out t))
            {
                list.Add(t);
            }

            ViewData["type"] = type;
            ViewData["PageSize"] = pageSize;
            ViewData["PageNumber"] = curPage;

            ViewData["sql"] = tbl.lastsql;

            return View(list);
        }

        public ActionResult PrcDecrypt()
        {
            //StringBuilder ret = new StringBuilder();
            //ret.Clear();
            List<string> decrypted = new List<string>();

            Response.Write("<script>\n");
            int count = 0;

            string cipher = Request.Form["Cipher"];
            string[] lines = cipher.Split( new string[] {"\n", "\r\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                string line_trim = line.Trim();
                string[] cells = line_trim.Split(new string[] { "\t" }, StringSplitOptions.None);
                foreach (string cell in cells)
                {
                    string cell_trim = cell.Trim();
                    decrypted.Add(Encryption.Decrypt(cell_trim));
                    count++;
                }
                Response.Write("parent.setDecrypted('" + string.Join("\t", decrypted.ToArray()) + "');\n");
                decrypted.Clear();
                //ret.AppendLine(string.Join("\t", decrypted.ToArray()));
                //ret.Clear();
            }
            Response.Write("</script>");

            lm = new LogManager(db, Request, Session);
            lm.Log("DecodeOk", "복호화 작업 : ID=[" + Session["userid"] + "], 총 [" + count + "]건의 데이터 복호화 완료");

            return null;
        }

        static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }

        public void AddQueryHistory(NameValueCollection form, string sq_id)
        {
            string md5hash = GetMd5Hash(form["query"]);

            Table tbl = db.Table("STOREDQUERY");
            Dictionary<string, object> row = tbl.SelectFetch("SQ_HASH", "SQ_HASH='" + md5hash + "' and SQ_ID='" + sq_id + "'");
            if (row == null)
            {
                tbl.rowinit();
                tbl.rowdata("SQ_HASH", md5hash);
                tbl.rowdata("SQ_ID", sq_id);
                tbl.rowdata("SQ_DESC", form["query_desc"].Replace("'", "''"));
                tbl.rowdata("SQ_QUERY", form["query"].Replace("'", "''"));
                tbl.rowdata("SQ_DATE", "FN:TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd") + "','YYYY.MM.DD')");

                tbl.Insert(tbl.rowvalues(), tbl.rowfields());
                Debug.WriteLine(tbl.lastsql);
            }
        }
    }

    public class UserManagerModel
    {
        public List<Dictionary<string, object>> list = null;
        public string id = null;
        public Dictionary<string, object> editData = null;

        public string val(string key) {
            if (editData.ContainsKey(key) == true)
            {
                if (key == "UM_DATE_BEGIN" || key == "UM_DATE_END")
                {
                    if (editData[key].ToString().Length > 10)
                    {
                        return editData[key].ToString().Substring(0, 10);
                    }
                    else return editData[key].ToString();
                }
                else return editData[key].ToString();
            }
            else return "";
        }
    }

}
