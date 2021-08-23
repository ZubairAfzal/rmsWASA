using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wasaRms.Models;

namespace wasaRms.Controllers
{
    public class DisposalController : Controller
    {
        // GET: Disposal
        public List<DisposalTableClass> getDisposalList() {
            var tablList = new List<DisposalTableClass>();
            DataTable Dashdt = new DataTable();
            string Dashdtquery = ";WITH cte AS ( SELECT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, s.sheetInsertionDateTime AS tim, p.parameterUnit as pUnit, CONVERT(float,s.parameterValue - LEAD(s.parameterValue) OVER (ORDER BY s.sheetInsertionDateTime DESC))/10 AS FLOW_RATE, r.resourceNumber as rnum, s.parameterValue/(CONVERT(float,s.parameterValue - LEAD(s.parameterValue) OVER (ORDER BY s.sheetInsertionDateTime DESC))/10+0.0001) AS EstimatedTime, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Disposal Stations' ) SELECT * FROM cte WHERE rn = 1 order by cast(rnum as INT) ASC ";
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
                        var tableclass = new DisposalTableClass();
                        tableclass.srNo = Convert.ToInt32(Dashdt.Rows.IndexOf(dr) + 1).ToString();
                        tableclass.pondingLocation = dr["Location"].ToString();
                        tableclass.currLevel = Math.Round(Convert.ToDouble(dr["Current_Level"])).ToString();
                        tableclass.pUnit = dr["pUnit"].ToString();
                        tableclass.RecTime = dr["tim"].ToString();
                        if (Convert.ToDouble(dr["FLOW_RATE"]) > 0)
                        {
                            tableclass.flowRateUp = Math.Abs(Math.Round((Convert.ToDouble(dr["FLOW_RATE"])), 1)).ToString();
                            tableclass.flowRateDown = "-";
                        }
                        else
                        {
                            tableclass.flowRateDown = Math.Abs(Math.Round((Convert.ToDouble(dr["FLOW_RATE"])), 1)).ToString();
                            tableclass.flowRateUp = "-";
                        }
                        if (Convert.ToDouble(dr["EstimatedTime"]) == 0)
                        {
                            tableclass.estClTime = "Cleared";
                        }
                        else if (Convert.ToDouble(dr["EstimatedTime"]) < 0)
                        {
                            var d = decimal.Parse(dr["EstimatedTime"].ToString());
                            double val = Math.Abs(Convert.ToDouble(d)) / 60;
                            double hours = Math.Floor(val);
                            double minutes = Math.Floor(val * 60 % 60);
                            double seconds = Math.Floor(val * 360 % 360);
                            if (hours > 0)
                            {
                                tableclass.estClTime = hours + " Hours: " + minutes + " Minutes";
                            }
                            else
                            {
                                tableclass.estClTime = minutes + " Minutes";
                            }
                        }
                        else
                        {
                            tableclass.estClTime = "Undefined";
                        }
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
        public ActionResult Dashboard()
        {
            DataTable dt = new DataTable();
            string markers = "[";
            string parameterValuesString = "";
            string JSONresult = "";
            string query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT * FROM cte WHERE rn = 1";
            string S1query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT COUNT(*) FROM cte WHERE rn = 1";
            string S2query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT COUNT(*) FROM cte WHERE DeltaMinutes < 30 AND rn = 1";
            string S3query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 30 AND rn = 1";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlCommand cmd1 = new SqlCommand(S1query, conn);
                    SqlCommand cmd2 = new SqlCommand(S2query, conn);
                    SqlCommand cmd3 = new SqlCommand(S3query, conn);
                    ViewBag.TotalDisposal= Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
                    ViewBag.RunningDisposal = Convert.ToInt32(cmd2.ExecuteScalar()).ToString();
                    ViewBag.InactiveDisposal = Convert.ToInt32(cmd3.ExecuteScalar()).ToString();
                    SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                    sda.Fill(dt);
                    JSONresult = JsonConvert.SerializeObject(dt);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            parameterValuesString += " {";
                            //markers += string.Format("'Status': '{0}',", optionStatus);
                            parameterValuesString += string.Format("'category': '{0}', ", sdr["category"]);
                            parameterValuesString += string.Format("'lat': '{0}',", sdr["lat"]);
                            parameterValuesString += string.Format("'lng': '{0}',", sdr["lng"]);
                            //parameterValuesString += "center: { lat:" + Convert.ToDouble(sdr["lat"].ToString());
                            //parameterValuesString += ", lng: " + Convert.ToDouble(sdr["lng"].ToString()) + "}, ";
                            parameterValuesString += string.Format("'pName': '{0}',", sdr["pName"]);
                            parameterValuesString += string.Format("'ponding': '{0}',", sdr["ponding"]);
                            parameterValuesString += string.Format("'pUnit': '{0}',", sdr["unit"]);
                            parameterValuesString += string.Format("'title': '{0}',", sdr["category"]);
                            parameterValuesString += string.Format("'time': '{0}',", sdr["sheetInsertionDateTime"].ToString());
                            parameterValuesString += string.Format("'Delta': '{0}',", sdr["DeltaMinutes"].ToString());
                            parameterValuesString += " },";
                            //parameterValuesString += "ponding: " + Convert.ToDouble(sdr["ponding"].ToString()) + ",";
                            //parameterValuesString += "title: " + sdr["category"].ToString() + ",";
                            //parameterValuesString += "time: " + sdr["sheetInsertionDateTime"].ToString() + "},";
                            markers += parameterValuesString;
                            parameterValuesString = "";
                        }
                    }
                    markers = markers.Remove(markers.Length - 1, 1);
                    markers += "]";
                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
            var data = new { status = "Success" };
            ViewBag.Markers = markers;
            ViewBag.jsonRes = JSONresult;
            var tabList = getDisposalList();
            return View(tabList);
        }

