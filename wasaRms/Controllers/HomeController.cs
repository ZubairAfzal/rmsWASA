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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //DataTable dt = new DataTable();
            //DataTable dt1 = new DataTable();
            //DataTable dt2 = new DataTable();
            //string markers = "[";
            //string markers1 = "[";
            //string markers2 = "[";
            //string parameterValuesString = "";
            //string parameterValuesString1 = "";
            //string parameterValuesString2 = "";
            //string JSONresult = "";
            //string JSONresult1 = "";
            //string JSONresult2 = "";
            //string query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT * FROM cte WHERE rn = 1";
            //string query1 = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages') SELECT * FROM cte WHERE rn = 1";
            //string query2 = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT * FROM cte WHERE rn = 1";
            //using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //{
            //    try
            //    {
            //        conn.Open();
            //        SqlCommand cmd = new SqlCommand(query, conn);
            //        SqlCommand cmd1 = new SqlCommand(query1, conn);
            //        SqlCommand cmd2 = new SqlCommand(query2, conn);
            //        SqlDataAdapter sda = new SqlDataAdapter(query, conn);
            //        SqlDataAdapter sda1 = new SqlDataAdapter(query1, conn);
            //        SqlDataAdapter sda2 = new SqlDataAdapter(query2, conn);
            //        sda.Fill(dt);
            //        sda1.Fill(dt1);
            //        sda2.Fill(dt2);
            //        JSONresult = JsonConvert.SerializeObject(dt);
            //        JSONresult1 = JsonConvert.SerializeObject(dt1);
            //        JSONresult2 = JsonConvert.SerializeObject(dt2);
            //        using (SqlDataReader sdr = cmd.ExecuteReader())
            //        {
            //            while (sdr.Read())
            //            {
            //                parameterValuesString += " {";
            //                //markers += string.Format("'Status': '{0}',", optionStatus);
            //                parameterValuesString += string.Format("'category': '{0}', ", sdr["category"]);
            //                parameterValuesString += string.Format("'lat': '{0}',", sdr["lat"]);
            //                parameterValuesString += string.Format("'lng': '{0}',", sdr["lng"]);
            //                //parameterValuesString += "center: { lat:" + Convert.ToDouble(sdr["lat"].ToString());
            //                //parameterValuesString += ", lng: " + Convert.ToDouble(sdr["lng"].ToString()) + "}, ";
            //                parameterValuesString += string.Format("'pName': '{0}',", sdr["pName"]);
            //                parameterValuesString += string.Format("'ponding': '{0}',", sdr["ponding"]);
            //                parameterValuesString += string.Format("'pUnit': '{0}',", sdr["unit"]);
            //                parameterValuesString += string.Format("'title': '{0}',", sdr["category"]);
            //                parameterValuesString += string.Format("'time': '{0}',", sdr["sheetInsertionDateTime"].ToString());
            //                parameterValuesString += " },";
            //                //parameterValuesString += "ponding: " + Convert.ToDouble(sdr["ponding"].ToString()) + ",";
            //                //parameterValuesString += "title: " + sdr["category"].ToString() + ",";
            //                //parameterValuesString += "time: " + sdr["sheetInsertionDateTime"].ToString() + "},";
            //                markers += parameterValuesString;
            //                parameterValuesString = "";
            //            }
            //        }
            //        markers = markers.Remove(markers.Length - 1, 1);
            //        markers += "]";

            //        using (SqlDataReader sdr1 = cmd1.ExecuteReader())
            //        {
            //            while (sdr1.Read())
            //            {
            //                parameterValuesString1 += " {";
            //                //markers += string.Format("'Status': '{0}',", optionStatus);
            //                parameterValuesString1 += string.Format("'category': '{0}', ", sdr1["category"]);
            //                parameterValuesString1 += string.Format("'lat': '{0}',", sdr1["lat"]);
            //                parameterValuesString1 += string.Format("'lng': '{0}',", sdr1["lng"]);
            //                //parameterValuesString += "center: { lat:" + Convert.ToDouble(sdr["lat"].ToString());
            //                //parameterValuesString += ", lng: " + Convert.ToDouble(sdr["lng"].ToString()) + "}, ";
            //                parameterValuesString1 += string.Format("'pName': '{0}',", sdr1["pName"]);
            //                parameterValuesString1 += string.Format("'ponding': '{0}',", sdr1["ponding"]);
            //                parameterValuesString1 += string.Format("'pUnit': '{0}',", sdr1["unit"]);
            //                parameterValuesString1 += string.Format("'title': '{0}',", sdr1["category"]);
            //                parameterValuesString1 += string.Format("'time': '{0}',", sdr1["sheetInsertionDateTime"].ToString());
            //                parameterValuesString1 += " },";
            //                //parameterValuesString += "ponding: " + Convert.ToDouble(sdr["ponding"].ToString()) + ",";
            //                //parameterValuesString += "title: " + sdr["category"].ToString() + ",";
            //                //parameterValuesString += "time: " + sdr["sheetInsertionDateTime"].ToString() + "},";
            //                markers1 += parameterValuesString1;
            //                parameterValuesString1 = "";
            //            }
            //        }
            //        markers1 = markers1.Remove(markers1.Length - 1, 1);
            //        markers1 += "]";

            //        using (SqlDataReader sdr2 = cmd2.ExecuteReader())
            //        {
            //            while (sdr2.Read())
            //            {
            //                parameterValuesString2 += " {";
            //                //markers += string.Format("'Status': '{0}',", optionStatus);
            //                parameterValuesString2 += string.Format("'category': '{0}', ", sdr2["category"]);
            //                parameterValuesString2 += string.Format("'lat': '{0}',", sdr2["lat"]);
            //                parameterValuesString2 += string.Format("'lng': '{0}',", sdr2["lng"]);
            //                //parameterValuesString += "center: { lat:" + Convert.ToDouble(sdr["lat"].ToString());
            //                //parameterValuesString += ", lng: " + Convert.ToDouble(sdr["lng"].ToString()) + "}, ";
            //                parameterValuesString2 += string.Format("'pName': '{0}',", sdr2["pName"]);
            //                parameterValuesString2 += string.Format("'ponding': '{0}',", sdr2["ponding"]);
            //                parameterValuesString2 += string.Format("'pUnit': '{0}',", sdr2["unit"]);
            //                parameterValuesString2 += string.Format("'title': '{0}',", sdr2["category"]);
            //                parameterValuesString2 += string.Format("'time': '{0}',", sdr2["sheetInsertionDateTime"].ToString());
            //                parameterValuesString2 += " },";
            //                //parameterValuesString += "ponding: " + Convert.ToDouble(sdr["ponding"].ToString()) + ",";
            //                //parameterValuesString += "title: " + sdr["category"].ToString() + ",";
            //                //parameterValuesString += "time: " + sdr["sheetInsertionDateTime"].ToString() + "},";
            //                markers2 += parameterValuesString2;
            //                parameterValuesString2 = "";
            //            }
            //        }
            //        markers2 = markers2.Remove(markers2.Length - 1, 1);
            //        markers2 += "]";
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    conn.Close();
            //}
            //var data = new { status = "Success" };
            //ViewBag.Markers = markers;
            //ViewBag.RainMarkers = markers1;
            //ViewBag.PondingMarkers = markers2;
            //ViewBag.jsonRes = JSONresult;
            //ViewBag.jsonResRain = JSONresult1;
            //ViewBag.jsonResPonding = JSONresult2;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Dashboard()
        {
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                //string S1query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT COUNT(*) FROM cte WHERE rn = 1";
                string S2query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S2query += "inner join tblResource r on s.resourceID = r.resourceID ";
                S2query += "inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S2query += "where r.resourceTypeID = 1 and sheetInsertionDateTime > DATEADD(MINUTE, -21, GETDATE())";
                //string S3query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 20 AND rn = 1";
                //string S4query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages') SELECT COUNT(*) FROM cte WHERE rn = 1";
                string S5query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S5query += "inner join tblResource r on s.resourceID = r.resourceID ";
                S5query += "inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S5query += "where r.resourceTypeID = 2 and sheetInsertionDateTime > DATEADD(MINUTE, -21, GETDATE())";
                //string S6query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages') SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 20 AND rn = 1";
                //string S7query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT COUNT(*) FROM cte WHERE rn = 1";
                //string S8query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT COUNT(*) FROM cte WHERE DeltaMinutes <= 20 AND rn = 1";
                //string S9query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations') SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 20 AND rn = 1";
                //string S10query = "select COUNT(t.parameterValue) from tblSheet t ";
                //S10query += "inner join( ";
                //S10query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
                //S10query += " group by resourceID ";
                //S10query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
                //S10query += " inner join tblResource r on t.resourceID = r.resourceID ";
                //S10query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                //S10query += " where rt.resourceTypeName = 'Pumps Status' ";
                //S10query += " and t.sheetInsertionDateTime >  GETDATE() - 3600 ";
                //rmsWasa01Entities db = new rmsWasa01Entities();
                //var date = DateTime.Now.AddHours(-6);
                //var queryS10 = (from t in
                //(from t in db.tblSheets
                // join tm in (
                //     (from tblSheets in db.tblSheets
                //      group tblSheets by new
                //      {
                //          tblSheets.resourceID
                //      } into g
                //      select new
                //      {
                //          g.Key.resourceID,
                //          MaxDate = (DateTime?)g.Max(p => p.sheetInsertionDateTime)
                //      }))
                //       on new { t.resourceID, t.sheetInsertionDateTime }
                //   equals new { resourceID = (int)tm.resourceID, sheetInsertionDateTime = (DateTime)tm.MaxDate }
                // where
                //   t.tblResource.tblResourceType.resourceTypeName == "Pumps Status"
                //   &&
                //   DateTime.Compare(date, t.sheetInsertionDateTime) >= 0
                // select new
                // {
                //     t.parameterValue,
                //     Dummy = "x"
                // })
                //group t by new { t.Dummy } into g
                //select new
                //{
                //    Column1 = g.Count(p => p.parameterValue != null)
                //}).FirstOrDefault();
                //string pums = queryS10.Column1.ToString();
                //string S11query = "select COUNT(t.parameterValue) from tblSheet t ";
                //S11query += "inner join( ";
                //S11query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
                //S11query += " group by resourceID ";
                //S11query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
                //S11query += " inner join tblResource r on t.resourceID = r.resourceID ";
                //S11query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                //S11query += " where rt.resourceTypeName = 'Pumps Status' ";
                //S11query += " and t.sheetInsertionDateTime > GETDATE() - 3600 ";
                //S11query += "and t.parameterValue = 1";
                //string S12query = "select COUNT(t.parameterValue) from tblSheet t ";
                //S12query += "inner join( ";
                //S12query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
                //S12query += " group by resourceID ";
                //S12query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
                //S12query += " inner join tblResource r on t.resourceID = r.resourceID ";
                //S12query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                //S12query += " where rt.resourceTypeName = 'Pumps Status' ";
                ////S12query += " and t.sheetInsertionDateTime >  GETDATE() - 3600 ";
                //S12query += "and t.parameterValue = 0";
                //string S13query = "select count(*) from tblResource where resourceTypeID = 1002";
                string S14query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S14query += "inner join tblResource r on s.resourceID = r.resourceID ";
                S14query += "inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S14query += "where r.resourceTypeID = 1002 and sheetInsertionDateTime > DATEADD(MINUTE, -21, GETDATE())";
                //string S15query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Storm Water Tank') SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 20 AND rn = 1";
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        //SqlCommand cmd1 = new SqlCommand(S1query, conn);
                        SqlCommand cmd2 = new SqlCommand(S2query, conn);
                        //SqlCommand cmd3 = new SqlCommand(S3query, conn);
                        //SqlCommand cmd4 = new SqlCommand(S4query, conn);
                        SqlCommand cmd5 = new SqlCommand(S5query, conn);
                        //SqlCommand cmd6 = new SqlCommand(S6query, conn);
                        //SqlCommand cmd7 = new SqlCommand(S7query, conn);
                        //SqlCommand cmd8 = new SqlCommand(S8query, conn);
                        //SqlCommand cmd9 = new SqlCommand(S9query, conn);
                        //SqlCommand cmd10 = new SqlCommand(S10query, conn);
                        //SqlCommand cmd11 = new SqlCommand(S11query, conn);
                        //SqlCommand cmd12 = new SqlCommand(S12query, conn);
                        //SqlCommand cmd13 = new SqlCommand(S13query, conn);
                        SqlCommand cmd14 = new SqlCommand(S14query, conn);
                        //SqlCommand cmd15 = new SqlCommand(S15query, conn);
                        ViewBag.TotalPonding = 22;
                        int rp = Convert.ToInt32(cmd2.ExecuteScalar());
                        ViewBag.RunningPonding = rp.ToString();
                        ViewBag.InactivePonding = (22 - rp).ToString();
                        ViewBag.TotalRain = 1;
                        int rr = Convert.ToInt32(cmd5.ExecuteScalar());
                        ViewBag.RunningRain = rr.ToString();
                        ViewBag.InactiveRain = (1 - rr).ToString();
                        ViewBag.TotalDisposal = 1.ToString();
                        ViewBag.RunningDisposal = 0.ToString();
                        ViewBag.InactiveDisposal = 1.ToString();
                        ViewBag.TotalPumps = 1.ToString();
                        ViewBag.RunningPumps = 0.ToString();
                        ViewBag.InactivePumps = 1.ToString();
                        ViewBag.TotalStorageTank = 1;
                        int rst = Convert.ToInt32(cmd14.ExecuteScalar());
                        ViewBag.ActiveStorageTank = rst.ToString();
                        ViewBag.InactiveStorageTank = (1-rst).ToString();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else
            {
                string S1query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S1query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S1query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S1query += " where r.resourceTypeID = 1 ";
                S1query += " and r.managedBy = "+ Convert.ToInt32(Session["UserID"]) + " ";
                string S2query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S2query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S2query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S2query += " where r.resourceTypeID = 1 ";
                S2query += " and sheetInsertionDateTime > DATEADD(MINUTE,-21,GETDATE()) ";
                S2query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                //string S3query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points' and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + ") SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 20 AND rn = 1";
                string S4query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S4query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S4query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S4query += " where r.resourceTypeID = 2 ";
                S4query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                string S5query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S5query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S5query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S5query += " where r.resourceTypeID = 2 ";
                S5query += " and sheetInsertionDateTime > DATEADD(MINUTE,-21,GETDATE()) ";
                S5query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                //string S6query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages' and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + ") SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 20 AND rn = 1";
                string S7query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S7query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S7query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S7query += " where r.resourceTypeID = 3 ";
                S7query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                string S8query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S8query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S8query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S8query += " where r.resourceTypeID = 3 ";
                S8query += " and sheetInsertionDateTime > DATEADD(MINUTE,-21,GETDATE()) ";
                S8query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                //string S9query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Disposal Stations' and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + ") SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 20 AND rn = 1";
                //string S10query = "select COUNT(t.parameterValue) from tblSheet t ";
                //S10query += "inner join( ";
                //S10query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
                //S10query += " group by resourceID ";
                //S10query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
                //S10query += " inner join tblResource r on t.resourceID = r.resourceID ";
                //S10query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                //S10query += " where rt.resourceTypeName = 'Pumps Status'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                ////S10query += " and t.sheetInsertionDateTime >  GETDATE() - 3600 ";
                ////rmsWasa01Entities db = new rmsWasa01Entities();
                ////var date = DateTime.Now.AddHours(-6);
                ////var queryS10 = (from t in
                ////(from t in db.tblSheets
                //// join tm in (
                ////     (from tblSheets in db.tblSheets
                ////      group tblSheets by new
                ////      {
                ////          tblSheets.resourceID
                ////      } into g
                ////      select new
                ////      {
                ////          g.Key.resourceID,
                ////          MaxDate = (DateTime?)g.Max(p => p.sheetInsertionDateTime)
                ////      }))
                ////       on new { t.resourceID, t.sheetInsertionDateTime }
                ////   equals new { resourceID = (int)tm.resourceID, sheetInsertionDateTime = (DateTime)tm.MaxDate }
                //// where
                ////   t.tblResource.tblResourceType.resourceTypeName == "Pumps Status"
                ////   &&
                ////   DateTime.Compare(date, t.sheetInsertionDateTime) >= 0
                //// select new
                //// {
                ////     t.parameterValue,
                ////     Dummy = "x"
                //// })
                ////group t by new { t.Dummy } into g
                ////select new
                ////{
                ////    Column1 = g.Count(p => p.parameterValue != null)
                ////}).FirstOrDefault();
                ////string pums = queryS10.Column1.ToString();
                //string S11query = "select COUNT(t.parameterValue) from tblSheet t ";
                //S11query += "inner join( ";
                //S11query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
                //S11query += " group by resourceID ";
                //S11query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
                //S11query += " inner join tblResource r on t.resourceID = r.resourceID ";
                //S11query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                //S11query += " where rt.resourceTypeName = 'Pumps Status'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                ////S11query += " and t.sheetInsertionDateTime > GETDATE() - 3600 ";
                //S11query += "and t.parameterValue = 1";

                string S10query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S10query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S10query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S10query += " where r.resourceTypeID = 4 ";
                S10query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                string S11query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S11query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S11query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S11query += " where r.resourceTypeID = 4 ";
                S11query += " and sheetInsertionDateTime > DATEADD(MINUTE,-21,GETDATE()) ";
                S11query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";

                //string S12query = "select COUNT(t.parameterValue) from tblSheet t ";
                //S12query += "inner join( ";
                //S12query += " select resourceID, max(sheetInsertionDateTime) as MaxDate from tblSheet ";
                //S12query += " group by resourceID ";
                //S12query += " ) tm on t.resourceID = tm.resourceID and t.sheetInsertionDateTime = tm.MaxDate ";
                //S12query += " inner join tblResource r on t.resourceID = r.resourceID ";
                //S12query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                //S12query += " where rt.resourceTypeName = 'Pumps Status'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                ////S12query += " and t.sheetInsertionDateTime >  GETDATE() - 3600 ";
                //S12query += "and t.parameterValue = 0";
                //string S13query = "select count(*) from tblResource where resourceTypeID = 1002";
                //string S14query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Storm Water Tank') SELECT COUNT(*) FROM cte WHERE DeltaMinutes <= 20 AND rn = 1";

                string S13query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S13query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S13query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S13query += " where r.resourceTypeID = 1002 ";
                S13query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                string S14query = "select count( DISTINCT r.resourceID )from tblSheet s ";
                S14query += " inner join tblResource r on s.resourceID = r.resourceID ";
                S14query += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
                S14query += " where r.resourceTypeID = 1002 ";
                S14query += " and sheetInsertionDateTime > DATEADD(MINUTE,-21,GETDATE()) ";
                S14query += " and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";

                //string S15query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Storm Water Tank') SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 20 AND rn = 1";


                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd1 = new SqlCommand(S1query, conn);
                        SqlCommand cmd2 = new SqlCommand(S2query, conn);
                        //SqlCommand cmd3 = new SqlCommand(S3query, conn);
                        SqlCommand cmd4 = new SqlCommand(S4query, conn);
                        SqlCommand cmd5 = new SqlCommand(S5query, conn);
                        //SqlCommand cmd6 = new SqlCommand(S6query, conn);
                        SqlCommand cmd7 = new SqlCommand(S7query, conn);
                        SqlCommand cmd8 = new SqlCommand(S8query, conn);
                        //SqlCommand cmd9 = new SqlCommand(S9query, conn);
                        SqlCommand cmd10 = new SqlCommand(S10query, conn);
                        SqlCommand cmd11 = new SqlCommand(S11query, conn);
                        //SqlCommand cmd12 = new SqlCommand(S12query, conn);
                        SqlCommand cmd13 = new SqlCommand(S13query, conn);
                        SqlCommand cmd14 = new SqlCommand(S14query, conn);
                        //SqlCommand cmd15 = new SqlCommand(S15query, conn);
                        ViewBag.TotalPonding = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
                        ViewBag.RunningPonding = Convert.ToInt32(cmd2.ExecuteScalar()).ToString();
                        ViewBag.InactivePonding = (Convert.ToInt32(cmd1.ExecuteScalar()) - Convert.ToInt32(cmd2.ExecuteScalar())).ToString();
                        ViewBag.TotalRain = Convert.ToInt32(cmd4.ExecuteScalar()).ToString();
                        ViewBag.RunningRain = Convert.ToInt32(cmd5.ExecuteScalar()).ToString();
                        ViewBag.InactiveRain = (Convert.ToInt32(cmd4.ExecuteScalar()) - Convert.ToInt32(cmd5.ExecuteScalar())).ToString();
                        ViewBag.TotalDisposal = Convert.ToInt32(cmd7.ExecuteScalar()).ToString();
                        ViewBag.RunningDisposal = Convert.ToInt32(cmd8.ExecuteScalar()).ToString();
                        ViewBag.InactiveDisposal = (Convert.ToInt32(cmd7.ExecuteScalar()) - Convert.ToInt32(cmd8.ExecuteScalar())).ToString();
                        ViewBag.TotalPumps = Convert.ToInt32(cmd10.ExecuteScalar()).ToString();
                        //ViewBag.TotalPumps = pums;
                        ViewBag.RunningPumps = Convert.ToInt32(cmd11.ExecuteScalar()).ToString();
                        ViewBag.InactivePumps = (Convert.ToInt32(cmd10.ExecuteScalar()) - Convert.ToInt32(cmd11.ExecuteScalar())).ToString();
                        ViewBag.TotalStorageTank = Convert.ToInt32(cmd13.ExecuteScalar()).ToString();
                        ViewBag.ActiveStorageTank = Convert.ToInt32(cmd14.ExecuteScalar()).ToString();
                        ViewBag.InactiveStorageTank = (Convert.ToInt32(cmd13.ExecuteScalar()) - Convert.ToInt32(cmd14.ExecuteScalar())).ToString();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return View();
        }
    }
}