using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using wasaRms.Models;

namespace wasaRms.Controllers
{
    public class PumpsController : Controller
    {
        // GET: Pumps
        public ActionResult Dashboard()
        {
            string query = "WITH ranked_messages AS ( ";
            query += "SELECT r.resourceName as nm, SUM(s.parameterValue) as sm, Count(s.parameterValue) as cn, ROW_NUMBER() OVER(PARTITION BY r.resourceName ORDER BY s.sheetInsertionDateTime DESC) AS rn ";
            query += "FROM tblSheet s ";
            query += "inner join tblResource r on s.resourceID = r.resourceID ";
            query += "inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
            query += "where rt.resourceTypeName = 'Pumps Status' ";
            query += "group by s.sheetInsertionDateTime, r.resourceName ";
            query += ") ";
            query += "SELECT nm, sm, cn FROM ranked_messages WHERE rn = 1; ";
            string tempName = "";
            //string query = "select resourceTypeID, resourceTypeName from tblResourceType where resourceTypeName = 'Pumps Status' ";
            //query += "and resourceTypeID in (select resourceTypeID from tblResource) ";
            string S1query = "select COUNT(t.parameterValue) from tblSheet t ";
            S1query += "inner join( ";
            S1query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
            S1query += " group by resourceID ";
            S1query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
            S1query += " inner join tblResource r on t.resourceID = r.resourceID ";
            S1query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
            S1query += " where rt.resourceTypeName = 'Pumps Status' ";
            //S1query += " and t.sheetInsertionDateTime > GETDATE() - 3600 ";
            string S2query = "select COUNT(t.parameterValue) from tblSheet t ";
            S2query += "inner join( ";
            S2query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
            S2query += " group by resourceID ";
            S2query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
            S2query += " inner join tblResource r on t.resourceID = r.resourceID ";
            S2query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
            S2query += " where rt.resourceTypeName = 'Pumps Status' ";
            //S2query += " and t.sheetInsertionDateTime > GETDATE() - 3600 ";
            S2query += "and t.parameterValue = 1";
            string S3query = "select COUNT(t.parameterValue) from tblSheet t ";
            S3query += "inner join( ";
            S3query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
            S3query += " group by resourceID ";
            S3query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
            S3query += " inner join tblResource r on t.resourceID = r.resourceID ";
            S3query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
            S3query += " where rt.resourceTypeName = 'Pumps Status' ";
            //S3query += " and t.sheetInsertionDateTime > GETDATE() - 3600 ";
            S3query += "and t.parameterValue = 0";
            //List<SelectListItem> items = new List<SelectListItem>();
            //using (SqlConnection con1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand(query))
            //    {
            //        cmd.Connection = con1;
            //        con1.Open();
            //        SqlCommand cmd1 = new SqlCommand(S1query, con1);
            //        SqlCommand cmd2 = new SqlCommand(S2query, con1);
            //        SqlCommand cmd3 = new SqlCommand(S3query, con1);
            //        ViewBag.TotalPumps = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
            //        ViewBag.RunningPumps = Convert.ToInt32(cmd2.ExecuteScalar()).ToString();
            //        ViewBag.InactivePumps = Convert.ToInt32(cmd3.ExecuteScalar()).ToString();
            //        using (SqlDataReader sdr = cmd.ExecuteReader())
            //        {
            //            while (sdr.Read())
            //            {
            //                items.Add(new SelectListItem
            //                {
            //                    Text = sdr["resourceTypeName"].ToString(),
            //                    Value = sdr["resourceTypeID"].ToString()
            //                });
            //            }
            //        }
            //        con1.Close();
            //    }
            //}
            string parameterValuesString = "";
            string datetimed = "";
            string markers = "[";
            rmsWasa01Entities db = new rmsWasa01Entities(); 
            string q = "select r.resourceName, r.resourceID, r.resourceGeoLocatin, t.resourceTypeName from tblResource r left join tblResourceType t on r.resourceTypeID = t.resourceTypeID where t.resourceTypeName = 'Pumps Status'";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd11 = new SqlCommand(S1query, conn);
                    SqlCommand cmd2 = new SqlCommand(S2query, conn);
                    SqlCommand cmd3 = new SqlCommand(S3query, conn);
                    ViewBag.TotalPumps = Convert.ToInt32(cmd11.ExecuteScalar()).ToString();
                    ViewBag.RunningPumps = Convert.ToInt32(cmd2.ExecuteScalar()).ToString();
                    ViewBag.InactivePumps = Convert.ToInt32(cmd3.ExecuteScalar()).ToString();
                    SqlCommand cmd = new SqlCommand(q, conn);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            string q1 = "select distinct r.resourceName,  t.resourceTypeName, p.parameterName, ";
                            q1 += " e.parameterValue, e.sheetInsertionDateTime from tblSheet e ";
                            q1 += "left join tblParameter p on e.parameterID = p.parameterID ";
                            q1 += "left join tblResource r on e.resourceID = r.resourceID ";
                            q1 += "left join tblResourceType t on r.resourceTypeID = t.resourceTypeID ";
                            q1 += "where e.sheetInsertionDateTime = (select max(sheetInsertionDateTime) from tblSheet where resourceID = " + sdr["resourceID"] + " ";
                            q1 += " and r.resourceID = " + sdr["resourceID"] + ")";
                            SqlCommand cmd1 = new SqlCommand(q1, conn);
                            using (SqlDataReader sdr1 = cmd1.ExecuteReader())
                            {
                                while (sdr1.Read())
                                {
                                    string valuee = "";
                                    parameterValuesString += "";
                                    parameterValuesString += sdr1["parameterName"].ToString() + ": ";
                                    if (sdr1["parameterName"].ToString() == "pumpStatus1")
                                    {
                                        if (sdr1["parameterValue"].ToString() == "0")
                                        {
                                            valuee = "OFF";
                                        }
                                        else
                                        {
                                            valuee = "ON";
                                        }
                                    }
                                    else if (sdr1["parameterName"].ToString() == "pumpStatus2")
                                    {
                                        if (sdr1["parameterValue"].ToString() == "0")
                                        {
                                            valuee = "OFF";
                                        }
                                        else
                                        {
                                            valuee = "ON";
                                        }
                                    }
                                    else if (sdr1["parameterName"].ToString() == "pumpStatus3")
                                    {
                                        if (sdr1["parameterValue"].ToString() == "0")
                                        {
                                            valuee = "OFF";
                                        }
                                        else
                                        {
                                            valuee = "ON";
                                        }
                                    }
                                    else if (sdr1["parameterName"].ToString() == "pumpStatus4")
                                    {
                                        if (sdr1["parameterValue"].ToString() == "0")
                                        {
                                            valuee = "OFF";
                                        }
                                        else
                                        {
                                            valuee = "ON";
                                        }
                                    }
                                    else if (sdr1["parameterName"].ToString() == "pumpStatus5")
                                    {
                                        if (sdr1["parameterValue"].ToString() == "0")
                                        {
                                            valuee = "OFF";
                                        }
                                        else
                                        {
                                            valuee = "ON";
                                        }
                                    }
                                    else if (sdr1["parameterName"].ToString() == "pumpStatus6")
                                    {
                                        if (sdr1["parameterValue"].ToString() == "0")
                                        {
                                            valuee = "OFF";
                                        }
                                        else
                                        {
                                            valuee = "ON";
                                        }
                                    }
                                    parameterValuesString += valuee;
                                    parameterValuesString += "<br />";
                                    datetimed = sdr1["sheetInsertionDateTime"].ToString();
                                }
                            }
                            tempName = sdr["resourceTypeName"].ToString().Substring(0, 1);
                            string newstring = "<b>" + sdr["resourceTypeName"].ToString() + "</b>";
                            newstring += "<br />";
                            newstring += "<b>" + sdr["resourceName"].ToString() + "</b>";
                            newstring += "<br />";
                            newstring += datetimed;
                            newstring += "<br />";
                            newstring += parameterValuesString;
                            TimeSpan duration = (Convert.ToDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pakistan Standard Time").ToString()) - Convert.ToDateTime(datetimed.ToString()));
                            double minu = duration.TotalMinutes;
                            parameterValuesString = "";
                            markers += "{";
                            markers += string.Format("'Template': '{0}',", tempName);
                            markers += string.Format("'title': '{0}',", sdr["resourceName"]);
                            //markers += string.Format("'time': '{0}',", sdr1["InsertionDateTime"]);
                            markers += string.Format("'lat':'{0}',", sdr["resourceGeoLocatin"].ToString().Split(',')[0]);
                            markers += string.Format("'lnt':'{0}',", sdr["resourceGeoLocatin"].ToString().Split(',')[1]);
                            //markers += string.Format("'parameter':'{0}',", sdr1["ParameterName"]);
                            //markers += string.Format("'value':'{0}'", sdr1["ParameterValue"]);
                            markers += string.Format("'DelTime': '{0}',", minu);
                            markers += string.Format("'description': '{0}'", newstring);
                            markers += "},";
                        }
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {

                }
            }