        public ActionResult DisposalLevelReport()
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Disposal Stations"))
            {
                ResourceList.Add(item.resourceLocationName);
            }
            ViewBag.ResourceList = ResourceList;
            /////////////////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'water level'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'water level' ";
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Disposal Stations\" },";
                        scriptString += "subtitles: [{text: \" All Disposal Stations Recent Water Level \" }],";
                        scriptString += "axisY: {suffix: \" ft\" },";
                        //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                        scriptString += "toolTip: { shared: true },";
                        scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries},";
                        scriptString += " data: [";
                        foreach (DataRow drPar in dtPar.Rows)
                        {
                            //string parName = drPar["parameterName"].ToString();
                            string aquery = ";WITH CTE AS ( ";
                            aquery += "SELECT e.parameterID, e.parameterValue, e.sheetInsertionDateTime,  ";
                            aquery += " RN = ROW_NUMBER() OVER(PARTITION BY e.parameterID ";
                            aquery += "ORDER BY e.sheetInsertionDateTime DESC) ";
                            aquery += "FROM tblSheet e ";
                            aquery += "inner join tblResource r on e.resourceID = r.resourceID ";
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " and e.sheetInsertionDateTime >= DATEADD(day,-2,GETDATE()) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} ft</strong> at {x}\", ";
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
            ////////////////////////////////////////////////////////////////////////////////////////////
            /*
            DataTable dt = new DataTable();
            string markers = "[{";
            string parameterValuesString = "";
            string JSONresult = "";
            string query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT * FROM cte WHERE rn = 1";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                    sda.Fill(dt);
                    JSONresult = JsonConvert.SerializeObject(dt);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            parameterValuesString += "";
                            parameterValuesString += sdr["category"].ToString() + ":";
                            parameterValuesString += " {";
                            parameterValuesString += "center: { lat:" + Convert.ToDouble(sdr["lat"].ToString());
                            parameterValuesString += ", lng: " + Convert.ToDouble(sdr["lng"].ToString()) + "}, ";
                            parameterValuesString += "ponding: " + Convert.ToDouble(sdr["ponding"].ToString()) + ",";
                            parameterValuesString += "title: " + sdr["category"].ToString() + ",";
                            parameterValuesString += "time: " + sdr["sheetInsertionDateTime"].ToString() + "},";
                            markers += parameterValuesString;
                            parameterValuesString = "";
                        }
                    }
                    markers = markers.Remove(markers.Length - 1, 1);
                    markers += "}]";
                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
            var data = new { status = "Success" };
            ViewBag.Markers = markers;
            */
            var tabList = getDisposalList();
            return View(tabList);
        }
        [HttpPost]
        public ActionResult DisposalLevelReport(FormCollection review)
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
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Disposal Stations"))
            {
                ResourceList.Add(item.resourceLocationName);
            }
            ViewBag.ResourceList = ResourceList;
            ////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where resourceLocationName  = '" + resource + "' and rt.resourceTypeName = 'Disposal Stations'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["ResourceID"]) + " and tp.parameterName = 'water level'";
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Water Level\" },";
                        scriptString += "subtitles: [{text: \"" + resName + "\" }],";
                        scriptString += "axisY: {suffix: \" ft\" },";
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
                            scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} ft</strong> at {x}\", ";
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
            var tabList = getDisposalList();
            return View(tabList);
        }
        public JsonResult LoadDisposalChartData()
        {
            DataTable dt = new DataTable();
            string JSONresult = "";
            string query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, p.parameterName as pName, s.parameterValue AS ponding, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT * FROM cte WHERE rn = 1";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                    sda.Fill(dt);
                    //var abc = "{ Disposal1: { center: { lat: 31.476386, lng: 74.311471 }, ponding: 7 }, Disposal2: { center: { lat: 31.415303, lng: 74.301616 }, ponding: 7 }};";
                    //JSONresult = JsonConvert.SerializeObject(abc);
                    //var newAbc = JSONresult.Replace("\"","");
                    //var newnewabc = newAbc;
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

    }
}