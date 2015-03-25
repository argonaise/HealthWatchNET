using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace HealthWatchNET.Controllers
{
    public class ModulesController : Controller
    {
        //
        // GET: /Modules/
        Database db = new Database();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TestPackage()
        {
            string pkgname = Request.QueryString["pkgname"];
            List<string> where = new List<string>();

            string sort = Request.QueryString["sort"];
            string dir = Request.QueryString["dir"];

            if (pkgname != null && pkgname != "")
            {
                where.Add(Database.GetParsedWhereRegex("PKGNM", pkgname));
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
            Debug.WriteLine("Where count: " + where.Count);
            if (where.Count > 0)
            {
                cond = string.Join(" and ", where.ToArray());
            }

            //string sql = "SELECT * FROM ( SELECT a.NO, a.PKGCODE, a.PKGNAME, a.FDATE, b.PKGCOUNT, ROWNUM rnum FROM ( SELECT NO, PKGCODE, PKGNAME, FDATE FROM TESTPACKAGE " + cond + " " + order + " ) a, ( SELECT PKGCODE, count(*) PKGCOUNT GROUP BY PKGCODE ) b WHERE a.PKGCODE=b.PKGCODE AND a.ROWNUM <= " + (offset + to) + " ) WHERE rnum > " + offset;
            //string sql = "SELECT PKGCODE, PKGNM as PKGNAME, EDATE FROM HCSUPKGM " + cond + " " + order;
            //Debug.WriteLine(sql);

            Table t = db.Table("HCSUPKGM");
            t.Select("PKGCODE, PKGNM as PKGNAME, EDATE", cond, order);

            return View(t);
        }

        public ActionResult TestItem()
        {
            string offset = Request.QueryString["offset"];
            string pkgcode = Request.QueryString["pkgcode"];
            string itemname = Request.QueryString["itemname"];
            List<string> where = new List<string>();

            int offset_cnt = 0;
            if (offset != null && offset != "")
            {
                offset_cnt = int.Parse(offset);
            }

            string sort = Request.QueryString["sort"];
            string dir = Request.QueryString["dir"];

            string order = null;
            if (sort != null && sort != "")
            {
                order = "order by " + sort + " " + dir;
            }
            else
            {
                order = "order by b.STARDATE asc";
            }

            //string sql = "SELECT a.LCODE AS TST_CD, a.CODENAME as TST_NAME FROM HCSCODLT a, HCSUPKGE b WHERE b.PKGCODE='" + query + "' and a.LCODE=b.EXAMCODE(+) and a.GRPCODE='C0001' and a.USEYN='Y' " + order;
            //Debug.WriteLine(sql);

            where.Add("a.LCODE=b.EXAMCODE(+) and a.GRPCODE='C0001' and a.USEYN='Y'");

            if (pkgcode != null && pkgcode != "")
            {
                where.Add("( b.PKGCODE='" + pkgcode + "' )");
            }

            if (itemname != null && itemname != "")
            {
                where.Add(Database.GetParsedWhereRegex("a.CODENAME", itemname));
            }

            string cond = null;
            Debug.WriteLine("Where count: " + where.Count);
            if (where.Count > 0)
            {
                cond = string.Join(" and ", where.ToArray());
            }

            TestItemModel tm = new TestItemModel();
            tm.bMoreData = false;
            tm.offset = offset_cnt;
            tm.next_offset = offset_cnt + 1000;

            Table s = db.Table("HCSCODLT a, HCSUPKGE b");
            Dictionary<string,object> row = s.SelectFetch("count(*) as CNT", cond);
            int count;
            if (int.TryParse((string)row["CNT"], out count))
            {
                if (count > tm.next_offset)
                {
                    tm.bMoreData = true;
                }
            }

            Table t = db.Table();
            if (tm.offset > 0)
            {
                t.SelectQuery("SELECT TST_CD, TST_NAME FROM ( SELECT c.*, ROWNUM bnum FROM ( select a.LCODE AS TST_CD, a.CODENAME as TST_NAME, ROWNUM as anum FROM HCSCODLT a, HCSUPKGE b WHERE " + cond + " " + order + " ) c WHERE anum >= " + tm.offset + " ) WHERE bnum <= 1000 GROUP BY TST_CD, TST_NAME");
            }
            else
            {
                t.SelectQuery("SELECT TST_CD, TST_NAME FROM ( select a.LCODE AS TST_CD, a.CODENAME as TST_NAME, ROWNUM as anum FROM HCSCODLT a, HCSUPKGE b WHERE " + cond + " " + order + " ) WHERE anum <= 1000 GROUP BY TST_CD, TST_NAME");
            }
            Debug.WriteLine(t.lastsql);

            tm.t = t;

            return View(tm);
        }

        public class TestItemModel
        {
            public Table t;
            public bool bMoreData = false;
            public int offset;
            public int next_offset;
        }

        public ActionResult AddTestItemByCode()
        {
            string tst_cd_get = Request.QueryString["tst_cd_each"];
            string tst_cd_post = Request.Form["tst_cd[]"];
            Table s = db.Table("SELECTEDITEM");

            List<string> tst_cd_loop = new List<string>();
            bool isPostMethod = false;

            // GET으로 데이터가 들어온 경우..
            if (tst_cd_get != null && tst_cd_get != "")
            {
                tst_cd_loop.Add(tst_cd_get.Trim());
            }

            // POST로 데이터가 들어온 경우..
            if (tst_cd_post != null && tst_cd_post != "")
            {
                tst_cd_post = tst_cd_post.Trim();
                string[] tmp = tst_cd_post.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                tmp.ToList().ForEach(t => { tst_cd_loop.Add(t); });
                isPostMethod = true;
            }

            foreach (string tst_cd in tst_cd_loop)
            {
                if (tst_cd != "" && tst_cd != null)
                {
                    Dictionary<string, object> row = null;
                    Dictionary<string, object> in_row = null;
                    int seq = 0;

                    // max seq값 조회
                    row = s.SelectFetch("MAX(SEQ) as MAXSEQ", "USER_ID='" + Session["userid"] + "' and PSET_NAME='^^'");
                    if (row != null && row["MAXSEQ"].ToString() != "")
                    {
                        seq = Int16.Parse(row["MAXSEQ"].ToString());
                    }

                    // 패널 코드인지 검사 - 하위 항목들이 있나 조회해 봄..
                    Table t = db.Table("PANEL2CODE");
                    Table t1 = db.Table("HCSCODLT");
                    t.Select("PANL_TST_NM, TST_CD", "PANL_TST_CD='" + tst_cd + "' and TST_CD!='" + tst_cd + "'", "group by PANL_TST_NM, TST_CD");
                    Debug.WriteLine(t.lastsql);

                    int cnt = 0;
                    while (t.FetchTo(out row) == true)
                    {
                        in_row = t1.SelectFetch("LCODE, CODENAME", "LCODE='" + row["TST_CD"] + "'");

                        cnt++;
                        //content += "parent.addOption('" + row["TST_SNM"] + "','" + row["TST_CD"] + "')\n";

                        s.rowinit();
                        s.rowdata("PSET_NAME", "^^");
                        s.rowdata("PANEL_CD", tst_cd);
                        s.rowdata("PANEL_NM", (string)row["PANL_TST_NM"]);
                        s.rowdata("TEST_CD", (string)in_row["LCODE"]);
                        s.rowdata("TEST_NM", (string)in_row["CODENAME"]);
                        s.rowdata("SEQ", (seq + cnt).ToString());
                        s.rowdata("USER_ID", (string)Session["userid"]);
                        s.Insert(s.rowvalues(), s.rowfields());
                        Debug.WriteLine(s.lastsql);
                    }

                    if (cnt == 0)
                    {
                        // 패널 코드가 아님
                        t1.Select("LCODE, CODENAME", "LCODE='" + tst_cd + "'");
                        if (t1.FetchTo(out row) == true)
                        {
                            //content += "parent.addOption('" + row["CODENAME"] + "','" + row["LCODE"] + "')\n";
                            s.rowinit();
                            s.rowdata("PSET_NAME", "^^");
                            s.rowdata("PANEL_CD", (string)row["LCODE"]);
                            s.rowdata("PANEL_NM", (string)row["CODENAME"]);
                            s.rowdata("TEST_CD", (string)row["LCODE"]);
                            s.rowdata("TEST_NM", (string)row["CODENAME"]);
                            s.rowdata("SEQ", (seq + 1).ToString());
                            s.rowdata("USER_ID", (string)Session["userid"]);
                            s.Insert(s.rowvalues(), s.rowfields());
                            Debug.WriteLine(s.lastsql);
                        }
                    }
                }
            }

            if (isPostMethod == true)
            {
                return Content("<script> parent.refreshSelectedList(); location.href = '/Modules/TestItem'; </script>\n");
            }
            else
            {
                return Content("<script> parent.refreshSelectedList(); </script>\n");
            }
        }

        public ActionResult PatientIvItem()
        {
            DataTable t = InterviewItem.getInterviewItems();
            return View(t);
        }

        public ActionResult AddPatIvItemByCode()
        {
            string tst_cd_get = Request.QueryString["intv_cd_each"];
            string tst_cd_post = Request.Form["intv_cd[]"];
            Table s = db.Table("SELECTEDITEM");

            List<string> tst_cd_loop = new List<string>();
            bool isPostMethod = false;

            // GET으로 데이터가 들어온 경우..
            if (tst_cd_get != null && tst_cd_get != "")
            {
                tst_cd_loop.Add(tst_cd_get.Trim());
            }

            // POST로 데이터가 들어온 경우..
            if (tst_cd_post != null && tst_cd_post != "")
            {
                tst_cd_post = tst_cd_post.Trim();
                string[] tmp = tst_cd_post.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                tmp.ToList().ForEach(t => { tst_cd_loop.Add(t); });
                isPostMethod = true;
            }

            InterviewItem ii = new InterviewItem();
            Dictionary<string, string> names = ii.getDictionary();

            foreach (string tst_cd in tst_cd_loop)
            {
                if (tst_cd != "" && tst_cd != null)
                {
                    Dictionary<string, object> row = null;
                    int seq = 0;

                    row = s.SelectFetch("MAX(SEQ) as MAXSEQ", "USER_ID='" + Session["userid"] + "' and PSET_NAME='^^'");
                    if (row != null && row["MAXSEQ"].ToString() != "")
                    {
                        seq = Int16.Parse(row["MAXSEQ"].ToString());
                    }

                    s.rowinit();
                    s.rowdata("PSET_NAME", "^^");
                    s.rowdata("PANEL_CD", tst_cd);
                    s.rowdata("PANEL_NM", names[tst_cd]);
                    s.rowdata("TEST_CD", tst_cd);
                    s.rowdata("TEST_NM", names[tst_cd]);
                    s.rowdata("SEQ", (seq + 1).ToString());
                    s.rowdata("USER_ID", (string)Session["userid"]);
                    s.Insert(s.rowvalues(), s.rowfields());
                    Debug.WriteLine(s.lastsql);
                }
            }

            if (isPostMethod == true)
            {
                return Content("<script> parent.refreshSelectedList(); location.href = '/Modules/PatientIvItem'; </script>\n");
            }
            else
            {
                return Content("<script> parent.refreshSelectedList(); </script>\n");
            }
        }

        public ActionResult PresetList()
        {
            Table t = db.Table("SELECTEDITEM");
            t.Select("PSET_NAME", "USER_ID='" + Session["userid"] + "' and PSET_NAME<>'^^'", "group by PSET_NAME order by PSET_NAME");

            return View(t);
        }

        public ActionResult AddPreset()
        {
            string name = Request.QueryString["name"];
            string clear = Request.QueryString["clear"];
            if (name == null || name.Equals("") == true)
            {
                return Content("<script> alert('프리셋 이름을 입력해 주세요'); </script>\n");
            }
            else
            {
                Table t = db.Table("SELECTEDITEM");

                // 먼저 같은 이름의 preset을 지운다..
                t.Delete("USER_ID='" + Session["userid"] + "' and PSET_NAME='" + name + "'");

                if (clear != null && clear != "")
                {
                    // Preset name ^^를 name으로 update하면 끝남..
                    t.Update("PSET_NAME='" + name + "'", "USER_ID='" + Session["userid"] + "' and PSET_NAME='^^'");
                }
                else
                {
                    // Default preset ^^을 PSET_NAME으로 복사..
                    Table s = db.Table("SELECTEDITEM");
                    t.Select("*", "USER_ID='" + Session["userid"] + "' and PSET_NAME='^^'");
                    Dictionary<string, object> row;
                    while (t.FetchTo(out row) == true)
                    {
                        s.rowinit();
                        s.rowdata("PSET_NAME", name);
                        s.rowdata("PANEL_CD", (string)row["PANEL_CD"]);
                        s.rowdata("PANEL_NM", (string)row["PANEL_NM"]);
                        s.rowdata("TEST_CD", (string)row["TEST_CD"]);
                        s.rowdata("TEST_NM", (string)row["TEST_NM"]);
                        s.rowdata("SEQ", (string)row["SEQ"]);
                        s.rowdata("USER_ID", (string)Session["userid"]);
                        s.Insert(s.rowvalues(), s.rowfields());
                        Debug.WriteLine(s.lastsql);
                    }
                }
                return Content("<script> parent.refreshSelectedList(); parent.refreshPresetList(); </script>\n");
            }
        }

        public ActionResult UsePreset()
        {
            string name_get = Request.QueryString["name_each"];
            string name_post = Request.Form["pset_name[]"];

            bool isPostMethod = false;

            List<string> nameLoop = new List<string>();

            // GET으로 데이터가 들어온 경우..
            if (name_get != null && name_get != "")
            {
                nameLoop.Add(name_get.Trim());
            }

            // POST로 데이터가 들어온 경우..
            if (name_post != null && name_post != "")
            {
                name_post = name_post.Trim();
                string[] tmp = name_post.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                tmp.ToList().ForEach(p => { nameLoop.Add(p); });
                isPostMethod = true;
            }

            Table t = db.Table("SELECTEDITEM");
            Table s = db.Table("SELECTEDITEM");

            // 먼저 사용중인 preset을 정리한다..
            t.Delete("USER_ID='" + Session["userid"] + "' and PSET_NAME='^^'");

            foreach (string name in nameLoop)
            {
                t.Select("*", "USER_ID='" + Session["userid"] + "' and PSET_NAME='" + name + "'");
                Dictionary<string, object> row;
                while (t.FetchTo(out row) == true)
                {
                    s.rowinit();
                    s.rowdata("PSET_NAME", "^^");
                    s.rowdata("PANEL_CD", (string)row["PANEL_CD"]);
                    s.rowdata("PANEL_NM", (string)row["PANEL_NM"]);
                    s.rowdata("TEST_CD", (string)row["TEST_CD"]);
                    s.rowdata("TEST_NM", (string)row["TEST_NM"]);
                    s.rowdata("SEQ", (string)row["SEQ"]);
                    s.rowdata("USER_ID", (string)row["USER_ID"]);
                    s.Insert(s.rowvalues(), s.rowfields());
                    Debug.WriteLine(s.lastsql);
                }
            }

            if (isPostMethod == true)
            {
                return Content("<script> parent.refreshSelectedList(); location.href = '/Modules/PresetList'; </script>\n");
            }
            else
            {
                return Content("<script> parent.refreshSelectedList(); </script>\n");
            }
        }

        public ActionResult DelPreset()
        {
            string name_post = Request.Form["pset_name[]"];

            List<string> nameLoop = new List<string>();

            // POST로 데이터가 들어온 경우..
            if (name_post != null && name_post != "")
            {
                name_post = name_post.Trim();
                string[] tmp = name_post.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                tmp.ToList().ForEach(p => { nameLoop.Add(p); });
            }

            Table t = db.Table("SELECTEDITEM");

            foreach (string name in nameLoop)
            {
                t.Delete("USER_ID='" + Session["userid"] + "' and PSET_NAME='" + name + "'");
            }

            return Content("<script> location.href = '/Modules/PresetList'; </script>\n");
        }

        public ActionResult SelectedItems()
        {
            Table t = db.Table("SELECTEDITEM");
            t.Select("*", "PSET_NAME='^^' and USER_ID='" + Session["userid"] + "'", "order by SEQ");

            return View(t);
        }

        public ActionResult DelSelectedItems()
        {
            //string[] keys = Request.Form.AllKeys;
            //keys.ToList().ForEach(key => { content += key + " = " + Request.Form[key] + "<br />"; });
            Table t = db.Table("SELECTEDITEM");

            string[] values = Request.Form["cd[]"].Split(',');
            foreach (string val in values)
            {
                var tmp = val.Split(new String[] {"!!"},StringSplitOptions.RemoveEmptyEntries);
                Debug.WriteLine(val);
                // @ret["PANEL_CD"]!!@ret["TEST_CD"]!!@ret["SEQ"]!!@ret["USER_ID"]
                t.Delete("PSET_NAME='^^' and PANEL_CD='" + tmp[0] + "' and TEST_CD='" + tmp[1] + "' and SEQ='" + tmp[2] + "' and USER_ID='" + tmp[3] + "'");
                Debug.WriteLine(t.lastsql);
            }

            return Content("<script> location.href='/Modules/SelectedItems'; </script>\n");
            //return Content(content);
        }

    }
}
