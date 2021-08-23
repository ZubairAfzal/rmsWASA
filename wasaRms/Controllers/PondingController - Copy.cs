using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using wasaRms.Models;

namespace wasaRms.Controllers
{
    public class PondingController : Controller
    {

        public List<summaryPondingReportTable> getPostSummaryTableList(DateTime FinalTimeFrom, DateTime FinalTimeTo, string resourceName)
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<summaryPondingReportTable>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "select DISTINCT r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points' and s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "' ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the desired set of resources chosen
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " AND s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "') SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        string currLevl = "";
                        string location = "";
                        string punit = "";
                        string isCleared = "";
                        int pondingentrycounter = 0;
                        double pondingsummer = 0;
                        double maxLevel = 0;
                        double minLevel = 0;
                        int spellCounter = 0;
                        string cclt = DateTime.Now.ToString();
                        if (Dashdt.Rows.Count > 0)
                        {
                            cclt = (Dashdt.Rows[Dashdt.Rows.Count - 1]["tim"]).ToString();
                        }
                        DateTime currLevlTime = DateTime.Parse(cclt);
                        DateTime maxLevelTime = DateTime.Parse(cclt);
                        DateTime pondingStartTime = DateTime.Parse(cclt);
                        DateTime pondingEndTime = DateTime.Parse(cclt);
                        DateTime ClearanceTime = DateTime.Parse(cclt);
                        var tableclass = new summaryPondingReportTable();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            if (location == "") //first entry of a resource
                            {
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2).ToString(); //save current level
                                ite = ite + 1; //for serial number
                                punit = dr["pUnit"].ToString(); //unit of data
                                if (Convert.ToDouble(currLevl) <= 0) //consider the cleared location
                                {
                                    isCleared = "Cleared";
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellOn = false;
                                }
                                else //consider the not cleared location
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) / 2.54; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 <= 0) //for second to last entry below the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellStartTime = Convert.ToDateTime(dr["tim"]).ToString(); //time when there is zero reading determines the start of the spell for an onging spell
                                    spellOn = false; // turn off the spell
                                    spellData.SpellMax = spellData.SpellDataArray.Max();
                                    spellData.SpellMin = spellData.SpellDataArray.Min();
                                    int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                                    int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                                    //int indexMax = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(maxind);
                                    spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(minind);
                                    spellData.SpellDataArray.Reverse();
                                    spellData.SpellTimeArray.Reverse();
                                    spellDataList.Add(spellData); //save spell to the list
                                    spellData = new SpellData(); // new instance
                                }
                                if (isCleared == "Cleared")
                                {
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]); //updates the clearance time
                                }
                                if (spellCounter == 0)
                                {
                                    pondingStartTime = Convert.ToDateTime(dr["tim"]);
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellCounter += 1;
                                }
                                //spellCounter += 1;
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) / 2.54;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) / 2.54)
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) / 2.54, 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                        }
                        if (spellData.SpellDataArray.Count > 0)
                        {
                            var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                            spellData.SpellStartTime = spellStartTime;
                            var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                            var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                            var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                            var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                            spellData.spellPeriod = (Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().FirstOrDefault()) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                            if (spellData.spellPeriod == 0)
                            {
                                spellData.spellPeriod = 10;
                            }
                            if (spellLastData > spellAvg)
                            {
                                spellData.spellFlowDown = 0;
                                spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                            }
                            else
                            {
                                spellData.spellFlowUp = 0;
                                spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                            }
                            spellData.SpellMax = spellMax;
                            spellData.SpellMin = spellMin;
                            spellData.SpellAvg = spellAvg;
                            //int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                            //int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                            int indexMax = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            int indexMin = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(indexMax);
                            spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(indexMin);
                            spellData.SpellDataArray.Reverse();
                            spellData.SpellTimeArray.Reverse();
                            spellDataList.Add(spellData);
                        }
                        string stringlist = string.Join(",", spellDataList);
                        double pondingPeriod = Math.Abs(Math.Round((pondingEndTime.Subtract(pondingStartTime).TotalMinutes), 2));
                        double flowRate = 0;
                        if (pondingentrycounter > 0)
                        {
                            flowRate = Math.Round(((pondingsummer / pondingentrycounter) - Convert.ToDouble(currLevl)) / pondingPeriod, 2);
                        }
                        double EstclearanceTime = Math.Round((flowRate * pondingPeriod), 2);
                        tableclass.srNo = ite.ToString();
                        tableclass.pondingPoint = location;
                        tableclass.rainStopTime = FinalTimeFrom.ToString("hh:mm tt");
                        tableclass.reportDate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                        tableclass.reportTime = DateTime.Now.ToString("hh:mm tt");
                        string statusCurrent = "";
                        if (pondingsummer == 0)
                        {
                            statusCurrent = "No Ponding";
                        }
                        else if (Convert.ToDouble(currLevl) == 0 && pondingsummer != 0)
                        {
                            statusCurrent = "Clear at: " + Convert.ToDateTime(ClearanceTime).ToString("hh: mm tt") + "";
                        }
                        else if (Convert.ToDouble(currLevl) != 0 && pondingsummer != 0)
                        {
                            statusCurrent = "Ponding Continued";
                        }
                        tableclass.statusCurrent = statusCurrent;
                        string tctasr = "";
                        if (pondingsummer == 0)
                        {
                            tctasr = "-";
                        }
                        else if (Convert.ToDouble(currLevl) == 0 && pondingsummer != 0)
                        {
                            var diff = Convert.ToDateTime(ClearanceTime).Subtract(Convert.ToDateTime(FinalTimeFrom));
                            var res = "";
                            if (diff.Hours == 0)
                            {
                                res = String.Format("{0} Minutes ", diff.Minutes);
                            }
                            else
                            {
                                res = String.Format("{0} Hours {1} Minutes ", diff.Hours, diff.Minutes);
                            }
                            //tctasr = Convert.ToDateTime(ClearanceTime).Subtract(Convert.ToDateTime(FinalTimeFrom)).TotalHours.ToString();
                            tctasr = res.ToString();
                        }
                        else if (Convert.ToDouble(currLevl) != 0 && pondingsummer != 0)
                        {
                            tctasr = "Ponding Continued";
                        }
                        tableclass.clearanceTime = tctasr;
                        tablList.Add(tableclass);
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            return tablList;
        }
        public List<PondingTableClass> getPostPondingTableList(DateTime FinalTimeFrom, DateTime FinalTimeTo, string resourceName)
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            DataTable Dashdt1 = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<PondingTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "";
                    if (Convert.ToInt32(Session["CompanyID"]) == 1)
                    {
                        getResFromTemp = "select DISTINCT r.resourceID, r.resourceNumber, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points' and s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "' ";
                    }
                    else
                    {
                        getResFromTemp = "select DISTINCT r.resourceID, r.resourceNumber, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points' and s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "'   and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + "  ";
                    }
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the desired set of resources chosen
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        string resourceLocation = drRes["resourceLocationName"].ToString();
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + "  and s.parameterValue >= 0  AND s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "') SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        if (Dashdt.Rows.Count > 0)
                        {
                            PondingTableClass sd = getAllSpellsFiltered(Dashdt, Convert.ToInt32(drRes["resourceNumber"]));
                            tablList.Add(sd);
                        }
                        else
                        {
                            string Dashdtquery1 = "select '" + resourceLocation + "' as Location, 0 as Min_Level, 0 as Current_Level, 'inch' as pUnit, 1 as rnum, 0 as DeltaMinutes, DATEADD(hour,9,GETDATE()) as tim, 1 as rn";
                            SqlCommand cmd1 = new SqlCommand(Dashdtquery1, conn);
                            SqlDataAdapter sda1 = new SqlDataAdapter(Dashdtquery1, conn);
                            Dashdt1.Clear();
                            sda1.Fill(Dashdt1);
                            PondingTableClass sd = getAllSpellsFiltered(Dashdt1, Convert.ToInt32(drRes["resourceNumber"]));
                            tablList.Add(sd);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            return tablList;
        }
        public List<PondingTableClass> getPostPondingTableListOld(DateTime FinalTimeFrom, DateTime FinalTimeTo, string resourceName)
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<PondingTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "select DISTINCT r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points' and s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "' ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the desired set of resources chosen
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " AND s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "') SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        string currLevl = "";
                        string currTim = "";
                        string location = "";
                        string punit = "";
                        string isCleared = "";
                        int pondingentrycounter = 0;
                        double pondingsummer = 0;
                        double maxLevel = 0;
                        double minLevel = 0;
                        int spellCounter = 0;
                        string cclt = DateTime.Now.ToString();
                        if (Dashdt.Rows.Count > 0)
                        {
                            cclt = (Dashdt.Rows[0]["tim"]).ToString();
                        }
                        DateTime currLevlTime = DateTime.Parse(cclt);
                        DateTime maxLevelTime = DateTime.Parse(cclt);
                        DateTime pondingStartTime = DateTime.Parse(cclt);
                        DateTime pondingEndTime = DateTime.Parse(cclt);
                        DateTime ClearanceTime = DateTime.Parse(cclt);
                        var tableclass = new PondingTableClass();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            if (location == "") //first entry of a resource
                            {
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2).ToString(); //save current level
                                currTim = Convert.ToDateTime(dr["tim"]).ToString();
                                ite = ite + 1; //for serial number
                                punit = dr["pUnit"].ToString(); //unit of data
                                if (Convert.ToDouble(currLevl) <= 0) //consider the cleared location
                                {
                                    isCleared = "Cleared";
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellOn = false;
                                }
                                else //consider the not cleared location
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) / 2.54; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 <= 0) //for second to last entry below the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellStartTime = Convert.ToDateTime(dr["tim"]).ToString(); //time when there is zero reading determines the start of the spell for an onging spell
                                    spellOn = false; // turn off the spell
                                    spellData.SpellMax = spellData.SpellDataArray.Max();
                                    spellData.SpellMin = spellData.SpellDataArray.Min();
                                    int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                                    int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                                    //int indexMax = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(maxind);
                                    spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(minind);
                                    spellData.SpellDataArray.Reverse();
                                    spellData.SpellTimeArray.Reverse();
                                    spellDataList.Add(spellData); //save spell to the list
                                    spellData = new SpellData(); // new instance
                                }
                                if (isCleared == "Cleared")
                                {
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]); //updates the clearance time
                                }
                                if (spellCounter == 0)
                                {
                                    pondingStartTime = Convert.ToDateTime(dr["tim"]);
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellCounter += 1;
                                }
                                //spellCounter += 1;
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) / 2.54;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) / 2.54)
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) / 2.54, 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                        }
                        if (spellData.SpellDataArray.Count > 0)
                        {
                            var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                            spellData.SpellStartTime = spellStartTime;
                            var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                            var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                            var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                            var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                            spellData.spellPeriod = (Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().FirstOrDefault()) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                            if (spellData.spellPeriod == 0)
                            {
                                spellData.spellPeriod = 10;
                            }
                            if (spellLastData > spellAvg)
                            {
                                spellData.spellFlowDown = 0;
                                spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                            }
                            else
                            {
                                spellData.spellFlowUp = 0;
                                spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                            }
                            spellData.SpellMax = spellMax;
                            spellData.SpellMin = spellMin;
                            spellData.SpellAvg = spellAvg;
                            //int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                            //int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                            int indexMax = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            int indexMin = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(indexMax);
                            spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(indexMin);
                            spellData.SpellDataArray.Reverse();
                            spellData.SpellTimeArray.Reverse();
                            spellDataList.Add(spellData);
                        }
                        string stringlist = string.Join(",", spellDataList);
                        double pondingPeriod = Math.Abs(Math.Round((pondingEndTime.Subtract(pondingStartTime).TotalMinutes), 2));
                        double flowRate = 0;
                        if (pondingentrycounter > 0)
                        {
                            flowRate = Math.Round(((pondingsummer / pondingentrycounter) - Convert.ToDouble(currLevl)) / pondingPeriod, 2);
                        }
                        double EstclearanceTime = Math.Round((flowRate * pondingPeriod), 2);
                        tableclass.srNo = ite.ToString();
                        tableclass.pondingLocation = location;
                        tableclass.currLevel = currLevl;
                        tableclass.currTime = currTim;
                        tableclass.pUnit = punit;

                        tableclass.maxLevel = maxLevel.ToString();
                        tableclass.avgPonding = Math.Round((Convert.ToDouble(pondingsummer) / Convert.ToDouble(pondingentrycounter)), 2).ToString();
                        tableclass.maxLevelTime = maxLevelTime.ToString();
                        string spell = spellCounter.ToString();
                        if (isCleared == "")
                        {
                            tableclass.clearanceTime = ClearanceTime.ToString();
                            if (flowRate > 0)
                            {
                                tableclass.flowRateUp = Math.Abs(flowRate).ToString();
                                tableclass.flowRateDown = "-";
                            }
                            else
                            {
                                tableclass.flowRateUp = "-";
                                tableclass.flowRateDown = Math.Abs(flowRate).ToString();
                            }
                            tableclass.estClTime = Math.Abs(EstclearanceTime).ToString() + "  minutes";
                            tableclass.pondingPeriod = pondingPeriod.ToString() + "  minutes";
                        }
                        else
                        {
                            tableclass.clearanceTime = isCleared;
                            tableclass.flowRateUp = isCleared;
                            tableclass.flowRateDown = isCleared;
                            tableclass.estClTime = isCleared;
                            tableclass.pondingPeriod = isCleared;
                        }
                        tablList.Add(tableclass);
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            return tablList;
        }
        public List<PondingTableClass> getDefaultPondingTableList()
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            string resourceLocation = "";
            var tablList = new List<PondingTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    //getting resources belonging to ponding points
                    string getResFromTemp = " select r.resourceID, r.resourceNumber, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Ponding Points' order by CAST(resourceNumber as int) ASC ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the ponding points
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //getting resourceID 
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //getting resourceLocation 
                        resourceLocation = drRes["resourceLocationName"].ToString();
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " and s.sheetInsertionDateTime > DATEADD(DAY, -30, GETDATE())) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        double DeltaMinutes = 0;
                        string currLevl = "";
                        string currTim = "";
                        string location = "";
                        string punit = "";
                        string isCleared = "";
                        int pondingentrycounter = 0;
                        double pondingsummer = 0;
                        double maxLevel = 0;
                        double minLevel = 0;
                        int spellCounter = 0;
                        string cclt = DateTime.Now.ToString();
                        if (Dashdt.Rows.Count > 0)
                        {
                            cclt = (Dashdt.Rows[Dashdt.Rows.Count - 1]["tim"]).ToString();
                        }
                        DateTime currLevlTime = DateTime.Parse(cclt);
                        DateTime maxLevelTime = DateTime.Parse(cclt);
                        DateTime pondingStartTime = DateTime.Parse(cclt);
                        DateTime pondingEndTime = DateTime.Parse(cclt);
                        DateTime ClearanceTime = DateTime.Parse(cclt);
                        var tableclass = new PondingTableClass();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        DeltaMinutes = Convert.ToDouble(Dashdt.Rows[0]["DeltaMinutes"]);
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            //DeltaMinutes = Convert.ToDouble(dr["DeltaMinutes"]);
                            if (location == "") //first entry of a resource
                            {
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                spellData.resourceName = resourceLocation;
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2).ToString(); //save current level
                                currTim = Convert.ToDateTime(dr["tim"]).ToString();
                                ite = ite + 1; //for serial number
                                punit = dr["pUnit"].ToString(); //unit of data
                                if (Convert.ToDouble(currLevl) <= 1) //consider the cleared location
                                {
                                    isCleared = "Cleared";
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellOn = false;
                                }
                                else //consider the not cleared location
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) / 2.54; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 <= 0 && spellData.SpellNumber == 1) //for second to last entry below the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.resourceName = resourceLocation;
                                    spellData.SpellStartTime = Convert.ToDateTime(dr["tim"]).ToString(); //time when there is zero reading determines the start of the spell for an onging spell
                                    spellOn = false; // turn off the spell
                                    spellData.SpellMax = spellData.SpellDataArray.Max();
                                    spellData.SpellMin = spellData.SpellDataArray.Min();
                                    int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                                    int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                                    //int indexMax = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(maxind);
                                    spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(minind);
                                    spellData.SpellDataArray.Reverse();
                                    spellData.SpellTimeArray.Reverse();
                                    var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                                    var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                                    var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                                    var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                                    spellData.spellPeriod = (Convert.ToDateTime(spellData.spellMaxTime) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                                    if (spellData.spellPeriod == 0)
                                    {
                                        spellData.spellPeriod = 10;
                                    }
                                    if (spellLastData > spellAvg)
                                    {
                                        spellData.spellFlowDown = 0;
                                        spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                                    }
                                    else
                                    {
                                        spellData.spellFlowUp = 0;
                                        spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                                    }
                                    spellData.SpellMax = spellMax;
                                    spellData.SpellMin = spellMin;
                                    spellData.SpellAvg = spellAvg;
                                    spellDataList.Add(spellData); //save spell to the list
                                    spellData = new SpellData(); // new instance
                                }
                                if (isCleared == "Cleared")
                                {
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]); //updates the clearance time
                                }
                                if (spellCounter == 0)
                                {
                                    pondingStartTime = Convert.ToDateTime(dr["tim"]);
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellCounter += 1;
                                }
                                //spellCounter += 1;
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) / 2.54;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) / 2.54)
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) / 2.54, 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                        }
                        if (spellData.SpellDataArray.Count > 0)
                        {
                            var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                            spellData.SpellStartTime = spellStartTime;
                            //int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                            //int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                            int indexMax = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            int indexMin = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(indexMax);
                            spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(indexMin);
                            var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                            var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                            var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                            var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                            spellData.spellPeriod = (Convert.ToDateTime(spellData.spellMaxTime) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                            if (spellData.spellPeriod == 0)
                            {
                                spellData.spellPeriod = 10;
                            }
                            if (spellLastData > spellAvg)
                            {
                                spellData.spellFlowDown = 0;
                                spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                            }
                            else
                            {
                                spellData.spellFlowUp = 0;
                                spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                            }
                            spellData.SpellMax = spellMax;
                            spellData.SpellMin = spellMin;
                            spellData.SpellAvg = spellAvg;
                            spellData.SpellDataArray.Reverse();
                            spellData.SpellTimeArray.Reverse();
                            spellDataList.Add(spellData);
                        }
                        string stringlist = string.Join(",", spellDataList);
                        //double listmaxspellmax = spellDataList.Max(r=>r.SpellMax);
                        double pondingPeriod = Math.Abs(Math.Round(spellDataList[0].spellPeriod, 2));
                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = jsonSerialiser.Serialize(spellDataList);
                        double flowRate = 0;
                        if (pondingentrycounter > 0)
                        {
                            flowRate = Math.Round(((pondingsummer / pondingentrycounter) - Convert.ToDouble(currLevl)) / pondingPeriod, 2);
                        }
                        double EstclearanceTime = Math.Round((flowRate * pondingPeriod), 2);

                        tableclass.deltaTime = DeltaMinutes.ToString();
                        tableclass.srNo = ite.ToString();
                        tableclass.pondingLocation = location;
                        //if (DeltaMinutes > -520)

                        ///////////////////////////////
                        double ThePrevious = 0;
                        double TheflowRate = 0;
                        string theFlowUp = "";
                        string theFlowDown = "";
                        string TheEstimatT = "";
                        double max = ThePrevious;
                        string theMaxT = "";
                        DateTime ThePreviousT = DateTime.Now;
                        double TheCurrent = Convert.ToDouble(Dashdt.Rows[0]["Current_Level"]);
                        DateTime TheCurrentT = Convert.ToDateTime(Dashdt.Rows[0]["tim"]);
                        if (Dashdt.Rows.Count > 1)
                        {
                            ThePrevious = Convert.ToDouble(Dashdt.Rows[1]["Current_Level"]);
                            ThePreviousT = Convert.ToDateTime(Dashdt.Rows[1]["tim"]);
                            theMaxT = ThePreviousT.ToString();
                            double timeDifference = (ThePreviousT - TheCurrentT).TotalMinutes;
                            if (TheCurrent > max)
                            {
                                max = TheCurrent;
                                theMaxT = TheCurrentT.ToString();
                            }
                            TheflowRate = Math.Round(((TheCurrent - ThePrevious) / timeDifference), 2);
                            if (TheflowRate > 0)
                            {
                                theFlowUp = "-";
                                theFlowDown = Math.Abs(TheflowRate).ToString();
                                //TheEstimatT = Math.Round((TheCurrent / Math.Abs(TheflowRate)), 2).ToString();
                                TheEstimatT = Math.Round((TheCurrent / Math.Abs(spellDataList[dtRes.Rows.IndexOf(drRes)].spellFlowDown + spellDataList[dtRes.Rows.IndexOf(drRes)].spellFlowUp)), 2).ToString();
                            }
                            else
                            {
                                theFlowUp = Math.Abs(TheflowRate).ToString();
                                theFlowDown = "-";
                                TheEstimatT = "In Progress";
                            }

                        }
                        else
                        {

                        }
                        ///////////////////////////////
                        tableclass.currLevel = currLevl;
                        tableclass.currTime = currTim;
                        tableclass.pUnit = punit;
                        //tableclass.avgPonding = Math.Round((Convert.ToDouble(pondingsummer) / Convert.ToDouble(pondingentrycounter)), 2).ToString();
                        max = Math.Round((Convert.ToDouble(max) / 2.54), 2);
                        //tableclass.maxLevel = max.ToString();
                        //tableclass.maxLevel = Math.Round(spellDataList[dtRes.Rows.IndexOf(drRes)].SpellMax, 2).ToString();
                        tableclass.maxLevel = Math.Round(spellDataList.Max(i => i.SpellMax), 2).ToString();
                        //tableclass.avgPonding = spellDataList[dtRes.Rows.IndexOf(drRes)].SpellAvg.ToString();
                        tableclass.avgPonding = Math.Round(spellDataList[dtRes.Rows.IndexOf(drRes)].SpellMax, 2).ToString();
                        //tableclass.maxLevelTime = theMaxT.ToString();
                        //tableclass.maxLevelTime = spellDataList[dtRes.Rows.IndexOf(drRes)].spellMaxTime.ToString();
                        tableclass.maxLevelTime = spellDataList.Max(i => i.spellMaxTime);
                        string spell = spellCounter.ToString();
                        var pondingperiodod = TimeSpan.FromMinutes(pondingPeriod);
                        int hour = pondingperiodod.Hours;
                        int min = pondingperiodod.Minutes;
                        int sec = pondingperiodod.Seconds;
                        string str = " " + hour.ToString() + " Hours, " + min.ToString() + " Minutes";
                        if (isCleared == "")
                        {
                            //tableclass.clearanceTime = ClearanceTime.ToString();
                            //tableclass.clearanceTime = spellDataList[dtRes.Rows.IndexOf(drRes)].SpellStartTime.ToString();
                            tableclass.clearanceTime = "-";
                            //if (flowRate > 0)
                            //{
                            //    tableclass.flowRateUp = Math.Abs(flowRate).ToString();
                            //    tableclass.flowRateDown = "-";
                            //}
                            //else
                            //{
                            //    tableclass.flowRateUp = "-";
                            //    tableclass.flowRateDown = Math.Abs(flowRate).ToString();
                            //}
                            //tableclass.estClTime = Math.Abs(EstclearanceTime).ToString() + "  minutes";
                            //tableclass.flowRateUp = theFlowUp;
                            //tableclass.flowRateUp = spellDataList[dtRes.Rows.IndexOf(drRes)].spellFlowUp.ToString();
                            ////tableclass.flowRateDown = theFlowDown; 
                            //tableclass.flowRateDown = spellDataList[dtRes.Rows.IndexOf(drRes)].spellFlowDown.ToString();
                            //double theFlowRate = Convert.ToDouble(tableclass.flowRateDown) + Convert.ToDouble(tableclass.flowRateUp);
                            //if (theFlowRate >= 0)
                            //{
                            //    tableclass.flowRateDown = "-";
                            //    tableclass.flowRateUp = Math.Round(Math.Abs(theFlowRate), 2).ToString();
                            //}
                            //else
                            //{
                            //    tableclass.flowRateUp = "-";
                            //    tableclass.flowRateDown = Math.Round(Math.Abs(theFlowRate), 2).ToString();
                            //}
                            if (TheEstimatT == "-")
                            {
                                tableclass.estClTime = TheEstimatT;
                            }
                            else
                            {
                                tableclass.estClTime = TheEstimatT + "minutes";
                            }
                            //tableclass.pondingPeriod = pondingPeriod.ToString() + "  minutes";
                            tableclass.pondingPeriod = str;
                            /////////average flow up and down///////////
                            double avgFlUp = Math.Round(Convert.ToDouble(spellDataList[0].SpellMax) / (Convert.ToDateTime(spellDataList[0].spellMaxTime) - Convert.ToDateTime(spellDataList[0].SpellStartTime)).TotalMinutes, 2);
                            double avgFlDn = Math.Round(Convert.ToDouble(spellDataList[0].SpellMax) / (Convert.ToDateTime(spellDataList[0].SpellEndTime) - Convert.ToDateTime(spellDataList[0].spellMaxTime)).TotalMinutes, 2);
                            tableclass.flowRateUp = avgFlUp.ToString();
                            tableclass.flowRateDown = avgFlDn.ToString();
                            ////////////////////
                        }
                        else
                        {
                            tableclass.comment = "Cleared";
                            //tableclass.clearanceTime = isCleared;
                            tableclass.clearanceTime = spellDataList[dtRes.Rows.IndexOf(drRes)].SpellEndTime.ToString();
                            //tableclass.flowRateUp = isCleared;
                            //tableclass.flowRateDown = isCleared;
                            double theFlowRate = Convert.ToDouble(tableclass.flowRateDown) + Convert.ToDouble(tableclass.flowRateUp);
                            if (theFlowRate >= 0)
                            {
                                tableclass.flowRateDown = "0";
                                tableclass.flowRateUp = Math.Round(Math.Abs(theFlowRate), 2).ToString();
                            }
                            else
                            {
                                tableclass.flowRateUp = "0";
                                tableclass.flowRateDown = Math.Round(Math.Abs(theFlowRate), 2).ToString();
                            }
                            tableclass.estClTime = isCleared;
                            //tableclass.pondingPeriod = isCleared;
                            tableclass.pondingPeriod = str;
                            /////////average flow up and down///////////
                            double avgFlUp = Math.Round(Convert.ToDouble(spellDataList[0].SpellMax) / (Convert.ToDateTime(spellDataList[0].spellMaxTime) - Convert.ToDateTime(spellDataList[0].SpellStartTime)).TotalMinutes, 2);
                            double avgFlDn = Math.Round(Convert.ToDouble(spellDataList[0].SpellMax) / (Convert.ToDateTime(spellDataList[0].SpellEndTime) - Convert.ToDateTime(spellDataList[0].spellMaxTime)).TotalMinutes, 2);
                            tableclass.flowRateUp = avgFlUp.ToString();
                            tableclass.flowRateDown = avgFlDn.ToString();
                            ////////////////////
                        }
                        string abc = spellDataList[dtRes.Rows.IndexOf(drRes)].resourceName;
                        tablList.Add(tableclass);
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            string t = JsonConvert.SerializeObject(tablList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            return tablList;
        }

        public List<PondingSpellClass> getPondingTableList()
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            DataTable Dashdt1 = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<PondingSpellClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "";
                    if (Convert.ToInt32(Session["CompanyID"]) == 1)
                    {
                        getResFromTemp = "select DISTINCT r.resourceID, r.resourceNumber, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points' ";
                    }
                    else
                    {
                        getResFromTemp = "select DISTINCT r.resourceID, r.resourceNumber, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points'   and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ";
                    }
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    string resourceLocation = "";
                    int ite = 0;
                    //iterate through the list of resources within the desired set of resources chosen
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //getting resourceID 
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //getting resourceLocation 
                        resourceLocation = drRes["resourceLocationName"].ToString();
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + "  and s.parameterValue >= 0  and s.sheetInsertionDateTime > DATEADD(hour,-15,GETDATE())) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        if (Dashdt.Rows.Count > 0)
                        {
                            var tablList2 = getAllSpells(Dashdt, Convert.ToInt32(drRes["resourceNumber"]));
                            //PondingSpellClass sd = getAllSpells(Dashdt, Convert.ToInt32(drRes["resourceNumber"]));
                            //tablList.Add(sd);
                            tablList.AddRange(tablList2);
                        }
                        else {
                            string Dashdtquery1 = "select '"+ resourceLocation + "' as Location, 0 as Min_Level, 0 as Current_Level, 'inch' as pUnit, 1 as rnum, 0 as DeltaMinutes, DATEADD(hour,9,GETDATE()) as tim, 1 as rn";
                            SqlCommand cmd1 = new SqlCommand(Dashdtquery1, conn);
                            SqlDataAdapter sda1 = new SqlDataAdapter(Dashdtquery1, conn);
                            Dashdt1.Clear();
                            sda1.Fill(Dashdt1);
                            var tablList2 = getAllSpells(Dashdt1, Convert.ToInt32(drRes["resourceNumber"]));
                            //PondingSpellClass sd = getAllSpells(Dashdt1, Convert.ToInt32(drRes["resourceNumber"]));
                            //tablList.Add(sd);
                            tablList.AddRange(tablList2);
                        }

                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(tablList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            return tablList;
        }

        public List<PondingTableClass> getPondingTableListOld()
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<PondingTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "select DISTINCT r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points' ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the desired set of resources chosen
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " ) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        double DeltaMinutes = 0;
                        string currLevl = "";
                        string currTim = "";
                        string location = "";
                        string punit = "";
                        string isCleared = "";
                        int pondingentrycounter = 0;
                        double pondingsummer = 0;
                        double maxLevel = 0;
                        double minLevel = 0;
                        int spellCounter = 0;
                        string cclt = DateTime.Now.ToString();
                        if (Dashdt.Rows.Count > 0)
                        {
                            cclt = (Dashdt.Rows[Dashdt.Rows.Count - 1]["tim"]).ToString();
                        }
                        DateTime currLevlTime = DateTime.Parse(cclt);
                        DateTime maxLevelTime = DateTime.Parse(cclt);
                        DateTime pondingStartTime = DateTime.Parse(cclt);
                        DateTime pondingEndTime = DateTime.Parse(cclt);
                        DateTime ClearanceTime = DateTime.Parse(cclt);
                        var tableclass = new PondingTableClass();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        DeltaMinutes = Convert.ToDouble(Dashdt.Rows[0]["DeltaMinutes"]);
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            //DeltaMinutes = Convert.ToDouble(dr["DeltaMinutes"]);
                            if (location == "") //first entry of a resource
                            {
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2).ToString(); //save current level
                                currTim = Convert.ToDateTime(dr["tim"]).ToString();
                                ite = ite + 1; //for serial number
                                punit = dr["pUnit"].ToString(); //unit of data
                                if (Convert.ToDouble(currLevl) <= 1) //consider the cleared location
                                {
                                    isCleared = "Cleared";
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellOn = false;
                                }
                                else //consider the not cleared location
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) / 2.54; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 <= 0) //for second to last entry below the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellStartTime = Convert.ToDateTime(dr["tim"]).ToString(); //time when there is zero reading determines the start of the spell for an onging spell
                                    spellOn = false; // turn off the spell
                                    spellData.SpellMax = spellData.SpellDataArray.Max();
                                    spellData.SpellMin = spellData.SpellDataArray.Min();
                                    int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                                    int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                                    //int indexMax = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(maxind);
                                    spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(minind);
                                    spellData.SpellDataArray.Reverse();
                                    spellData.SpellTimeArray.Reverse();
                                    spellDataList.Add(spellData); //save spell to the list
                                    spellData = new SpellData(); // new instance
                                }
                                if (isCleared == "Cleared")
                                {
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]); //updates the clearance time
                                }
                                if (spellCounter == 0)
                                {
                                    pondingStartTime = Convert.ToDateTime(dr["tim"]);
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellCounter += 1;
                                }
                                //spellCounter += 1;
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) / 2.54;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) / 2.54)
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) / 2.54, 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                        }
                        if (spellData.SpellDataArray.Count > 0)
                        {
                            var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                            spellData.SpellStartTime = spellStartTime;
                            var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                            var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                            var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                            var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                            spellData.spellPeriod = (Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().FirstOrDefault()) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                            if (spellData.spellPeriod == 0)
                            {
                                spellData.spellPeriod = 10;
                            }
                            if (spellLastData > spellAvg)
                            {
                                spellData.spellFlowDown = 0;
                                spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                            }
                            else
                            {
                                spellData.spellFlowUp = 0;
                                spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                            }
                            spellData.SpellMax = spellMax;
                            spellData.SpellMin = spellMin;
                            spellData.SpellAvg = spellAvg;
                            //int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                            //int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                            int indexMax = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            int indexMin = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(indexMax);
                            spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(indexMin);
                            spellData.SpellDataArray.Reverse();
                            spellData.SpellTimeArray.Reverse();
                            spellDataList.Add(spellData);
                        }
                        string stringlist = string.Join(",", spellDataList);
                        double pondingPeriod = Math.Abs(Math.Round((pondingEndTime.Subtract(pondingStartTime).TotalMinutes), 2));
                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = jsonSerialiser.Serialize(spellDataList);
                        double flowRate = 0;
                        if (pondingentrycounter > 0)
                        {
                            flowRate = Math.Round(((pondingsummer / pondingentrycounter) - Convert.ToDouble(currLevl)) / pondingPeriod, 2);
                        }
                        double EstclearanceTime = Math.Round((flowRate * pondingPeriod), 2);

                        tableclass.deltaTime = DeltaMinutes.ToString();
                        tableclass.srNo = ite.ToString();
                        tableclass.pondingLocation = location;
                        if (DeltaMinutes > 5000)
                        {
                            tableclass.currLevel = "-";
                            tableclass.currTime = "-";
                            tableclass.pUnit = "";
                            tableclass.avgPonding = "-";
                            tableclass.maxLevel = "-";
                            tableclass.maxLevelTime = "-";
                            tableclass.comment = "Inactive";
                            string spell = spellCounter.ToString();
                            if (isCleared == "")
                            {
                                tableclass.clearanceTime = "-";
                                if (flowRate > 0)
                                {
                                    tableclass.flowRateUp = "-";
                                    tableclass.flowRateDown = "-";
                                }
                                else
                                {
                                    tableclass.flowRateUp = "-";
                                    tableclass.flowRateDown = "-";
                                }
                                tableclass.estClTime = "-";
                                tableclass.pondingPeriod = "-";
                            }
                            else
                            {
                                tableclass.clearanceTime = "-";
                                tableclass.flowRateUp = "-";
                                tableclass.flowRateDown = "-";
                                tableclass.estClTime = "-";
                                tableclass.pondingPeriod = "-";
                            }
                        }
                        else
                        {
                            ///////////////////////////////
                            double ThePrevious = 0;
                            double TheflowRate = 0;
                            string theFlowUp = "";
                            string theFlowDown = "";
                            string TheEstimatT = "";
                            double max = ThePrevious;
                            string theMaxT = "";
                            DateTime ThePreviousT = DateTime.Now;
                            double TheCurrent = Convert.ToDouble(Dashdt.Rows[0]["Current_Level"]);
                            DateTime TheCurrentT = Convert.ToDateTime(Dashdt.Rows[0]["tim"]);
                            if (Dashdt.Rows.Count > 1)
                            {
                                ThePrevious = Convert.ToDouble(Dashdt.Rows[1]["Current_Level"]);
                                ThePreviousT = Convert.ToDateTime(Dashdt.Rows[1]["tim"]);
                                theMaxT = ThePreviousT.ToString();
                                double timeDifference = (ThePreviousT - TheCurrentT).TotalMinutes;
                                if (TheCurrent > max)
                                {
                                    max = TheCurrent;
                                    theMaxT = TheCurrentT.ToString();
                                }
                                TheflowRate = Math.Round(((TheCurrent - ThePrevious) / timeDifference), 2);
                                if (TheflowRate > 0)
                                {
                                    theFlowUp = "-";
                                    theFlowDown = Math.Abs(TheflowRate).ToString();
                                    TheEstimatT = Math.Round((TheCurrent / Math.Abs(TheflowRate)), 2).ToString();
                                }
                                else
                                {
                                    theFlowUp = Math.Abs(TheflowRate).ToString();
                                    theFlowDown = "-";
                                    TheEstimatT = "In Progress";
                                }

                            }
                            else
                            {

                            }
                            ///////////////////////////////
                            tableclass.currLevel = currLevl;
                            tableclass.currTime = currTim;
                            tableclass.pUnit = punit;
                            tableclass.avgPonding = Math.Round((Convert.ToDouble(pondingsummer) / Convert.ToDouble(pondingentrycounter)), 2).ToString();
                            max = Math.Round((Convert.ToDouble(max) / 2.54), 2);
                            tableclass.maxLevel = max.ToString();
                            tableclass.maxLevelTime = theMaxT.ToString();
                            string spell = spellCounter.ToString();
                            if (isCleared == "")
                            {
                                tableclass.clearanceTime = ClearanceTime.ToString();
                                //if (flowRate > 0)
                                //{
                                //    tableclass.flowRateUp = Math.Abs(flowRate).ToString();
                                //    tableclass.flowRateDown = "-";
                                //}
                                //else
                                //{
                                //    tableclass.flowRateUp = "-";
                                //    tableclass.flowRateDown = Math.Abs(flowRate).ToString();
                                //}
                                //tableclass.estClTime = Math.Abs(EstclearanceTime).ToString() + "  minutes";
                                tableclass.flowRateUp = theFlowUp;
                                tableclass.flowRateDown = theFlowDown;
                                if (TheEstimatT == "In Progress")
                                {
                                    tableclass.estClTime = TheEstimatT;
                                }
                                else
                                {
                                    tableclass.estClTime = TheEstimatT + "minutes";
                                }
                                tableclass.pondingPeriod = pondingPeriod.ToString() + "  minutes";
                            }
                            else
                            {
                                tableclass.comment = "No Ponding";
                                tableclass.clearanceTime = isCleared;
                                tableclass.flowRateUp = isCleared;
                                tableclass.flowRateDown = isCleared;
                                tableclass.estClTime = isCleared;
                                tableclass.pondingPeriod = isCleared;
                            }
                        }
                        tablList.Add(tableclass);
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            return tablList;
        }

        /// <summary>
        /// //////////////////////////////////////////////
        public List<PondingSpellClass> getAllSpells(DataTable dt, int order)
        {
            var tableData = new PondingSpellClass();
            var spelldata = new SpellData();
            //int resourceID = Convert.ToInt32(dt.Rows[0]["resourceID"]);
            string location = dt.Rows[0]["Location"].ToString();
            double currentPonding = Math.Round((Convert.ToDouble(dt.Rows[0]["Current_Level"]) / 2.54), 2);
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
                double currValue = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2);
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
                            if (currValue < 1)
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
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod,2);
                                spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes),2);
                                spellDataList.Add(spelldata);
                                spelldata = new SpellData();
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
                            if (currValue < 1)
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
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                spellDataList.Add(spelldata);
                                spelldata = new SpellData();
                                string s = JsonConvert.SerializeObject(spellDataList);
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
            if (DeltaMinutes > 1440 || spelldata.SpellDataArray.Count == 0 || spellDataList.Count == 0)
            {
                tableData.date = DateTime.Now.AddHours(9).ToShortDateString();
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
                if (currentPonding < 1 && DeltaMinutes < 2000)
                {
                    if (dt.Rows.Count > 1)
                    {
                        tableData.estSpellClearanceTime = "No Ponding";
                        tableData.spellPeriod = "No Ponding";
                        tableData.SpellEndTime = "No Ponding";
                    }
                    else
                    {
                        tableData.estSpellClearanceTime = "-";
                        tableData.spellPeriod = "-";
                        tableData.SpellEndTime = "-";
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
                    tableData.spellMinTime = item.spellMinTime;
                    tableData.spellFlowDown = Math.Round(item.spellFlowDown,2).ToString();
                    tableData.spellFlowUp = Math.Round(item.spellFlowUp,2).ToString();
                    double pondingPer = Math.Abs((Convert.ToDateTime(tableData.spellMaxTime)-Convert.ToDateTime(tableData.SpellEndTime)).TotalMinutes);
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
                    }
                    tableDataList.Add(tableData);
                }
                //tableData.srNo = order.ToString();
                //tableData.pondingLocation = location;
                //tableData.currLevel = currentPonding.ToString();
                //tableData.currTime = currentTime;
                //tableData.maxLevel = spellDataList.FirstOrDefault().SpellMax.ToString();
                //tableData.maxLevelTime = spellDataList.FirstOrDefault().spellMaxTime.ToString();
                //double pondingPer = spellDataList.FirstOrDefault().spellPeriod;
                //TimeSpan pp = TimeSpan.FromMinutes(Convert.ToDouble(pondingPer));
                //int phour = pp.Hours;
                //int pmin = pp.Minutes;
                //int psec = pp.Seconds;
                //string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                //tableData.pondingPeriod = pstr;
                //if (Convert.ToDateTime(tableData.clearanceTime) < Convert.ToDateTime(tableData.maxLevelTime))
                //{
                //    //tableData.clearanceTime = currentTime;
                //    tableData.clearanceTime = spellDataList.FirstOrDefault().SpellEndTime.ToString();
                //}
                //else
                //{
                //    tableData.clearanceTime = spellDataList.FirstOrDefault().SpellEndTime.ToString();
                //}
                //tableData.flowRateUp = Math.Round(spellDataList.FirstOrDefault().spellFlowUp,2).ToString();
                //tableData.flowRateDown = Math.Round(spellDataList.FirstOrDefault().spellFlowDown, 2).ToString();
                //if (currentPonding < 1)
                //{
                //    tableData.estClTime = "Cleared";
                //    tableData.comment = "Cleared";
                //}
                //else
                //{
                //    tableData.comment = "Continue...";
                //    //tableData.clearanceTime = "-";
                //    if (currentPonding < Convert.ToDouble(tableData.maxLevel))
                //    {
                //        double minutesToClear = (currentPonding / Convert.ToDouble(tableData.flowRateDown));
                //        TimeSpan runningTime = TimeSpan.FromMinutes(Convert.ToDouble(minutesToClear));
                //        int hour = runningTime.Hours;
                //        int min = runningTime.Minutes;
                //        int sec = runningTime.Seconds;
                //        string str = " " + hour.ToString() + " Hours, " + min.ToString() + " Minutes";
                //        tableData.estClTime = str;
                //    }
                //    else
                //    {
                //        tableData.estClTime = "In Progress";
                //    }
                //}
            }
            return tableDataList;
        }
        /// //////////////////////////////////////////////
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="order"></param>
        /// <returns></returns>


        ///////////////////////
        public PondingTableClass getAllSpellsFiltered(DataTable dt, int order)
        {
            var tableData = new PondingTableClass();
            var spelldata = new SpellData();
            //int resourceID = Convert.ToInt32(dt.Rows[0]["resourceID"]);
            string location = dt.Rows[0]["Location"].ToString();
            double currentPonding = Math.Round((Convert.ToDouble(dt.Rows[0]["Current_Level"]) / 2.54), 2);
            string currentTime = dt.Rows[0]["tim"].ToString();
            //double DeltaMinutes = Convert.ToDouble(dt.Rows[0]["DeltaMinutes"]);
            bool S = false;
            bool E = false;
            bool T = true;
            bool F = false;
            int spell = 0;
            List<SpellData> spellDataList = new List<SpellData>();
            foreach (DataRow dr in dt.Rows)
            {
                double currValue = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2);
                string currTime = dr["tim"].ToString();
                string clearaceTime = "";
                if (currValue < 0)
                { }
                else
                { 
                    //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                    if (currentPonding < 1)
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
                            if (currValue < 1)
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
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                spellDataList.Add(spelldata);
                                spelldata = new SpellData();
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
                            if (currValue < 1)
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
                                spelldata.spellPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                                if (spelldata.spellPeriod == 0)
                                {
                                    spelldata.spellPeriod = 1;
                                }
                                spelldata.spellFlowDown = Math.Round(spelldata.SpellMax / spelldata.spellPeriod, 2);
                                spelldata.spellFlowUp = Math.Round(spelldata.SpellMax / Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellStartTime)).TotalMinutes), 2);
                                spellDataList.Add(spelldata);
                                spelldata = new SpellData();
                                string s = JsonConvert.SerializeObject(spellDataList);
                            }
                        }
                    }
                    // end  scenario 2 (uncleared/ ponding continues)
                }

            }
            string c = JsonConvert.SerializeObject(spellDataList);
            if (spelldata.SpellDataArray.Count == 0)
            {
                if (currentPonding < 0)
                {
                    spelldata.SpellDataArray.Add(0);
                }
                else
                {
                    spelldata.SpellDataArray.Add(currentPonding);
                }
                spelldata.SpellTimeArray.Add(currentTime);
                spelldata.SpellStartTime = currentTime;
                spelldata.SpellEndTime = currentTime;
            }
            if (spelldata.SpellDataArray.Count == 0 || spellDataList.Count == 0)
            {
                tableData.clearanceTime = "-";
                tableData.comment = "Inactive";
                tableData.currLevel = "-";
                tableData.currTime = "-";
                tableData.estClTime = "-";
                tableData.flowRateDown = "-";
                tableData.flowRateUp = "-";
                tableData.maxLevel = "-";
                tableData.maxLevelTime = "-";
                tableData.pondingPeriod = "-";
                tableData.pondingLocation = location;
                tableData.srNo = order.ToString();
                if (currentPonding < 1)
                {
                    if (dt.Rows.Count > 1)
                    {
                        tableData.estClTime = "No Ponding";
                        tableData.pondingPeriod = "No Ponding";
                        tableData.clearanceTime = "No Ponding";
                    }
                }
            }
            else
            {
                tableData.srNo = order.ToString();
                tableData.pondingLocation = location;
                if (currentPonding < 0)
                { tableData.currLevel = "0"; }
                else
                { tableData.currLevel = currentPonding.ToString(); }
                
                tableData.currTime = currentTime;
                tableData.maxLevel = spellDataList.FirstOrDefault().SpellMax.ToString();
                tableData.maxLevelTime = spellDataList.Max(i => i.spellMaxTime).ToString();
                double pondingPer = spellDataList.FirstOrDefault().spellPeriod;
                TimeSpan pp = TimeSpan.FromMinutes(Convert.ToDouble(pondingPer));
                int phour = pp.Hours;
                int pmin = pp.Minutes;
                int psec = pp.Seconds;
                string pstr = " " + phour.ToString() + " Hours, " + pmin.ToString() + " Minutes";
                tableData.pondingPeriod = pstr;
                if (Convert.ToDateTime(tableData.clearanceTime) < Convert.ToDateTime(tableData.maxLevelTime))
                {
                    //tableData.clearanceTime = currentTime;
                    tableData.clearanceTime = spellDataList.FirstOrDefault().SpellEndTime.ToString();
                }
                else
                {
                    tableData.clearanceTime = spellDataList.FirstOrDefault().SpellEndTime.ToString();
                }
                tableData.flowRateUp = Math.Round(spellDataList.FirstOrDefault().spellFlowUp, 2).ToString();
                tableData.flowRateDown = Math.Round(spellDataList.FirstOrDefault().spellFlowDown, 2).ToString();
                if (currentPonding < 1)
                {
                    tableData.estClTime = "Cleared";
                    tableData.comment = "Cleared";
                }
                else
                {
                    tableData.comment = "Continue...";
                    //tableData.clearanceTime = "-";
                    if (currentPonding < Convert.ToDouble(tableData.maxLevel))
                    {
                        double minutesToClear = (currentPonding / Convert.ToDouble(tableData.flowRateDown));
                        TimeSpan runningTime = TimeSpan.FromMinutes(Convert.ToDouble(minutesToClear));
                        int hour = runningTime.Hours;
                        int min = runningTime.Minutes;
                        int sec = runningTime.Seconds;
                        string str = " " + hour.ToString() + " Hours, " + min.ToString() + " Minutes";
                        tableData.estClTime = str;
                    }
                    else
                    {
                        tableData.estClTime = "In Progress";
                    }
                }
            }
            return tableData;
        }
        ///////////////////////


        public PondingTableClass getFirstSpells(DataTable dt, int order)
        {
            var tableData = new PondingTableClass();
            var spelldata = new SpellData();
            //int resourceID = Convert.ToInt32(dt.Rows[0]["resourceID"]);
            string location = dt.Rows[0]["Location"].ToString();
            double currentPonding = Math.Round((Convert.ToDouble(dt.Rows[0]["Current_Level"]) / 2.54), 2);
            string currentTime = dt.Rows[0]["tim"].ToString();
            double DeltaMinutes = Convert.ToDouble(dt.Rows[0]["DeltaMinutes"]);
            bool startOfSpell = false;
            bool endOfSpell = false;
            foreach (DataRow dr in dt.Rows)
            {
                double currValue = Math.Round((Convert.ToDouble(dr["Current_Level"])/2.54),2);
                string currTime = dr["tim"].ToString();
                string clearaceTime = "";
                //start scenario 3 (inactive)
                if (DeltaMinutes > -520)
                {

                }
                // end  scenario 3 (inactive)
                else
                {
                    //start scenario 1 (No Ponding since many time/cleared/ zero received (find out what is the last ponding time if any))
                    if (currentPonding < 1)
                    {
                        if (endOfSpell == false && startOfSpell == false)
                        {
                            if (currValue < 1)
                            {
                                if (spelldata.SpellDataArray.Count > 0)
                                {
                                    string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                    double lastvalue = spelldata.SpellDataArray.LastOrDefault();
                                    if (Math.Abs(((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes) <= 30)
                                    {

                                    }
                                    else
                                    {
                                        endOfSpell = true;
                                        startOfSpell = true;
                                        spelldata.SpellDataArray.Add(lastvalue);
                                        spelldata.SpellTimeArray.Add(lastTime);
                                        spelldata.SpellEndTime = currTime;
                                        clearaceTime = currTime;
                                    }
                                }

                            }
                            else
                            {
                                if (Math.Abs(((Convert.ToDateTime(currentTime)) - (Convert.ToDateTime(currTime))).TotalMinutes) <= 30)
                                {
                                    endOfSpell = true;
                                    spelldata.SpellDataArray.Add(currValue);
                                    spelldata.SpellTimeArray.Add(currTime);
                                    spelldata.SpellEndTime = currTime;
                                    clearaceTime = currTime;
                                }
                                else
                                {
                                    endOfSpell = true;
                                    spelldata.SpellDataArray.Add(currentPonding);
                                    spelldata.SpellTimeArray.Add(currentTime);
                                    spelldata.SpellEndTime = currentTime;
                                    clearaceTime = currentTime;
                                }
                                
                            }
                        }
                        else if (endOfSpell == true && startOfSpell == false)
                        {
                            if (currValue < 1)
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (Math.Abs(((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes) <= 30)
                                {
                                    spelldata.SpellStartTime = currentTime;
                                }
                                else
                                {

                                    spelldata.SpellStartTime = lastTime;
                                }
                                startOfSpell = true;
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (Math.Abs(((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes) <= 30)
                                {
                                    spelldata.SpellDataArray.Add(currValue);
                                    spelldata.SpellTimeArray.Add(currTime);
                                }
                            }
                        }
                        else if (endOfSpell == true && startOfSpell == true)
                        {

                        }
                        else if (endOfSpell == false && startOfSpell == true)
                        {

                        }

                    }
                    // end  scenario 1 (No Ponding since many time/cleared/ zero received)
                    //////////////////////////////////////////////////////////////////////
                    //start scenario 2 (uncleared/ ponding continues (find out when the ponding is started))
                    else
                    {
                        if (endOfSpell == false && startOfSpell == false)
                        {
                            if (currValue < 1)
                            {

                            }
                            else
                            {
                                endOfSpell = true;
                                spelldata.SpellDataArray.Add(currValue);
                                spelldata.SpellTimeArray.Add(currTime);
                                spelldata.SpellEndTime = currTime;
                            }
                        }
                        else if (endOfSpell == true && startOfSpell == false)
                        {
                            if (currValue < 1)
                            {
                                startOfSpell = true;
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (Math.Abs(((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes) <= 30)
                                {
                                    spelldata.SpellStartTime = currentTime;
                                    clearaceTime = currentTime;
                                }
                                else
                                {
                                    spelldata.SpellStartTime = lastTime;
                                    clearaceTime = lastTime;
                                }
                            }
                            else
                            {
                                string lastTime = spelldata.SpellTimeArray.LastOrDefault().ToString();
                                if (Math.Abs(((Convert.ToDateTime(lastTime)) - (Convert.ToDateTime(currTime))).TotalMinutes) <= 30)
                                {
                                    spelldata.SpellDataArray.Add(currValue);
                                    spelldata.SpellTimeArray.Add(currTime);
                                }
                            }
                        }
                        else if (endOfSpell == true && startOfSpell == true)
                        {

                        }
                        else if (endOfSpell == false && startOfSpell == true)
                        {

                        }
                    }
                    // end  scenario 2 (uncleared/ ponding continues)
                }

            }
            if (spelldata.SpellDataArray.Count == 0)
            {
                spelldata.SpellDataArray.Add(currentPonding);
                spelldata.SpellTimeArray.Add(currentTime);
                spelldata.SpellStartTime = currentTime;
                spelldata.SpellEndTime = currentTime;
            }
            if (DeltaMinutes > -520)
            {
                tableData.deltaTime = DeltaMinutes.ToString(); ;
                tableData.clearanceTime = "-";
                tableData.comment = "Inactive";
                tableData.currLevel = "-";
                tableData.currTime = "-";
                tableData.estClTime = "-";
                tableData.flowRateDown = "-";
                tableData.flowRateUp = "-";
                tableData.maxLevel = "-";
                tableData.maxLevelTime = "-";
                tableData.pondingPeriod = "-";
                tableData.pondingLocation = location;
                tableData.srNo = order.ToString();
                tableData.minThr = (dt.Rows[0]["minTh"]).ToString();
                tableData.maxThr = (dt.Rows[0]["maxTh"]).ToString();
            }
            else
            {
                tableData.minThr = (dt.Rows[0]["minTh"]).ToString();
                tableData.maxThr = (dt.Rows[0]["maxTh"]).ToString();
                tableData.deltaTime = DeltaMinutes.ToString();
                string pondingPeriod = "";
                int indexMax = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                int indexMin = !spelldata.SpellDataArray.Any() ? 0 : spelldata.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                spelldata.spellMaxTime = spelldata.SpellTimeArray.ElementAt(indexMax);
                spelldata.spellMinTime = spelldata.SpellTimeArray.ElementAt(indexMin);
                tableData.srNo = order.ToString();
                tableData.pondingLocation = location;
                tableData.currLevel = currentPonding.ToString();
                tableData.currTime = currentTime;
                tableData.maxLevel = spelldata.SpellDataArray.DefaultIfEmpty().Max().ToString();
                tableData.maxLevelTime = spelldata.spellMaxTime;
                double pondinPeriod = 10;
                if (currentPonding < 1)
                {

                    pondinPeriod = Math.Abs((Convert.ToDateTime(spelldata.spellMaxTime) - Convert.ToDateTime(spelldata.SpellEndTime)).TotalMinutes);
                    TimeSpan runningTime = TimeSpan.FromMinutes(Convert.ToDouble(pondinPeriod));
                    int hour = runningTime.Hours;
                    int min = runningTime.Minutes;
                    int sec = runningTime.Seconds;
                    string str = " " + hour.ToString() + " Hours, " + min.ToString() + " Minutes";
                    pondingPeriod = str;
                    tableData.pondingPeriod = pondingPeriod;
                    tableData.clearanceTime = currentTime;
                    double DiffForFu = ((Convert.ToDateTime(spelldata.SpellStartTime)) - (Convert.ToDateTime(spelldata.spellMaxTime))).TotalMinutes;
                    if (DiffForFu == 0)
                    {
                        DiffForFu = 1;
                    }
                    double DiffForFd = ((Convert.ToDateTime(spelldata.spellMaxTime)) - (Convert.ToDateTime(spelldata.SpellEndTime))).TotalMinutes;
                    if (DiffForFd == 0)
                    {
                        DiffForFd = 1;
                    }
                    double fu = Math.Round((Convert.ToDouble(tableData.maxLevel) / DiffForFu), 2);
                    double fd = Math.Abs(Math.Round((Convert.ToDouble(tableData.maxLevel) / DiffForFd), 2));
                    tableData.flowRateUp = fu.ToString();
                    tableData.flowRateDown = fd.ToString();
                    if (Convert.ToInt32(pondinPeriod) < 1)
                    {
                        tableData.pondingPeriod = "-";
                        tableData.flowRateUp = "-";
                        tableData.flowRateDown = "-";
                        tableData.estClTime = "-";
                        tableData.comment = "No Ponding";
                        tableData.clearanceTime = "-";
                    }
                    else
                    {
                        tableData.estClTime = "Cleared";
                        tableData.comment = "Cleared";
                    }
                }
                else
                {
                    pondingPeriod = "-";
                    tableData.comment = "Continue...";
                    tableData.pondingPeriod = pondingPeriod;
                    tableData.clearanceTime = "-";
                    if (currentPonding < spelldata.SpellDataArray.DefaultIfEmpty().Max())
                    {
                        double DiffForFd = ((Convert.ToDateTime(spelldata.spellMaxTime)) - (Convert.ToDateTime(spelldata.SpellEndTime))).TotalMinutes;
                        if (DiffForFd == 0)
                        {
                            DiffForFd = 1;
                        }
                        double fd = Math.Abs(Math.Round((Convert.ToDouble(tableData.maxLevel) / DiffForFd), 2));
                        tableData.flowRateUp = "-";
                        tableData.flowRateDown = fd.ToString();
                        double minutesToClear = (currentPonding / fd);
                        TimeSpan runningTime = TimeSpan.FromMinutes(Convert.ToDouble(minutesToClear));
                        int hour = runningTime.Hours;
                        int min = runningTime.Minutes;
                        int sec = runningTime.Seconds;
                        string str = " " + hour.ToString() + " Hours, " + min.ToString() + " Minutes";
                        tableData.estClTime = str;
                    }
                    else
                    {
                        double DiffForFu = ((Convert.ToDateTime(spelldata.SpellStartTime)) - (Convert.ToDateTime(spelldata.spellMaxTime))).TotalMinutes;
                        if (DiffForFu == 0)
                        {
                            DiffForFu = 1;
                        }
                        double fu = Math.Round((Convert.ToDouble(tableData.maxLevel) / DiffForFu), 2);
                        tableData.flowRateUp = fu.ToString();
                        tableData.flowRateDown = "-";
                        tableData.estClTime = "In Progress";
                    }
                }
            }
            return tableData;
        }
        public List<PondingTableClass> getPondingTableListUpdated()
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            string resourceLocation = "";
            var tablList = new List<PondingTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    //getting resources belonging to ponding points
                    string getResFromTemp = " select r.resourceID, r.resourceNumber, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Ponding Points' order by CAST(resourceNumber as int) ASC ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the ponding points
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //getting resourceID 
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //getting resourceLocation 
                        resourceLocation = drRes["resourceLocationName"].ToString();
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " ) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        double DeltaMinutes = 0;
                        string currLevl = "";
                        string currTim = "";
                        string location = "";
                        string punit = "";
                        string isCleared = "";
                        int pondingentrycounter = 0;
                        double pondingsummer = 0;
                        double maxLevel = 0;
                        double minLevel = 0;
                        int spellCounter = 0;
                        string cclt = DateTime.Now.ToString();
                        if (Dashdt.Rows.Count > 0)
                        {
                            cclt = (Dashdt.Rows[Dashdt.Rows.Count - 1]["tim"]).ToString();
                        }
                        DateTime currLevlTime = DateTime.Parse(cclt);
                        DateTime maxLevelTime = DateTime.Parse(cclt);
                        DateTime pondingStartTime = DateTime.Parse(cclt);
                        DateTime pondingEndTime = DateTime.Parse(cclt);
                        DateTime ClearanceTime = DateTime.Parse(cclt);
                        var tableclass = new PondingTableClass();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        DeltaMinutes = Convert.ToDouble(Dashdt.Rows[0]["DeltaMinutes"]);
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            //DeltaMinutes = Convert.ToDouble(dr["DeltaMinutes"]);
                            if (location == "") //first entry of a resource
                            {
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                spellData.resourceName = resourceLocation;
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2).ToString(); //save current level
                                currTim = Convert.ToDateTime(dr["tim"]).ToString();
                                ite = ite + 1; //for serial number
                                punit = dr["pUnit"].ToString(); //unit of data
                                if (Convert.ToDouble(currLevl) <= 1) //consider the cleared location
                                {
                                    isCleared = "Cleared";
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellOn = false;
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                }
                                else //consider the not cleared location
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) / 2.54; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 <= 0) //for second to last entry below the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.resourceName = resourceLocation;
                                    spellData.SpellStartTime = Convert.ToDateTime(dr["tim"]).ToString(); //time when there is zero reading determines the start of the spell for an onging spell
                                    spellOn = false; // turn off the spell
                                    spellData.SpellMax = spellData.SpellDataArray.Max();
                                    spellData.SpellMin = spellData.SpellDataArray.Min();
                                    int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                                    int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                                    //int indexMax = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(maxind);
                                    spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(minind);
                                    spellData.SpellDataArray.Reverse();
                                    spellData.SpellTimeArray.Reverse();
                                    var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                                    var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                                    var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                                    var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                                    spellData.spellPeriod = (Convert.ToDateTime(spellData.spellMaxTime) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                                    if (spellData.spellPeriod == 0)
                                    {
                                        spellData.spellPeriod = 10;
                                    }
                                    if (spellLastData > spellAvg)
                                    {
                                        spellData.spellFlowDown = 0;
                                        spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                                    }
                                    else
                                    {
                                        spellData.spellFlowUp = 0;
                                        spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                                    }
                                    spellData.SpellMax = spellMax;
                                    spellData.SpellMin = spellMin;
                                    spellData.SpellAvg = spellAvg;
                                    spellDataList.Add(spellData); //save spell to the list
                                    spellData = new SpellData(); // new instance
                                }
                                if (isCleared == "Cleared")
                                {
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]); //updates the clearance time
                                }
                                if (spellCounter == 0)
                                {
                                    pondingStartTime = Convert.ToDateTime(dr["tim"]);
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellCounter += 1;
                                }
                                //spellCounter += 1;
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) / 2.54;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) / 2.54)
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) / 2.54, 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                        }
                        if (spellData.SpellDataArray.Count > 0)
                        //if (spellData.SpellDataArray.Count > 0)
                        {
                            var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                            spellData.SpellStartTime = spellStartTime;
                            //int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                            //int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                            int indexMax = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            int indexMin = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(indexMax);
                            spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(indexMin);
                            var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                            var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                            var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                            var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                            spellData.spellPeriod = (Convert.ToDateTime(spellData.spellMaxTime) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                            if (spellData.spellPeriod == 0)
                            {
                                spellData.spellPeriod = 10;
                            }
                            if (spellLastData > spellAvg)
                            {
                                spellData.spellFlowDown = 0;
                                spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                            }
                            else
                            {
                                spellData.spellFlowUp = 0;
                                spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                            }
                            spellData.SpellMax = spellMax;
                            spellData.SpellMin = spellMin;
                            spellData.SpellAvg = spellAvg;
                            spellData.SpellDataArray.Reverse();
                            spellData.SpellTimeArray.Reverse();
                            spellDataList.Add(spellData);
                            spellData = new SpellData(); // new instance
                        }
                        string stringlist = string.Join(",", spellDataList);
                        //double listmaxspellmax = spellDataList.Max(r=>r.SpellMax);
                        int ron = dtRes.Rows.IndexOf(drRes);
                        double pondingPeriod = Math.Abs(Math.Round(spellDataList[dtRes.Rows.IndexOf(drRes)].spellPeriod, 2));
                        var jsonSerialiser = new JavaScriptSerializer();
                        var json = jsonSerialiser.Serialize(spellDataList);
                        double flowRate = 0;
                        if (pondingentrycounter > 0)
                        {
                            flowRate = Math.Round(((pondingsummer / pondingentrycounter) - Convert.ToDouble(currLevl)) / pondingPeriod, 2);
                        }
                        double EstclearanceTime = Math.Round((flowRate * pondingPeriod), 2);

                        tableclass.deltaTime = DeltaMinutes.ToString();
                        tableclass.srNo = ite.ToString();
                        tableclass.pondingLocation = location;
                        //if (DeltaMinutes > -420)
                        if (DeltaMinutes > 2200)
                        {
                            tableclass.currLevel = "-";
                            tableclass.currTime = "-";
                            tableclass.pUnit = "";
                            tableclass.avgPonding = "-";
                            tableclass.maxLevel = "-";
                            tableclass.maxLevelTime = "-";
                            tableclass.comment = "Inactive";
                            string spell = spellCounter.ToString();
                            if (isCleared == "")
                            {
                                tableclass.clearanceTime = "-";
                                if (flowRate > 0)
                                {
                                    tableclass.flowRateUp = "-";
                                    tableclass.flowRateDown = "-";
                                }
                                else
                                {
                                    tableclass.flowRateUp = "-";
                                    tableclass.flowRateDown = "-";
                                }
                                tableclass.estClTime = "-";
                                tableclass.pondingPeriod = "-";
                            }
                            else
                            {
                                tableclass.clearanceTime = "-";
                                tableclass.flowRateUp = "-";
                                tableclass.flowRateDown = "-";
                                tableclass.estClTime = "-";
                                tableclass.pondingPeriod = "-";
                            }
                        }
                        else
                        {
                            ///////////////////////////////
                            double ThePrevious = 0;
                            double TheflowRate = 0;
                            string theFlowUp = "";
                            string theFlowDown = "";
                            string TheEstimatT = "";
                            double max = ThePrevious;
                            string theMaxT = "";
                            DateTime ThePreviousT = DateTime.Now;
                            double TheCurrent = Convert.ToDouble(Dashdt.Rows[0]["Current_Level"]);
                            DateTime TheCurrentT = Convert.ToDateTime(Dashdt.Rows[0]["tim"]);
                            if (Dashdt.Rows.Count > 1)
                            {
                                ThePrevious = Convert.ToDouble(Dashdt.Rows[1]["Current_Level"]);
                                ThePreviousT = Convert.ToDateTime(Dashdt.Rows[1]["tim"]);
                                theMaxT = ThePreviousT.ToString();
                                double timeDifference = (ThePreviousT - TheCurrentT).TotalMinutes;
                                if (TheCurrent > max)
                                {
                                    max = TheCurrent;
                                    theMaxT = TheCurrentT.ToString();
                                }
                                TheflowRate = Math.Round(((TheCurrent - ThePrevious) / timeDifference), 2);
                                if (TheflowRate > 0 && TheCurrent != 0)
                                {
                                    theFlowUp = "-";
                                    theFlowDown = Math.Abs(TheflowRate).ToString();
                                    //TheEstimatT = Math.Round((TheCurrent / Math.Abs(TheflowRate)), 2).ToString();
                                    TheEstimatT = Math.Round((TheCurrent / Math.Abs(spellDataList[dtRes.Rows.IndexOf(drRes)].spellFlowDown + spellDataList[dtRes.Rows.IndexOf(drRes)].spellFlowUp)), 2).ToString();
                                    //if (!Double.IsNaN(TheEstimatT ?? 0.0))
                                    if (TheEstimatT.ToLower() == "nan")
                                    {
                                        TheEstimatT = "-";
                                    }
                                    else
                                    {
                                        double minns = Convert.ToDouble(TheEstimatT);
                                        TimeSpan runningTime = TimeSpan.FromMinutes(Convert.ToDouble(TheEstimatT));
                                        int hour = runningTime.Hours;
                                        int min = runningTime.Minutes;
                                        int sec = runningTime.Seconds;
                                        TheEstimatT = " " + hour.ToString() + " Hours, " + min.ToString() + " Minutes";
                                    }
                                }
                                else
                                {
                                    theFlowUp = Math.Abs(TheflowRate).ToString();
                                    theFlowDown = "-";
                                    TheEstimatT = "-";
                                }

                            }
                            else
                            {

                            }
                            ///////////////////////////////
                            tableclass.currLevel = currLevl;
                            tableclass.currTime = currTim;
                            tableclass.pUnit = punit;
                            //tableclass.avgPonding = Math.Round((Convert.ToDouble(pondingsummer) / Convert.ToDouble(pondingentrycounter)), 2).ToString();
                            max = Math.Round((Convert.ToDouble(max) / 2.54), 2);
                            //tableclass.maxLevel = max.ToString();
                            tableclass.maxLevel = Math.Round(spellDataList[dtRes.Rows.IndexOf(drRes)].SpellMax, 2).ToString();
                            tableclass.avgPonding = spellDataList[dtRes.Rows.IndexOf(drRes)].SpellAvg.ToString();
                            //tableclass.maxLevelTime = theMaxT.ToString();
                            tableclass.maxLevelTime = spellDataList[dtRes.Rows.IndexOf(drRes)].spellMaxTime.ToString();
                            string spell = spellCounter.ToString();
                            if (isCleared == "")
                            {
                                //tableclass.clearanceTime = ClearanceTime.ToString();
                                //tableclass.clearanceTime = spellDataList[dtRes.Rows.IndexOf(drRes)].SpellStartTime.ToString();
                                tableclass.clearanceTime = "-";
                                //if (flowRate > 0)
                                //{
                                //    tableclass.flowRateUp = Math.Abs(flowRate).ToString();
                                //    tableclass.flowRateDown = "-";
                                //}
                                //else
                                //{
                                //    tableclass.flowRateUp = "-";
                                //    tableclass.flowRateDown = Math.Abs(flowRate).ToString();
                                //}
                                //tableclass.estClTime = Math.Abs(EstclearanceTime).ToString() + "  minutes";
                                //tableclass.flowRateUp = theFlowUp;
                                tableclass.flowRateUp = spellDataList[dtRes.Rows.IndexOf(drRes)].spellFlowUp.ToString();
                                //tableclass.flowRateDown = theFlowDown; 
                                tableclass.flowRateDown = spellDataList[dtRes.Rows.IndexOf(drRes)].spellFlowDown.ToString();
                                double theFlowRate = Convert.ToDouble(tableclass.flowRateDown) + Convert.ToDouble(tableclass.flowRateUp);
                                if (theFlowRate >= 0)
                                {
                                    tableclass.flowRateDown = "-";
                                    tableclass.flowRateUp = Math.Round(Math.Abs(theFlowRate), 2).ToString();
                                }
                                else
                                {
                                    tableclass.flowRateUp = "-";
                                    tableclass.flowRateDown = Math.Round(Math.Abs(theFlowRate), 2).ToString();
                                }
                                if (TheEstimatT == "-")
                                {
                                    tableclass.estClTime = TheEstimatT;
                                }
                                else
                                {
                                    tableclass.estClTime = TheEstimatT;
                                }
                                //tableclass.pondingPeriod = pondingPeriod.ToString() + "  minutes";
                                tableclass.pondingPeriod = "-";
                            }
                            else
                            {
                                tableclass.comment = "No Ponding";
                                //tableclass.clearanceTime = isCleared;
                                tableclass.clearanceTime = spellDataList[dtRes.Rows.IndexOf(drRes)].SpellEndTime.ToString();
                                //tableclass.flowRateUp = isCleared;
                                //tableclass.flowRateDown = isCleared;
                                double theFlowRate = Convert.ToDouble(tableclass.flowRateDown) + Convert.ToDouble(tableclass.flowRateUp);
                                if (theFlowRate >= 0)
                                {
                                    tableclass.flowRateDown = "0";
                                    tableclass.flowRateUp = Math.Round(Math.Abs(theFlowRate), 2).ToString();
                                }
                                else
                                {
                                    tableclass.flowRateUp = "0";
                                    tableclass.flowRateDown = Math.Round(Math.Abs(theFlowRate), 2).ToString();
                                }
                                tableclass.estClTime = isCleared;
                                //tableclass.pondingPeriod = isCleared;
                                TimeSpan runningTime = TimeSpan.FromMinutes(Convert.ToDouble(pondingPeriod));
                                int hour = runningTime.Hours;
                                int min = runningTime.Minutes;
                                int sec = runningTime.Seconds;
                                string str = " " + hour.ToString() + " Hours, " + min.ToString() + " Minutes";
                                tableclass.pondingPeriod = str;
                                /////////average flow up and down///////////
                                double avgFlUp = Math.Round(Convert.ToDouble(spellDataList[dtRes.Rows.IndexOf(drRes)].SpellMax) / (Convert.ToDateTime(spellDataList[dtRes.Rows.IndexOf(drRes)].spellMaxTime) - Convert.ToDateTime(spellDataList[dtRes.Rows.IndexOf(drRes)].SpellStartTime)).TotalMinutes, 2);
                                double avgFlDn = Math.Round(Convert.ToDouble(spellDataList[dtRes.Rows.IndexOf(drRes)].SpellMax) / (Convert.ToDateTime(spellDataList[dtRes.Rows.IndexOf(drRes)].SpellEndTime) - Convert.ToDateTime(spellDataList[dtRes.Rows.IndexOf(drRes)].spellMaxTime)).TotalMinutes, 2);
                                tableclass.flowRateUp = avgFlUp.ToString();
                                tableclass.flowRateDown = avgFlDn.ToString();
                                ////////////////////
                            }
                        }
                        string abc = spellDataList[dtRes.Rows.IndexOf(drRes)].resourceName;
                        tablList.Add(tableclass);
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            string t = JsonConvert.SerializeObject(tablList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            ViewData["TableData"] = t;
            return tablList;
        }
        // GET: Ponding
        public List<PondingTableClass> getPondingTableListUpdated2()
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            string resourceLocation = "";
            var tablList = new List<PondingTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    //getting resources belonging to ponding points
                    string getResFromTemp = "";
                    if (Convert.ToInt32(Session["CompanyID"]) == 1)
                    {
                        getResFromTemp = " select r.resourceID, r.resourceNumber, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Ponding Points' order by CAST(resourceNumber as int) ASC ";
                    }
                    else
                    {
                        getResFromTemp = " select r.resourceID, r.resourceNumber, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Ponding Points' and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " order by CAST(resourceNumber as int) ASC ";
                    }
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the ponding points
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //getting resourceID 
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //getting resourceLocation 
                        resourceLocation = drRes["resourceLocationName"].ToString();
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.minThr as minTh, r.maxThr as maxTh, r.resourceNumber as rnum, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " and s.parameterValue >= 0   and s.sheetInsertionDateTime > DATEADD(hour,-15,GETDATE()) ) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        if (Dashdt.Rows.Count > 0)
                        {
                            PondingTableClass sd = getFirstSpells(Dashdt, Convert.ToInt32(drRes["resourceNumber"]));
                            tablList.Add(sd);
                        }
                        else
                        {
                            string Dashdtquery1 = "select '" + resourceLocation + "' as Location, 0 as Min_Level, 0 as Current_Level, 'inch' as pUnit, 1 as rnum, 0 as DeltaMinutes, DATEADD(hour,9,GETDATE()) as tim, 1 as rn";
                            SqlCommand cmd1 = new SqlCommand(Dashdtquery1, conn);
                            SqlDataAdapter sda1 = new SqlDataAdapter(Dashdtquery1, conn);
                            Dashdt.Clear();
                            sda1.Fill(Dashdt);
                            var tablList2 = getFirstSpells(Dashdt, Convert.ToInt32(drRes["resourceNumber"]));
                            //PondingSpellClass sd = getAllSpells(Dashdt1, Convert.ToInt32(drRes["resourceNumber"]));
                            //tablList.Add(sd);
                            tablList.Add(tablList2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            string t = JsonConvert.SerializeObject(tablList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            ViewData["TableData"] = t;
            return tablList;
        }

        public List<SpellData> getPostPondingSpellList(DateTime FinalTimeFrom, DateTime FinalTimeTo, string resourceName)
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<PondingTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "select DISTINCT r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points' and s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "' ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the desired set of resources chosen
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " AND s.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and s.sheetInsertionDateTime <= '" + FinalTimeTo + "' ) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        string currLevl = "";
                        string currTim = "";
                        string location = "";
                        string punit = "";
                        string isCleared = "";
                        int pondingentrycounter = 0;
                        double pondingsummer = 0;
                        double maxLevel = 0;
                        double minLevel = 0;
                        int spellCounter = 0;
                        string cclt = DateTime.Now.ToString();
                        if (Dashdt.Rows.Count > 0)
                        {
                            cclt = (Dashdt.Rows[Dashdt.Rows.Count - 1]["tim"]).ToString();
                        }
                        DateTime currLevlTime = DateTime.Parse(cclt);
                        DateTime maxLevelTime = DateTime.Parse(cclt);
                        DateTime pondingStartTime = DateTime.Parse(cclt);
                        DateTime pondingEndTime = DateTime.Parse(cclt);
                        DateTime ClearanceTime = DateTime.Parse(cclt);
                        var tableclass = new PondingTableClass();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            if (location == "") //first entry of a resource
                            {
                                spellData.resourceName = dr["Location"].ToString();
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2).ToString(); //save current level
                                currTim = Convert.ToDateTime(dr["tim"]).ToString();
                                ite = ite + 1; //for serial number
                                punit = dr["pUnit"].ToString(); //unit of data
                                if (Convert.ToDouble(currLevl) <= 0) //consider the cleared location
                                {
                                    isCleared = "Cleared";
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellOn = false;
                                }
                                else //consider the not cleared location
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) / 2.54; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 <= 0) //for second to last entry below the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellStartTime = Convert.ToDateTime(dr["tim"]).ToString(); //time when there is zero reading determines the start of the spell for an onging spell
                                    spellOn = false; // turn off the spell
                                    spellData.SpellMax = spellData.SpellDataArray.Max();
                                    spellData.SpellMin = spellData.SpellDataArray.Min();
                                    int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                                    int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                                    //int indexMax = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(maxind);
                                    spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(minind);
                                    spellData.SpellDataArray.Reverse();
                                    spellData.SpellTimeArray.Reverse();
                                    spellDataList.Add(spellData); //save spell to the list
                                    spellData = new SpellData(); // new instance
                                }
                                if (isCleared == "Cleared")
                                {
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]); //updates the clearance time
                                }
                                if (spellCounter == 0)
                                {
                                    pondingStartTime = Convert.ToDateTime(dr["tim"]);
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellCounter += 1;
                                }
                                //spellCounter += 1;
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) / 2.54;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) / 2.54)
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) / 2.54, 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                        }
                        if (spellData.SpellDataArray.Count > 0)
                        {
                            var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                            spellData.SpellStartTime = spellStartTime;
                            var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                            var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                            var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                            var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                            spellData.spellPeriod = (Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().FirstOrDefault()) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                            if (spellData.spellPeriod == 0)
                            {
                                spellData.spellPeriod = 10;
                            }
                            if (spellLastData > spellAvg)
                            {
                                spellData.spellFlowDown = 0;
                                spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                            }
                            else
                            {
                                spellData.spellFlowUp = 0;
                                spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                            }
                            spellData.SpellMax = spellMax;
                            spellData.SpellMin = spellMin;
                            spellData.SpellAvg = spellAvg;
                            //int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                            //int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                            int indexMax = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            int indexMin = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(indexMax);
                            spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(indexMin);
                            spellData.SpellDataArray.Reverse();
                            spellData.SpellTimeArray.Reverse();
                            spellDataList.Add(spellData);
                        }
                        string stringlist = string.Join(",", spellDataList);
                        double pondingPeriod = Math.Abs(Math.Round((pondingEndTime.Subtract(pondingStartTime).TotalMinutes), 2));
                        double flowRate = 0;
                        if (pondingentrycounter > 0)
                        {
                            flowRate = Math.Round(((pondingsummer / pondingentrycounter) - Convert.ToDouble(currLevl)) / pondingPeriod, 2);
                        }
                        double EstclearanceTime = Math.Round((flowRate * pondingPeriod), 2);
                        tableclass.srNo = ite.ToString();
                        tableclass.pondingLocation = location;
                        tableclass.currLevel = currLevl;
                        tableclass.currTime = currTim;
                        tableclass.pUnit = punit;
                        tableclass.maxLevel = maxLevel.ToString();
                        tableclass.maxLevelTime = maxLevelTime.ToString();
                        string spell = spellCounter.ToString();
                        if (isCleared == "")
                        {
                            tableclass.clearanceTime = ClearanceTime.ToString();
                            if (flowRate > 0)
                            {
                                tableclass.flowRateUp = Math.Abs(flowRate).ToString();
                                tableclass.flowRateDown = "-";
                            }
                            else
                            {
                                tableclass.flowRateUp = "-";
                                tableclass.flowRateDown = Math.Abs(flowRate).ToString();
                            }
                            tableclass.estClTime = Math.Abs(EstclearanceTime).ToString() + "  minutes";
                            tableclass.pondingPeriod = pondingPeriod.ToString() + "  minutes";
                        }
                        else
                        {
                            tableclass.clearanceTime = isCleared;
                            tableclass.flowRateUp = isCleared;
                            tableclass.flowRateDown = isCleared;
                            tableclass.estClTime = isCleared;
                            tableclass.pondingPeriod = isCleared;
                        }
                        tablList.Add(tableclass);
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            return spellDataList;
        }

        public List<SpellData> getPondingSpellList()
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<PondingTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "select DISTINCT r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Ponding Points' and s.sheetInsertionDateTime > DATEADD(hour,-24,CURRENT_TIMESTAMP) ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the desired set of resources chosen
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " AND s.sheetInsertionDateTime > DATEADD(hour,-24,CURRENT_TIMESTAMP )) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        string currLevl = "";
                        string location = "";
                        string punit = "";
                        string isCleared = "";
                        int pondingentrycounter = 0;
                        double pondingsummer = 0;
                        double maxLevel = 0;
                        double minLevel = 0;
                        int spellCounter = 0;
                        string cclt = DateTime.Now.ToString();
                        if (Dashdt.Rows.Count > 0)
                        {
                            cclt = (Dashdt.Rows[Dashdt.Rows.Count - 1]["tim"]).ToString();
                        }
                        DateTime currLevlTime = DateTime.Parse(cclt);
                        DateTime maxLevelTime = DateTime.Parse(cclt);
                        DateTime pondingStartTime = DateTime.Parse(cclt);
                        DateTime pondingEndTime = DateTime.Parse(cclt);
                        DateTime ClearanceTime = DateTime.Parse(cclt);
                        var tableclass = new PondingTableClass();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            if (location == "") //first entry of a resource
                            {
                                spellData.resourceName = dr["Location"].ToString();
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) / 2.54), 2).ToString(); //save current level
                                ite = ite + 1; //for serial number
                                punit = dr["pUnit"].ToString(); //unit of data
                                if (Convert.ToDouble(currLevl) <= 0) //consider the cleared location
                                {
                                    isCleared = "Cleared";
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellOn = false;
                                }
                                else //consider the not cleared location
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) / 2.54; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 <= 0) //for second to last entry below the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellStartTime = Convert.ToDateTime(dr["tim"]).ToString(); //time when there is zero reading determines the start of the spell for an onging spell
                                    spellOn = false; // turn off the spell
                                    spellData.SpellMax = spellData.SpellDataArray.Max();
                                    spellData.SpellMin = spellData.SpellDataArray.Min();
                                    int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                                    int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                                    //int indexMax = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                                    //int indexMin = !spellData.SpellDataArray.Any() ? -1 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                                    spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(maxind);
                                    spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(minind);
                                    spellData.SpellDataArray.Reverse();
                                    spellData.SpellTimeArray.Reverse();
                                    spellDataList.Add(spellData); //save spell to the list
                                    spellData = new SpellData(); // new instance
                                }
                                if (isCleared == "Cleared")
                                {
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]); //updates the clearance time
                                }
                                if (spellCounter == 0)
                                {
                                    pondingStartTime = Convert.ToDateTime(dr["tim"]);
                                    ClearanceTime = Convert.ToDateTime(dr["tim"]);
                                    spellCounter += 1;
                                }
                                //spellCounter += 1;
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) / 2.54 > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) / 2.54);
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) / 2.54;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) / 2.54)
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) / 2.54, 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                        }
                        if (spellData.SpellDataArray.Count > 0)
                        {
                            var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                            spellData.SpellStartTime = spellStartTime;
                            var spellMax = spellData.SpellDataArray.DefaultIfEmpty().Max();
                            var spellMin = spellData.SpellDataArray.DefaultIfEmpty().Min();
                            var spellAvg = spellData.SpellDataArray.DefaultIfEmpty().Average();
                            var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                            spellData.spellPeriod = (Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().FirstOrDefault()) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                            if (spellData.spellPeriod == 0)
                            {
                                spellData.spellPeriod = 10;
                            }
                            if (spellLastData > spellAvg)
                            {
                                spellData.spellFlowDown = 0;
                                spellData.spellFlowUp = (spellLastData - spellAvg) / spellData.spellPeriod;
                            }
                            else
                            {
                                spellData.spellFlowUp = 0;
                                spellData.spellFlowDown = (spellAvg - spellMin) / spellData.spellPeriod;
                            }
                            spellData.SpellMax = spellMax;
                            spellData.SpellMin = spellMin;
                            spellData.SpellAvg = spellAvg;
                            //int maxind = spellData.SpellDataArray.IndexOf(spellData.SpellMax);
                            //int minind = spellData.SpellDataArray.IndexOf(spellData.SpellMin);
                            int indexMax = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                            int indexMin = !spellData.SpellDataArray.Any() ? 0 : spellData.SpellDataArray.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value < b.Value) ? a : b).Index;
                            spellData.spellMaxTime = spellData.SpellTimeArray.ElementAt(indexMax);
                            spellData.spellMinTime = spellData.SpellTimeArray.ElementAt(indexMin);
                            spellData.SpellDataArray.Reverse();
                            spellData.SpellTimeArray.Reverse();
                            spellDataList.Add(spellData);
                        }
                        string stringlist = string.Join(",", spellDataList);
                        double pondingPeriod = Math.Abs(Math.Round((pondingEndTime.Subtract(pondingStartTime).TotalMinutes), 2));
                        double flowRate = 0;
                        if (pondingentrycounter > 0)
                        {
                            flowRate = Math.Round(((pondingsummer / pondingentrycounter) - Convert.ToDouble(currLevl)) / pondingPeriod, 2);
                        }
                        double EstclearanceTime = Math.Round((flowRate * pondingPeriod), 2);
                        tableclass.srNo = ite.ToString();
                        tableclass.pondingLocation = location;
                        tableclass.currLevel = currLevl;
                        tableclass.pUnit = punit;
                        tableclass.maxLevel = maxLevel.ToString();
                        tableclass.maxLevelTime = maxLevelTime.ToString();
                        string spell = spellCounter.ToString();
                        if (isCleared == "")
                        {
                            tableclass.clearanceTime = ClearanceTime.ToString();
                            if (flowRate > 0)
                            {
                                tableclass.flowRateUp = Math.Abs(flowRate).ToString();
                                tableclass.flowRateDown = "-";
                            }
                            else
                            {
                                tableclass.flowRateUp = "-";
                                tableclass.flowRateDown = Math.Abs(flowRate).ToString();
                            }
                            tableclass.estClTime = Math.Abs(EstclearanceTime).ToString() + "  minutes";
                            tableclass.pondingPeriod = pondingPeriod.ToString() + "  minutes";
                        }
                        else
                        {
                            tableclass.clearanceTime = isCleared;
                            tableclass.flowRateUp = isCleared;
                            tableclass.flowRateDown = isCleared;
                            tableclass.estClTime = isCleared;
                            tableclass.pondingPeriod = isCleared;
                        }
                        tablList.Add(tableclass);
                    }
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                }
                conn.Close();
            }
            string c = JsonConvert.SerializeObject(spellDataList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            return spellDataList;
        }

        public ActionResult Dashboard()
        {
            DataTable dt = new DataTable();
            string markers = "[";
            string parameterValuesString = "";
            string JSONresult = "";
            string query, S1query, S2query, S3query = "";
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, r.maxThr as maxTh, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT * FROM cte WHERE rn = 1";
                S1query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT COUNT(*) FROM cte WHERE rn = 1";
                S2query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT COUNT(*) FROM cte WHERE DeltaMinutes <= -520 AND rn = 1";
                S3query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT COUNT(*) FROM cte WHERE DeltaMinutes > -520 AND rn = 1";
            }
            else
            {
                query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, r.maxThr as maxTh, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ) SELECT * FROM cte WHERE rn = 1";
                S1query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ) SELECT COUNT(*) FROM cte WHERE rn = 1";
                S2query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ) SELECT COUNT(*) FROM cte WHERE DeltaMinutes <= -520 AND rn = 1";
                S3query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ) SELECT COUNT(*) FROM cte WHERE DeltaMinutes > -520 AND rn = 1";
            }
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlCommand cmd1 = new SqlCommand(S1query, conn);
                    SqlCommand cmd2 = new SqlCommand(S2query, conn);
                    SqlCommand cmd3 = new SqlCommand(S3query, conn);
                    ViewBag.TotalPonding = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
                    ViewBag.RunningPonding = Convert.ToInt32(cmd2.ExecuteScalar()).ToString();
                    ViewBag.InactivePonding = Convert.ToInt32(cmd3.ExecuteScalar()).ToString();
                    SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                    sda.Fill(dt);
                    double minVal = double.MaxValue;
                    double maxVal = double.MinValue;
                    foreach (DataRow dr in dt.Rows)
                    {
                        double accountLevel = dr.Field<double>("ponding");
                        minVal = Math.Min(minVal, accountLevel);
                        maxVal = Math.Max(maxVal, accountLevel);
                    }
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
                            if (Math.Abs(Convert.ToDouble(sdr["ponding"])) == maxVal)
                            {
                                parameterValuesString += string.Format("'colr': '{0}',", "#ff0000");
                            }
                            else
                            {
                                parameterValuesString += string.Format("'colr': '{0}',", "#08a3cc");
                            }
                            parameterValuesString += string.Format("'pName': '{0}',", sdr["pName"]);
                            parameterValuesString += string.Format("'ponding': '{0}',", Math.Abs(Convert.ToDouble(sdr["ponding"])));
                            parameterValuesString += string.Format("'pUnit': '{0}',", sdr["unit"]);
                            parameterValuesString += string.Format("'title': '{0}',", sdr["category"]);
                            parameterValuesString += string.Format("'maxThrs': '{0}',", sdr["maxTh"]);
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
            ViewBag.PondingMarkers = markers;
            ViewBag.PondingjsonRes = JSONresult;
            var tablList = getPondingTableListUpdated2();
            return View(tablList);
        }

        public JsonResult LoadPondingChartData()
        {
            DataTable dt = new DataTable();
            string JSONresult = "";
            string query = "";
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.maxThr as maxThrs, r.resourceName as category, r.resourceNumber as resnum, 'Lahore' as townName, r.resourceName as townName2, p.parameterName as pName, abs(s.parameterValue) AS ponding, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points') SELECT * FROM cte WHERE rn = 1 order by cast(resnum as int) asc";
            }
            else
            {
                query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.maxThr as maxThrs, r.resourceName as category, r.maxThr as maxTh, r.resourceNumber as resnum, 'Lahore' as townName, r.resourceName as townName2, p.parameterName as pName, abs(s.parameterValue) AS ponding, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Ponding Points'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " ) SELECT * FROM cte WHERE rn = 1 order by cast(resnum as int) asc";
            }
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

        public ActionResult PondingReport()
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                int uID = Convert.ToInt32(Session["UserID"]);
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points" && item.managedBy == uID))
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
                    string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'ponding'";
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
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'  order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'  and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + " order by cast(r.resourceNumber as int) asc";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Ponding\" },";
                        scriptString += "subtitles: [{text: \" All Ponding Points Recent Ponding \" }],";
                        scriptString += "axisY: {suffix: \" inch\" },";
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
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterValue >= 0 and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " and e.sheetInsertionDateTime >= DATEADD(day,-2,GETDATE()) ";
                            aquery += ") ";
                            aquery += "SELECT top 1400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            var tablList = getPondingTableList();
            return View(tablList);
        }
        [HttpPost]
        public ActionResult PondingReport(FormCollection review)
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
            ResourceList.Add("All");
            int uID = Convert.ToInt32(Session["UserID"]);
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
            }
            else
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
            }
                
            ViewBag.ResourceList = ResourceList;
            ////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            if (resource == "All")
            {
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'ponding'";
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
                                getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'  order by cast(r.resourceNumber as int) asc";
                            }
                            else
                            {
                                getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'   and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + "  order by cast(r.resourceNumber as int) asc";
                            }
                            SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                            DataTable dtPar = new DataTable();
                            sdaPar.Fill(dtPar);
                            scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                            scriptString += "theme: \"light2\",";
                            scriptString += "animationEnabled: true,";
                            scriptString += "zoomEnabled: true, ";
                            scriptString += "title: {text: \"Ponding\" },";
                            scriptString += "subtitles: [{text: \" All Ponding Points Recent Ponding \" }],";
                            scriptString += "axisY: {suffix: \" inch\" },";
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
                                aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterValue >= 0 and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " and e.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and e.sheetInsertionDateTime < '" + FinalTimeTo + "'  ";
                                aquery += ") ";
                                aquery += "SELECT top 14400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                                string theQuery = aquery;
                                SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                                DataTable dtVal = new DataTable();
                                sdaVal.Fill(dtVal);
                                scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                                List<DataPoint> dataPoints = new List<DataPoint>();
                                DateTime dt = DateTime.Now;
                                foreach (DataRow drVal in dtVal.Rows)
                                {
                                    if (dtVal.Rows.IndexOf(drVal) != 0)
                                    {
                                        if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                        else
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        string getResFromTemp = "";
                        //getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Ponding Points'";
                        getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where resourceLocationName  = '" + resource + "' and rt.resourceTypeName = 'Ponding Points'";
                        SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                        DataTable dtRes = new DataTable();
                        sdaRes.Fill(dtRes);
                        int ite = 0;
                        foreach (DataRow drRes in dtRes.Rows)
                        {
                            string resName = drRes["resourceLocationName"].ToString();
                            ite += 1;
                            string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["ResourceID"]) + " and tp.parameterName = 'ponding'";
                            SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                            DataTable dtPar = new DataTable();
                            sdaPar.Fill(dtPar);
                            scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                            scriptString += "theme: \"light2\",";
                            scriptString += "animationEnabled: true,";
                            scriptString += "zoomEnabled: true, ";
                            scriptString += "title: {text: \"Ponding\" },";
                            scriptString += "subtitles: [{text: \"" + resName + "\" }],";
                            scriptString += "axisY: {suffix: \" inch\" },";
                            //scriptString += "axisY: {includeZero: false, prefix: \"\", labelFormatter: function(e){if(e.value == NaN){return \"No Data\";}else{return e.value;}} },";
                            scriptString += "toolTip: { shared: false },";
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
                                aquery += "WHERE e.resourceID = " + Convert.ToInt32(drRes["resourceID"]) + " and e.parameterValue >= 0 and e.parameterID = " + Convert.ToInt32(drPar["parameterID"]) + " and e.sheetInsertionDateTime >='" + FinalTimeFrom + "' and e.sheetInsertionDateTime <='" + FinalTimeTo + "' ";
                                aquery += ") ";
                                aquery += "SELECT top 14400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                                string theQuery = aquery;
                                SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                                DataTable dtVal = new DataTable();
                                sdaVal.Fill(dtVal);
                                scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                                List<DataPoint> dataPoints = new List<DataPoint>();
                                DateTime dt = DateTime.Now;
                                foreach (DataRow drVal in dtVal.Rows)
                                {
                                    if (dtVal.Rows.IndexOf(drVal) != 0)
                                    {
                                        if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                        else
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            var tablList = getPostPondingTableList(FinalTimeFrom, FinalTimeTo, resource);
            return View(tablList);
        }
        public string getRainStartTime()
        {
            return "";
        }
        public string getRainStopTime()
        {
            return "";
        }
        public ActionResult SummaryReport()
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points"))
            {
                ResourceList.Add(item.resourceLocationName);
            }

            ViewBag.ResourceList = ResourceList;
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            int resourceID = 0;
            string rainEndTime = "";
            string rainStartTime = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "select DISTINCT r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Rain Guages' and s.sheetInsertionDateTime > DATEADD(hour,-24,CURRENT_TIMESTAMP) ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " AND s.sheetInsertionDateTime > DATEADD(hour,-24,CURRENT_TIMESTAMP )) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
                        SqlCommand cmd = new SqlCommand(Dashdtquery, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(Dashdtquery, conn);
                        Dashdt.Clear();
                        sda.Fill(Dashdt);
                        rainEndTime = (Dashdt.Rows[0]["tim"]).ToString();
                        rainStartTime = (Dashdt.Rows[Dashdt.Rows.Count - 1]["tim"]).ToString();
                    }
                }
                catch (Exception ex)
                {
                }
            }
            if (rainEndTime == "")
            {
                return View();
            }
            else
            {
                var tablList = getPostSummaryTableList(Convert.ToDateTime(rainEndTime), DateTime.Now.AddHours(5), "");
                if (tablList.Count > 0)
                {
                    Session["rst"] = tablList.FirstOrDefault().rainStopTime;
                    Session["rd"] = tablList.FirstOrDefault().reportDate;
                    Session["rt"] = tablList.FirstOrDefault().reportTime;
                    return View(tablList);
                }
                else
                {
                    return View();
                }
            }

        }
        [HttpPost]
        public ActionResult SummaryReport(FormCollection review)
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            string resource = review["resource"].ToString();
            string datefr = review["dateFrom"].ToString();
            DateTime dateFrom = DateTime.Parse(review["dateFrom"].ToString());
            DateTime dateTo = DateTime.Parse(review["dateTo"].ToString());
            //DateTime dateFrom = Convert.ToDateTime(review["dateFrom"].ToString(), System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            //DateTime dateTo = Convert.ToDateTime(review["dateTo"].ToString(), System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
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
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points"))
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
                    string getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where resourceLocationName  = '" + resource + "' and rt.resourceTypeName = 'Ponding Points'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["ResourceID"]) + " and tp.parameterName = 'ponding'";
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Ponding\" },";
                        scriptString += "subtitles: [{text: \"" + resName + "\" }],";
                        scriptString += "axisY: {suffix: \" inch\" },";
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
                            aquery += "SELECT top 14400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            var tablList = getPostPondingTableList(FinalTimeFrom, FinalTimeTo, resource);
            return View(tablList);
        }

        public ActionResult TimeClearanceReport()
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            int uID = Convert.ToInt32(Session["UserID"]);
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points" && item.managedBy == uID))
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
                    string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'ponding'";
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
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'   order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'   and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + "  order by cast(r.resourceNumber as int) asc ";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Ponding\" },";
                        scriptString += "subtitles: [{text: \" All Ponding Points Recent Ponding \" }],";
                        scriptString += "axisY: {suffix: \" inch\" },";
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
                            aquery += "SELECT top 1400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            var tablList = getPondingTableList();
            return View(tablList);
        }

        [HttpPost]
        public ActionResult TimeClearanceReport(FormCollection review)
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
            ResourceList.Add("All");
            int uID = Convert.ToInt32(Session["UserID"]);
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            ////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            if (resource == "All")
            {
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'ponding'";
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
                                getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'  order by cast(r.resourceNumber as int) asc";
                            }
                            else
                            {
                                getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'   and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + "  order by cast(r.resourceNumber as int) asc";
                            }
                            SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                            DataTable dtPar = new DataTable();
                            sdaPar.Fill(dtPar);
                            scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                            scriptString += "theme: \"light2\",";
                            scriptString += "animationEnabled: true,";
                            scriptString += "zoomEnabled: true, ";
                            scriptString += "title: {text: \"Ponding\" },";
                            scriptString += "subtitles: [{text: \" All Ponding Points Recent Ponding \" }],";
                            scriptString += "axisY: {suffix: \" inch\" },";
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
                                aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " and e.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and e.sheetInsertionDateTime < '" + FinalTimeTo + "'  ";
                                aquery += ") ";
                                aquery += "SELECT top 1400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                                string theQuery = aquery;
                                SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                                DataTable dtVal = new DataTable();
                                sdaVal.Fill(dtVal);
                                scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                                List<DataPoint> dataPoints = new List<DataPoint>();
                                DateTime dt = DateTime.Now;
                                foreach (DataRow drVal in dtVal.Rows)
                                {
                                    if (dtVal.Rows.IndexOf(drVal) != 0)
                                    {
                                        if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                        else
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        string getResFromTemp = "";
                        //getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Ponding Points'";
                        getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where resourceLocationName  = '" + resource + "' and rt.resourceTypeName = 'Ponding Points'";
                        SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                        DataTable dtRes = new DataTable();
                        sdaRes.Fill(dtRes);
                        int ite = 0;
                        foreach (DataRow drRes in dtRes.Rows)
                        {
                            string resName = drRes["resourceLocationName"].ToString();
                            ite += 1;
                            string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["ResourceID"]) + " and tp.parameterName = 'ponding'";
                            SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                            DataTable dtPar = new DataTable();
                            sdaPar.Fill(dtPar);
                            scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                            scriptString += "theme: \"light2\",";
                            scriptString += "animationEnabled: true,";
                            scriptString += "zoomEnabled: true, ";
                            scriptString += "title: {text: \"Ponding\" },";
                            scriptString += "subtitles: [{text: \"" + resName + "\" }],";
                            scriptString += "axisY: {suffix: \" inch\" },";
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
                                aquery += "SELECT top 14400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                                string theQuery = aquery;
                                SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                                DataTable dtVal = new DataTable();
                                sdaVal.Fill(dtVal);
                                scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                                List<DataPoint> dataPoints = new List<DataPoint>();
                                DateTime dt = DateTime.Now;
                                foreach (DataRow drVal in dtVal.Rows)
                                {
                                    if (dtVal.Rows.IndexOf(drVal) != 0)
                                    {
                                        if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                        else
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            var tablList = getPostPondingTableList(FinalTimeFrom, FinalTimeTo, resource);
            return View(tablList);
        }

        public ActionResult FlowRateReport()
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            ResourceList.Add("All");
            int uID = Convert.ToInt32(Session["UserID"]);
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points" && item.managedBy == uID))
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
                    string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'ponding'";
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
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'  order by cast(r.resourceNumber as int) asc";
                        }
                        else
                        {
                            getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'   and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + "  order by cast(r.resourceNumber as int) asc";
                        }
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Ponding\" },";
                        scriptString += "subtitles: [{text: \" All Ponding Points Recent Ponding \" }],";
                        scriptString += "axisY: {suffix: \" inch\" },";
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
                            aquery += "SELECT top 1400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                            List<DataPoint> dataPoints = new List<DataPoint>();
                            DateTime dt = DateTime.Now;
                            foreach (DataRow drVal in dtVal.Rows)
                            {
                                if (dtVal.Rows.IndexOf(drVal) != 0)
                                {
                                    if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                        dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                    }
                                }
                                else
                                {
                                    dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            var tablList = getPondingTableList();
            return View(tablList);
        }

        [HttpPost]
        public ActionResult FlowRateReport(FormCollection review)
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
            ResourceList.Add("All");
            int uID = Convert.ToInt32(Session["UserID"]);
            if (Convert.ToInt32(Session["CompanyID"]) == 1)
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points"))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            else
            {
                foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Ponding Points" && item.managedBy == uID))
                {
                    ResourceList.Add(item.resourceLocationName);
                }
                ViewBag.ResourceList = ResourceList;
            }
            ////////////////////////////////////////////////////////////////////////////
            string scriptString = "";
            if (resource == "All")
            {
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'ponding'";
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
                                getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum  from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'  order by cast(r.resourceNumber as int) asc";
                            }
                            else
                            {
                                getParamsFromRes = "select r.resourceID, r.resourceLocationName, r.resourceNumber as resnum  from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'ponding'   and r.managedBy = " + Convert.ToInt32(Session["UserID"]) + "  order by cast(r.resourceNumber as int) asc";
                            }
                            SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                            DataTable dtPar = new DataTable();
                            sdaPar.Fill(dtPar);
                            scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                            scriptString += "theme: \"light2\",";
                            scriptString += "animationEnabled: true,";
                            scriptString += "zoomEnabled: true, ";
                            scriptString += "title: {text: \"Ponding\" },";
                            scriptString += "subtitles: [{text: \" All Ponding Points Recent Ponding \" }],";
                            scriptString += "axisY: {suffix: \" inch\" },";
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
                                aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " and e.sheetInsertionDateTime >= '" + FinalTimeFrom + "' and e.sheetInsertionDateTime < '" + FinalTimeTo + "'  ";
                                aquery += ") ";
                                aquery += "SELECT top 1400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                                string theQuery = aquery;
                                SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                                DataTable dtVal = new DataTable();
                                sdaVal.Fill(dtVal);
                                scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                                List<DataPoint> dataPoints = new List<DataPoint>();
                                DateTime dt = DateTime.Now;
                                foreach (DataRow drVal in dtVal.Rows)
                                {
                                    if (dtVal.Rows.IndexOf(drVal) != 0)
                                    {
                                        if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                        else
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    try
                    {
                        string getResFromTemp = "";
                        //getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Ponding Points'";
                        getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where resourceLocationName  = '" + resource + "' and rt.resourceTypeName = 'Ponding Points'";
                        SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                        DataTable dtRes = new DataTable();
                        sdaRes.Fill(dtRes);
                        int ite = 0;
                        foreach (DataRow drRes in dtRes.Rows)
                        {
                            string resName = drRes["resourceLocationName"].ToString();
                            ite += 1;
                            string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["ResourceID"]) + " and tp.parameterName = 'ponding'";
                            SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                            DataTable dtPar = new DataTable();
                            sdaPar.Fill(dtPar);
                            scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                            scriptString += "theme: \"light2\",";
                            scriptString += "animationEnabled: true,";
                            scriptString += "zoomEnabled: true, ";
                            scriptString += "title: {text: \"Ponding\" },";
                            scriptString += "subtitles: [{text: \"" + resName + "\" }],";
                            scriptString += "axisY: {suffix: \" inch\" },";
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
                                aquery += "SELECT top 14400 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 14401 Order by sheetInsertionDateTime ASC";
                                string theQuery = aquery;
                                SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                                DataTable dtVal = new DataTable();
                                sdaVal.Fill(dtVal);
                                scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} inch</strong> at {x}\", ";
                                List<DataPoint> dataPoints = new List<DataPoint>();
                                DateTime dt = DateTime.Now;
                                foreach (DataRow drVal in dtVal.Rows)
                                {
                                    if (dtVal.Rows.IndexOf(drVal) != 0)
                                    {
                                        if (Convert.ToDateTime(drVal["sheetInsertionDateTime"]).Subtract(dt).TotalSeconds < 6000)
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                        else
                                        {
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(dt).AddHours(-5).AddMinutes(1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5).AddMinutes(-1) - new DateTime(1970, 1, 1)).TotalMilliseconds), double.NaN));
                                            dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
                                            dt = Convert.ToDateTime(drVal["sheetInsertionDateTime"]);
                                        }
                                    }
                                    else
                                    {
                                        dataPoints.Add(new DataPoint(Convert.ToDouble((long)(Convert.ToDateTime(drVal["sheetInsertionDateTime"]).AddHours(-5) - new DateTime(1970, 1, 1)).TotalMilliseconds), Convert.ToDouble(drVal["parameterValue"]) / 2.54));
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
            var tablList = getPostPondingTableList(FinalTimeFrom, FinalTimeTo, resource);
            return View(tablList);
        }

        public JsonResult LoadPondingTableData()
        {
            string JSONresult = "";
            DataTable dt = new DataTable();
            string query = ";WITH cte AS ( SELECT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, CONVERT(float,s.parameterValue - LEAD(s.parameterValue) OVER (ORDER BY s.sheetInsertionDateTime DESC))/10 AS FLOW_RATE, s.sheetInsertionDateTime as tim, s.parameterValue/(CONVERT(float,s.parameterValue - LEAD(s.parameterValue) OVER (ORDER BY s.sheetInsertionDateTime DESC))/10+0.0001) AS EstimatedTime, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Ponding Points' ) SELECT * FROM cte WHERE rn = 1";
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
    }
}