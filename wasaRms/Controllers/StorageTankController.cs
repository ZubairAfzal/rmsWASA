using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wasaRms.Models;

namespace wasaRms.Controllers
{
    public class StorageTankController : Controller
    {
        // GET: StorageTank
        public ActionResult Dashboard()
        {
            int pumpsRunning = 0;
            double tankLevel = 0;
            string tempName = "";
            string parameterValuesString = "";
            string datetimed = "";
            string markers = "[";
            //string queryP1Status = "select top (1) parameterValue from tblSheet where parameterID = 1026 order by sheetInsertionDateTime DESC";
            //string queryP2Status = "select top (1) parameterValue from tblSheet where parameterID = 1027 order by sheetInsertionDateTime DESC";
            //string queryTankLevel = "select top (1) parameterValue from tblSheet where parameterID = 1034 order by sheetInsertionDateTime DESC";
            string S13query = "select count( DISTINCT r.resourceID )from tblSheet s ";
            S13query += "inner join tblResource r on s.resourceID = r.resourceID ";
            S13query += "inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID ";
            S13query += "where r.resourceTypeID = 1002 and sheetInsertionDateTime > DATEADD(MINUTE, -21, GETDATE()) ";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd1 = new SqlCommand(S13query, conn);
                    ViewBag.TotalStorageTank = 1;
                    int workingStorageTanks = Convert.ToInt32(cmd1.ExecuteScalar());
                    ViewBag.ActiveStorageTank = workingStorageTanks;
                    ViewBag.InactiveStorageTank = 1 - workingStorageTanks;
                    string q1 = "";
                    q1 += "select top(28)  r.resourceLocationName,  t.resourceTypeName, p.ParameterName, e.parameterValue, e.sheetInsertionDateTime  from tblSheet e ";
                    q1 += "left join tblParameter p on e.parameterID = p.parameterID ";
                    q1 += "left join tblResource r on e.resourceID = r.resourceID ";
                    q1 += "left join tblResourceType t on r.resourceTypeID = t.resourceTypeID ";
                    q1 += "where e.sheetInsertionDateTime = (select max(sheetInsertionDateTime) from tblSheet where ResourceID = 1085) ";
                    q1 += "and r.resourceID = 1085 order by p.parameterMaxThr";
                    SqlCommand cmd = new SqlCommand(q1, conn);
                    using (SqlDataReader sdr1 = cmd.ExecuteReader())
                    {
                        while (sdr1.Read())
                        {
                            string valuee = "";
                            parameterValuesString += "";
                            if (sdr1["parameterName"].ToString() == "P1 Status")
                            {
                                parameterValuesString += "Pump No. 1 " + ": ";
                                if (sdr1["parameterValue"].ToString() == "0")
                                {
                                    valuee = "OFF";
                                }
                                else
                                {
                                    valuee = "ON";
                                }
                            }
                            else if (sdr1["parameterName"].ToString() == "P2 Status")
                            {
                                parameterValuesString += "Pump No. 2 " + ": ";
                                if (sdr1["parameterValue"].ToString() == "0")
                                {
                                    valuee = "OFF";
                                }
                                else
                                {
                                    valuee = "ON";
                                }
                            }
                            else if (sdr1["parameterName"].ToString() == "P3 Status")
                            {
                                parameterValuesString += "Pump No. 3 " + ": ";
                                if (sdr1["parameterValue"].ToString() == "0")
                                {
                                    valuee = "OFF";
                                }
                                else
                                {
                                    valuee = "ON";
                                }
                            }
                            else if (sdr1["parameterName"].ToString() == "P4 Status")
                            {
                                parameterValuesString += "Pump No. 4 " + ": ";
                                if (sdr1["parameterValue"].ToString() == "0")
                                {
                                    valuee = "OFF";
                                }
                                else
                                {
                                    valuee = "ON";
                                }
                            }
                            else if (sdr1["parameterName"].ToString() == "P1 Auto/Mannual")
                            {
                                parameterValuesString += "Pump No. 1 Mode" + ": ";
                                if (sdr1["parameterValue"].ToString() == "0")
                                {
                                    valuee = "Manual";
                                }
                                else
                                {
                                    valuee = "Auto";
                                }
                            }
                            else if (sdr1["parameterName"].ToString() == "P2 Auto/Mannual")
                            {
                                parameterValuesString += "Pump No. 2 Mode" + ": ";
                                if (sdr1["parameterValue"].ToString() == "0")
                                {
                                    valuee = "Manual";
                                }
                                else
                                {
                                    valuee = "Auto";
                                }
                            }
                            else if (sdr1["parameterName"].ToString() == "P3 Auto/Mannual")
                            {
                                parameterValuesString += "Pump No. 3 Mode" + ": ";
                                if (sdr1["parameterValue"].ToString() == "0")
                                {
                                    valuee = "Manual";
                                }
                                else
                                {
                                    valuee = "Auto";
                                }
                            }
                            else if (sdr1["parameterName"].ToString() == "P4 Auto/Mannual")
                            {
                                parameterValuesString += "Pump No. 4 Mode" + ": ";
                                if (sdr1["parameterValue"].ToString() == "0")
                                {
                                    valuee = "Manual";
                                }
                                else
                                {
                                    valuee = "Auto";
                                }
                            }
                            else if (sdr1["ParameterName"].ToString() == "V12 (v)")
                            {
                                parameterValuesString += "Line Voltage (V12) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "V13 (v)")
                            {
                                parameterValuesString += "Line Voltage (V13) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "V23 (v)")
                            {
                                parameterValuesString += "Line Voltage (V23) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "V1N (V)")
                            {
                                parameterValuesString += "Phase Voltage (V1N) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "V2N (V)")
                            {
                                parameterValuesString += "Phase Voltage (V2N) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "V3N (V)")
                            {
                                parameterValuesString += "Phase Voltage (V3N) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "I1 (A)")
                            {
                                parameterValuesString += "Current (I1) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "I2 (A)")
                            {
                                parameterValuesString += "Current (I2) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "I3 (A)")
                            {
                                parameterValuesString += "Current (I3) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "PF")
                            {
                                parameterValuesString += "Power Factor (PF) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "Freq (Hz)")
                            {
                                parameterValuesString += "Frequency (Hz) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "VA (kva)")
                            {
                                parameterValuesString += "Power (KVA) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "W (kwatt)")
                            {
                                parameterValuesString += "Power (KW) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "VAR (kvar)")
                            {
                                parameterValuesString += "Power (KVAR)  " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "VA-SUM (kva)")
                            {
                                parameterValuesString += "Power (VA-SM) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "V1 THD (%)")
                            {
                                parameterValuesString += "V1 Total Harmonics Distortion (%) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "V2 THD (%)")
                            {
                                parameterValuesString += "V2 Total Harmonics Distortion (%) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "V3 THD (%)")
                            {
                                parameterValuesString += "V3 Total Harmonics Distortion (%) " + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            else if (sdr1["ParameterName"].ToString() == "Tank Level1 (ft)")
                            {
                                parameterValuesString += "Tank Level (ft)" + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 1).ToString();
                            }
                            else
                            {
                                parameterValuesString += sdr1["ParameterName"].ToString() + ": ";
                                valuee = Math.Round(Convert.ToDouble(sdr1["parameterValue"]), 2).ToString();
                            }
                            parameterValuesString += valuee;
                            parameterValuesString += "<br />";
                            datetimed = sdr1["sheetInsertionDateTime"].ToString();
                        }
                    }
                    tempName = "S";
                    string newstring = "<b>Storm Water Storage Tank</b>";
                    newstring += "<br />";
                    newstring += "<b>Lawrence Road</b>";
                    newstring += "<br />";
                    newstring += datetimed;
                    newstring += "<br />";
                    newstring += parameterValuesString;
                    TimeSpan duration = (Convert.ToDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Pakistan Standard Time").ToString()) - Convert.ToDateTime(datetimed.ToString()));
                    double minu = Math.Abs(duration.TotalMinutes);
                    parameterValuesString = "";
                    markers += "{";
                    markers += string.Format("'Template': '{0}',", tempName);
                    markers += string.Format("'title': '{0}',", "Lawrence Road");
                    markers += string.Format("'lat':'{0}',", "31.55407167");
                    markers += string.Format("'lnt':'{0}',", "74.32604167");
                    markers += string.Format("'Delta': '{0}',", minu.ToString());
                    markers += string.Format("'description': '{0}'", newstring);
                    markers += "},";
                    conn.Close();
                }
                catch (Exception ex)
                {

                }
            }
            markers = markers.Remove(markers.Length - 1, 1);
            markers += "]";
            var data = new { status = "Success" };
            ViewBag.PumpsRunning = pumpsRunning.ToString();
            ViewBag.TankLevel = tankLevel.ToString();
            ViewBag.MapMarkers = markers;
            return View();
        }

        public ActionResult TankThreshold()
        {
            string query1from = "select maxThr from tblResource where resourceID = 1085";
            //string query1to = "select MaxRange from tblResourceRangeSelector where ParameterID = 118 and ResourceID = 1068";
            string threshold = "";
            //string time1to = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd1from = new SqlCommand(query1from, conn);
                    //SqlCommand cmd1to = new SqlCommand(query1to, conn);
                    threshold = cmd1from.ExecuteScalar().ToString();
                    //time1to = cmd1to.ExecuteScalar().ToString();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    threshold = "0";
                    //time1to = "0";
                }
            }
            ViewBag.threshold = threshold;
            //ViewBag.time1to = time1to;
            IList<string> ResourceList = new List<string>();
            rmsWasa01Entities db = new rmsWasa01Entities();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.resourceTypeID == 1002))
            {
                ResourceList.Add(item.resourceLocationName);
            }
            ViewBag.ResourceList = ResourceList;
            //ResourceList.Add("Update for All Locations");
            return View();
        }

        [HttpPost]
        public ActionResult TankThreshold(FormCollection review)
        {
            string thresholdValue = review["threshold"].ToString();
            string updateThreshold = "";
            updateThreshold = "update tblResource set maxThr = "+thresholdValue+ " where resourceID = 1085";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd1from = new SqlCommand(updateThreshold, conn);
                    cmd1from.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {

                }
            }
            string query1from = "select maxThr from tblResource where resourceID = 1085";
            //string query1to = "select MaxRange from tblResourceRangeSelector where ParameterID = 118 and ResourceID = 1068";
            string threshold = "";
            //string time1to = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd1from = new SqlCommand(query1from, conn);
                    //SqlCommand cmd1to = new SqlCommand(query1to, conn);
                    threshold = cmd1from.ExecuteScalar().ToString();
                    //time1to = cmd1to.ExecuteScalar().ToString();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    threshold = "0";
                    //time1to = "0";
                }
            }
            ViewBag.threshold = threshold;
            //ViewBag.time1to = time1to;
            IList<string> ResourceList = new List<string>();
            rmsWasa01Entities db = new rmsWasa01Entities();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.resourceTypeID == 1002))
            {
                ResourceList.Add(item.resourceLocationName);
            }
            ViewBag.ResourceList = ResourceList;
            return View();
        }

        public ActionResult Switch()
        {
            int isP1Manual;
            int isP2Manual;
            int isP3Manual;
            int isP4Manual;
            int masterAutoEnable = 0;
            int GetAutoRemoteMode;
            int P1ButtonStatus, P2ButtonStatus, P3ButtonStatus, P4ButtonStatus;
            int P1Status, P2Status, P3Status, P4Status;
            string P1ModeQ = "select top(1) parameterValue from tblSheet where parameterID = 1030 and resourceID = 1085 order by sheetID DESC";
            string P2ModeQ = "select top(1) parameterValue from tblSheet where parameterID = 1031 and resourceID = 1085 order by sheetID DESC";
            string P3ModeQ = "select top(1) parameterValue from tblSheet where parameterID = 1032 and resourceID = 1085 order by sheetID DESC";
            string P4ModeQ = "select top(1) parameterValue from tblSheet where parameterID = 1033 and resourceID = 1085 order by sheetID DESC";
            string P1StatusQ = "select top(1) parameterValue from tblSheet where parameterID = 1026 and resourceID = 1085 order by sheetID DESC";
            string P2StatusQ = "select top(1) parameterValue from tblSheet where parameterID = 1027 and resourceID = 1085 order by sheetID DESC";
            string P3StatusQ = "select top(1) parameterValue from tblSheet where parameterID = 1028 and resourceID = 1085 order by sheetID DESC";
            string P4StatusQ = "select top(1) parameterValue from tblSheet where parameterID = 1029 and resourceID = 1085 order by sheetID DESC";
            string GetAutoRemoteModeQ = "select modeStatus from tblResource where resourceID = 1085";
            string P1BtnQ = "select paramButtonStatus from tblParameter where parameterID = 1026";
            string P2BtnQ = "select paramButtonStatus from tblParameter where parameterID = 1027";
            string P3BtnQ = "select paramButtonStatus from tblParameter where parameterID = 1028";
            string P4BtnQ = "select paramButtonStatus from tblParameter where parameterID = 1029";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand P1Modecmd = new SqlCommand(P1ModeQ, conn);
                    SqlCommand P2Modecmd = new SqlCommand(P2ModeQ, conn);
                    SqlCommand P3Modecmd = new SqlCommand(P3ModeQ, conn);
                    SqlCommand P4Modecmd = new SqlCommand(P4ModeQ, conn);
                    SqlCommand GetAutoRemoteModecmd = new SqlCommand(GetAutoRemoteModeQ, conn);
                    SqlCommand P1Statuscmd = new SqlCommand(P1StatusQ, conn);
                    SqlCommand P2Statuscmd = new SqlCommand(P2StatusQ, conn);
                    SqlCommand P3Statuscmd = new SqlCommand(P3StatusQ, conn);
                    SqlCommand P4Statuscmd = new SqlCommand(P4StatusQ, conn);
                    SqlCommand P1Btncmd = new SqlCommand(P1BtnQ, conn);
                    SqlCommand P2Btncmd = new SqlCommand(P2BtnQ, conn);
                    SqlCommand P3Btncmd = new SqlCommand(P3BtnQ, conn);
                    SqlCommand P4Btncmd = new SqlCommand(P4BtnQ, conn);
                    isP1Manual = Convert.ToInt32(P1Modecmd.ExecuteScalar());
                    isP2Manual = Convert.ToInt32(P2Modecmd.ExecuteScalar());
                    isP3Manual = Convert.ToInt32(P3Modecmd.ExecuteScalar());
                    isP4Manual = Convert.ToInt32(P4Modecmd.ExecuteScalar());
                    GetAutoRemoteMode = Convert.ToInt32(GetAutoRemoteModecmd.ExecuteScalar());
                    P1ButtonStatus = Convert.ToInt32(P1Btncmd.ExecuteScalar());
                    P2ButtonStatus = Convert.ToInt32(P2Btncmd.ExecuteScalar());
                    P3ButtonStatus = Convert.ToInt32(P3Btncmd.ExecuteScalar());
                    P4ButtonStatus = Convert.ToInt32(P4Btncmd.ExecuteScalar());
                    P1Status = Convert.ToInt32(P1Statuscmd.ExecuteScalar());
                    P2Status = Convert.ToInt32(P2Statuscmd.ExecuteScalar());
                    P3Status = Convert.ToInt32(P3Statuscmd.ExecuteScalar());
                    P4Status = Convert.ToInt32(P4Statuscmd.ExecuteScalar());
                    if (isP1Manual + isP2Manual + isP3Manual + isP4Manual > 0)
                    {
                        masterAutoEnable = 1;
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    isP1Manual = 0;
                    isP2Manual = 0;
                    isP3Manual = 0;
                    isP4Manual = 0;
                    GetAutoRemoteMode = 0;
                    P1ButtonStatus = 0;
                    P2ButtonStatus = 0;
                    P3ButtonStatus = 0;
                    P4ButtonStatus = 0;
                    P1Status = 0;
                    P2Status = 0;
                    P3Status = 0;
                    P4Status = 0;
                }
            }
            ViewBag.masterAutoEnable = masterAutoEnable;
            ViewBag.GetAutoRemoteMode = GetAutoRemoteMode;
            ViewBag.isP1Manual = isP1Manual;
            ViewBag.isP2Manual = isP2Manual;
            ViewBag.isP3Manual = isP3Manual;
            ViewBag.isP4Manual = isP4Manual;
            ViewBag.P1ButtonStatus = P1ButtonStatus;
            ViewBag.P2ButtonStatus = P2ButtonStatus;
            ViewBag.P3ButtonStatus = P3ButtonStatus;
            ViewBag.P4ButtonStatus = P4ButtonStatus;
            ViewBag.P1Status = P1Status;
            ViewBag.P2Status = P2Status;
            ViewBag.P3Status = P3Status;
            ViewBag.P4Status = P4Status;
            IList<string> ResourceList = new List<string>();
            rmsWasa01Entities db = new rmsWasa01Entities();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.resourceTypeID == 1002))
            {
                ResourceList.Add(item.resourceLocationName);
            }
            ViewBag.ResourceList = ResourceList;
            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (id == 0)
            {
                string GetAutoRemoteModeQ = "select modeStatus from tblResource where resourceID = 1085";
                int mode;
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand GetAutoRemoteModeCmd = new SqlCommand(GetAutoRemoteModeQ, conn);
                        mode = Convert.ToInt32(GetAutoRemoteModeCmd.ExecuteScalar());
                        int newMode;
                        if (mode == 0)
                        {
                            newMode = 1;
                        }
                        else
                        {
                            newMode = 0;
                        }
                        string UpdateGetAutoRemoteModeQ = "update tblResource set modeStatus = "+newMode+" where resourceID = 1085";
                        SqlCommand cmd = new SqlCommand(UpdateGetAutoRemoteModeQ, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        mode = 0;
                    }
                }
            }

            else if (id == 1)
            {
                string GetP1BtnStatusQ = "select paramButtonStatus from tblParameter where parameterID = 1026";
                int P1BtnStatus;
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand GetP1BtnStatusCmd = new SqlCommand(GetP1BtnStatusQ, conn);
                        P1BtnStatus = Convert.ToInt32(GetP1BtnStatusCmd.ExecuteScalar());
                        int newP1BtnStatus;
                        if (P1BtnStatus == 0)
                        {
                            newP1BtnStatus = 1;
                        }
                        else
                        {
                            newP1BtnStatus = 0;
                        }
                        string UpdateButtonStatusQ = "update tblParameter set paramButtonStatus = " + newP1BtnStatus + " where parameterID = 1026";
                        SqlCommand cmd = new SqlCommand(UpdateButtonStatusQ, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        P1BtnStatus = 0;
                    }
                }
            }

            else if (id == 2)
            {
                string GetP2BtnStatusQ = "select paramButtonStatus from tblParameter where parameterID = 1027";
                int P2BtnStatus;
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand GetP2BtnStatusCmd = new SqlCommand(GetP2BtnStatusQ, conn);
                        P2BtnStatus = Convert.ToInt32(GetP2BtnStatusCmd.ExecuteScalar());
                        int newP2BtnStatus;
                        if (P2BtnStatus == 0)
                        {
                            newP2BtnStatus = 1;
                        }
                        else
                        {
                            newP2BtnStatus = 0;
                        }
                        string UpdateButtonStatusQ = "update tblParameter set paramButtonStatus = " + newP2BtnStatus + " where parameterID = 1027";
                        SqlCommand cmd = new SqlCommand(UpdateButtonStatusQ, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        P2BtnStatus = 0;
                    }
                }
            }

            if (id == 3)
            {
                string GetP3BtnStatusQ = "select paramButtonStatus from tblParameter where parameterID = 1028";
                int P3BtnStatus;
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand GetP3BtnStatusCmd = new SqlCommand(GetP3BtnStatusQ, conn);
                        P3BtnStatus = Convert.ToInt32(GetP3BtnStatusCmd.ExecuteScalar());
                        int newP3BtnStatus;
                        if (P3BtnStatus == 0)
                        {
                            newP3BtnStatus = 1;
                        }
                        else
                        {
                            newP3BtnStatus = 0;
                        }
                        string UpdateButtonStatusQ = "update tblParameter set paramButtonStatus = " + newP3BtnStatus + " where parameterID = 1028";
                        SqlCommand cmd = new SqlCommand(UpdateButtonStatusQ, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        P3BtnStatus = 0;
                    }
                }
            }