            markers += "]";
            ViewBag.PondingMarkers = markers;
            var tablList = getPumpTableList();
            return View(tablList);
        }
        public JsonResult LoadPumpsChartData()
        {
            DataTable dt = new DataTable();
            string JSONresult = "";
            string query = "WITH ranked_messages AS ( ";
            query += "SELECT r.resourceName as category, r.resourceName as townName2, 'Lahore' as townName, SUM(s.parameterValue) as ponding, Count(s.parameterValue) as cn, ROW_NUMBER() OVER(PARTITION BY r.resourceName ORDER BY s.sheetInsertionDateTime DESC) AS rn ";
            query += "FROM tblSheet s ";
            query += "inner join tblResource r on s.resourceID = r.resourceID ";
            query += "inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
            query += "where rt.resourceTypeName = 'Pumps Status' ";
            query += "group by s.sheetInsertionDateTime, r.resourceName ";
            query += ") ";
            query += "SELECT category, townName2, townName, ponding, cn FROM ranked_messages WHERE rn = 1; ";

            //string query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, p.parameterName as pName, s.parameterValue AS ponding, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT * FROM cte WHERE rn = 1";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                    sda.Fill(dt);
                    JSONresult = JsonConvert.SerializeObject(dt);
                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
            var data = new { status = "Success" };
            return Json(JSONresult, JsonRequestBehavior.AllowGet);
        }

