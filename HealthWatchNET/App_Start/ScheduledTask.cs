using Quartz;
using Quartz.Impl;
using System.Collections.Generic;
using System;

namespace HealthWatchNET
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler s = StdSchedulerFactory.GetDefaultScheduler();
            s.Start();

            IJobDetail job = JobBuilder.Create<CodeScanner>().Build();
            ITrigger trigger = TriggerBuilder.Create().WithDailyTimeIntervalSchedule(
                t => t.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(
                        TimeOfDay.HourAndMinuteOfDay(0, 0)
                    )
                ).Build();

            s.ScheduleJob(job, trigger);
        }
    }

    public class CodeScanner : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Database db = new Database();

            Table tmp_table = db.Table("CODE2TABLETMP");
            tmp_table.Query("truncate table CODE2TABLETMP");
            Dictionary<string, object> row;

            // Code Scan : HCSUREST
            Table tbl_rest = db.Table("HCSUREST a, HCSCODLT b");
            tbl_rest.Select("a.EXAMCODE as TST_CD, b.CODENAME as TST_NM", "a.EXAMCODE=b.LCODE", "group by a.EXAMCODE, b.CODENAME");
            while (tbl_rest.FetchTo(out row) == true)
            {
                tmp_table.rowinit();
                tmp_table.rowdata("TST_CD", row["TST_CD"].ToString());
                tmp_table.rowdata("TST_NM", row["TST_NM"].ToString());
                tmp_table.rowdata("TABLENAME", "HCSUREST");
                tmp_table.Insert(tmp_table.rowvalues(), tmp_table.rowfields());
            }

            // Code Scan with Name : HCSURE0T
            Table tbl_re0t = db.Table("HCSURE0T");
            tbl_re0t.Select("TST_CD, TST_NM", "", "group by TST_CD, TST_NM");
            while (tbl_re0t.FetchTo(out row) == true)
            {
                tmp_table.rowinit();
                tmp_table.rowdata("TST_CD", row["TST_CD"].ToString());
                tmp_table.rowdata("TST_NM", row["TST_NM"].ToString());
                tmp_table.rowdata("TABLENAME", "HCSURE0T");
                tmp_table.Insert(tmp_table.rowvalues(), tmp_table.rowfields());
            }

            // Code Scan : HCSUXW0T
            Table tbl_xw0t = db.Table("HCSUXW0T");
            tbl_xw0t.Select("TST_CD, TST_SNM as TST_NM", "", "group by TST_CD, TST_SNM");
            while (tbl_xw0t.FetchTo(out row) == true)
            {
                tmp_table.rowinit();
                tmp_table.rowdata("TST_CD", row["TST_CD"].ToString());
                tmp_table.rowdata("TST_NM", row["TST_NM"].ToString());
                tmp_table.rowdata("TABLENAME", "HCSUXW0T");
                tmp_table.Insert(tmp_table.rowvalues(), tmp_table.rowfields());
            }

            // Code Scan : TMP table to REAL table
            tmp_table.Query("truncate table CODE2TABLE");
            tmp_table.Query("insert into CODE2TABLE (TST_CD, TST_NM, TABLENAME) select TST_CD, TST_NM, TABLENAME from CODE2TABLETMP");

            // Panel Code Scan : HCSUXW0T
            Table panl_tmp_table = db.Table("PANEL2CODETMP");
            Table tbl_xw0t_pnl = db.Table("HCSUXW0T a, HCSCODLT b");
            tbl_xw0t_pnl.Select("a.TST_CD, a.TST_SNM, a.PANL_TST_CD as PANL_TST_CD, b.CODENAME as PANL_TST_NM", "a.PANL_TST_CD=b.LCODE", "group by a.TST_CD, a.TST_SNM, a.PANL_TST_CD, b.CODENAME");
            while (tbl_xw0t_pnl.FetchTo(out row) == true)
            {
                panl_tmp_table.rowinit();
                panl_tmp_table.rowdata("TST_CD", row["TST_CD"].ToString());
                panl_tmp_table.rowdata("TST_NM", row["TST_NM"].ToString());
                panl_tmp_table.rowdata("PANL_TST_CD", row["PANL_TST_CD"].ToString());
                panl_tmp_table.rowdata("PANL_TST_NM", row["PANL_TST_NM"].ToString());
                panl_tmp_table.Insert(panl_tmp_table.rowvalues(), panl_tmp_table.rowfields());
            }

            // Panel Code Scan : TMP table to REAL table
            panl_tmp_table.Query("truncate table PANEL2CODE");
            panl_tmp_table.Query("insert into PANEL2CODE (TST_CD, TST_NM, PANL_TST_CD, PANL_TST_NM) select TST_CD, TST_NM, PANL_TST_CD, PANL_TST_NM from PANEL2CODETMP");

            // Daily Stat - Counts
            row = tbl_re0t.SelectFetch("count(*) as ROWCOUNT");
            string row_re0t = row["ROWCOUNT"].ToString();

            row = tbl_rest.SelectFetch("count(*) as ROWCOUNT");
            string row_rest = row["ROWCOUNT"].ToString();

            row = tbl_xw0t.SelectFetch("count(*) as ROWCOUNT");
            string row_xw0t = row["ROWCOUNT"].ToString();

            Table tbl_patb = db.Table("HCSUPATB");
            row = tbl_patb.SelectFetch("count(*) as ROWCOUNT");
            string row_patb = row["ROWCOUNT"].ToString();

            Table tbl_rsvt = db.Table("HCSURSVT");
            row = tbl_rsvt.SelectFetch("count(*) as ROWCOUNT");
            string row_rsvt = row["ROWCOUNT"].ToString();

            tmp_table.Query(
                "insert into TABLEROWSTAT (CNTDATE, HCSUPATB, HCSURSVT, HCSURE0T, HCSUREST, HCSUXW0T) values " +
                "(TO_DATE('" + DateTime.Now.ToString("yyyy-MM-dd") + "','yyyy-mm-dd'), '" + row_patb + "', '" + row_rsvt + "', '" + row_re0t + "', '" + row_rest + "', '" + row_xw0t + "'"
            );
        }
    }
}