            if (id == 4)
            {
                string GetP4BtnStatusQ = "select paramButtonStatus from tblParameter where parameterID = 1029";
                int P4BtnStatus;
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand GetP4BtnStatusCmd = new SqlCommand(GetP4BtnStatusQ, conn);
                        P4BtnStatus = Convert.ToInt32(GetP4BtnStatusCmd.ExecuteScalar());
                        int newP4BtnStatus;
                        if (P4BtnStatus == 0)
                        {
                            newP4BtnStatus = 1;
                        }
                        else
                        {
                            newP4BtnStatus = 0;
                        }
                        string UpdateButtonStatusQ = "update tblParameter set paramButtonStatus = " + newP4BtnStatus + " where parameterID = 1029";
                        SqlCommand cmd = new SqlCommand(UpdateButtonStatusQ, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        P4BtnStatus = 0;
                    }
                }
            }
            return RedirectToAction("Switch");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CurrentRangeIndexChanged(string value)
        {
            if (value == "Update for All Locations")
            { }
            int c_id = Convert.ToInt32(Session["CompanyID"]);
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> resourceList = new List<string>();
            ////var slots = new ScheduleSlot();
            string query1from = "select MinRange from tblResourceRangeSelector where ParameterID = 118 and ResourceID = (select ResourceID from tblResource where ResourceLocation = '" + value + "')";
            string query1to = "select MaxRange from tblResourceRangeSelector where ParameterID = 118 and ResourceID = (select ResourceID from tblResource where ResourceLocation = '" + value + "')";
            string time1from = "";
            string time1to = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd1from = new SqlCommand(query1from, conn);
                    SqlCommand cmd1to = new SqlCommand(query1to, conn);
                    time1from = cmd1from.ExecuteScalar().ToString();
                    time1to = cmd1to.ExecuteScalar().ToString();
                    conn.Close();

                }
                catch (Exception ex)
                {
                    time1from = "0";
                    time1to = "0";
                }
            }
            //slots.time1From = time1from;
            //slots.time1To = time1to;
            //return Json(new { data = slots }, JsonRequestBehavior.AllowGet);
            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        }





        public JsonResult LoadStorageChartData()
        {
            DataTable dt = new DataTable();
            string JSONresult = "";
            string query = "";
            //query += ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.maxThr as maxThrs, r.resourceName as category, r.resourceNumber as resnum, 'Lahore' as townName, r.resourceName as townName2, p.parameterName as pName, abs(s.parameterValue) AS ponding, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, DATEDIFF(minute, s.sheetInsertionDateTime, CONVERT(CHAR(24), CONVERT(DATETIME, GETDATE(), 103), 121)) as DeltaMinutes, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Storm Water Tank' and p.parameterName = 'Tank Level1 (ft)') SELECT * FROM cte WHERE rn = 27 order by cast(resnum as int) asc";
            query += "select top(1) 'StorageTank' as Type_, '0' as maxThrs, 'Lawrence Road Storage Tank'	as category, '1' as resunm, 'Lahore' as townName, 'Lawrence Road Storage Tank' as townName2, 'Tank Level' as pName, abs(s.parameterValue) as ponding, 'ft' as unit from tblSheet s inner join tblParameter p on s.parameterID = p.parameterID where p.parameterName = 'Tank Level1 (ft)' order by s.sheetInsertionDateTime DESC";
            //query += "SELECT top(28) e.sheetID, r.resourceName as Location, ";
            //query += "SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat, SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ";
            //query += "p.parameterName as pName, e.parameterValue as pVal, e.sheetInsertionDateTime as tim , ";
            //query += "DATEDIFF(minute, e.sheetInsertionDateTime, CONVERT(CHAR(24), CONVERT(DATETIME, GETDATE(), 103), 121)) as DeltaMinutes ";
            //query += "FROM tblSheet e ";
            //query += "inner join tblParameter p on e.parameterID = p.parameterID ";
            //query += "inner join tblResource r on e.resourceID = r.resourceID ";
            //query += "WHERE ";
            //query += "e.resourceID = 1085 AND ";
            //query += "sheetInsertionDateTime = (Select max(sheetInsertionDateTime) from tblSheet where resourceID = 1085) ";
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

        public ActionResult PumpStatusReport()
        {
            DateTime fromDt = DateTime.Now.AddHours(0).Date;
            DateTime toDt = DateTime.Now.AddHours(24).Date;
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                int uID = Convert.ToInt32(Session["UserID"]);
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            /////////////////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select  parameterID, parameterName from tblParameter where parameterID IN (1026,1027,1028,1029)";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "";
                        if (Convert.ToInt32(Session["CompanyID"]) == 1)
                        {
                            getParamsFromRes = "select p.parameterID, p.parameterName, r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterID IN (1026,1027,1028,1029) order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select p.parameterID, p.parameterName, r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterID IN (1026,1027,1028,1029) and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " order by cast(r.resourceNumber as int) asc";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Pumps Working\" },";
                        //Report of All Ponding Locations between " + fd + " to " + td + "
                        scriptString += "subtitles: [{text: \" Pump Status between " + fromDt + " to " + toDt + " \" }],";
                        //scriptString += "axisY: {suffix: \" \" },";
                        scriptString += "axisY: {labelFontSize: 10, labelFormatter: function(){ return \" \"; }},";
                        //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                        //scriptString += "toolTip: { shared: false },";
                        scriptString += "toolTip: { shared: false , contentFormatter: function(e){ var str = \" \" ; for (var i = 0; i < e.entries.length; i++){ var utcSeconds = e.entries[i].dataPoint.x; var d = new Date(utcSeconds); if(e.entries[i].dataPoint.y == 0){ var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: OFF</b> at  \" + d.toLocaleString('en-IN'); str = str+temp; } else { var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: ON</b> at \" + d.toLocaleString('en-IN'); str = str+temp; } } return (str); }},";
                        //scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries},";
                        scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries, fontSize: 12},";
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
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterValue >= 0 and e.parameterID = " + Convert.ToInt32(drPar["parameterID"]) + " ";
                            //aquery += " and e.sheetInsertionDateTime >= DATEADD(day,-2,GETDATE()) ";
                            aquery += " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + fromDt + "', 103), 121) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            //scriptString += "{ type: \"line\", name: \"" + drPar["parameterName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"DD-MM-YYYY hh:mm:ss TT\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} </strong> at {x}\", ";
                            scriptString += "{ type: \"area\", name: \"" + drPar["parameterName"].ToString() + "\", showInLegend: true,  markerSize: 1, xValueType: \"dateTime\", xValueFormatString: \"hh:mm TT DD-MM-YYYY\",  ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 1));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 1));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 1));
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
            var dtabc = getStorageTankTableList(fromDt, toDt, "");
            string NewscripString = scriptString;
            ViewData["chartData"] = NewscripString;
            ////////////////////////////////////////////////////////////////////////////////////////////

            //var tablList = getStorageTankTableList(fromDt, toDt, "All");
            //return View(tablList);
            return View(dtabc);
        }
        //public List<StorageTankTableData> getStorageTankTableList(DateTime fromDT, DateTime toDt, string res)
        [HttpPost]
        public ActionResult PumpStatusReport(FormCollection review)
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
            if (tt_time.ToUpper() == "12:00 AM")
            {
                tt_time = "11:59 PM";
            }
            DateTime FinalTimeFrom = Convert.ToDateTime(df_date + " " + tf_time);
            DateTime FinalTimeTo = Convert.ToDateTime(dt_date + " " + tt_time);
            DateTime fromDt = FinalTimeFrom;
            DateTime toDt = FinalTimeTo;
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                int uID = Convert.ToInt32(Session["UserID"]);
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            #region

            /////////////////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select  parameterID, parameterName from tblParameter where parameterID IN (1026,1027,1028,1029)";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "";
                        if (Convert.ToInt32(Session["CompanyID"]) == 1)
                        {
                            getParamsFromRes = "select p.parameterID, p.parameterName, r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterID IN (1026,1027,1028,1029)  order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select p.parameterID, p.parameterName, r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterID IN (1026,1027,1028,1029)  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " order by cast(r.resourceNumber as int) asc";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Pumps Working\" },";
                        //Report of All Ponding Locations between " + fd + " to " + td + "
                        scriptString += "subtitles: [{text: \" Pump Status between " + fromDt + " to " + toDt + " \" }],";
                        //scriptString += "axisY: {suffix: \" \" },";
                        scriptString += "axisY: {labelFontSize: 10, labelFormatter: function(){ return \" \"; }},";
                        //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                        //scriptString += "toolTip: { shared: false },";
                        scriptString += "toolTip: { shared: false , contentFormatter: function(e){ var str = \" \" ; for (var i = 0; i < e.entries.length; i++){ var utcSeconds = e.entries[i].dataPoint.x; var d = new Date(utcSeconds); if(e.entries[i].dataPoint.y == 0){ var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: OFF</b> at  \" + d.toLocaleString('en-IN'); str = str+temp; } else { var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: ON</b> at \" + d.toLocaleString('en-IN'); str = str+temp; } } return (str); }},";
                        //scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries},";
                        scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries, fontSize: 12},";
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
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterValue >= 0 and e.parameterID = " + Convert.ToInt32(drPar["parameterID"]) + " ";
                            //aquery += " and e.sheetInsertionDateTime >= DATEADD(day,-2,GETDATE()) ";
                            aquery += " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + fromDt + "', 103), 121) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            //scriptString += "{ type: \"line\", name: \"" + drPar["parameterName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"DD-MM-YYYY hh:mm:ss TT\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} </strong> at {x}\", ";
                            scriptString += "{ type: \"area\", name: \"" + drPar["parameterName"].ToString() + "\", showInLegend: true,  markerSize: 1, xValueType: \"dateTime\", xValueFormatString: \"hh:mm TT DD-MM-YYYY\",  ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        //dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["InsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["ParameterValue"])));
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
            ///
            #endregion
            ////////////////////////////////////////////////////////////////////////////////////////////
            var dtabc = getStorageTankTableList(fromDt, toDt, "");
            //var tablList = getStorageTankTableList(fromDt, toDt, "All");
            //return View(tablList);
            ViewBag.SelectedResource = resource;
            ViewBag.SelectedTimeFrom = TF;
            ViewBag.SelectedTimeTo = TT;
            ViewBag.SelectedTimeFrom = TF.ToString();
            ViewBag.SelectedTimeTo = TT.ToString();
            ViewBag.timeFrom = TF;
            ViewBag.timeTo = TT;
            ViewBag.dateFrom = df_date;
            ViewBag.dateTo = dt_date;
            return View(dtabc);
        }

        public ActionResult EnergyMonitoringReport()
        {
            DateTime fromDt = DateTime.Now.AddHours(0).Date;
            DateTime toDt = DateTime.Now.AddHours(24).Date;
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                int uID = Convert.ToInt32(Session["UserID"]);
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }

            //var tablList = getStorageTankTableList(fromDt, toDt, "All");
            //return View(tablList);
            var dtabc = getStorageTankTableList(fromDt, toDt, "");

            IEnumerable<StorageTankTableData> itoms = (IEnumerable<StorageTankTableData>)dtabc;
            StorageTankTableData itom = itoms.FirstOrDefault();
            double pff = 0;
            double i1s = 0;
            double i2s = 0;
            double i3s = 0;
            double pkvas = 0;
            double pkvars = 0;
            double pkws = 0;
            int counter = 0;
            double v1ns = 0;
            double v2ns = 0;
            double v3ns = 0;
            double v12s = 0;
            double v13s = 0;
            double v23s = 0;
            double freqs = 0;
            if (itom.pumpStatus1 != null)
            {
                for (int i = 0; i < itom.pumpStatus1.Count; i++)
                {
                    if (itom.pumpStatus1[i] + itom.P3Status[i] + itom.pumpStatus2[i] + itom.P4Status[i] > 0)
                    {
                        pff += itom.PF[i];
                        i1s += itom.I1A[i];
                        i2s += itom.I2A[i];
                        i3s += itom.I3A[i];
                        pkvas += itom.VA_kva[i];
                        pkvars += itom.VAR_kvar[i];
                        pkws += itom.W_kwatt[i];
                        v1ns += itom.V1N_V[i];
                        v2ns += itom.V2N_V[i];
                        v3ns += itom.V3N_V[i];

                        v12s += itom.V12_V[i];
                        v13s += itom.V13_V[i];
                        v23s += itom.V23_V[i];

                        freqs += itom.FreqHz[i];
                        counter++;
                    }
                    else
                    {

                    }
                }
            }
            if (counter == 0)
            {
                counter = 1;
            }
            pff = Math.Round((pff / counter), 2);
            i1s = Math.Round((i1s / counter), 2);
            i2s = Math.Round((i2s / counter), 2);
            i3s = Math.Round((i3s / counter), 2);
            pkvas = Math.Round((pkvas / counter), 2);
            pkvars = Math.Round((pkvars / counter), 2);
            pkws = Math.Round((pkws / counter), 2);
            double averageis = Math.Round(((i1s + i2s + i3s) / 3), 2);
            double units = Math.Round((pkws * (itom.P1WorkingInHours + itom.P2WorkingInHours + itom.P3WorkingInHours + itom.P4WorkingInHours)), 2);
            v1ns = Math.Round((v1ns / counter), 2);
            v2ns = Math.Round((v2ns / counter), 2);
            v3ns = Math.Round((v3ns / counter), 2);

            v12s = Math.Round((v12s / counter), 2);
            v13s = Math.Round((v13s / counter), 2);
            v23s = Math.Round((v23s / counter), 2);

            freqs = Math.Round((freqs / counter), 2);

            ViewData["LocationName"] = itom.locationName;
            ViewData["Pump1Working"] = Math.Round(itom.P1WorkingInHours,2);
            ViewData["Pump2Working"] = Math.Round(itom.P2WorkingInHours, 2);
            ViewData["Pump3Working"] = Math.Round(itom.P3WorkingInHours, 2);
            ViewData["Pump4Working"] = Math.Round(itom.P4WorkingInHours, 2);
            ViewData["PF"] = pff;
            ViewData["AverageVoltage"] = Math.Round(((v1ns + v2ns + v3ns) / 3), 2);
            ViewData["AverageLineVoltage"] = Math.Round(((v12s + v13s + v23s) / 3), 2);
            ViewData["AverageCurrent"] = averageis;
            ViewData["Frequency"] = freqs;
            ViewData["PKVA"] = pkvas;
            ViewData["PKVAR"] = pkvars;
            ViewData["PKW"] = pkws;
            ViewData["Units"] = units;

            /////////////////////////////////
            ///
            string scriptString = "";
            string chartdata = "";
            string chartdata1 = "";
            //scriptString = "var chart1 = new CanvasJS.Chart(\"chartContainer1\", { theme: \"light2\", animationEnabled: true, title:{ text: \"Energy Monitoring Stats\" }, data: [ { type: \"stackedColumn\", dataPoints: [";
            scriptString = "var chart1 = new CanvasJS.Chart(\"chartContainer1\", { theme: \"light2\", animationEnabled: true, title:{ text: \"Energy Monitoring Stats\" }, exportEnabled: true, dataPointWidth: 30, subtitles: [{text: \" Energy Data Fetched from Storage Tank Today  \" }], axisY:{labelFontSize: 10,includeZero: true },axisX:{labelFontSize: 10, title: \"All Energy Data (Effective Energy Stats)\",titleFontSize: 12,labelAngle: 45}, legend: { cursor: \"pointer\", itemclick: toogleDataSeries, fontSize: 11, horizontalAlign: \"center\"}, toolTip: {fontSize: 12, fontWeight: \"bold\", shared: true, content: \"{label}: {y}\" }, data: [ { type: \"stackedColumn\", dataPoints: [";
            if (itom.pumpStatus1 != null)
            {
                //scriptString += "{ type: \"stackedColumn\", name: \"Storage Tank\", showInLegend: true, dataPoints: [";
                scriptString += "{ y: " + itom.P1WorkingInHours + " , label: \"Pump 1 Hours\" },";
                scriptString += "{ y: " + itom.P2WorkingInHours + " , label: \"Pump 2 Hours\" },";
                scriptString += "{ y: " + itom.P3WorkingInHours + " , label: \"Pump 3 Hours\" },";
                scriptString += "{ y: " + itom.P4WorkingInHours + " , label: \"Pump 4 Hours\" },";
                scriptString += "{ y: " + pff + " , label: \"PF\" },";
                scriptString += "{ y: " + Math.Round(((v1ns + v2ns + v3ns) / 3), 2) + " , label: \"Avg. V\" },";
                scriptString += "{ y: " + averageis + " , label: \"Avg. A\" },";
                scriptString += "{ y: " + freqs + " , label: \"Freq.\" },";
                scriptString += "{ y: " + pkvas + " , label: \"Power (KVA)\" },";
                scriptString += "{ y: " + pkvars + " , label: \"Power (KVAR)\" },";
                scriptString += "{ y: " + pkws + " , label: \"Power (KW)\" }";
                scriptString += "]}]});";
                chartdata += "[";
                chartdata += "{\"category\":\"Pump 1 Hours\",\"tooltip\":\"" + itom.workingHoursTodayP1 + "\",\"value\":\"" + itom.P1WorkingInHours + " Hours\"},";
                chartdata += "{\"category\":\"Pump 2 Hours\",\"tooltip\":\"" + itom.workingHoursTodayP2 + "\",\"value\":\"" + itom.P2WorkingInHours + " Hours\"},";
                chartdata += "{\"category\":\"Pump 3 Hours\",\"tooltip\":\"" + itom.workingHoursTodayP3 + "\",\"value\":\"" + itom.P3WorkingInHours + " Hours\"},";
                chartdata += "{\"category\":\"Pump 4 Hours\",\"tooltip\":\"" + itom.workingHoursTodayP4 + "\",\"value\":\"" + itom.P4WorkingInHours + " Hours\"}]";
                ViewData["amChartData"] = chartdata;

                chartdata1 += "[";
                chartdata1 += "{\"category\":\"PF\",\"tooltip\":\"" + pff + "\",\"value\":\"" + pff + "\"},";
                chartdata1 += "{\"category\":\"Avg. Phase V\",\"tooltip\":\"" + Math.Round(((v1ns + v2ns + v3ns) / 3), 2) + "\",\"value\":\"" + Math.Round(((v1ns + v2ns + v3ns) / 3), 2) + "\"},";
                chartdata1 += "{\"category\":\"Avg. Line V\",\"tooltip\":\"" + Math.Round(((v12s + v13s + v23s) / 3), 2) + "\",\"value\":\"" + Math.Round(((v12s + v13s + v23s) / 3), 2) + "\"},";
                chartdata1 += "{\"category\":\"Avg. A\",\"tooltip\":\"" + averageis + "\",\"value\":\"" + averageis + "\"},";
                chartdata1 += "{\"category\":\"Freq.\",\"tooltip\":\"" + freqs + "\",\"value\":\"" + freqs + "\"},";
                chartdata1 += "{\"category\":\"Power (KVA)\",\"tooltip\":\"" + pkvas + "\",\"value\":\"" + pkvas + "\"},";
                chartdata1 += "{\"category\":\"Power (KVAR)\",\"tooltip\":\"" + pkvars + "\",\"value\":\"" + pkvars + "\"},";
                chartdata1 += "{\"category\":\"Power (KW)\",\"tooltip\":\"" + pkws + "\",\"value\":\"" + pkws + "\"},";
                chartdata1 += "{\"category\":\"Units Consumed\",\"tooltip\":\"" + units + "\",\"value\":\"" + units + "\"}]";
                ViewData["amChartData1"] = chartdata1;
            }
            else
            {
                ViewData["amChartData"] = chartdata;
                ViewData["amChartData1"] = chartdata1;
            }
            /// 
            /////////////////////////////////////////////////////////////////////////////////////////

            


            string NewscripString = scriptString;
            ViewData["chartData"] = NewscripString;
            ////////////////////////////////////////////////////////////////////////////////////////////

            return View(dtabc);
        }
        //public List<StorageTankTableData> getStorageTankTableList(DateTime fromDT, DateTime toDt, string res)
        [HttpPost]
        public ActionResult EnergyMonitoringReport(FormCollection review)
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
            if (tt_time.ToUpper() == "12:00 AM")
            {
                tt_time = "11:59 PM";
            }
            DateTime FinalTimeFrom = Convert.ToDateTime(df_date + " " + tf_time);
            DateTime FinalTimeTo = Convert.ToDateTime(dt_date + " " + tt_time);
            DateTime fromDt = FinalTimeFrom;
            DateTime toDt = FinalTimeTo;
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                int uID = Convert.ToInt32(Session["UserID"]);
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            var dtabc = getStorageTankTableList(fromDt, toDt, "");

            IEnumerable<StorageTankTableData> itoms = (IEnumerable<StorageTankTableData>)dtabc;
            StorageTankTableData itom = itoms.FirstOrDefault();
            double pff = 0;
            double i1s = 0;
            double i2s = 0;
            double i3s = 0;
            double pkvas = 0;
            double pkvars = 0;
            double pkws = 0;
            int counter = 0;
            double v1ns = 0;
            double v2ns = 0;
            double v3ns = 0;
            double v12s = 0;
            double v13s = 0;
            double v23s = 0;
            double freqs = 0;
            double averageis = 0;
            if (itoms.FirstOrDefault().pumpStatus1 != null)
            {
                for (int i = 0; i < itom.pumpStatus1.Count; i++)
                {
                    if (itom.pumpStatus1[i] + itom.P3Status[i] + itom.pumpStatus2[i] + itom.P4Status[i] > 0)
                    {
                        pff += itom.PF[i];
                        i1s += itom.I1A[i];
                        i2s += itom.I2A[i];
                        i3s += itom.I3A[i];
                        pkvas += itom.VA_kva[i];
                        pkvars += itom.VAR_kvar[i];
                        pkws += itom.W_kwatt[i];
                        v1ns += itom.V1N_V[i];
                        v2ns += itom.V2N_V[i];
                        v3ns += itom.V3N_V[i];

                        v12s += itom.V12_V[i];
                        v13s += itom.V13_V[i];
                        v23s += itom.V23_V[i];

                        freqs += itom.FreqHz[i];
                        counter++;
                    }
                    else
                    {

                    }
                }
                if (counter == 0)
                {
                    counter = 1;
                }
                pff = Math.Round((pff / counter), 2);
                i1s = Math.Round((i1s / counter), 2);
                i2s = Math.Round((i2s / counter), 2);
                i3s = Math.Round((i3s / counter), 2);
                pkvas = Math.Round((pkvas / counter), 2);
                pkvars = Math.Round((pkvars / counter), 2);
                pkws = Math.Round((pkws / counter), 2);
                averageis = Math.Round(((i1s + i2s + i3s) / 3), 2);
                double units = Math.Round((pkws * (itom.P1WorkingInHours + itom.P2WorkingInHours + itom.P3WorkingInHours + itom.P4WorkingInHours)), 2);

                v1ns = Math.Round((v1ns / counter), 2);
                v2ns = Math.Round((v2ns / counter), 2);
                v3ns = Math.Round((v3ns / counter), 2);

                v12s = Math.Round((v12s / counter), 2);
                v13s = Math.Round((v13s / counter), 2);
                v23s = Math.Round((v23s / counter), 2);

                freqs = Math.Round((freqs / counter), 2);

                ViewData["LocationName"] = itom.locationName;
                ViewData["Pump1Working"] = Math.Round(itom.P1WorkingInHours, 2);
                ViewData["Pump2Working"] = Math.Round(itom.P2WorkingInHours, 2);
                ViewData["Pump3Working"] = Math.Round(itom.P3WorkingInHours, 2);
                ViewData["Pump4Working"] = Math.Round(itom.P4WorkingInHours, 2);
                ViewData["PF"] = pff;
                ViewData["AverageVoltage"] = Math.Round(((v1ns + v2ns + v3ns) / 3), 2);
                ViewData["AverageLineVoltage"] = Math.Round(((v12s + v13s + v23s) / 3), 2);
                ViewData["AverageCurrent"] = averageis;
                ViewData["Frequency"] = freqs;
                ViewData["PKVA"] = pkvas;
                ViewData["PKVAR"] = pkvars;
                ViewData["PKW"] = pkws;
                ViewData["Units"] = units;

                /////////////////////////////////
                ///
                string scriptString = "";
                string chartdata = "";
                string chartdata1 = "";
                //scriptString = "var chart1 = new CanvasJS.Chart(\"chartContainer1\", { theme: \"light2\", animationEnabled: true, title:{ text: \"Energy Monitoring Stats\" }, data: [ { type: \"stackedColumn\", dataPoints: [";
                scriptString = "var chart1 = new CanvasJS.Chart(\"chartContainer1\", { theme: \"light2\", animationEnabled: true, title:{ text: \"Energy Monitoring Stats\" }, exportEnabled: true, dataPointWidth: 30, subtitles: [{text: \" Energy Data Fetched from Storage Tank Today  \" }], axisY:{labelFontSize: 10,includeZero: true },axisX:{labelFontSize: 10, title: \"All Energy Data (Effective Energy Stats)\",titleFontSize: 12,labelAngle: 45}, legend: { cursor: \"pointer\", itemclick: toogleDataSeries, fontSize: 11, horizontalAlign: \"center\"}, toolTip: {fontSize: 12, fontWeight: \"bold\", shared: true, content: \"{label}: {y}\" }, data: [ { type: \"stackedColumn\", dataPoints: [";
                if (itom.pumpStatus1 != null)
                {
                    //scriptString += "{ type: \"stackedColumn\", name: \"Storage Tank\", showInLegend: true, dataPoints: [";
                    scriptString += "{ y: " + itom.P1WorkingInHours + " , label: \"Pump 1 Hours\" },";
                    scriptString += "{ y: " + itom.P2WorkingInHours + " , label: \"Pump 2 Hours\" },";
                    scriptString += "{ y: " + itom.P3WorkingInHours + " , label: \"Pump 3 Hours\" },";
                    scriptString += "{ y: " + itom.P4WorkingInHours + " , label: \"Pump 4 Hours\" },";
                    scriptString += "{ y: " + pff + " , label: \"PF\" },";
                    scriptString += "{ y: " + Math.Round(((v1ns + v2ns + v3ns) / 3), 2) + " , label: \"Avg. V\" },";
                    scriptString += "{ y: " + averageis + " , label: \"Avg. A\" },";
                    scriptString += "{ y: " + freqs + " , label: \"Freq.\" },";
                    scriptString += "{ y: " + pkvas + " , label: \"Power (KVA)\" },";
                    scriptString += "{ y: " + pkvars + " , label: \"Power (KVAR)\" },";
                    scriptString += "{ y: " + pkws + " , label: \"Power (KW)\" }";
                    scriptString += "]}]});";
                    chartdata += "[";
                    chartdata += "{\"category\":\"Pump 1 Hours\",\"tooltip\":\"" + itom.workingHoursTodayP1 + "\",\"value\":\"" + itom.P1WorkingInHours + " Hours\"},";
                    chartdata += "{\"category\":\"Pump 2 Hours\",\"tooltip\":\"" + itom.workingHoursTodayP2 + "\",\"value\":\"" + itom.P2WorkingInHours + " Hours\"},";
                    chartdata += "{\"category\":\"Pump 3 Hours\",\"tooltip\":\"" + itom.workingHoursTodayP3 + "\",\"value\":\"" + itom.P3WorkingInHours + " Hours\"},";
                    chartdata += "{\"category\":\"Pump 4 Hours\",\"tooltip\":\"" + itom.workingHoursTodayP4 + "\",\"value\":\"" + itom.P4WorkingInHours + " Hours\"}]";
                    ViewData["amChartData"] = chartdata;

                    chartdata1 += "[";
                    chartdata1 += "{\"category\":\"PF\",\"tooltip\":\"" + pff + "\",\"value\":\"" + pff + "\"},";
                    chartdata1 += "{\"category\":\"Avg. Phase V\",\"tooltip\":\"" + Math.Round(((v1ns + v2ns + v3ns) / 3), 2) + "\",\"value\":\"" + Math.Round(((v1ns + v2ns + v3ns) / 3), 2) + "\"},";
                    chartdata1 += "{\"category\":\"Avg. Line V\",\"tooltip\":\"" + Math.Round(((v12s + v13s + v23s) / 3), 2) + "\",\"value\":\"" + Math.Round(((v12s + v13s + v23s) / 3), 2) + "\"},";
                    chartdata1 += "{\"category\":\"Avg. A\",\"tooltip\":\"" + averageis + "\",\"value\":\"" + averageis + "\"},";
                    chartdata1 += "{\"category\":\"Freq.\",\"tooltip\":\"" + freqs + "\",\"value\":\"" + freqs + "\"},";
                    chartdata1 += "{\"category\":\"Power (KVA)\",\"tooltip\":\"" + pkvas + "\",\"value\":\"" + pkvas + "\"},";
                    chartdata1 += "{\"category\":\"Power (KVAR)\",\"tooltip\":\"" + pkvars + "\",\"value\":\"" + pkvars + "\"},";
                    chartdata1 += "{\"category\":\"Power (KW)\",\"tooltip\":\"" + pkws + "\",\"value\":\"" + pkws + "\"},";
                    chartdata1 += "{\"category\":\"Units Consumed\",\"tooltip\":\"" + units + "\",\"value\":\"" + units + "\"}]";
                    ViewData["amChartData1"] = chartdata1;
                }
                else
                {
                    ViewData["amChartData"] = chartdata;
                    ViewData["amChartData1"] = chartdata1;
                }
                /// 
                /////////////////////////////////////////////////////////////////////////////////////////



            }
                //var tablList = getStorageTankTableList(fromDt, toDt, "All");
                //return View(tablList);
            ViewBag.SelectedResource = resource;
            ViewBag.SelectedTimeFrom = TF;
            ViewBag.SelectedTimeTo = TT;
            ViewBag.SelectedTimeFrom = TF.ToString();
            ViewBag.SelectedTimeTo = TT.ToString();
            ViewBag.timeFrom = TF;
            ViewBag.timeTo = TT;
            ViewBag.dateFrom = df_date;
            ViewBag.dateTo = dt_date;
            string NewscripString = "";
            ViewData["chartData"] = NewscripString;
            return View(dtabc);
        }

        public ActionResult ParameterizedData()
        {
            DateTime fromDt = DateTime.Now.AddHours(0).Date;
            DateTime toDt = DateTime.Now.AddHours(24).Date;
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> parameterList = new List<string>();
            IList<string> resourceList = new List<string>();
            DataTable dtVal = new DataTable();
            string paramsListQuery = "select p.parameterDescription from tblParameter p inner join tblResourceTypeParameter tp on tp.parameterID = p.ParameterID where tp.resourceTypeID = 1002 order by p.parameterMaxThr";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    DataTable dtParams = new DataTable();
                    conn.Open();
                    SqlDataAdapter sdaParams = new SqlDataAdapter(paramsListQuery, conn);
                    sdaParams.Fill(dtParams);
                    foreach (DataRow dr in dtParams.Rows)
                    {
                        parameterList.Add(dr["parameterDescription"].ToString());
                    }
                }
                catch (Exception ex)
                {

                }
            }
            foreach (var item in db.tblResources.AsEnumerable().Where(item => item.companyID == 1 && item.resourceTypeID == 1002))
            {
                resourceList.Add(item.resourceLocationName);
            }
            ViewBag.ResourceList = resourceList;
            ViewBag.ParameterList = parameterList;
            ResourceAndRangeSelector rs = new ResourceAndRangeSelector();
            rs.resourceID = "Lawrence Road Storage Tank";
            rs.parameterID = "Pump No. 1 Status";
            ////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select  parameterID from tblParameter where parameterDescription = '" + rs.parameterID + "' ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "";
                        if (Convert.ToInt32(Session["CompanyID"]) == 1)
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterDescription = '"+ rs.parameterID + "'  order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterDescription = '" + rs.parameterID + "'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " order by cast(r.resourceNumber as int) asc";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \""+ rs.resourceID + "\" },";
                        //Report of All Ponding Locations between " + fd + " to " + td + "
                        scriptString += "subtitles: [{text: \" "+ rs.parameterID + " Data between " + fromDt + " to " + toDt + " \" }],";
                        //scriptString += "axisY: {suffix: \" \",labelFontSize: 15, minimum: 0 },";
                        //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                        //scriptString += "toolTip: { shared: false },";
                        scriptString += "axisY: {labelFontSize: 10, labelFormatter: function(){ return \" \"; }},";
                        scriptString += "toolTip: { shared: false , contentFormatter: function(e){ var str = \" \" ; for (var i = 0; i < e.entries.length; i++){ var utcSeconds = e.entries[i].dataPoint.x; var d = new Date(utcSeconds); if(e.entries[i].dataPoint.y == 0){ var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: OFF</b> at  \" + d.toLocaleString('en-IN'); str = str+temp; } else { var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: ON</b> at \" + d.toLocaleString('en-IN'); str = str+temp; } } return (str); }},";
                        scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries},";
                        scriptString += " data: [  ";
                        foreach (DataRow drPar in dtPar.Rows)
                        {
                            //string parName = drPar["parameterName"].ToString();
                            //string aquery = ";WITH CTE AS ( ";
                            //aquery += "SELECT e.parameterID, e.parameterValue, e.sheetInsertionDateTime,  ";
                            //aquery += " RN = ROW_NUMBER() OVER(PARTITION BY e.parameterID ";
                            //aquery += "ORDER BY e.sheetInsertionDateTime DESC) ";
                            //aquery += "FROM tblSheet e ";
                            //aquery += "inner join tblResource r on e.resourceID = r.resourceID ";
                            //aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterValue >= 0 and e.parameterID IN (1026,1027,1028,1029,1030,1031,1032,1033, " + Convert.ToInt32(drRes["parameterID"]) + " )";
                            ////aquery += " and e.sheetInsertionDateTime >= DATEADD(day,-2,GETDATE()) ";
                            //aquery += " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + fromDt + "', 103), 121) and e.sheetInsertionDateTime <= CONVERT(CHAR(24), CONVERT(DATETIME, '" + toDt + "', 103), 121) ";
                            //aquery += ") ";
                            //aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";

                            string aquery = "";
                            aquery += " WITH cte AS(";
                            aquery += " SELECT * FROM";
                            aquery += " (";
                            aquery += " SELECT DISTINCT r.resourceName AS Location,";
                            aquery += " r.ResourceID, p.parameterDescription AS pID,";
                            aquery += " CAST(s.ParameterValue AS NUMERIC(18, 2)) AS pVal,";
                            aquery += " s.sheetInsertionDateTime as tim";
                            aquery += " FROM tblSheet s";
                            aquery += " inner join tblResource r on s.ResourceID = r.ResourceID";
                            aquery += " inner join tblParameter p on s.ParameterID = p.ParameterID";
                            aquery += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID";
                            aquery += " where";
                            aquery += " r.ResourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and";
                            aquery += " sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + fromDt + "', 103), 121) and sheetInsertionDateTime <= CONVERT(CHAR(24), CONVERT(DATETIME, '" + toDt + "', 103), 121)";
                            aquery += " )";
                            aquery += " AS SourceTable";
                            aquery += " PIVOT";
                            aquery += " (";
                            aquery += " SUM(pVal) FOR pID";
                            aquery += " IN";
                            aquery += " ( [Pump No. 1 Status],[Pump No. 2 Status],[Pump No. 3 Status],[Pump No. 4 Status],[Pump No. 1 Mode],[Pump No. 2 Mode],[Pump No. 3 Mode],[Pump No. 4 Mode]";
                            aquery += " )";
                            aquery += " )";
                            aquery += " AS PivotTable";
                            aquery += " )  ";
                            aquery += " SELECT* FROM cte";
                            aquery += " order by cast(ResourceID as INT) ASC, ";
                            aquery += " tim DESC"; 


                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            sdaVal.Fill(dtVal);
                            //scriptString += "{ type: \"area\", name: \"" + rs.parameterID + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"DD-MM-YYYY hh:mm:ss TT\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} </strong> at {x}\", ";
                            scriptString += "{ type: \"area\", name: \"" + rs.parameterID + "\", showInLegend: true,  markerSize: 1, xValueType: \"dateTime\", xValueFormatString: \"hh:mm TT DD-MM-YYYY\",  ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                //if (dtVal.Rows.IndexOf(drVal) != 0)
                                //{
                                //    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                //    {
                                //        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal["parameterValue"]) / 1), 1)));
                                //        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                //    }
                                //    else
                                //    {
                                //        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                //        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                //        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal["parameterValue"]) / 1), 1)));
                                //        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                //    }
                                //}
                                //else
                                //{
                                //    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 1));
                                //    dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                //}

                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["tim"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["tim"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal["Pump No. 1 Status"]) / 1), 1)));
                                        dt = Convert.ToDateTime(drVal["tim"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["tim"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["tim"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal["Pump No. 1 Status"]) / 1), 1)));
                                        dt = Convert.ToDateTime(drVal["tim"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["tim"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["Pump No. 1 Status"]) / 1));
                                    dt = Convert.ToDateTime(drVal["tim"]);
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
            ////////////////////////////////////////////////////////////////
            var storageDataList = new List<StorageParameterChartClass>();
            StorageParameterChartClass tableData = new StorageParameterChartClass();
            string parameterName = "Pump No. 1 Status";
            DateTime FinalTimeFrom = fromDt;
            DateTime FinalTimeTo = toDt;
            string resources = "Lawrence Road Storage Tank";
            string parameter = "Pump No. 1 Status";
            if (dtVal.Rows.Count > 0)
            {
                try
                {
                    tableData = getAllSpellsForStorageParameterizedChartPumps(dtVal, parameterName, FinalTimeFrom, FinalTimeTo, 1);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                tableData.locationName = resources;
                tableData.parameterName = parameter;
                tableData.workingInHours = 0;
                tableData.totalHours = 0;
                tableData.availableHours = 0;
                tableData.nonAvailableHours = 0;
                tableData.workingInHoursManual = 0;
                tableData.workingInHoursRemote = 0;
                tableData.workingInHoursScheduling = 0;
                tableData.minValue = 0;
                tableData.maxValue = 0;
                tableData.avgVale = 0;
                tableData.avgOfAvailableHours = 0;
                tableData.avgOfNonAvailableHours = 0;
            }
            tableData.parameterName = parameter;
            storageDataList.Add(tableData);
            ////////////////////////////////////////////////////////////////
            return View(storageDataList);
        }

        [HttpPost]
        public ActionResult ParameterizedData(FormCollection review)
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            string resource = review["resource"].ToString();
            string parameter = review["parameter"].ToString();
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
            if (tt_time.ToUpper() == "12:00 AM")
            {
                tt_time = "11:59 PM";
            }

            DateTime FinalTimeFrom = Convert.ToDateTime(df_date + " " + tf_time);
            DateTime FinalTimeTo = Convert.ToDateTime(dt_date + " " + tt_time);
            DateTime fromDt = FinalTimeFrom;
            DateTime toDt = FinalTimeTo;
            IList<string> parameterList = new List<string>();
            IList<string> resourceList = new List<string>();
            DataTable dtVal = new DataTable();
            string paramsListQuery = "select p.parameterDescription from tblParameter p inner join tblResourceTypeParameter tp on tp.parameterID = p.ParameterID where tp.resourceTypeID = 1002 order by p.parameterMaxThr";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    DataTable dtParams = new DataTable();
                    conn.Open();
                    SqlDataAdapter sdaParams = new SqlDataAdapter(paramsListQuery, conn);
                    sdaParams.Fill(dtParams);
                    foreach (DataRow dr in dtParams.Rows)
                    {
                        parameterList.Add(dr["parameterDescription"].ToString());
                    }
                }
                catch (Exception ex)
                {

                }
            }
            foreach (var item in db.tblResources.AsEnumerable().Where(item => item.companyID == 1 && item.resourceTypeID == 1002))
            {
                resourceList.Add(item.resourceLocationName);
            }
            ViewBag.ResourceList = resourceList;
            ViewBag.ParameterList = parameterList;
            ViewBag.SelectedParameter = parameter;
            ResourceAndRangeSelector rs = new ResourceAndRangeSelector();
            rs.resourceID = "Lawrence Road Storage Tank";
            rs.parameterID = parameter;
            //////////////////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select  parameterID from tblParameter where parameterDescription = '" + rs.parameterID + "' ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "";
                        if (Convert.ToInt32(Session["CompanyID"]) == 1)
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterDescription = '" + rs.parameterID + "'  order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterDescription = '" + rs.parameterID + "'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " order by cast(r.resourceNumber as int) asc";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"" + rs.resourceID + "\" },";
                        //Report of All Ponding Locations between " + fd + " to " + td + "
                        scriptString += "subtitles: [{text: \" " + rs.parameterID + " Data between " + fromDt + " to " + toDt + " \" }],";
                        //scriptString += "axisY: {suffix: \" \",labelFontSize: 15, minimum: 0 },";
                        //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                        //P1 Auto/Mannual
                        if (rs.parameterID == "Pump No. 1 Status" || rs.parameterID == "Pump No. 2 Status" || rs.parameterID == "Pump No. 3 Status" || rs.parameterID == "Pump No. 4 Status")
                        {
                            scriptString += "axisY: {labelFontSize: 10, labelFormatter: function(){ return \" \"; }},";
                            scriptString += "toolTip: { shared: false , contentFormatter: function(e){ var str = \" \" ; for (var i = 0; i < e.entries.length; i++){ var utcSeconds = e.entries[i].dataPoint.x; var d = new Date(utcSeconds); if(e.entries[i].dataPoint.y == 0){ var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: OFF</b> at  \" + d.toLocaleString('en-IN'); str = str+temp; } else { var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: ON</b> at \" + d.toLocaleString('en-IN'); str = str+temp; } } return (str); }},";
                        }
                        else if (rs.parameterID == "Pump No. 1 Mode" || rs.parameterID == "Pump No. 2 Mode" || rs.parameterID == "Pump No. 3 Mode" || rs.parameterID == "Pump No. 4 Mode")
                        {
                            scriptString += "axisY: {minimum: -1, maximum: 2, labelFontSize: 10, labelFormatter: function(){ return \" \"; }},";
                            scriptString += "toolTip: { shared: false , contentFormatter: function(e){ var str = \" \" ; for (var i = 0; i < e.entries.length; i++){ var utcSeconds = e.entries[i].dataPoint.x; var d = new Date(utcSeconds); if(e.entries[i].dataPoint.y == 0){ var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: MANUAL</b> at  \" + d.toLocaleString('en-IN'); str = str+temp; } else { var temp = e.entries[i].dataSeries.name + \" \" +\"<b>: AUTO</b> at \" + d.toLocaleString('en-IN'); str = str+temp; } } return (str); }},";
                        }
                        else
                        {
                            scriptString += "axisY:{labelFontSize: 15},";
                            scriptString += "toolTip: { shared: false },";
                        }
                        //scriptString += "toolTip: { shared: false },";
                        scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries},";
                        scriptString += " data: [  ";
                        foreach (DataRow drPar in dtPar.Rows)
                        {
                            //string parName = drPar["parameterName"].ToString();
                            string aquery = "";
                            aquery += " WITH cte AS(";
                            aquery += " SELECT * FROM";
                            aquery += " (";
                            aquery += " SELECT DISTINCT r.resourceName AS Location,";
                            aquery += " r.ResourceID, p.parameterDescription AS pID,";
                            aquery += " CAST(s.ParameterValue AS NUMERIC(18, 2)) AS pVal,";
                            aquery += " s.sheetInsertionDateTime as tim";
                            aquery += " FROM tblSheet s";
                            aquery += " inner join tblResource r on s.ResourceID = r.ResourceID";
                            aquery += " inner join tblParameter p on s.ParameterID = p.ParameterID";
                            aquery += " inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID";
                            aquery += " where";
                            aquery += " r.ResourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and";
                            aquery += " sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + fromDt + "', 103), 121) and sheetInsertionDateTime <= CONVERT(CHAR(24), CONVERT(DATETIME, '" + toDt + "', 103), 121)";
                            aquery += " )";
                            aquery += " AS SourceTable";
                            aquery += " PIVOT";
                            aquery += " (";
                            aquery += " SUM(pVal) FOR pID";
                            aquery += " IN ( ";
                            aquery += "[Pump No. 1 Status],[Pump No. 2 Status],[Pump No. 3 Status],[Pump No. 4 Status],[Pump No. 1 Mode],[Pump No. 2 Mode],[Pump No. 3 Mode],[Pump No. 4 Mode]";
                            if (rs.parameterID == "Pump No. 1 Status" || rs.parameterID == "Pump No. 2 Status" || rs.parameterID == "Pump No. 3 Status" || rs.parameterID == "Pump No. 4 Status" || rs.parameterID == "Pump No. 1 Mode" || rs.parameterID == "Pump No. 2 Mode" || rs.parameterID == "Pump No. 3 Mode" || rs.parameterID == "Pump No. 4 Mode")
                            {

                            }
                            else
                            {
                                aquery += ",["+rs.parameterID+"] ";
                            }
                            
                            aquery += " )";
                            aquery += " )";
                            aquery += " AS PivotTable";
                            aquery += " )  ";
                            aquery += " SELECT* FROM cte";
                            aquery += " order by cast(ResourceID as INT) ASC, ";
                            aquery += " tim DESC"; string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                           
                            sdaVal.Fill(dtVal);
                            if (rs.parameterID == "Pump No. 1 Status" || rs.parameterID == "Pump No. 2 Status" || rs.parameterID == "Pump No. 3 Status" || rs.parameterID == "Pump No. 4 Status" || rs.parameterID == "Pump No. 1 Mode" || rs.parameterID == "Pump No. 2 Mode" || rs.parameterID == "Pump No. 3 Mode" || rs.parameterID == "Pump No. 4 Mode")
                            {
                                if (rs.parameterID == "P1 Auto/Mannual" || rs.parameterID == "P2 Auto/Mannual" || rs.parameterID == "P3 Auto/Mannual" || rs.parameterID == "P4 Auto/Mannual")
                                {
                                    scriptString += "{ type: \"area\", name: \"" + rs.parameterID.Replace("Auto/Mannual","Mode") + "\", showInLegend: true,  markerSize: 1, xValueType: \"dateTime\", xValueFormatString: \"hh:mm TT DD-MM-YYYY\",  ";
                                }
                                else 
                                { 
                                    scriptString += "{ type: \"area\", name: \"" + rs.parameterID + "\", showInLegend: true,  markerSize: 1, xValueType: \"dateTime\", xValueFormatString: \"hh:mm TT DD-MM-YYYY\",  "; 
                                }
                            }
                            else
                            {
                                scriptString += "{ type: \"line\", name: \"" + rs.parameterID + "\", showInLegend: true,  markerSize: 1, xValueType: \"dateTime\", xValueFormatString: \"hh:mm TT DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} </strong> at {x}\", ";
                            }
                            //scriptString += "{ type: \"area\", name: \"" + rs.parameterID + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"DD-MM-YYYY hh:mm:ss TT\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} </strong> at {x}\", ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["tim"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["tim"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal[rs.parameterID]) / 1), 1)));
                                        dt = Convert.ToDateTime(drVal["tim"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["tim"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["tim"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal[rs.parameterID]) / 1), 1)));
                                        dt = Convert.ToDateTime(drVal["tim"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["tim"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal[rs.parameterID]) / 1));
                                    dt = Convert.ToDateTime(drVal["tim"]);
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

            ////////////////////////////////////////////////////////////////
            var storageDataList = new List<StorageParameterChartClass>();
            StorageParameterChartClass tableData = new StorageParameterChartClass();
            string parameterName = rs.parameterID;
            if (dtVal.Rows.Count > 0)
            {
                try
                {
                    if (rs.parameterID == "Pump No. 1 Status" || rs.parameterID == "Pump No. 2 Status" || rs.parameterID == "Pump No. 3 Status" || rs.parameterID == "Pump No. 4 Status" || rs.parameterID == "Pump No. 1 Mode" || rs.parameterID == "Pump No. 2 Mode" || rs.parameterID == "Pump No. 3 Mode" || rs.parameterID == "Pump No. 4 Mode")
                    {
                        if (rs.parameterID == "Pump No. 1 Status" || rs.parameterID == "Pump No. 1 Mode")
                        {
                            tableData = getAllSpellsForStorageParameterizedChartPumps(dtVal, parameterName, FinalTimeFrom, FinalTimeTo, 1);
                        }
                        else if (rs.parameterID == "Pump No. 2 Status" || rs.parameterID == "Pump No. 2 Mode")
                        {
                            tableData = getAllSpellsForStorageParameterizedChartPumps(dtVal, parameterName, FinalTimeFrom, FinalTimeTo, 2);
                        }
                        else if (rs.parameterID == "Pump No. 3 Status" || rs.parameterID == "Pump No. 3 Mode")
                        {
                            tableData = getAllSpellsForStorageParameterizedChartPumps(dtVal, parameterName, FinalTimeFrom, FinalTimeTo, 3);
                        }
                        else if (rs.parameterID == "Pump No. 4 Status" || rs.parameterID == "Pump No. 4 Mode")
                        {
                            tableData = getAllSpellsForStorageParameterizedChartPumps(dtVal, parameterName, FinalTimeFrom, FinalTimeTo, 4);
                        }
                    }
                    else
                    {
                        tableData = getAllSpellsForStorageParameterizedChart(dtVal, parameterName, FinalTimeFrom, FinalTimeTo);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                tableData.locationName = resource;
                tableData.parameterName = parameter;
                tableData.workingInHours = 0;
                tableData.totalHours = 0;
                tableData.availableHours = 0;
                tableData.nonAvailableHours = 0;
                tableData.workingInHoursManual = 0;
                tableData.workingInHoursRemote = 0;
                tableData.workingInHoursScheduling = 0;
                tableData.minValue = 0;
                tableData.maxValue = 0;
                tableData.avgVale = 0;
                tableData.avgOfAvailableHours = 0;
                tableData.avgOfNonAvailableHours = 0;
            }
            tableData.parameterName = parameter;
            storageDataList.Add(tableData);
            ////////////////////////////////////////////////////////////////
            
            ViewBag.SelectedTimeFrom = TF;
            ViewBag.SelectedTimeTo = TT;
            ViewBag.SelectedTimeFrom = TF.ToString();
            ViewBag.SelectedTimeTo = TT.ToString();
            ViewBag.timeFrom = TF;
            ViewBag.timeTo = TT;
            ViewBag.dateFrom = df_date;
            ViewBag.dateTo = dt_date;
            return View(storageDataList);
        }
        public ActionResult TankStorageReport()
        {
            DateTime fromDt = DateTime.Now.AddHours(0).Date;
            DateTime toDt = DateTime.Now.AddHours(24).Date;
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                int uID = Convert.ToInt32(Session["UserID"]);
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            /////////////////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'Tank Level1 (ft)'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "";
                        if (Convert.ToInt32(Session["CompanyID"]) == 1)
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'Tank Level1 (ft)'  order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'Tank Level1 (ft)'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " order by cast(r.resourceNumber as int) asc";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Tank Level\" },";
                        //Report of All Ponding Locations between " + fd + " to " + td + "
                        scriptString += "subtitles: [{text: \" Storage Tank Level between " + fromDt + " to " + toDt + " \" }],";
                        scriptString += "axisY: {suffix: \" ft\",labelFontSize: 15,  interval: 2, minimum: 0, maximum: 16  },";
                        //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                        scriptString += "toolTip: { shared: false },";
                        scriptString += "legend: { cursor: \"pointer\", itemclick: toogleDataSeries},";
                        scriptString += " data: [  ";
                        foreach (DataRow drPar in dtPar.Rows)
                        {
                            //string parName = drPar["parameterName"].ToString();
                            string aquery = ";WITH CTE AS ( ";
                            aquery += "SELECT e.parameterID, e.parameterValue, e.sheetInsertionDateTime,  ";
                            aquery += " RN = ROW_NUMBER() OVER(PARTITION BY e.parameterID ";
                            aquery += "ORDER BY e.sheetInsertionDateTime DESC) ";
                            aquery += "FROM tblSheet e ";
                            aquery += "inner join tblResource r on e.resourceID = r.resourceID ";
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterValue >= 0 and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " ";
                            //aquery += " and e.sheetInsertionDateTime >= DATEADD(day,-2,GETDATE()) ";
                            aquery += " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + fromDt + "', 103), 121) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ color: \"rgb(0, 167, 236)\", type: \"area\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"DD-MM-YYYY hh:mm:ss TT\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} ft</strong> at {x}\", ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal["parameterValue"]) / 1),3)));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal["parameterValue"]) / 1), 3)));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 1));
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
            var dtabc = getStorageTankTableList(fromDt, toDt, "");
            string NewscripString = scriptString;
            ViewData["chartData"] = NewscripString;
            dynamic mymodel = new ExpandoObject();
            ////////////////////////////////////////////////////////////////////////////////////////////
            mymodel.SummaryList = dtabc;
            mymodel.DaywiseList = ViewData["tableData"];
            //var tablList = getStorageTankTableList(fromDt, toDt, "All");
            //return View(tablList);
            return View(dtabc);
        }
        //public List<StorageTankTableData> getStorageTankTableList(DateTime fromDT, DateTime toDt, string res)
        [HttpPost]
        public ActionResult TankStorageReport(FormCollection review)
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
            if (tt_time.ToUpper() == "12:00 AM")
            {
                tt_time = "11:59 PM";
            }

            DateTime FinalTimeFrom = Convert.ToDateTime(df_date + " " + tf_time);
            DateTime FinalTimeTo = Convert.ToDateTime(dt_date + " " + tt_time);
            DateTime fromDt = FinalTimeFrom;
            DateTime toDt = FinalTimeTo;
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                int uID = Convert.ToInt32(Session["UserID"]);
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Storm Water Tank" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            /////////////////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'Tank Level1 (ft)'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "";
                        if (Convert.ToInt32(Session["CompanyID"]) == 1)
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'Tank Level1 (ft)'  order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'Tank Level1 (ft)'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " order by cast(r.resourceNumber as int) asc";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Tank Level\" },";
                        //Report of All Ponding Locations between " + fd + " to " + td + "
                        scriptString += "subtitles: [{text: \" Storage Tank Level between " + fromDt + " to " + toDt + " \" }],";
                        scriptString += "axisY: {suffix: \" ft\",labelFontSize: 15,  interval: 2, minimum: 0, maximum: 16  },";
                        //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                        scriptString += "toolTip: { shared: false },";
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
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterValue >= 0 and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " ";
                            //aquery += " and e.sheetInsertionDateTime >= DATEADD(day,-2,GETDATE()) ";
                            aquery += " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + fromDt + "', 103), 121) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ color: \"rgb(0, 167, 236)\", type: \"area\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"DD-MM-YYYY hh:mm:ss TT\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} ft</strong> at {x}\", ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal["parameterValue"]) / 1), 1)));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Math.Round((Convert.ToDouble(drVal["parameterValue"]) / 1), 1)));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 1));
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
            var dtabc = getStorageTankTableList(fromDt, toDt, "");
            string NewscripString = scriptString;
            ViewData["chartData"] = NewscripString;
            ////////////////////////////////////////////////////////////////////////////////////////////

            //var tablList = getStorageTankTableList(fromDt, toDt, "All");
            //return View(tablList);
            ViewBag.SelectedResource = resource;
            ViewBag.SelectedTimeFrom = TF;
            ViewBag.SelectedTimeTo = TT;
            ViewBag.SelectedTimeFrom = TF.ToString();
            ViewBag.SelectedTimeTo = TT.ToString();
            ViewBag.timeFrom = TF;
            ViewBag.timeTo = TT;
            ViewBag.dateFrom = df_date;
            ViewBag.dateTo = dt_date;
            return View(dtabc);
        }

        public List<StorageTankTableData> getStorageTankTableList(DateTime fromDT, DateTime toDt, string res)
        {
            DataTable dt = new DataTable();
            string query = ";WITH cte AS ( SELECT * FROM ( SELECT DISTINCT r.resourceName AS Location, r.resourceID, p.parameterName AS pID, s.parameterValue AS pVal, s.sheetInsertionDateTime as tim , DATEDIFF(minute, s.sheetInsertionDateTime, DATEADD(hour, 0,GETDATE ())) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = 1085 and sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + fromDT + "', 103), 121) and sheetInsertionDateTime <= CONVERT(CHAR(24), CONVERT(DATETIME, '" + toDt + "', 103), 121)  ) AS SourceTable PIVOT ( SUM(pVal) FOR pID IN ( [V1N (V)],[V2N (V)],[V3N (V)],[I1 (A)],[I2 (A)],[I3 (A)],[W (kwatt)],[VAR (kvar)],[VA (kva)],[VA-SUM (kva)],[PF],[Freq (Hz)],[V12 (v)],[V23 (v)],[V13 (v)],[V1 THD (%)],[V2 THD (%)],[V3 THD (%)],[P1 Status],[P2 Status],[P3 Status],[P4 Status],[P1 Auto/Mannual],[P2 Auto/Mannual],[P3 Auto/Mannual],[P4 Auto/Mannual],[Tank Level1 (ft)] ) ) AS PivotTable ) SELECT * FROM cte order by cast(resourceID as INT) ASC, tim DESC ";
            using (SqlConnection con1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                con1.Open();
                SqlCommand cmd = new SqlCommand(query);
                cmd.Connection = con1;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }
            if (dt.Rows.Count == 1)
            {

            }
            var abc = getPump1WorkingHours(dt);
            var bca = getPump2WorkingHours(dt);
            var pump2Data = getPumpWorkingHours(dt,2);
            var pump3Data = getPumpWorkingHours(dt,3);
            var pump4Data = getPumpWorkingHours(dt, 4);
            //var storageList = new List<StorageTankTableData>();
            //ViewBag.Pump1WorkingHours = abc.workingHoursTodayP1;
            ViewBag.Pump2WorkingHours = bca.workingHoursTodayP2;
            abc.workingHoursTodayP2 = bca.workingHoursTodayP2;
            abc.pumpStatus2 = bca.pumpStatus2;
            abc.P3Status = pump3Data.pumpStatus1;
            abc.workingHoursTodayP3 = pump3Data.workingHoursTodayP1;
            abc.P4Status = pump4Data.pumpStatus1;
            abc.workingHoursTodayP4 = pump4Data.workingHoursTodayP1;

            abc.workingHoursTodayManualP2 = pump2Data.workingHoursTodayManualP1;
            abc.workingHoursTodayRemoteP2 = pump2Data.workingHoursTodayRemoteP1;
            abc.workingHoursTodaySchedulingP2 = pump2Data.workingHoursTodaySchedulingP1;

            abc.workingHoursTodayManualP3 = pump3Data.workingHoursTodayManualP1;
            abc.workingHoursTodayRemoteP3 = pump3Data.workingHoursTodayRemoteP1;
            abc.workingHoursTodaySchedulingP3 = pump3Data.workingHoursTodaySchedulingP1;

            abc.workingHoursTodayManualP4 = pump4Data.workingHoursTodayManualP1;
            abc.workingHoursTodayRemoteP4 = pump4Data.workingHoursTodayRemoteP1;
            abc.workingHoursTodaySchedulingP4 = pump4Data.workingHoursTodaySchedulingP1;

            abc.P1WorkingInHours = abc.P1WorkingInHours;
            abc.P2WorkingInHours = bca.P2WorkingInHours;
            abc.P3WorkingInHours = pump3Data.P3WorkingInHours;
            abc.P4WorkingInHours = pump4Data.P4WorkingInHours;
            var tablList = new List<PondingSpellClass>();
            var abclist = new List<StorageTankTableData>();
            if (dt.Rows.Count > 0)
            {
                var tablList2 = getAllSpells(dt, Convert.ToInt32(dt.Rows[0]["resourceID"]), fromDT);
                //PondingSpellClass sd = getAllSpells(Dashdt, Convert.ToInt32(drRes["resourceNumber"]));
                //tablList.Add(sd);
                tablList.AddRange(tablList2);

                double maxxxx = tablList[0].SpellDataArray.Max();
                int maxIndex = tablList[0].SpellDataArray.ToList().IndexOf(maxxxx);
                string timeOfMaxxxx = tablList[0].SpellTimeArray[maxIndex].ToString();

                double minsMaxToEnd = Convert.ToDouble(tablList[0].NoPondingComment);
                double levelAtStart = Convert.ToDouble(tablList[0].minThr);
                double minsStartToMax = Convert.ToDouble(tablList[0].maxThr);
                double levelAtEnd = Convert.ToDouble(tablList[0].currLevel);
                double MaxLevel = tablList[0].SpellMax;
                double spellRateUp = 0;
                double spellRateDown = 0;
                if (MaxLevel == levelAtStart)
                {
                    spellRateUp = 0;
                }
                else
                {
                    spellRateUp = Math.Round((MaxLevel - levelAtStart) / minsStartToMax, 5);
                }
                if (MaxLevel == levelAtEnd)
                {
                    spellRateDown = 0;
                }
                else
                {
                    spellRateDown = Math.Round((MaxLevel - levelAtEnd) / minsMaxToEnd, 5);
                }

                //double waterIncreaseInFt = spellRateUp * minsStartToMax;
                //double waterDecreaseInFt = spellRateDown * minsMaxToEnd;

                double waterIncreaseInFt = MaxLevel - levelAtStart;
                double waterDecreaseInFt = MaxLevel - levelAtEnd;

                double waterInGallon = waterIncreaseInFt * 89695.23;
                double waterInCubFt = waterIncreaseInFt * 14400;
                double waterInLit = waterIncreaseInFt * 407762.6;

                double waterOutGallon = waterDecreaseInFt * 89695.23;
                double waterOutCubFt = waterDecreaseInFt * 14400;
                double waterOutLit = waterDecreaseInFt * 407762.6;

                double netFlowRate = Math.Round(((levelAtEnd - levelAtStart) / (minsStartToMax + minsMaxToEnd)), 5);


                ViewData["waterInGallon"] = Math.Round(waterIncreaseInFt * 89695.23, 2);
                ViewData["waterInCubFt"] = Math.Round(waterIncreaseInFt * 14400);
                ViewData["waterInLit"] = Math.Round(waterIncreaseInFt * 407762.6);

                ViewData["waterOutGallon"] = Math.Round(waterDecreaseInFt * 89695.23);
                ViewData["waterOutCubFt"] = Math.Round(waterDecreaseInFt * 14400);
                ViewData["waterOutLit"] = Math.Round(waterDecreaseInFt * 407762.6);

                Decimal RateUP = 0;
                Decimal.TryParse(spellRateUp.ToString(), out RateUP);

                Decimal RateDown = 0;
                Decimal.TryParse(spellRateDown.ToString(), out RateDown);


                Decimal netRate = 0;
                Decimal.TryParse(netFlowRate.ToString(), out netRate);
                string netRateString = "";
                string rateUpString = "";
                string rateDownString = "";
                if (netRate == 0)
                {
                    netRateString = "≈ 0";
                }
                else
                {
                    netRateString = netRate.ToString();
                }

                if (RateUP == 0)
                {
                    rateUpString = "≈ 0";
                }
                else
                {
                    rateUpString = RateUP.ToString();
                }

                if (RateDown == 0)
                {
                    rateDownString = "≈ 0";
                }
                else
                {
                    rateDownString = RateDown.ToString();
                }

                ViewData["NetFlowRate"] = netRateString;
                ViewData["SpellRateUp"] = rateUpString;
                ViewData["SpellRateDown"] = rateDownString;


                string printTitle = "Storm Water Storage Tank at " + dt.Rows[0]["Location"].ToString() + " Water In/Out Report between " + fromDT + " to " + toDt + "";
                Session["ReportTitle"] = printTitle;
            }
            
            abclist.Add(abc);
            return abclist;
        }

        public List<PondingSpellClass> getAllSpells(DataTable dt, int order, DateTime td)
        {
            var tableData = new PondingSpellClass();
            var spelldata = new SpellData();
            //int resourceID = Convert.ToInt32(dt.Rows[0]["resourceID"]);
            string location = dt.Rows[0]["Location"].ToString();
            double currentPonding = Math.Round((Convert.ToDouble(dt.Rows[0]["Tank Level1 (ft)"]) / 1), 2);
            string currentTime = dt.Rows[0]["tim"].ToString();
            double minutes = (Convert.ToDateTime(currentTime) - DateTime.Now.AddHours(10)).TotalMinutes;
            double DeltaMinutes = Convert.ToDouble(dt.Rows[0]["DeltaMinutes"]);
            bool S = false;
            bool E = false;
            bool T = true;
            bool F = false;
            int spell = 0;
            List<PondingSpellClass> tableDataList = new List<PondingSpellClass>();
            List<SpellData> spellDataList = new List<SpellData>();
            foreach (DataRow dr in dt.Rows)
            {
                double currValue = Math.Round((Convert.ToDouble(dr["Tank Level1 (ft)"]) / 1), 2);
                string currTime = dr["tim"].ToString();
                string clearaceTime = "";
                //start scenario 3 (inactive)
                if (DeltaMinutes > 28800)
                {

                }
                // end  scenario 3 (inactive)
                else
                {
                    //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                    if (currentPonding < 1)
                    {
                        if (E == F && S == F)
                        {
                            if (currValue < 1.19)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    E = T;
                                    S = T;
                                    spelldata.SpellDataArray.Add(lastvalue);
                                    spelldata.SpellTimeArray.Add(lastTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                }

                            }
                            else
                            {
                                E = T;
                                spell = spell + 1;
                                spelldata.SpellNumber = spell;
                                spelldata.SpellDataArray.Add(currValue);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                                clearaceTime = currTime;

                            }
                        }
                        else if (E == T && S == F)
                        {
                            if (currValue < 1.19 || dr == dt.Rows[dt.Rows.Count - 1])
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                spelldata.SpellStartTime = currTime;
                                S = T;
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                spelldata.SpellDataArray.Add(currValue);
                                spelldata.SpellTimeArray.Add(currTime);
                            }
                        }
                        if (E == T && S == T)
                        {
                            E = F;
                            S = F;
                            if (spelldata.SpellDataArray.Count > 1 && spelldata.SpellDataArray.Sum() > 0)
                            {
                                int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                                spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                                spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                                if (spelldata.SpellMax == currentPonding)
                                {
                                    spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes);
                                    if (spelldata.spellPeriod == 0)
                                    {
                                        spelldata.spellPeriod = 1;
                                    }
                                    double diffMinutes = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes);
                                    if (diffMinutes == 0)
                                    {
                                        diffMinutes = 1;
                                    }
                                    spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / diffMinutes, 5);
                                    spelldata.spellFlowDown = 0;
                                }
                                else
                                {
                                    spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                    if (spelldata.spellPeriod == 0)
                                    {
                                        spelldata.spellPeriod = 1;
                                    }
                                    double diffMinutes = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes);
                                    if (diffMinutes == 0)
                                    {
                                        diffMinutes = 1;
                                    }
                                    spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / diffMinutes, 5);
                                    spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 5);
                                }
                                //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes),2);
                                if (spelldata.spellPeriod > 1)
                                {
                                    spellDataList.Add(spelldata);
                                    spelldata = new SpellData();
                                    string s = JsonConvert.SerializeObject(spellDataList);
                                }
                            }
                        }
                    }
                    // end  scenario 1 (No Ponding since many time/cleared/ zero received)
                    //////////////////////////////////////////////////////////////////////
                    //start scenario 2 (uncleared/ ponding continues (find out when the ponding is started))
                    else
                    {
                        if (E == F && S == F)
                        {
                            if (currValue < 1.19)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    E = T;
                                    S = T;
                                    spelldata.SpellDataArray.Add(lastvalue);
                                    spelldata.SpellTimeArray.Add(lastTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                }

                            }
                            else
                            {
                                E = T;
                                spell = spell + 1;
                                spelldata.SpellNumber = spell;
                                spelldata.SpellDataArray.Add(currValue);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                                clearaceTime = currTime;

                            }
                        }
                        else if (E == T && S == F)
                        {
                            if (currValue < 1.19 || dr == dt.Rows[dt.Rows.Count - 1])
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                spelldata.SpellStartTime = currTime;
                                S = T;
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                spelldata.SpellDataArray.Add(currValue);
                                spelldata.SpellTimeArray.Add(currTime);
                            }
                        }
                        if (E == T && S == T)
                        {
                            E = F;
                            S = F;
                            if (spelldata.SpellDataArray.Count > 1 && spelldata.SpellDataArray.Sum() > 0)
                            {
                                int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                                spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                                spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                                if (spelldata.SpellMax == currentPonding)
                                {
                                    spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes);
                                    if (spelldata.spellPeriod == 0)
                                    {
                                        spelldata.spellPeriod = 1;
                                    }
                                    double diffMinutes = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes);
                                    if (diffMinutes == 0)
                                    {
                                        diffMinutes = 1;
                                    }
                                    spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / diffMinutes, 5);
                                    spelldata.spellFlowDown = 0;
                                }
                                else
                                {
                                    spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                    if (spelldata.spellPeriod == 0)
                                    {
                                        spelldata.spellPeriod = 1;
                                    }
                                    double diffMinutes = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes);
                                    if (diffMinutes == 0)
                                    {
                                        diffMinutes = 1;
                                    }
                                    spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / diffMinutes, 5);
                                    spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 5);
                                }
                                //spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes);
                                //if (spelldata.spellPeriod == 0)
                                //{
                                //    spelldata.spellPeriod = 1;
                                //}
                                //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                if (spelldata.spellPeriod > 1)
                                {
                                    spellDataList.Add(spelldata);
                                    spelldata = new SpellData();
                                    string s = JsonConvert.SerializeObject(spellDataList);
                                }
                                //currentPonding = currValue;
                            }
                        }
                    }
                    // end  scenario 2 (uncleared/ ponding continues)
                }

            }
            string c = JsonConvert.SerializeObject(spellDataList);
            if (spelldata.SpellDataArray.Count == 0)
            {
                spelldata.SpellDataArray.Add(currentPonding);
                spelldata.SpellTimeArray.Add(currentTime);
                spelldata.SpellStartTime = currentTime;
                spelldata.SpellEndTime = currentTime;
            }
            if (/*DeltaMinutes > 1440 || */spelldata.SpellDataArray.Count == 0 || spellDataList.Count == 0)
            {
                tableData.date = td.ToShortDateString();
                tableData.pondingLocation = location;
                tableData.SpellNumber = 0;
                tableData.currLevel = "-";
                tableData.currTime = "-";
                tableData.SpellMax = 0;
                tableData.spellMaxTime = "-";
                tableData.spellFlowUp = "-";
                tableData.spellFlowDown = "-";
                tableData.SpellEndTime = "-";
                tableData.spellPeriod = "-";
                tableData.srNo = order.ToString();
                if (currentPonding < 1 /*&& DeltaMinutes < 2000*/)
                {
                    if (dt.Rows.Count > 1)
                    {
                        tableData.estSpellClearanceTime = "No Ponding";
                        tableData.spellPeriod = "No Ponding";
                        tableData.SpellEndTime = "No Ponding";
                        tableData.comment = "No Ponding";
                        tableData.NoPondingComment = "No Ponding Till " + currentTime + "";
                        tableData.date = Convert.ToDateTime(currentTime).Date.ToShortDateString();
                    }
                    else
                    {
                        tableData.estSpellClearanceTime = "-";
                        tableData.spellPeriod = "-";
                        tableData.SpellEndTime = "-";
                        tableData.comment = "Inactive";
                    }
                }
                else
                {
                    tableData.estSpellClearanceTime = "-";
                    tableData.spellPeriod = "-";
                    tableData.SpellEndTime = "-";
                }
                tableDataList.Add(tableData);
            }
            else
            {
                int spelNo = 0;
                spellDataList.Reverse();
                foreach (var item in spellDataList)
                {
                    int spells = spellDataList.Count;
                    tableData = new PondingSpellClass();
                    tableData.date = Convert.ToDateTime(item.SpellStartTime).Date.ToShortDateString();
                    tableData.pondingLocation = location;
                    tableData.srNo = order.ToString();
                    tableData.currLevel = currentPonding.ToString();
                    tableData.currTime = currentTime;
                    spelNo = spelNo + 1;
                    tableData.SpellNumber = spelNo;
                    tableData.SpellStartTime = item.SpellStartTime;
                    tableData.SpellEndTime = item.SpellEndTime;
                    tableData.SpellDataArray = item.SpellDataArray;
                    tableData.SpellTimeArray = item.SpellTimeArray;
                    tableData.SpellMax = item.SpellMax;
                    tableData.spellMaxTime = item.spellMaxTime;
                    tableData.SpellMin = item.SpellMin;
                    if (Convert.ToDateTime(tableData.SpellEndTime) <= (Convert.ToDateTime(tableData.spellMaxTime)))
                    {
                        tableData.SpellEndTime = Convert.ToDateTime(tableData.spellMaxTime).AddMinutes(1).ToString();
                    }
                    tableData.spellMinTime = item.spellMinTime;
                    tableData.spellFlowDown = Math.Round(item.spellFlowDown, 5).ToString();
                    tableData.spellFlowUp = Math.Round(item.spellFlowUp, 5).ToString();
                    tableData.flowRateUp = Math.Round(item.spellFlowUp, 5).ToString();
                    double pondingPer = Math.Abs((Convert.ToDateTime(tableData.spellMaxTime) - Convert.ToDateTime(tableData.SpellEndTime)).TotalMinutes);
                    TimeSpan pp = TimeSpan.FromMinutes(Convert.ToDouble(pondingPer));
                    int phour = pp.Hours;
                    int pmin = pp.Minutes;
                    int psec = pp.Seconds;
                    string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                    tableData.spellPeriod = pstr;
                    if (currentPonding < 1)
                    {
                        tableData.estSpellClearanceTime = "Cleared";
                        tableData.comment = "Cleared";
                    }
                    else
                    {
                        tableData.comment = "Continue...";
                        //tableData.clearanceTime = "-";
                        if (tableData.SpellDataArray.LastOrDefault() < Convert.ToDouble(tableData.SpellMax))
                        {
                            if (Convert.ToDouble(tableData.spellFlowDown) == 0)
                            {
                                tableData.spellFlowDown = "1";
                            }
                            double minutesToClear = (tableData.SpellDataArray.LastOrDefault() / Convert.ToDouble(tableData.spellFlowDown));
                            TimeSpan runningTime = TimeSpan.FromMinutes(Convert.ToDouble(minutesToClear));
                            int hour = runningTime.Hours;
                            int min = runningTime.Minutes;
                            int sec = runningTime.Seconds;
                            string str = " " + hour.ToString() + " Hours, " + min.ToString() + " Minutes";
                            tableData.estSpellClearanceTime = str;
                        }
                        else
                        {
                            tableData.estSpellClearanceTime = "In Progress";
                        }
                    }
                    if (tableData.SpellDataArray.LastOrDefault() < 1)
                    {
                        tableData.clearanceTime = item.SpellEndTime.ToString();
                    }
                    else
                    {
                        tableData.clearanceTime = "-";
                    }
                    if (DeltaMinutes > 10)
                    {
                        //tableData.estSpellClearanceTime = "-";
                    }
                    if (tableData.SpellDataArray.LastOrDefault() > 1)
                    {
                        //tableData.estSpellClearanceTime = "-";
                    }
                    if (tableData.SpellNumber < spells)
                    {
                        tableData.estSpellClearanceTime = "-";
                        tableData.comment = "Cleared";
                        if (Convert.ToDouble(tableData.spellFlowUp) > 0 && Convert.ToDouble(tableData.spellFlowDown) == 0)
                        {
                            tableData.spellFlowDown = tableData.spellFlowUp;
                        }
                    }
                    else
                    {

                    }
                    if (tableData.estSpellClearanceTime == "0 Hours, 0 Minutes" || tableData.estSpellClearanceTime == "In Progress")
                    {
                        tableData.estSpellClearanceTime = "In Progress";
                        tableData.spellFlowDown = "-";
                        tableData.comment = "In Progress";
                    }
                    if (tableData.comment == "Continue...")
                    {
                        double DiffForSpellDown = Math.Abs(tableData.SpellMax - tableData.SpellDataArray.LastOrDefault());
                        double TimeDifference = Math.Abs(((Convert.ToDateTime(tableData.SpellEndTime)) - (Convert.ToDateTime(tableData.spellMaxTime))).TotalMinutes);
                        if (TimeDifference == 0)
                        {
                            TimeDifference = 1;
                        }
                        tableData.spellFlowDown = Math.Round((DiffForSpellDown / TimeDifference), 5).ToString();
                        tableData.flowRateDown = Math.Round((DiffForSpellDown / TimeDifference), 5).ToString();
                        tableData.SpellEndTime = "-";
                    }
                    if (tableData.spellPeriod != "0 Hours, 1 Minutes")
                    {
                        tableDataList.Add(tableData);
                    }
                    tableData.NoPondingComment = pondingPer.ToString();
                    tableData.minThr = tableData.SpellDataArray.LastOrDefault().ToString();
                    tableData.maxLevelTime = tableData.SpellTimeArray.LastOrDefault().ToString();
                    tableData.maxThr = Math.Abs((Convert.ToDateTime(tableData.spellMaxTime) - Convert.ToDateTime(tableData.maxLevelTime)).TotalMinutes).ToString();
                }
            }
            return tableDataList;
        }

        public string getPump1WorkingHoursManual(DateTime fromDT, DateTime toDt, string res)
        {
            return "";
        }
        public string getPump1WorkingHoursAuto(DateTime fromDT, DateTime toDt, string res)
        {
            return "";
        }
        public string getPump1WorkingHoursRemote(DateTime fromDT, DateTime toDt, string res)
        {
            return "";
        }

        public StorageTankTableData getPumpWorkingHours(DataTable dt,int number)
        {
            var tableData = new StorageTankTableData();
            var spelldata = new StoragePump1SpellData();
            if (dt.Rows.Count > 1)
            {
                string location = dt.Rows[0]["Location"].ToString();
                double currentMotorStatus = Math.Round((Convert.ToDouble(dt.Rows[0]["P"+number+" Status"])), 2);
                string currentTime = dt.Rows[0]["tim"].ToString();
                double DeltaMinutes = Convert.ToDouble(dt.Rows[0]["DeltaMinutes"]);
                bool S = false;
                bool E = false;
                bool T = true;
                bool F = false;
                int spell = 0;
                List<StoragePump1SpellData> spellDataList = new List<StoragePump1SpellData>();
                string curtm = "";
                foreach (DataRow dr in dt.Rows)
                {
                    double currValue = Math.Round((Convert.ToDouble(dr["P" + number + " Status"])), 2);
                    double FlowRate = Math.Round((Convert.ToDouble(dr["Tank Level1 (ft)"])), 2);
                    double currValueManual = Math.Round((Convert.ToDouble(dr["P" + number + " Auto/Mannual"])), 2);
                    string currTime = dr["tim"].ToString();
                    string clearaceTime = "";
                    //start scenario 3 (inactive)
                    if (DeltaMinutes > 28800)
                    {

                    }
                    // end  scenario 3 (inactive)
                    else
                    {
                        //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                        if (currentMotorStatus < 1)
                        {
                            if (E == F && S == F)
                            {
                                if (currValue < 1)
                                {
                                    if (spelldata.SpellDataArray.Count > 0)
                                    {
                                        string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                        double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                        E = T;
                                        S = T;
                                        spelldata.SpellDataArray.Add(lastvalue);
                                        spelldata.SpellTimeArray.Add(lastTime);
                                        spelldata.SpellEndTime = currTime;
                                        if (currValueManual == 0)
                                        {
                                            spelldata.SpellMode = 1;
                                        }
                                        else
                                        {
                                            spelldata.SpellMode = 2;
                                        }
                                        clearaceTime = currTime;
                                    }

                                }
                                else
                                {
                                    E = T;
                                    spell = spell + 1;
                                    spelldata.SpellNumber = spell;
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                    if (currValueManual == 0)
                                    {
                                        spelldata.SpellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.SpellMode = 2;
                                    }
                                }
                            }
                            else if (E == T && S == F)
                            {
                                if (currValue < 1)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = lastTime;
                                        S = T;
                                    }
                                    else
                                    {

                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                }
                                else
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                    else
                                    {
                                        spelldata.SpellDataArray.Add(FlowRate);
                                        spelldata.SpellTimeArray.Add(currTime);
                                    }
                                }
                            }
                            if (E == T && S == T)
                            {
                                E = F;
                                S = F;
                                if (spelldata.SpellDataArray.Count > 0 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                                {
                                    spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                    if (spelldata.SpellPeriod == 0)
                                    {
                                        spelldata.SpellPeriod = 1;
                                    }
                                    spellDataList.Add(spelldata);
                                    spelldata = new StoragePump1SpellData();
                                    string s = JsonConvert.SerializeObject(spellDataList);
                                }
                            }
                        }
                        // end  scenario 1 (No Ponding since many time/cleared/ zero received)
                        //////////////////////////////////////////////////////////////////////
                        //start scenario 2 (uncleared/ ponding continues (find out when the ponding is started))
                        else
                        {
                            if (E == F && S == F)
                            {
                                if (currValue < 1)
                                {
                                    if (spelldata.SpellDataArray.Count > 0)
                                    {
                                        string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                        double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                        E = T;
                                        S = T;
                                        spelldata.SpellDataArray.Add(lastvalue);
                                        spelldata.SpellTimeArray.Add(lastTime);
                                        spelldata.SpellEndTime = currTime;
                                        clearaceTime = currTime;
                                        if (currValueManual == 0)
                                        {
                                            spelldata.SpellMode = 1;
                                        }
                                        else
                                        {
                                            spelldata.SpellMode = 2;
                                        }
                                    }

                                }
                                else
                                {
                                    E = T;
                                    spell = spell + 1;
                                    spelldata.SpellNumber = spell;
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                    if (currValueManual == 0)
                                    {
                                        spelldata.SpellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.SpellMode = 2;
                                    }
                                }
                            }
                            else if (E == T && S == F)
                            {
                                if (currValue < 1)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = lastTime;
                                        S = T;
                                    }
                                    else
                                    {

                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                }
                                else
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                    else
                                    {
                                        spelldata.SpellDataArray.Add(FlowRate);
                                        spelldata.SpellTimeArray.Add(currTime);
                                    }
                                }
                            }
                            if (E == T && S == T)
                            {
                                E = F;
                                S = F;
                                if (spelldata.SpellDataArray.Count > 0 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                                {
                                    //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                                    //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                                    //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                                    spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                    if (spelldata.SpellPeriod == 0)
                                    {
                                        spelldata.SpellPeriod = 1;
                                    }
                                    //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                    //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                    spellDataList.Add(spelldata);
                                    spelldata = new StoragePump1SpellData();
                                    string s = JsonConvert.SerializeObject(spellDataList);
                                }
                            }
                        }
                        // end  scenario 2 (uncleared/ ponding continues)
                    }
                    curtm = currTime;
                    if (dr == dt.Rows[dt.Rows.Count - 1] && currValue > 0)
                    {
                        spelldata.SpellStartTime = currTime;
                        if (spelldata.SpellDataArray.Count > 0 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                        {
                            //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                            //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                            //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                            spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                            if (spelldata.SpellPeriod == 0)
                            {
                                spelldata.SpellPeriod = 1;
                            }
                            //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                            //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                            spellDataList.Add(spelldata);
                            spelldata = new StoragePump1SpellData();
                            string s = JsonConvert.SerializeObject(spellDataList);
                        }
                    }
                }
                if (spellDataList.Count < 1)
                {
                    if (spelldata.SpellDataArray.Count > 0)
                    {
                        spelldata.SpellStartTime = curtm;
                        spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                        if (spelldata.SpellPeriod == 0)
                        {
                            spelldata.SpellPeriod = 1;
                        }
                        spellDataList.Add(spelldata);
                    }
                }
                string c = JsonConvert.SerializeObject(spellDataList);
                if (spelldata.SpellDataArray.Count == 0)
                {
                    spelldata.SpellDataArray.Add(currentMotorStatus);
                    spelldata.SpellTimeArray.Add(currentTime);
                    spelldata.SpellStartTime = currentTime;
                    spelldata.SpellEndTime = currentTime;
                    spelldata.SpellMode = 1;
                }
                if (/*DeltaMinutes > 1440 ||*/ spelldata.SpellDataArray.Count == 0 || spellDataList.Count == 0)
                {
                    tableData.pumpStatus1 = new List<double>();
                    tableData.TankLevel2ft = new List<double>();
                    tableData.SpellTimeArray = new List<string>();
                    tableData.V1N_V = new List<double>();
                    tableData.V2N_V = new List<double>();
                    tableData.V3N_V = new List<double>();
                    tableData.I1A = new List<double>();
                    tableData.I2A = new List<double>();
                    tableData.I3A = new List<double>();
                    tableData.W_kwatt = new List<double>();
                    tableData.VAR_kvar = new List<double>();
                    tableData.VA_kva = new List<double>();
                    tableData.VA_SUM_kva = new List<double>();
                    tableData.PF = new List<double>();
                    tableData.FreqHz = new List<double>();
                    tableData.V12_V = new List<double>();
                    tableData.V23_V = new List<double>();
                    tableData.V13_V = new List<double>();
                    tableData.V1THD = new List<double>();
                    tableData.V2THD = new List<double>();
                    tableData.V3THD = new List<double>();
                    tableData.CurrentTrip1 = new List<double>();
                    tableData.CurrentTrip2 = new List<double>();
                    tableData.VoltageTrip1 = new List<double>();
                    tableData.VoltageTrip2 = new List<double>();
                    tableData.P1AutoMannual = new List<double>();
                    tableData.P2AutoMannual = new List<double>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tableData.locationName = location;
                        tableData.pumpStatus1.Add(Convert.ToInt32(dr["P" + number + " Status"]));
                        tableData.TankLevel2ft.Add(Convert.ToDouble(dr["Tank Level1 (ft)"]));
                        tableData.SpellTimeArray.Add((dr["tim"]).ToString());
                        ///////////////////////////////
                        tableData.V1N_V.Add(Convert.ToDouble(dr["V1N (V)"]));
                        tableData.V2N_V.Add(Convert.ToDouble(dr["V2N (V)"]));
                        tableData.V3N_V.Add(Convert.ToDouble(dr["V3N (V)"]));
                        tableData.I1A.Add(Convert.ToDouble(dr["I1 (A)"]));
                        tableData.I2A.Add(Convert.ToDouble(dr["I2 (A)"]));
                        tableData.I3A.Add(Convert.ToDouble(dr["I3 (A)"]));
                        tableData.W_kwatt.Add(Convert.ToDouble(dr["W (kwatt)"]));
                        tableData.VAR_kvar.Add(Convert.ToDouble(dr["VAR (kvar)"]));
                        tableData.VA_kva.Add(Convert.ToDouble(dr["VA (kva)"]));
                        tableData.VA_SUM_kva.Add(Convert.ToDouble(dr["VA-SUM (kva)"]));
                        tableData.PF.Add(Convert.ToDouble(dr["PF"]));
                        tableData.FreqHz.Add(Convert.ToDouble(dr["Freq (Hz)"]));
                        tableData.V12_V.Add(Convert.ToDouble(dr["V12 (v)"]));
                        tableData.V23_V.Add(Convert.ToDouble(dr["V23 (v)"]));
                        tableData.V13_V.Add(Convert.ToDouble(dr["V13 (v)"]));
                        tableData.V1THD.Add(Convert.ToDouble(dr["V1 THD (%)"]));
                        tableData.V2THD.Add(Convert.ToDouble(dr["V2 THD (%)"]));
                        tableData.V3THD.Add(Convert.ToDouble(dr["V3 THD (%)"]));
                        //tableData.CurrentTrip1.Add(Convert.ToDouble(dr["Current Trip 1"]));
                        //tableData.CurrentTrip2.Add(Convert.ToDouble(dr["Current Trip 2"]));
                        //tableData.VoltageTrip1.Add(Convert.ToDouble(dr["Voltage Trip 1"]));
                        //tableData.VoltageTrip2.Add(Convert.ToDouble(dr["Voltage Trip 2"]));
                        tableData.P1AutoMannual.Add(Convert.ToDouble(dr["P" + number + " Auto/Mannual"]));
                        tableData.P2AutoMannual.Add(Convert.ToDouble(dr["P2 Auto/Mannual"]));
                        tableData.P1AutoMannual.Add(Convert.ToDouble(dr["P3 Auto/Mannual"]));
                        tableData.P2AutoMannual.Add(Convert.ToDouble(dr["P4 Auto/Mannual"]));
                        ///////////////////////////////
                    }
                    var pp = TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.SpellPeriod)));
                    if (number == 3)
                    {
                        tableData.P3WorkingInHours = Math.Round(Convert.ToDouble(pp.TotalMinutes / 60),2);
                    }
                    else if (number == 4)
                    {
                        tableData.P4WorkingInHours = Math.Round(Convert.ToDouble(pp.TotalMinutes / 60),2);
                    }
                    int phour = (int)pp.TotalHours;
                    int pmin = (int)pp.Minutes;
                    int psec = (int)pp.Seconds;
                    string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                    tableData.workingHoursTodayP1 = pstr;
                    double workingInHoursManual = spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod);
                    double workingInHoursAuto = spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod);
                    //tableData.WorkingInHoursPump1 = Math.Round(Convert.ToDouble(TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.spellPeriod))).TotalMinutes) / 60, 2).ToString();
                    //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                    tableData.workingHoursTodayManualP1 = minutesToTime(spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod));
                    tableData.workingHoursTodaySchedulingP1 = minutesToTime(spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod));
                    if (spellDataList.Count == 0)
                    {
                        tableData.tankLevelAverage = "0";
                    }
                    else
                    {
                        double avgWaterFlow = spellDataList.DefaultIfEmpty().Average(x => x.SpellDataArray.DefaultIfEmpty().Average());
                        if (avgWaterFlow == 0)
                        {
                            avgWaterFlow = 1;
                        }
                        tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
                        //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                        //tableData.accWaterDischargePerDay = ((tableData.waterFlow.Sum(x => Convert.ToDouble(x)) / Convert.ToDouble(tableData.workingHoursToday)) * 60).ToString();
                    }
                }
                else
                {
                    tableData.pumpStatus1 = new List<double>();
                    tableData.TankLevel2ft = new List<double>();
                    tableData.SpellTimeArray = new List<string>();
                    tableData.V1N_V = new List<double>();
                    tableData.V2N_V = new List<double>();
                    tableData.V3N_V = new List<double>();
                    tableData.I1A = new List<double>();
                    tableData.I2A = new List<double>();
                    tableData.I3A = new List<double>();
                    tableData.W_kwatt = new List<double>();
                    tableData.VAR_kvar = new List<double>();
                    tableData.VA_kva = new List<double>();
                    tableData.VA_SUM_kva = new List<double>();
                    tableData.PF = new List<double>();
                    tableData.FreqHz = new List<double>();
                    tableData.V12_V = new List<double>();
                    tableData.V23_V = new List<double>();
                    tableData.V13_V = new List<double>();
                    tableData.V1THD = new List<double>();
                    tableData.V2THD = new List<double>();
                    tableData.V3THD = new List<double>();
                    tableData.CurrentTrip1 = new List<double>();
                    tableData.CurrentTrip2 = new List<double>();
                    tableData.VoltageTrip1 = new List<double>();
                    tableData.VoltageTrip2 = new List<double>();
                    tableData.P1AutoMannual = new List<double>();
                    tableData.P2AutoMannual = new List<double>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tableData.locationName = location;
                        tableData.pumpStatus1.Add(Convert.ToInt32(dr["P" + number + " Status"]));
                        tableData.TankLevel2ft.Add(Convert.ToDouble(dr["Tank Level1 (ft)"]));
                        tableData.SpellTimeArray.Add((dr["tim"]).ToString());
                        ///////////////////////////////
                        tableData.V1N_V.Add(Convert.ToDouble(dr["V1N (V)"]));
                        tableData.V2N_V.Add(Convert.ToDouble(dr["V2N (V)"]));
                        tableData.V3N_V.Add(Convert.ToDouble(dr["V3N (V)"]));
                        tableData.I1A.Add(Convert.ToDouble(dr["I1 (A)"]));
                        tableData.I2A.Add(Convert.ToDouble(dr["I2 (A)"]));
                        tableData.I3A.Add(Convert.ToDouble(dr["I3 (A)"]));
                        tableData.W_kwatt.Add(Convert.ToDouble(dr["W (kwatt)"]));
                        tableData.VAR_kvar.Add(Convert.ToDouble(dr["VAR (kvar)"]));
                        tableData.VA_kva.Add(Convert.ToDouble(dr["VA (kva)"]));
                        tableData.VA_SUM_kva.Add(Convert.ToDouble(dr["VA-SUM (kva)"]));
                        tableData.PF.Add(Convert.ToDouble(dr["PF"]));
                        tableData.FreqHz.Add(Convert.ToDouble(dr["Freq (Hz)"]));
                        tableData.V12_V.Add(Convert.ToDouble(dr["V12 (v)"]));
                        tableData.V23_V.Add(Convert.ToDouble(dr["V23 (v)"]));
                        tableData.V13_V.Add(Convert.ToDouble(dr["V13 (v)"]));
                        tableData.V1THD.Add(Convert.ToDouble(dr["V1 THD (%)"]));
                        tableData.V2THD.Add(Convert.ToDouble(dr["V2 THD (%)"]));
                        tableData.V3THD.Add(Convert.ToDouble(dr["V3 THD (%)"]));
                        //tableData.CurrentTrip1.Add(Convert.ToDouble(dr["Current Trip 1"]));
                        //tableData.CurrentTrip2.Add(Convert.ToDouble(dr["Current Trip 2"]));
                        //tableData.VoltageTrip1.Add(Convert.ToDouble(dr["Voltage Trip 1"]));
                        //tableData.VoltageTrip2.Add(Convert.ToDouble(dr["Voltage Trip 2"]));
                        tableData.P1AutoMannual.Add(Convert.ToDouble(dr["P" + number + " Auto/Mannual"]));
                        tableData.P2AutoMannual.Add(Convert.ToDouble(dr["P2 Auto/Mannual"]));
                        tableData.P1AutoMannual.Add(Convert.ToDouble(dr["P3 Auto/Mannual"]));
                        tableData.P2AutoMannual.Add(Convert.ToDouble(dr["P4 Auto/Mannual"]));
                        ///////////////////////////////
                    }
                    var pp = TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.SpellPeriod)));
                    if (number == 3)
                    {
                        tableData.P3WorkingInHours = Math.Round(Convert.ToDouble(pp.TotalMinutes / 60),2);
                    }
                    else if (number == 4)
                    {
                        tableData.P4WorkingInHours = Math.Round(Convert.ToDouble(pp.TotalMinutes / 60),2);
                    }
                    int phour = (int)pp.TotalHours;
                    int pmin = (int)pp.Minutes;
                    int psec = (int)pp.Seconds;
                    string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                    tableData.workingHoursTodayP1 = pstr;
                    double workingInHoursManual = spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod);
                    double workingInHoursAuto = spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod);
                    //tableData.WorkingInHoursPump1 = Math.Round(Convert.ToDouble(TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.spellPeriod))).TotalMinutes) / 60, 2).ToString();
                    //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                    tableData.workingHoursTodayManualP1 = minutesToTime(spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod));
                    tableData.workingHoursTodaySchedulingP1 = minutesToTime(spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod));
                    double avgWaterFlow = spellDataList.Average(x => x.SpellDataArray.Average());
                    tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
                }
                tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
            }
            //tableData.Pump1TimeArray = spellDataList.FirstOrDefault().SpellTimeArray;
            //return tableData;
            return tableData;
        }

        public StorageTankTableData getPump1WorkingHours(DataTable dt)
        {
            var tableData = new StorageTankTableData();
            var spelldata = new StoragePump1SpellData();
            if (dt.Rows.Count > 1)
            {
                dt.Rows.RemoveAt(0);
                string location = dt.Rows[0]["Location"].ToString();
                double currentMotorStatus = Math.Round((Convert.ToDouble(dt.Rows[0]["P1 Status"])), 2);
                string currentTime = dt.Rows[0]["tim"].ToString();
                double DeltaMinutes = Convert.ToDouble(dt.Rows[0]["DeltaMinutes"]);
                bool S = false;
                bool E = false;
                bool T = true;
                bool F = false;
                int spell = 0;
                List<StoragePump1SpellData> spellDataList = new List<StoragePump1SpellData>();
                string curtm = "";
                foreach (DataRow dr in dt.Rows)
                {
                    double currValue = Math.Round((Convert.ToDouble(dr["P1 Status"])), 2);
                    double FlowRate = Math.Round((Convert.ToDouble(dr["Tank Level1 (ft)"])), 2);
                    double currValueManual = Math.Round((Convert.ToDouble(dr["P1 Auto/Mannual"])), 2);
                    string currTime = dr["tim"].ToString();
                    string clearaceTime = "";
                    //start scenario 3 (inactive)
                    if (DeltaMinutes > 28800)
                    {

                    }
                    // end  scenario 3 (inactive)
                    else
                    {
                        //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                        if (currentMotorStatus < 1)
                        {
                            if (E == F && S == F)
                            {
                                if (currValue < 1)
                                {
                                    if (spelldata.SpellDataArray.Count > 0)
                                    {
                                        string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                        double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                        E = T;
                                        S = T;
                                        spelldata.SpellDataArray.Add(lastvalue);
                                        spelldata.SpellTimeArray.Add(lastTime);
                                        spelldata.SpellEndTime = currTime;
                                        if (currValueManual == 0)
                                        {
                                            spelldata.SpellMode = 1;
                                        }
                                        else
                                        {
                                            spelldata.SpellMode = 2;
                                        }
                                        clearaceTime = currTime;
                                    }

                                }
                                else
                                {
                                    E = T;
                                    spell = spell + 1;
                                    spelldata.SpellNumber = spell;
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                    if (currValueManual == 0)
                                    {
                                        spelldata.SpellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.SpellMode = 2;
                                    }
                                }
                            }
                            else if (E == T && S == F)
                            {
                                if (currValue < 1)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = lastTime;
                                        S = T;
                                    }
                                    else
                                    {

                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                }
                                else
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                    else
                                    {
                                        spelldata.SpellDataArray.Add(FlowRate);
                                        spelldata.SpellTimeArray.Add(currTime);
                                    }
                                }
                            }
                            if (E == T && S == T)
                            {
                                E = F;
                                S = F;
                                if (spelldata.SpellDataArray.Count > 0 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                                {
                                    spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                    if (spelldata.SpellPeriod == 0)
                                    {
                                        spelldata.SpellPeriod = 1;
                                    }
                                    spellDataList.Add(spelldata);
                                    spelldata = new StoragePump1SpellData();
                                    string s = JsonConvert.SerializeObject(spellDataList);
                                }
                            }
                        }
                        // end  scenario 1 (No Ponding since many time/cleared/ zero received)
                        //////////////////////////////////////////////////////////////////////
                        //start scenario 2 (uncleared/ ponding continues (find out when the ponding is started))
                        else
                        {
                            if (E == F && S == F)
                            {
                                if (currValue < 1)
                                {
                                    if (spelldata.SpellDataArray.Count > 0)
                                    {
                                        string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                        double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                        E = T;
                                        S = T;
                                        spelldata.SpellDataArray.Add(lastvalue);
                                        spelldata.SpellTimeArray.Add(lastTime);
                                        spelldata.SpellEndTime = currTime;
                                        clearaceTime = currTime;
                                        if (currValueManual == 0)
                                        {
                                            spelldata.SpellMode = 1;
                                        }
                                        else
                                        {
                                            spelldata.SpellMode = 2;
                                        }
                                    }

                                }
                                else
                                {
                                    E = T;
                                    spell = spell + 1;
                                    spelldata.SpellNumber = spell;
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                    if (currValueManual == 0)
                                    {
                                        spelldata.SpellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.SpellMode = 2;
                                    }
                                }
                            }
                            else if (E == T && S == F)
                            {
                                if (currValue < 1)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = lastTime;
                                        S = T;
                                    }
                                    else
                                    {

                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                }
                                else
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                    else
                                    {
                                        spelldata.SpellDataArray.Add(FlowRate);
                                        spelldata.SpellTimeArray.Add(currTime);
                                    }
                                }
                            }
                            if (E == T && S == T)
                            {
                                E = F;
                                S = F;
                                if (spelldata.SpellDataArray.Count > 0 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                                {
                                    //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                                    //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                                    //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                                    spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                    if (spelldata.SpellPeriod == 0)
                                    {
                                        spelldata.SpellPeriod = 1;
                                    }
                                    //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                    //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                    spellDataList.Add(spelldata);
                                    spelldata = new StoragePump1SpellData();
                                    string s = JsonConvert.SerializeObject(spellDataList);
                                }
                            }
                        }
                        // end  scenario 2 (uncleared/ ponding continues)
                    }
                    curtm = currTime;
                    if (dr == dt.Rows[dt.Rows.Count - 1] && currValue > 0)
                    {
                        spelldata.SpellStartTime = currTime;
                        if (spelldata.SpellDataArray.Count > 0 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                        {
                            //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                            //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                            //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                            spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                            if (spelldata.SpellPeriod == 0)
                            {
                                spelldata.SpellPeriod = 1;
                            }
                            //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                            //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                            spellDataList.Add(spelldata);
                            spelldata = new StoragePump1SpellData();
                            string s = JsonConvert.SerializeObject(spellDataList);
                        }
                    }
                }
                if (spellDataList.Count < 1)
                {
                    if (spelldata.SpellDataArray.Count > 0)
                    {
                        spelldata.SpellStartTime = curtm;
                        spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                        if (spelldata.SpellPeriod == 0)
                        {
                            spelldata.SpellPeriod = 1;
                        }
                        spellDataList.Add(spelldata);
                    }
                }
                string c = JsonConvert.SerializeObject(spellDataList);
                if (spelldata.SpellDataArray.Count == 0)
                {
                    spelldata.SpellDataArray.Add(currentMotorStatus);
                    spelldata.SpellTimeArray.Add(currentTime);
                    spelldata.SpellStartTime = currentTime;
                    spelldata.SpellEndTime = currentTime;
                    spelldata.SpellMode = 1;
                }
                if (/*DeltaMinutes > 1440 ||*/ spelldata.SpellDataArray.Count == 0 || spellDataList.Count == 0)
                {
                    tableData.pumpStatus1 = new List<double>();
                    tableData.TankLevel2ft = new List<double>();
                    tableData.SpellTimeArray = new List<string>();
                    tableData.V1N_V = new List<double>();
                    tableData.V2N_V = new List<double>();
                    tableData.V3N_V = new List<double>();
                    tableData.I1A = new List<double>();
                    tableData.I2A = new List<double>();
                    tableData.I3A = new List<double>();
                    tableData.W_kwatt = new List<double>();
                    tableData.VAR_kvar = new List<double>();
                    tableData.VA_kva = new List<double>();
                    tableData.VA_SUM_kva = new List<double>();
                    tableData.PF = new List<double>();
                    tableData.FreqHz = new List<double>();
                    tableData.V12_V = new List<double>();
                    tableData.V23_V = new List<double>();
                    tableData.V13_V = new List<double>();
                    tableData.V1THD = new List<double>();
                    tableData.V2THD = new List<double>();
                    tableData.V3THD = new List<double>();
                    tableData.CurrentTrip1 = new List<double>();
                    tableData.CurrentTrip2 = new List<double>();
                    tableData.VoltageTrip1 = new List<double>();
                    tableData.VoltageTrip2 = new List<double>();
                    tableData.P1AutoMannual = new List<double>();
                    tableData.P2AutoMannual = new List<double>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tableData.locationName = location;
                        tableData.pumpStatus1.Add(Convert.ToInt32(dr["P1 Status"]));
                        tableData.TankLevel2ft.Add(Convert.ToDouble(dr["Tank Level1 (ft)"]));
                        tableData.SpellTimeArray.Add((dr["tim"]).ToString());
                        ///////////////////////////////
                        tableData.V1N_V.Add(Convert.ToDouble(dr["V1N (V)"]));
                        tableData.V2N_V.Add(Convert.ToDouble(dr["V2N (V)"]));
                        tableData.V3N_V.Add(Convert.ToDouble(dr["V3N (V)"]));
                        tableData.I1A.Add(Convert.ToDouble(dr["I1 (A)"]));
                        tableData.I2A.Add(Convert.ToDouble(dr["I2 (A)"]));
                        tableData.I3A.Add(Convert.ToDouble(dr["I3 (A)"]));
                        tableData.W_kwatt.Add(Convert.ToDouble(dr["W (kwatt)"]));
                        tableData.VAR_kvar.Add(Convert.ToDouble(dr["VAR (kvar)"]));
                        tableData.VA_kva.Add(Convert.ToDouble(dr["VA (kva)"]));
                        tableData.VA_SUM_kva.Add(Convert.ToDouble(dr["VA-SUM (kva)"]));
                        tableData.PF.Add(Convert.ToDouble(dr["PF"]));
                        tableData.FreqHz.Add(Convert.ToDouble(dr["Freq (Hz)"]));
                        tableData.V12_V.Add(Convert.ToDouble(dr["V12 (v)"]));
                        tableData.V23_V.Add(Convert.ToDouble(dr["V23 (v)"]));
                        tableData.V13_V.Add(Convert.ToDouble(dr["V13 (v)"]));
                        tableData.V1THD.Add(Convert.ToDouble(dr["V1 THD (%)"]));
                        tableData.V2THD.Add(Convert.ToDouble(dr["V2 THD (%)"]));
                        tableData.V3THD.Add(Convert.ToDouble(dr["V3 THD (%)"]));
                        //tableData.CurrentTrip1.Add(Convert.ToDouble(dr["Current Trip 1"]));
                        //tableData.CurrentTrip2.Add(Convert.ToDouble(dr["Current Trip 2"]));
                        //tableData.VoltageTrip1.Add(Convert.ToDouble(dr["Voltage Trip 1"]));
                        //tableData.VoltageTrip2.Add(Convert.ToDouble(dr["Voltage Trip 2"]));
                        tableData.P1AutoMannual.Add(Convert.ToDouble(dr["P1 Auto/Mannual"]));
                        tableData.P2AutoMannual.Add(Convert.ToDouble(dr["P2 Auto/Mannual"]));
                        tableData.P1AutoMannual.Add(Convert.ToDouble(dr["P3 Auto/Mannual"]));
                        tableData.P2AutoMannual.Add(Convert.ToDouble(dr["P2 Auto/Mannual"]));
                        ///////////////////////////////
                    }
                    var pp = TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.SpellPeriod)));
                    tableData.P1WorkingInHours = Math.Round(Convert.ToDouble(pp.TotalMinutes / 60),2);
                    int phour = (int)pp.TotalHours;
                    int pmin = (int)pp.Minutes;
                    int psec = (int)pp.Seconds;
                    string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                    tableData.workingHoursTodayP1 = pstr;
                    double workingInHoursManual = spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod);
                    double workingInHoursAuto = spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod);
                    //tableData.WorkingInHoursPump1 = Math.Round(Convert.ToDouble(TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.spellPeriod))).TotalMinutes) / 60, 2).ToString();
                    //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                    tableData.workingHoursTodayManualP1 = minutesToTime(spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod));
                    tableData.workingHoursTodaySchedulingP1 = minutesToTime(spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod));
                    if (spellDataList.Count == 0)
                    {
                        tableData.tankLevelAverage = "0";
                    }
                    else
                    {
                        double avgWaterFlow = spellDataList.DefaultIfEmpty().Average(x => x.SpellDataArray.DefaultIfEmpty().Average());
                        if (avgWaterFlow == 0)
                        {
                            avgWaterFlow = 1;
                        }
                        tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
                        //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                        //tableData.accWaterDischargePerDay = ((tableData.waterFlow.Sum(x => Convert.ToDouble(x)) / Convert.ToDouble(tableData.workingHoursToday)) * 60).ToString();
                    }
                }
                else
                {
                    tableData.pumpStatus1 = new List<double>();
                    tableData.TankLevel2ft = new List<double>();
                    tableData.SpellTimeArray = new List<string>();
                    tableData.V1N_V = new List<double>();
                    tableData.V2N_V = new List<double>();
                    tableData.V3N_V = new List<double>();
                    tableData.I1A = new List<double>();
                    tableData.I2A = new List<double>();
                    tableData.I3A = new List<double>();
                    tableData.W_kwatt = new List<double>();
                    tableData.VAR_kvar = new List<double>();
                    tableData.VA_kva = new List<double>();
                    tableData.VA_SUM_kva = new List<double>();
                    tableData.PF = new List<double>();
                    tableData.FreqHz = new List<double>();
                    tableData.V12_V = new List<double>();
                    tableData.V23_V = new List<double>();
                    tableData.V13_V = new List<double>();
                    tableData.V1THD = new List<double>();
                    tableData.V2THD = new List<double>();
                    tableData.V3THD = new List<double>();
                    tableData.CurrentTrip1 = new List<double>();
                    tableData.CurrentTrip2 = new List<double>();
                    tableData.VoltageTrip1 = new List<double>();
                    tableData.VoltageTrip2 = new List<double>();
                    tableData.P1AutoMannual = new List<double>();
                    tableData.P2AutoMannual = new List<double>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tableData.locationName = location;
                        tableData.pumpStatus1.Add(Convert.ToInt32(dr["P1 Status"]));
                        tableData.TankLevel2ft.Add(Convert.ToDouble(dr["Tank Level1 (ft)"]));
                        tableData.SpellTimeArray.Add((dr["tim"]).ToString());
                        ///////////////////////////////
                        tableData.V1N_V.Add(Convert.ToDouble(dr["V1N (V)"]));
                        tableData.V2N_V.Add(Convert.ToDouble(dr["V2N (V)"]));
                        tableData.V3N_V.Add(Convert.ToDouble(dr["V3N (V)"]));
                        tableData.I1A.Add(Convert.ToDouble(dr["I1 (A)"]));
                        tableData.I2A.Add(Convert.ToDouble(dr["I2 (A)"]));
                        tableData.I3A.Add(Convert.ToDouble(dr["I3 (A)"]));
                        tableData.W_kwatt.Add(Convert.ToDouble(dr["W (kwatt)"]));
                        tableData.VAR_kvar.Add(Convert.ToDouble(dr["VAR (kvar)"]));
                        tableData.VA_kva.Add(Convert.ToDouble(dr["VA (kva)"]));
                        tableData.VA_SUM_kva.Add(Convert.ToDouble(dr["VA-SUM (kva)"]));
                        tableData.PF.Add(Convert.ToDouble(dr["PF"]));
                        tableData.FreqHz.Add(Convert.ToDouble(dr["Freq (Hz)"]));
                        tableData.V12_V.Add(Convert.ToDouble(dr["V12 (v)"]));
                        tableData.V23_V.Add(Convert.ToDouble(dr["V23 (v)"]));
                        tableData.V13_V.Add(Convert.ToDouble(dr["V13 (v)"]));
                        tableData.V1THD.Add(Convert.ToDouble(dr["V1 THD (%)"]));
                        tableData.V2THD.Add(Convert.ToDouble(dr["V2 THD (%)"]));
                        tableData.V3THD.Add(Convert.ToDouble(dr["V3 THD (%)"]));
                        //tableData.CurrentTrip1.Add(Convert.ToDouble(dr["Current Trip 1"]));
                        //tableData.CurrentTrip2.Add(Convert.ToDouble(dr["Current Trip 2"]));
                        //tableData.VoltageTrip1.Add(Convert.ToDouble(dr["Voltage Trip 1"]));
                        //tableData.VoltageTrip2.Add(Convert.ToDouble(dr["Voltage Trip 2"]));
                        tableData.P1AutoMannual.Add(Convert.ToDouble(dr["P1 Auto/Mannual"]));
                        tableData.P2AutoMannual.Add(Convert.ToDouble(dr["P2 Auto/Mannual"]));
                        tableData.P1AutoMannual.Add(Convert.ToDouble(dr["P3 Auto/Mannual"]));
                        tableData.P2AutoMannual.Add(Convert.ToDouble(dr["P4 Auto/Mannual"]));
                        ///////////////////////////////
                    }
                    var pp = TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.SpellPeriod)));
                    tableData.P1WorkingInHours = Math.Round(Convert.ToDouble(pp.TotalMinutes / 60),2);
                    int phour = (int)pp.TotalHours;
                    int pmin = (int)pp.Minutes;
                    int psec = (int)pp.Seconds;
                    string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                    tableData.workingHoursTodayP1 = pstr;
                    double workingInHoursManual = spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod);
                    double workingInHoursAuto = spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod);
                    //tableData.WorkingInHoursPump1 = Math.Round(Convert.ToDouble(TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.spellPeriod))).TotalMinutes) / 60, 2).ToString();
                    //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                    tableData.workingHoursTodayManualP1 = minutesToTime(spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod));
                    tableData.workingHoursTodaySchedulingP1 = minutesToTime(spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod));
                    double avgWaterFlow = spellDataList.Average(x => x.SpellDataArray.Average());
                    tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
                }
                tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
            }
            //tableData.Pump1TimeArray = spellDataList.FirstOrDefault().SpellTimeArray;
            //return tableData;
            return tableData;
        }
        public string minutesToTime(double minutes)
        {
            var pTime = TimeSpan.FromMinutes(minutes);
            int phour = (int)pTime.TotalHours;
            int pmin = (int)pTime.Minutes;
            int psec = (int)pTime.Seconds;
            string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
            return pstr;
        }
        public string getPump2WorkingHoursManual(DateTime fromDT, DateTime toDt, string res)
        {
            return "";
        }
        public string getPump2WorkingHoursAuto(DateTime fromDT, DateTime toDt, string res)
        {
            return "";
        }
        public string getPump2WorkingHoursRemote(DateTime fromDT, DateTime toDt, string res)
        {
            return "";
        }
        public StorageTankTableData getPump2WorkingHours(DataTable dt)
        {
            var tableData = new StorageTankTableData();
            var spelldata = new StoragePump2SpellData();
            if (dt.Rows.Count > 1)
            {
                string location = dt.Rows[0]["Location"].ToString();
                double currentMotorStatus = Math.Round((Convert.ToDouble(dt.Rows[0]["P2 Status"])), 2);
                string currentTime = dt.Rows[0]["tim"].ToString();
                double DeltaMinutes = Convert.ToDouble(dt.Rows[0]["DeltaMinutes"]);
                bool S = false;
                bool E = false;
                bool T = true;
                bool F = false;
                int spell = 0;
                List<StoragePump2SpellData> spellDataList = new List<StoragePump2SpellData>();
                string curtm = "";
                foreach (DataRow dr in dt.Rows)
                {
                    double currValue = Math.Round((Convert.ToDouble(dr["P2 Status"])), 2);
                    double FlowRate = Math.Round((Convert.ToDouble(dr["Tank Level1 (ft)"])), 2);
                    double currValueManual = Math.Round((Convert.ToDouble(dr["P2 Auto/Mannual"])), 2);
                    string currTime = dr["tim"].ToString();
                    string clearaceTime = "";
                    //start scenario 3 (inactive)
                    if (DeltaMinutes > 28800)
                    {

                    }
                    // end  scenario 3 (inactive)
                    else
                    {
                        //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                        if (currentMotorStatus < 1)
                        {
                            if (E == F && S == F)
                            {
                                if (currValue < 1)
                                {
                                    if (spelldata.SpellDataArray.Count > 0)
                                    {
                                        string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                        double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                        E = T;
                                        S = T;
                                        spelldata.SpellDataArray.Add(lastvalue);
                                        spelldata.SpellTimeArray.Add(lastTime);
                                        spelldata.SpellEndTime = currTime;
                                        if (currValueManual == 0)
                                        {
                                            spelldata.SpellMode = 1;
                                        }
                                        else
                                        {
                                            spelldata.SpellMode = 2;
                                        }
                                        clearaceTime = currTime;
                                    }

                                }
                                else
                                {
                                    E = T;
                                    spell = spell + 1;
                                    spelldata.SpellNumber = spell;
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                    if (currValueManual == 0)
                                    {
                                        spelldata.SpellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.SpellMode = 2;
                                    }
                                }
                            }
                            else if (E == T && S == F)
                            {
                                if (currValue < 1)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = lastTime;
                                        S = T;
                                    }
                                    else
                                    {

                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                }
                                else
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                    else
                                    {
                                        spelldata.SpellDataArray.Add(FlowRate);
                                        spelldata.SpellTimeArray.Add(currTime);
                                    }
                                }
                            }
                            if (E == T && S == T)
                            {
                                E = F;
                                S = F;
                                if (spelldata.SpellDataArray.Count > 1 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                                {
                                    spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                    if (spelldata.SpellPeriod == 0)
                                    {
                                        spelldata.SpellPeriod = 1;
                                    }
                                    spellDataList.Add(spelldata);
                                    spelldata = new StoragePump2SpellData();
                                    string s = JsonConvert.SerializeObject(spellDataList);
                                }
                            }
                        }
                        // end  scenario 1 (No Ponding since many time/cleared/ zero received)
                        //////////////////////////////////////////////////////////////////////
                        //start scenario 2 (uncleared/ ponding continues (find out when the ponding is started))
                        else
                        {
                            if (E == F && S == F)
                            {
                                if (currValue < 1)
                                {
                                    if (spelldata.SpellDataArray.Count > 0)
                                    {
                                        string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                        double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                        E = T;
                                        S = T;
                                        spelldata.SpellDataArray.Add(lastvalue);
                                        spelldata.SpellTimeArray.Add(lastTime);
                                        spelldata.SpellEndTime = currTime;
                                        clearaceTime = currTime;
                                        if (currValueManual == 0)
                                        {
                                            spelldata.SpellMode = 1;
                                        }
                                        else
                                        {
                                            spelldata.SpellMode = 2;
                                        }
                                    }

                                }
                                else
                                {
                                    E = T;
                                    spell = spell + 1;
                                    spelldata.SpellNumber = spell;
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                    if (currValueManual == 0)
                                    {
                                        spelldata.SpellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.SpellMode = 2;
                                    }
                                }
                            }
                            else if (E == T && S == F)
                            {
                                if (currValue < 1)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = lastTime;
                                        S = T;
                                    }
                                    else
                                    {

                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                }
                                else
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                    {
                                        spelldata.SpellStartTime = currTime;
                                        S = T;
                                    }
                                    else
                                    {
                                        spelldata.SpellDataArray.Add(FlowRate);
                                        spelldata.SpellTimeArray.Add(currTime);
                                    }
                                }
                            }
                            if (E == T && S == T)
                            {
                                E = F;
                                S = F;
                                if (spelldata.SpellDataArray.Count > 1 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                                {
                                    //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                                    //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                                    //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                                    spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                    if (spelldata.SpellPeriod == 0)
                                    {
                                        spelldata.SpellPeriod = 1;
                                    }
                                    //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                    //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                    spellDataList.Add(spelldata);
                                    spelldata = new StoragePump2SpellData();
                                    string s = JsonConvert.SerializeObject(spellDataList);
                                }
                            }
                        }
                        // end  scenario 2 (uncleared/ ponding continues)
                    }
                    curtm = currTime;
                    if (dr == dt.Rows[dt.Rows.Count - 1] && currValue > 0)
                    {
                        spelldata.SpellStartTime = currTime;
                        if (spelldata.SpellDataArray.Count > 0 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                        {
                            //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                            //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                            //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                            spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                            if (spelldata.SpellPeriod == 0)
                            {
                                spelldata.SpellPeriod = 1;
                            }
                            //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                            //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                            spellDataList.Add(spelldata);
                            spelldata = new StoragePump2SpellData();
                            string s = JsonConvert.SerializeObject(spellDataList);
                        }
                    }
                }
                if (spellDataList.Count < 1)
                {
                    if (spelldata.SpellDataArray.Count > 0)
                    {
                        spelldata.SpellStartTime = curtm;
                        spelldata.SpellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                        if (spelldata.SpellPeriod == 0)
                        {
                            spelldata.SpellPeriod = 1;
                        }
                        spellDataList.Add(spelldata);
                    }
                }
                string c = JsonConvert.SerializeObject(spellDataList);
                if (spelldata.SpellDataArray.Count == 0)
                {
                    spelldata.SpellDataArray.Add(currentMotorStatus);
                    spelldata.SpellTimeArray.Add(currentTime);
                    spelldata.SpellStartTime = currentTime;
                    spelldata.SpellEndTime = currentTime;
                    spelldata.SpellMode = 1;
                }
                if (/*DeltaMinutes > 1440 ||*/ spelldata.SpellDataArray.Count == 0 || spellDataList.Count == 0)
                {
                    tableData.pumpStatus2 = new List<double>();
                    //tableData.PumpStatus2 = new List<double>();
                    //tableData.PumpStatus3 = new List<double>();
                    //tableData.PumpStatus4 = new List<double>();
                    //tableData.PumpStatus5 = new List<double>();
                    //tableData.PumpStatus6 = new List<double>();
                    //tableData.PumpStatus7 = new List<double>();
                    //tableData.PumpStatus8 = new List<double>();
                    //tableData.PumpStatus9 = new List<double>();
                    //tableData.PumpStatus10 = new List<double>();
                    tableData.TankLevel2ft = new List<double>();
                    tableData.SpellTimeArray = new List<string>();
                    //tableData.Well2Level = new List<double>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tableData.locationName = location;
                        tableData.pumpStatus2.Add(Convert.ToInt32(dr["P2 Status"]));
                        tableData.TankLevel2ft.Add(Convert.ToDouble(dr["Tank Level1 (ft)"]));
                        tableData.SpellTimeArray.Add((dr["tim"]).ToString());
                    }
                    var pp = TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.SpellPeriod)));
                    tableData.P2WorkingInHours = Math.Round(Convert.ToDouble(pp.TotalMinutes / 60),2);
                    int phour = (int)pp.TotalHours;
                    int pmin = (int)pp.Minutes;
                    int psec = (int)pp.Seconds;
                    string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                    tableData.workingHoursTodayP2 = pstr;
                    double workingInHoursManual = spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod);
                    double workingInHoursAuto = spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod);
                    //tableData.WorkingInHoursPump1 = Math.Round(Convert.ToDouble(TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.spellPeriod))).TotalMinutes) / 60, 2).ToString();
                    //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                    tableData.workingHoursTodayManualP2 = minutesToTime(spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod));
                    tableData.workingHoursTodaySchedulingP2 = minutesToTime(spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod));
                    if (spellDataList.Count == 0)
                    {
                        tableData.tankLevelAverage = "0";
                    }
                    else
                    {
                        double avgWaterFlow = spellDataList.DefaultIfEmpty().Average(x => x.SpellDataArray.DefaultIfEmpty().Average());
                        if (avgWaterFlow == 0)
                        {
                            avgWaterFlow = 1;
                        }
                        tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
                        //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                        //tableData.accWaterDischargePerDay = ((tableData.waterFlow.Sum(x => Convert.ToDouble(x)) / Convert.ToDouble(tableData.workingHoursToday)) * 60).ToString();
                    }
                }
                else
                {
                    tableData.pumpStatus2 = new List<double>();
                    //tableData.PumpStatus2 = new List<double>();
                    //tableData.PumpStatus3 = new List<double>();
                    //tableData.PumpStatus4 = new List<double>();
                    //tableData.PumpStatus5 = new List<double>();
                    //tableData.PumpStatus6 = new List<double>();
                    //tableData.PumpStatus7 = new List<double>();
                    //tableData.PumpStatus8 = new List<double>();
                    //tableData.PumpStatus9 = new List<double>();
                    //tableData.PumpStatus10 = new List<double>();
                    tableData.TankLevel2ft = new List<double>();
                    tableData.SpellTimeArray = new List<string>();
                    //tableData.Well2Level = new List<double>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tableData.locationName = location;
                        tableData.pumpStatus2.Add(Convert.ToInt32(dr["P2 Status"]));
                        tableData.TankLevel2ft.Add(Convert.ToDouble(dr["Tank Level1 (ft)"]));
                        tableData.SpellTimeArray.Add((dr["tim"]).ToString());
                    }
                    var pp = TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.SpellPeriod)));
                    tableData.P2WorkingInHours = Math.Round(Convert.ToDouble(pp.TotalMinutes / 60),2);
                    int phour = (int)pp.TotalHours;
                    int pmin = (int)pp.Minutes;
                    int psec = (int)pp.Seconds;
                    string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                    tableData.workingHoursTodayP2 = pstr;
                    double workingInHoursManual = spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod);
                    double workingInHoursAuto = spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod);
                    //tableData.WorkingInHoursPump1 = Math.Round(Convert.ToDouble(TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.spellPeriod))).TotalMinutes) / 60, 2).ToString();
                    //tableData.workingHoursToday = spellDataList.Sum(i => i.spellPeriod).ToString();
                    tableData.workingHoursTodayManualP2 = minutesToTime(spellDataList.Where(r => r.SpellMode == 1).Sum(i => i.SpellPeriod));
                    tableData.workingHoursTodaySchedulingP2 = minutesToTime(spellDataList.Where(r => r.SpellMode == 2).Sum(i => i.SpellPeriod));
                    double avgWaterFlow = spellDataList.Average(x => x.SpellDataArray.Average());
                    tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
                }
                tableData.tankLevelAverage = tableData.TankLevel2ft.DefaultIfEmpty().Average().ToString();
                //tableData.Pump1TimeArray = spellDataList.FirstOrDefault().SpellTimeArray;
                //return tableData;
            }
            return tableData;
        }
        public StorageParameterChartClass getAllSpellsForStorageParameterizedChartPumps(DataTable dt, string ParameterName, DateTime timeFrom, DateTime timeTo, int pumpNumber)
        {
            var tableData = new StorageParameterChartClass();
            var spelldata = new StoragePump1SpellData();
            string pumpNumberName = "";
            string pumpModeName = "";
            string location = dt.Rows[0]["Location"].ToString();
            if (pumpNumber == 1)
            {
                pumpNumberName = "Pump No. 1 Status";
                pumpModeName = "Pump No. 1 Mode";
            }
            else if (pumpNumber == 2)
            {
                pumpNumberName = "Pump No. 2 Status";
                pumpModeName = "Pump No. 2 Mode";
            }
            else if (pumpNumber == 3)
            {
                pumpNumberName = "Pump No. 3 Status";
                pumpModeName = "Pump No. 3 Mode";
            }
            else 
            {
                pumpNumberName = "Pump No. 4 Status";
                pumpModeName = "Pump No. 4 Mode";
            }
            var cms = dt.Rows[0][pumpNumberName];
            double currentMotorStatus = 0;
            if (cms == DBNull.Value)
            {
                currentMotorStatus = 0;
            }
            else
            {
                currentMotorStatus = Math.Round((Convert.ToDouble(dt.Rows[0][pumpNumberName])), 2);
            }
            string currentTime = dt.Rows[0]["tim"].ToString();
            timeFrom = Convert.ToDateTime(dt.Rows[dt.Rows.Count-1]["tim"].ToString());
            timeTo = Convert.ToDateTime(dt.Rows[0]["tim"].ToString());

            bool S = false;
            bool E = false;
            bool T = true;
            bool F = false;
            int spell = 0;
            List<StoragePump1SpellData> spellDataList = new List<StoragePump1SpellData>();
            string curtm = "";
            foreach (DataRow dr in dt.Rows)
            {
                var currV = dr[pumpNumberName];
                var currVR = dr[pumpModeName];
                var wf = dr[ParameterName];
                double currValue = 0;
                double currValueRemote = 0;
                double FlowRate = 0;
                if (currV == DBNull.Value)
                { }
                else
                {
                    currValue = Math.Round((Convert.ToDouble(dr[pumpNumberName])), 2);
                }
                if (currVR == DBNull.Value)
                { }
                else
                {
                    currValueRemote = Math.Round((Convert.ToDouble(dr[pumpModeName])), 2);
                }
                if (wf == DBNull.Value)
                { }
                else
                {
                    FlowRate = Math.Round((Convert.ToDouble(dr[ParameterName])), 2);
                }
                string currTime = dr["tim"].ToString();
                string clearaceTime = "";
                //start scenario 3 (inactive)
                if (0 > 1)
                {

                }
                // end  scenario 3 (inactive)
                else
                {
                    //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                    if (currentMotorStatus < 1)
                    {
                        if (E == F && S == F)
                        {
                            if (currValue < 1)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    E = T;
                                    S = T;
                                    spelldata.SpellDataArray.Add(lastvalue);
                                    spelldata.SpellTimeArray.Add(lastTime);
                                    spelldata.SpellEndTime = currTime;
                                    if (currValueRemote == 0 )
                                    {
                                        spelldata.spellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.spellMode = 2;
                                    }
                                    clearaceTime = currTime;
                                }

                            }
                            else
                            {
                                E = T;
                                spell = spell + 1;
                                spelldata.SpellNumber = spell;
                                spelldata.SpellDataArray.Add(FlowRate);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                                if (currValueRemote == 0)
                                {
                                    spelldata.spellMode = 1;
                                }
                                else
                                {
                                    spelldata.spellMode = 2;
                                }
                                clearaceTime = currTime;

                            }
                        }
                        else if (E == T && S == F)
                        {
                            if (currValue < 1 || dr == dt.Rows[dt.Rows.Count - 1])
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = lastTime;
                                    S = T;
                                }
                                else
                                {

                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                                else
                                {
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                }
                            }
                        }
                        if (E == T && S == T)
                        {
                            E = F;
                            S = F;
                            if (spelldata.SpellDataArray.Count > 1 )
                            {
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                spellDataList.Add(spelldata);
                                spelldata = new StoragePump1SpellData();
                            }
                        }
                    }
                    // end  scenario 1 (No Ponding since many time/cleared/ zero received)
                    //////////////////////////////////////////////////////////////////////
                    //start scenario 2 (uncleared/ ponding continues (find out when the ponding is started))
                    else
                    {
                        if (E == F && S == F)
                        {
                            if (currValue < 1)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    E = T;
                                    S = T;
                                    spelldata.SpellDataArray.Add(lastvalue);
                                    spelldata.SpellTimeArray.Add(lastTime);
                                    spelldata.SpellEndTime = currTime;
                                    if (currValueRemote == 0)
                                    {
                                        spelldata.spellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.spellMode = 2;
                                    }
                                    clearaceTime = currTime;
                                }

                            }
                            else
                            {
                                E = T;
                                spell = spell + 1;
                                spelldata.SpellNumber = spell;
                                spelldata.SpellDataArray.Add(FlowRate);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                                if (currValueRemote == 0)
                                {
                                    spelldata.spellMode = 1;
                                }
                                else
                                {
                                    spelldata.spellMode = 2;
                                }
                                clearaceTime = currTime;

                            }
                        }
                        else if (E == T && S == F)
                        {
                            if (currValue < 1 || dr == dt.Rows[dt.Rows.Count - 1])
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = lastTime;
                                    S = T;
                                }
                                else
                                {

                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                                else
                                {
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                }
                            }
                        }
                        if (E == T && S == T)
                        {
                            E = F;
                            S = F;
                            if (spelldata.SpellDataArray.Count > 1 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                            {
                                //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                                //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                                //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                spellDataList.Add(spelldata);
                                spelldata = new StoragePump1SpellData();
                                string s = JsonConvert.SerializeObject(spellDataList);
                            }
                        }
                    }
                    // end  scenario 2 (uncleared/ ponding continues)
                }
                curtm = currTime;
            }
            if (spellDataList.Count < 1)
            {
                if (spelldata.SpellDataArray.Count > 0)
                {
                    spelldata.SpellStartTime = curtm;
                    spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                    if (spelldata.spellPeriod == 0)
                    {
                        spelldata.spellPeriod = 1;
                    }
                    spellDataList.Add(spelldata);
                }
            }
            if (spellDataList.Count == 0)
            {
                tableData.locationName = location;
                tableData.workingInHours = 0;
                tableData.totalHours = 0;
                tableData.availableHours = 0;
                tableData.nonAvailableHours = 0;
                tableData.workingInHoursManual = 0;
                tableData.workingInHoursRemote = 0;
                tableData.workingInHoursScheduling = 0;
                tableData.minValue = 0;
                tableData.maxValue = 0;
                tableData.avgVale = 0;
                tableData.avgOfAvailableHours = 0;
                tableData.avgOfNonAvailableHours = 0;
            }
            if(0==0)
            {
                tableData.locationName = location;
                if (timeTo > DateTime.Now)
                {
                    timeTo = DateTime.Now;
                }
                tableData.totalHours = Math.Round((((timeTo-timeFrom).TotalMinutes)/60),2) ;
                if (tableData.totalHours == 23.98)
                {
                    tableData.totalHours = 24;
                }
                tableData.workingInHoursManual = Math.Round(((spellDataList.Where(r => r.spellMode == 1).Sum(i => i.spellPeriod) )/60), 2);
                tableData.workingInHoursRemote = Math.Round(((spellDataList.Where(r => r.spellMode == 2).Sum(i => i.spellPeriod))/60), 2);
                tableData.workingInHoursScheduling = Math.Round(((spellDataList.Where(r => r.spellMode == 3).Sum(i => i.spellPeriod))/60), 2);
                tableData.workingInHours = Math.Round(((Convert.ToDouble(tableData.workingInHoursManual) +
                Convert.ToDouble(tableData.workingInHoursRemote) +
                Convert.ToDouble(tableData.workingInHoursScheduling))), 2);
                tableData.nonWorkingInHours = Math.Round((tableData.totalHours - tableData.workingInHours), 2);
                tableData.availableHours = Math.Round((getAvailableHours(dt, ParameterName)), 2);
                tableData.nonAvailableHours = Math.Round((tableData.totalHours - tableData.availableHours), 2);
                if (tableData.nonAvailableHours == 0.02 && tableData.availableHours > 1)
                {
                    tableData.nonAvailableHours = 0;
                }
                if (tableData.availableHours >= 23.97)
                {
                    tableData.availableHours = 24;
                }
                double availableSum = 0;
                double unavailableSum = 0;
                int availableCount = 0;
                int unavailableCount = 0;
                List<double> valList = new List<double>();
                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToInt32(dr[pumpNumberName]) == 0)
                    {
                        unavailableSum += Convert.ToDouble(dr[ParameterName]);
                        unavailableCount += 1;
                        valList.Add(Convert.ToDouble(dr[ParameterName]));
                    }
                    else
                    {
                        availableSum += Convert.ToDouble(dr[ParameterName]);
                        availableCount += 1;
                        valList.Add(Convert.ToDouble(dr[ParameterName]));
                    }
                }
                if (availableCount == 0)
                {
                    availableCount = 1;
                }
                if (unavailableCount == 0)
                {
                    unavailableCount = 1;
                }
                tableData.avgOfAvailableHours = Math.Round((availableSum / availableCount), 2);
                tableData.avgOfNonAvailableHours = Math.Round((unavailableSum / unavailableCount), 2);


                if (ParameterName == "WaterFlow(Cusec).")
                {
                    tableData.avgOfAvailableHours = Math.Round((tableData.avgOfAvailableHours/101.94), 2);
                    tableData.avgOfNonAvailableHours = Math.Round((tableData.avgOfNonAvailableHours/101.94), 2);
                }

                tableData.minValue = Math.Round((valList.Min()), 2);
                tableData.maxValue = Math.Round((valList.Max()), 2);
                tableData.avgVale = Math.Round((valList.Average()), 2);

                tableData.totalHoursString = minutesToTime(tableData.totalHours*60);
                tableData.workingInHoursString = minutesToTime(tableData.workingInHours*60);
                tableData.nonWorkingInHoursString = minutesToTime(tableData.nonWorkingInHours*60);
                tableData.workingInHoursRemoteString = minutesToTime(tableData.workingInHoursRemote*60);
                tableData.workingInHoursManualString = minutesToTime(tableData.workingInHoursManual*60);
                tableData.workingInHoursSchedulingString = minutesToTime(tableData.workingInHoursScheduling*60);
                tableData.availableHoursString = minutesToTime(tableData.availableHours*60);
                tableData.nonAvailableHoursString = minutesToTime(tableData.nonAvailableHours*60);
                tableData.parameterName = ParameterName;
                if (tableData.workingInHoursString == "23 Hours, 58 Minutes" || tableData.workingInHoursString == "23 Hours, 57 Minutes") { tableData.workingInHoursString = "24 Hours, 0 Minutes"; }
                if (tableData.totalHoursString == "23 Hours, 58 Minutes" || tableData.totalHoursString == "23 Hours, 57 Minutes") { tableData.totalHoursString = "24 Hours, 0 Minutes"; }
                if (tableData.workingInHoursManualString == "23 Hours, 58 Minutes" || tableData.workingInHoursManualString == "23 Hours, 57 Minutes") { tableData.workingInHoursManualString = "24 Hours, 0 Minutes"; }
                if (tableData.workingInHoursRemoteString == "23 Hours, 58 Minutes" || tableData.workingInHoursRemoteString == "23 Hours, 57 Minutes") { tableData.workingInHoursRemoteString = "24 Hours, 0 Minutes"; }
                if (tableData.availableHoursString == "23 Hours, 58 Minutes" || tableData.availableHoursString == "23 Hours, 57 Minutes") { tableData.availableHoursString = "24 Hours, 0 Minutes"; }
                if (tableData.nonAvailableHoursString == "23 Hours, 58 Minutes" || tableData.nonAvailableHoursString == "23 Hours, 57 Minutes") { tableData.nonAvailableHoursString = "24 Hours, 0 Minutes"; }
                if (tableData.nonWorkingInHoursString == "23 Hours, 58 Minutes" || tableData.nonWorkingInHoursString == "23 Hours, 57 Minutes") { tableData.nonWorkingInHoursString = "24 Hours, 0 Minutes"; }
                if (tableData.workingInHours >= 23.95 && tableData.workingInHours <= 24) { tableData.workingInHours = 24; }
                if (tableData.nonWorkingInHours >= 23.95 && tableData.nonWorkingInHours <= 24) { tableData.nonWorkingInHours = 24; }
                if (tableData.totalHours >= 23.95 && tableData.totalHours <= 24) { tableData.totalHours = 24; }
                if (tableData.workingInHoursManual >= 23.95 && tableData.workingInHoursManual <= 24) { tableData.workingInHoursManual = 24; }
                if (tableData.workingInHoursRemote >= 23.95 && tableData.workingInHoursRemote <= 24) { tableData.workingInHoursRemote = 24; }
                if (tableData.availableHours >= 23.95 && tableData.availableHours <= 24) { tableData.availableHours = 24; }
                if (tableData.nonAvailableHours >= 23.95 && tableData.nonAvailableHours <= 24) { tableData.nonAvailableHours = 24; }
            }
            return tableData;
        }

        public StorageParameterChartClass getAllSpellsForStorageParameterizedChart(DataTable dt, string ParameterName, DateTime timeFrom, DateTime timeTo)
        {
            var tableData = new StorageParameterChartClass();
            var spelldata = new StoragePump1SpellData();
            
            string location = dt.Rows[0]["Location"].ToString();
            var cms1 = dt.Rows[0]["Pump No. 1 Status"];
            var cms2 = dt.Rows[0]["Pump No. 2 Status"];
            var cms3 = dt.Rows[0]["Pump No. 3 Status"];
            var cms4 = dt.Rows[0]["Pump No. 4 Status"];
            double currentMotorStatus = 0;
            double currentMotorStatus1 = 0;
            double currentMotorStatus2 = 0;
            double currentMotorStatus3 = 0;
            double currentMotorStatus4 = 0;
            if (cms1 == DBNull.Value)
            {
                currentMotorStatus1 = 0;
            }
            else
            {
                currentMotorStatus1 = Math.Round((Convert.ToDouble(dt.Rows[0]["Pump No. 1 Status"])), 2);
            }
            if (cms2 == DBNull.Value)
            {
                currentMotorStatus2 = 0;
            }
            else
            {
                currentMotorStatus2 = Math.Round((Convert.ToDouble(dt.Rows[0]["Pump No. 2 Status"])), 2);
            }
            if (cms3 == DBNull.Value)
            {
                currentMotorStatus3 = 0;
            }
            else
            {
                currentMotorStatus3 = Math.Round((Convert.ToDouble(dt.Rows[0]["Pump No. 3 Status"])), 2);
            }
            if (cms4 == DBNull.Value)
            {
                currentMotorStatus4 = 0;
            }
            else
            {
                currentMotorStatus4 = Math.Round((Convert.ToDouble(dt.Rows[0]["Pump No. 4 Status"])), 2);
            }
            currentMotorStatus = currentMotorStatus1 + currentMotorStatus2 + currentMotorStatus3 + currentMotorStatus4;
            string currentTime = dt.Rows[0]["tim"].ToString();
            timeFrom = Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["tim"].ToString());
            timeTo = Convert.ToDateTime(dt.Rows[0]["tim"].ToString());

            bool S = false;
            bool E = false;
            bool T = true;
            bool F = false;
            int spell = 0;
            List<StoragePump1SpellData> spellDataList = new List<StoragePump1SpellData>();
            string curtm = "";
            foreach (DataRow dr in dt.Rows)
            {
                var currV = dr["Pump No. 1 Status"];
                var currVR = dr["Pump No. 1 Mode"];
                var wf = dr[ParameterName];
                double currValue = 0;
                double currValueRemote = 0;
                double FlowRate = 0;
                if (currV == DBNull.Value)
                { }
                else
                {
                    currValue = Math.Round((Convert.ToDouble(dr["Pump No. 1 Status"])), 2) + Math.Round((Convert.ToDouble(dr["Pump No. 2 Status"])), 2) + Math.Round((Convert.ToDouble(dr["Pump No. 3 Status"])), 2) + Math.Round((Convert.ToDouble(dr["Pump No. 4 Status"])), 2);
                }
                if (currVR == DBNull.Value)
                { }
                else
                {
                    currValueRemote = Math.Round((Convert.ToDouble(dr["Pump No. 1 Mode"])), 2);
                }
                if (wf == DBNull.Value)
                { }
                else
                {
                    FlowRate = Math.Round((Convert.ToDouble(dr[ParameterName])), 2);
                }
                string currTime = dr["tim"].ToString();
                string clearaceTime = "";
                //start scenario 3 (inactive)
                if (0 > 1)
                {

                }
                // end  scenario 3 (inactive)
                else
                {
                    //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                    if (currentMotorStatus < 1)
                    {
                        if (E == F && S == F)
                        {
                            if (currValue < 1)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    E = T;
                                    S = T;
                                    spelldata.SpellDataArray.Add(lastvalue);
                                    spelldata.SpellTimeArray.Add(lastTime);
                                    spelldata.SpellEndTime = currTime;
                                    if (currValueRemote == 0)
                                    {
                                        spelldata.spellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.spellMode = 2;
                                    }
                                    clearaceTime = currTime;
                                }

                            }
                            else
                            {
                                E = T;
                                spell = spell + 1;
                                spelldata.SpellNumber = spell;
                                spelldata.SpellDataArray.Add(FlowRate);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                                if (currValueRemote == 0)
                                {
                                    spelldata.spellMode = 1;
                                }
                                else
                                {
                                    spelldata.spellMode = 2;
                                }
                                clearaceTime = currTime;

                            }
                        }
                        else if (E == T && S == F)
                        {
                            if (currValue < 1 || dr == dt.Rows[dt.Rows.Count - 1])
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = lastTime;
                                    S = T;
                                }
                                else
                                {

                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                                else
                                {
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                }
                            }
                        }
                        if (E == T && S == T)
                        {
                            E = F;
                            S = F;
                            if (spelldata.SpellDataArray.Count > 1)
                            {
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                spellDataList.Add(spelldata);
                                spelldata = new StoragePump1SpellData();
                            }
                        }
                    }
                    // end  scenario 1 (No Ponding since many time/cleared/ zero received)
                    //////////////////////////////////////////////////////////////////////
                    //start scenario 2 (uncleared/ ponding continues (find out when the ponding is started))
                    else
                    {
                        if (E == F && S == F)
                        {
                            if (currValue < 1)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    E = T;
                                    S = T;
                                    spelldata.SpellDataArray.Add(lastvalue);
                                    spelldata.SpellTimeArray.Add(lastTime);
                                    spelldata.SpellEndTime = currTime;
                                    if (currValueRemote == 0)
                                    {
                                        spelldata.spellMode = 1;
                                    }
                                    else
                                    {
                                        spelldata.spellMode = 2;
                                    }
                                    clearaceTime = currTime;
                                }

                            }
                            else
                            {
                                E = T;
                                spell = spell + 1;
                                spelldata.SpellNumber = spell;
                                spelldata.SpellDataArray.Add(FlowRate);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                                if (currValueRemote == 0)
                                {
                                    spelldata.spellMode = 1;
                                }
                                else
                                {
                                    spelldata.spellMode = 2;
                                }
                                clearaceTime = currTime;

                            }
                        }
                        else if (E == T && S == F)
                        {
                            if (currValue < 1 || dr == dt.Rows[dt.Rows.Count - 1])
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = lastTime;
                                    S = T;
                                }
                                else
                                {

                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                                else
                                {
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                }
                            }
                        }
                        if (E == T && S == T)
                        {
                            E = F;
                            S = F;
                            if (spelldata.SpellDataArray.Count > 1 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                            {
                                //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                                //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                                //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                spellDataList.Add(spelldata);
                                spelldata = new StoragePump1SpellData();
                                string s = JsonConvert.SerializeObject(spellDataList);
                            }
                        }
                    }
                    // end  scenario 2 (uncleared/ ponding continues)
                }
                curtm = currTime;
            }
            if (spellDataList.Count < 1)
            {
                if (spelldata.SpellDataArray.Count > 0)
                {
                    spelldata.SpellStartTime = curtm;
                    spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                    if (spelldata.spellPeriod == 0)
                    {
                        spelldata.spellPeriod = 1;
                    }
                    spellDataList.Add(spelldata);
                }
            }
            if (spellDataList.Count == 0)
            {
                tableData.locationName = location;
                tableData.workingInHours = 0;
                tableData.totalHours = 0;
                tableData.availableHours = 0;
                tableData.nonAvailableHours = 0;
                tableData.workingInHoursManual = 0;
                tableData.workingInHoursRemote = 0;
                tableData.workingInHoursScheduling = 0;
                tableData.minValue = 0;
                tableData.maxValue = 0;
                tableData.avgVale = 0;
                tableData.avgOfAvailableHours = 0;
                tableData.avgOfNonAvailableHours = 0;
            }
            if(0==0)
            {
                tableData.locationName = location;
                if (timeTo > DateTime.Now)
                {
                    timeTo = DateTime.Now;
                }
                tableData.totalHours = Math.Round((((timeTo - timeFrom).TotalMinutes) / 60), 2);
                if (tableData.totalHours == 23.98)
                {
                    tableData.totalHours = 24;
                }
                tableData.workingInHoursManual = Math.Round(((spellDataList.Where(r => r.spellMode == 1).Sum(i => i.spellPeriod)) / 60), 2);
                tableData.workingInHoursRemote = Math.Round(((spellDataList.Where(r => r.spellMode == 2).Sum(i => i.spellPeriod)) / 60), 2);
                tableData.workingInHoursScheduling = Math.Round(((spellDataList.Where(r => r.spellMode == 3).Sum(i => i.spellPeriod)) / 60), 2);
                tableData.workingInHours = Math.Round(((Convert.ToDouble(tableData.workingInHoursManual) +
                Convert.ToDouble(tableData.workingInHoursRemote) +
                Convert.ToDouble(tableData.workingInHoursScheduling))), 2);
                tableData.nonWorkingInHours = Math.Round((tableData.totalHours - tableData.workingInHours), 2);
                tableData.availableHours = Math.Round((getAvailableHours(dt, ParameterName)), 2);
                tableData.nonAvailableHours = Math.Round((tableData.totalHours - tableData.availableHours), 2);
                if (tableData.nonAvailableHours == 0.02 && tableData.availableHours > 1)
                {
                    tableData.nonAvailableHours = 0;
                }
                if (tableData.availableHours >= 23.97)
                {
                    tableData.availableHours = 24;
                }
                double availableSum = 0;
                double unavailableSum = 0;
                int availableCount = 0;
                int unavailableCount = 0;
                List<double> valList = new List<double>();
                foreach (DataRow dr in dt.Rows)
                {
                    if (Math.Round((Convert.ToDouble(dr["Pump No. 1 Status"])), 2) + Math.Round((Convert.ToDouble(dr["Pump No. 2 Status"])), 2) + Math.Round((Convert.ToDouble(dr["Pump No. 3 Status"])), 2) + Math.Round((Convert.ToDouble(dr["Pump No. 4 Status"])), 2) == 0)
                    {
                        unavailableSum += Convert.ToDouble(dr[ParameterName]);
                        unavailableCount += 1;
                        valList.Add(Convert.ToDouble(dr[ParameterName]));
                    }
                    else
                    {
                        availableSum += Convert.ToDouble(dr[ParameterName]);
                        availableCount += 1;
                        valList.Add(Convert.ToDouble(dr[ParameterName]));
                    }
                }
                if (availableCount == 0)
                {
                    availableCount = 1;
                }
                if (unavailableCount == 0)
                {
                    unavailableCount = 1;
                }
                tableData.avgOfAvailableHours = Math.Round((availableSum / availableCount), 2);
                tableData.avgOfNonAvailableHours = Math.Round((unavailableSum / unavailableCount), 2);


                if (ParameterName == "WaterFlow(Cusec).")
                {
                    tableData.avgOfAvailableHours = Math.Round((tableData.avgOfAvailableHours / 101.94), 2);
                    tableData.avgOfNonAvailableHours = Math.Round((tableData.avgOfNonAvailableHours / 101.94), 2);
                }

                tableData.minValue = Math.Round((valList.Min()), 2);
                tableData.maxValue = Math.Round((valList.Max()), 2);
                tableData.avgVale = Math.Round((valList.Average()), 2);

                tableData.totalHoursString = minutesToTime(tableData.totalHours * 60);
                tableData.workingInHoursString = minutesToTime(tableData.workingInHours * 60);
                tableData.nonWorkingInHoursString = minutesToTime(tableData.nonWorkingInHours * 60);
                tableData.workingInHoursRemoteString = minutesToTime(tableData.workingInHoursRemote * 60);
                tableData.workingInHoursManualString = minutesToTime(tableData.workingInHoursManual * 60);
                tableData.workingInHoursSchedulingString = minutesToTime(tableData.workingInHoursScheduling * 60);
                tableData.availableHoursString = minutesToTime(tableData.availableHours * 60);
                tableData.nonAvailableHoursString = minutesToTime(tableData.nonAvailableHours * 60);
                tableData.parameterName = ParameterName;
            }
            return tableData;
        }

        public double getAvailableHours(DataTable dt, string ParameterName)
        {
            var tableData = new StorageParameterChartClass();
            var spelldata = new StoragePump1SpellData();
            //int resourceID = Convert.ToInt32(dt.Rows[0]["resourceID"]);
            string location = dt.Rows[0]["Location"].ToString();
            double currentMotorStatus = Math.Round((Convert.ToDouble(dt.Rows[0][ParameterName])), 2);
            string currentTime = dt.Rows[0]["tim"].ToString();
            bool S = false;
            bool E = false;
            bool T = true;
            bool F = false;
            int spell = 0;
            List<StoragePump1SpellData> spellDataList = new List<StoragePump1SpellData>();
            string curtm = "";
            foreach (DataRow dr in dt.Rows)
            {
                double currValue = Math.Round((Convert.ToDouble(dr[ParameterName])), 2);
                //double currValueRemote = Math.Round((Convert.ToDouble(dr["Remote."])), 2);
                //double currValueManual = Math.Round((Convert.ToDouble(dr["Manual"])), 2);
                //double currValueScheduling = Math.Round((Convert.ToDouble(dr["TimeSchedule."])), 2);
                double FlowRate = Math.Round((Convert.ToDouble(dr[ParameterName])), 2);
                string currTime = dr["tim"].ToString();
                string clearaceTime = "";
                //start scenario 3 (inactive)
                if (0 > 1)
                {

                }
                // end  scenario 3 (inactive)
                else
                {
                    //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                    if (currentMotorStatus == 0)
                    {
                        if (E == F && S == F)
                        {
                            if (currValue == 0)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    E = T;
                                    S = T;
                                    spelldata.SpellDataArray.Add(lastvalue);
                                    spelldata.SpellTimeArray.Add(lastTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                }

                            }
                            else
                            {
                                E = T;
                                spell = spell + 1;
                                spelldata.SpellNumber = spell;
                                spelldata.SpellDataArray.Add(FlowRate);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                                clearaceTime = currTime;

                            }
                        }
                        else if (E == T && S == F)
                        {
                            if (currValue == 0 || dr == dt.Rows[dt.Rows.Count - 1])
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = lastTime;
                                    S = T;
                                }
                                else
                                {

                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                                else
                                {
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                }
                            }
                        }
                        if (E == T && S == T)
                        {
                            E = F;
                            S = F;
                            if (spelldata.SpellDataArray.Count > 1 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                            {
                                //int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                //int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                //spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                                //spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                                //spelldata.SpellMax = spelldata.SpellDataArray.DefaultIfEmpty().Max();
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                //spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                //spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                spellDataList.Add(spelldata);
                                spelldata = new StoragePump1SpellData();
                                string s = JsonConvert.SerializeObject(spellDataList);
                            }
                        }
                    }
                    // end  scenario 1 (No Ponding since many time/cleared/ zero received)
                    //////////////////////////////////////////////////////////////////////
                    //start scenario 2 (uncleared/ ponding continues (find out when the ponding is started))
                    else
                    {
                        if (E == F && S == F)
                        {
                            if (currValue == 0)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    E = T;
                                    S = T;
                                    spelldata.SpellDataArray.Add(lastvalue);
                                    spelldata.SpellTimeArray.Add(lastTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                }

                            }
                            else
                            {
                                E = T;
                                spell = spell + 1;
                                spelldata.SpellNumber = spell;
                                spelldata.SpellDataArray.Add(FlowRate);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                                clearaceTime = currTime;

                            }
                        }
                        else if (E == T && S == F)
                        {
                            if (currValue == 0 || dr == dt.Rows[dt.Rows.Count - 1])
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = lastTime;
                                    S = T;
                                }
                                else
                                {

                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes > 10)
                                {
                                    spelldata.SpellStartTime = currTime;
                                    S = T;
                                }
                                else
                                {
                                    spelldata.SpellDataArray.Add(FlowRate);
                                    spelldata.SpellTimeArray.Add(currTime);
                                }
                            }
                        }
                        if (E == T && S == T)
                        {
                            E = F;
                            S = F;
                            if (spelldata.SpellDataArray.Count > 1 /*&& spelldata.SpellDataArray.Sum() > 0*/)
                            {
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                spellDataList.Add(spelldata);
                                spelldata = new StoragePump1SpellData();
                                string s = JsonConvert.SerializeObject(spellDataList);
                            }
                        }
                    }
                }
                curtm = currTime;
            }
            if (spellDataList.Count < 1)
            {
                if (spelldata.SpellDataArray.Count > 0)
                {
                    spelldata.SpellStartTime = curtm;
                    spelldata.spellPeriod = Convert.ToDouble(Convert.ToInt32(Math.Abs((Convert.ToDateTime(spelldata.SpellStartTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes)));
                    if (spelldata.spellPeriod == 0)
                    {
                        spelldata.spellPeriod = 1;
                    }
                    spellDataList.Add(spelldata);
                }
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            if (spelldata.SpellDataArray.Count == 0)
            {
                spelldata.SpellDataArray.Add(currentMotorStatus);
                spelldata.SpellTimeArray.Add(currentTime);
                spelldata.SpellStartTime = currentTime;
                spelldata.SpellEndTime = currentTime;
            }
            double availableHours = 0;
            if (spelldata.SpellDataArray.Count == 0 || spellDataList.Count == 0)
            {
                availableHours = 0;
            }
            else
            {
                var pp = TimeSpan.FromMinutes(Convert.ToDouble(spellDataList.Sum(i => i.spellPeriod)));
                availableHours = Convert.ToDouble(pp.TotalMinutes) / 60;
            }
            return availableHours;
        }

    }
}