        public List<PumpTableClass> getPumpTableList()
        {
            var tablList = new List<PumpTableClass>();
            DataTable Dashdt = new DataTable();
            string query = "WITH ranked_messages AS ( ";
            query += "SELECT r.resourceLocationName as nm, SUM(s.parameterValue) as sm, Count(s.parameterValue) as cn, ROW_NUMBER() OVER(PARTITION BY r.resourceLocationName ORDER BY s.sheetInsertionDateTime DESC) AS rn ";
            query += "FROM tblSheet s ";
            query += "inner join tblResource r on s.resourceID = r.resourceID ";
            query += "inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
            query += "where rt.resourceTypeName = 'Pumps Status' ";
            query += "group by s.sheetInsertionDateTime, r.resourceLocationName ";
            query += ") ";
            query += "SELECT nm, sm, cn FROM ranked_messages WHERE rn = 1; ";
            string Dashdtquery = query;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                    sda.Fill(Dashdt);
                    foreach (DataRow dr in Dashdt.Rows)
                    {
                        var tableclass = new PumpTableClass();
                        tableclass.srNo = Convert.ToInt32(Dashdt.Rows.IndexOf(dr) + 1).ToString();
                        tableclass.pumpLocation = dr["nm"].ToString();
                        tableclass.runningPump = dr["sm"].ToString();
                        tableclass.inactivePump = (Convert.ToInt32(dr["cn"]) - Convert.ToInt32(dr["sm"])).ToString();
                        tablList.Add(tableclass);
                    }
                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
            return tablList;
        }

        public ActionResult PumpReport()
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Pumps Status"))
            {
                ResourceList.Add(item.resourceLocationName);
            }
            ViewBag.ResourceList = ResourceList;
            /////////////////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            string resourceQuery = "select DISTINCT resourceTypeID, resourceTypeName from tblResourceType where resourceTypeName = 'Pumps Status' ";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    DataTable dtTemp = new DataTable();
                    SqlDataAdapter sdaTemp = new SqlDataAdapter(resourceQuery, conn);
                    sdaTemp.Fill(dtTemp);
                    int ite = 0;
                    foreach (DataRow drTemp in dtTemp.Rows)
                    {
                        string tempName = drTemp["resourceTypeName"].ToString();
                        string getResFromTemp = "select r.resourceID, r.resourceName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Pumps Status' ";
                        SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                        DataTable dtRes = new DataTable();
                        sdaRes.Fill(dtRes);
                        foreach (DataRow drRes in dtRes.Rows)
                        {
                            string resName = drRes["resourceName"].ToString();
                            ite += 1;
                            string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["resourceID"]) + "";
                            SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                            DataTable dtPar = new DataTable();
                            sdaPar.Fill(dtPar);
                            scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                            scriptString += "theme: \"light2\",";
                            scriptString += "animationEnabled: true,";
                            scriptString += "zoomEnabled: true, ";
                            scriptString += "title: {text: \"Pumps\" },";
                            scriptString += "subtitles: [{text: \" All Pumps Recent Status \" }],";
                            //scriptString += "axisY: {includeZero: false, prefix: \"\" },";
                            //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                            if (tempName == "Consumption")
                            {
                                scriptString += "axisY: {includeZero: false, suffix: \" liter\" },";
                            }
                            else
                            {
                                scriptString += "axisY: {includeZero: false, suffix: \"\" },";
                            }
                            scriptString += "toolTip: { shared: true },";
                            scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries},";
                            scriptString += " data: [";
                            foreach (DataRow drPar in dtPar.Rows)
                            {
                                string parName = drPar["parameterName"].ToString();
                                string aquery = ";WITH CTE AS ( ";
                                aquery += "SELECT e.parameterID, e.parameterValue, e.sheetInsertionDateTime,  ";
                                aquery += " RN = ROW_NUMBER() OVER(PARTITION BY e.parameterID ";
                                aquery += "ORDER BY e.sheetInsertionDateTime DESC) ";
                                aquery += "FROM tblSheet e ";
                                aquery += "inner join tblResource r on e.resourceID = r.resourceID ";
                                aquery += "WHERE e.resourceID = " + Convert.ToInt32(drRes["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drPar["parameterID"]) + " and e.sheetInsertionDateTime >= DATEADD(day,-2,GETDATE()) ";
                                aquery += ") ";
                                aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                                string theQuery = aquery;
                                SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                                DataTable dtVal = new DataTable();
                                sdaVal.Fill(dtVal);
                                if (tempName == "Consumption")
                                {
                                    scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{name}, <strong>{(y}</strong> liter\", ";
                                }
                                else
                                {
                                    scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{name}, <strong>{y}</strong>\", ";
                                }
                                List<DataPoint> dataPoints = new List<DataPoint>();
                                DateTime dt = DateTime.Now;
                                foreach (DataRow drVal in dtVal.Rows)
                                {
                                    string val = "";
                                    if (Convert.ToDouble(drVal["parameterValue"]) == 0)
                                    {
                                        val = "OFF";
                                    }
                                    else
                                    {
                                        val = "ON";
                                    }
                                    if (dtVal.Rows.IndexOf(drVal) != 0)
                                    {
                                        if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 600)
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"])));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                        else
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"])));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"])));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    //dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["InsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["ParameterValue"])));
                                }
                                scriptString += "dataPoints: " + Newtonsoft.Json.JsonConvert.SerializeObject(dataPoints) + "";
                                scriptString += "},";
                            }
                            scriptString = scriptString.Remove(scriptString.Length - 1);
                            scriptString = scriptString + "]";
                            scriptString = scriptString + "}";
                            scriptString += ");";
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            string NewscripString = scriptString;
            ViewData["chartData"] = NewscripString;
            ////////////////////////////////////////////////////////////////////////////////////////////
            var tablList = getPumpTableList();
            return View(tablList);
        }

        [HttpPost]
        public ActionResult PumpReport(FormCollection review)
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            string resource = review["resource"].ToString();
            DateTime dateFrom = DateTime.Parse(review["dateFrom"].ToString());
            DateTime dateTo = DateTime.Parse(review["dateTo"].ToString());
            string df_date = dateFrom.ToString("d");
            string dt_date = dateTo.ToString("d");
            string TF = review["timeFrom"];
            string TT = review["timeTo"];
            string abc = review["timeFrom"];
            string[] abc1 = abc.Split(',');
            string a = abc1[0];
            if (abc1.Length > 1)
            {
                TF = abc1[1];
            }
            else
            {
                TF = abc1[0];
            }
            DataTable dt121 = new DataTable();
            Session["TimeFrom"] = TF;
            DateTime timeFrom = DateTime.Parse(TF);
            string cba = review["timeTo"];
            string[] cba1 = cba.Split(',');
            TT = cba1[0];
            DateTime timeTo = DateTime.Parse(TT);
            string tf_time = timeFrom.ToString("t");
            string tt_time = timeTo.ToString("t");
            if (tt_time == "12:00 AM")
            {
                tt_time = "11:59 PM";
            }
            DateTime FinalTimeFrom = Convert.ToDateTime(df_date + " " + tf_time);
            DateTime FinalTimeTo = Convert.ToDateTime(dt_date + " " + tt_time);
            IList<string> ResourceList = new List<string>();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Pumps Status"))
            {
                ResourceList.Add(item.resourceName);
            }
            ViewBag.ResourceList = ResourceList;
            ////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select r.resourceID, r.resourceName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where resourceName  = '" + resource + "' and rt.resourceTypeName = 'Pumps Status'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        string resName = drRes["resourceName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["ResourceID"]) + " and tp.parameterName like '%pumpStatus%'";
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Pumps Status\" },";
                        scriptString += "subtitles: [{text: \"" + resName + "\" }],";
                        scriptString += "axisY: {suffix: \" \" },";
                        //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                        scriptString += "toolTip: { shared: true },";
                        scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries},";
                        scriptString += " data: [";
                        foreach (DataRow drPar in dtPar.Rows)
                        {
                            string parName = drPar["parameterName"].ToString();
                            string aquery = ";WITH CTE AS ( ";
                            aquery += "SELECT e.parameterID, e.parameterValue, e.sheetInsertionDateTime,  ";
                            aquery += " RN = ROW_NUMBER() OVER(PARTITION BY e.parameterID ";
                            aquery += "ORDER BY e.sheetInsertionDateTime DESC) ";
                            aquery += "FROM tblSheet e ";
                            aquery += "inner join tblResource r on e.resourceID = r.resourceID ";
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drRes["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drPar["parameterID"]) + " and e.sheetInsertionDateTime >='" + FinalTimeFrom + "' and e.sheetInsertionDateTime <='" + FinalTimeTo + "' ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} </strong> at {x}\", ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"])));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"])));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"])));
                                    dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                }
                                //dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["InsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["ParameterValue"])));
                            }
                            scriptString += "dataPoints: " + Newtonsoft.Json.JsonConvert.SerializeObject(dataPoints) + "";
                            scriptString += "},";
                        }
                        scriptString = scriptString.Remove(scriptString.Length - 1);
                        scriptString = scriptString + "]";
                        scriptString = scriptString + "}";
                        scriptString += ");";
                    }
                }
                catch (Exception ex)
                {

                }
            }
            string NewscripString = scriptString;
            ViewData["chartData"] = NewscripString;
            ////////////////////////////////////////////////////////////////////////////
            ViewBag.SelectedResource = resource;
            ViewBag.SelectedTimeFrom = TF;
            ViewBag.SelectedTimeTo = TT;
            ViewBag.SelectedTimeFrom = TF.ToString();
            ViewBag.SelectedTimeTo = TT.ToString();
            ViewBag.timeFrom = TF;
            ViewBag.timeTo = TT;
            ViewBag.dateFrom = df_date;
            ViewBag.dateTo = dt_date;
            var tablList = getPumpTableList();
            return View(tablList);
        }
    }
}