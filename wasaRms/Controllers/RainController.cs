using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wasaRms.Models;

namespace wasaRms.Controllers
{
    public class RainController : Controller
    {

        //////////////////31-01-19 Start////////////////////////
        public List<SpellData> getPostRainSpellList(DateTime FinalTimeFrom, DateTime FinalTimeTo, string resourceName)
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<RainTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "select DISTINCT r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Rain Guages' and s.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeFrom + "', 103), 121) and s.sheetInsertionDateTime < CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeTo + "', 103), 121) ";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    dtRes.Clear();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    //iterate through the list of resources within the desired set of resources chosen
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        resourceID = Convert.ToInt32(drRes["resourceID"]);
                        //query will get the list of data available against given resourceID (latest first)
                        string Dashdtquery = ";WITH cte AS ( SELECT DISTINCT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, p.parameterUnit as pUnit, r.resourceNumber as rnum, s.sheetInsertionDateTime as tim,  ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = " + resourceID + " AND s.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeFrom + "', 103), 121) and s.sheetInsertionDateTime < CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeTo + "', 103), 121)) SELECT * FROM cte order by cast(rnum as INT) ASC, tim DESC ";
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
                        var tableclass = new RainTableClass();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            spellData.resourceName = dr["Location"].ToString();
                            if (location == "") //first entry of a resource
                            {
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) ), 2).ToString(); //save current level
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
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) );
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) ; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"])  <= 0) //for second to last entry below the level
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
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"]) > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) );
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) );
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) ;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) )
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) , 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                            if (spellData.SpellDataArray.Count > 0)
                            {
                                var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                                spellData.SpellStartTime = spellStartTime;
                                var spellMax = Math.Round(spellData.SpellDataArray.DefaultIfEmpty().Max(), 2);
                                var spellMin = Math.Round(spellData.SpellDataArray.DefaultIfEmpty().Min(), 2);
                                var spellAvg = Math.Round(spellData.SpellDataArray.DefaultIfEmpty().Average(), 2);
                                var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                                double spellPeriod = (Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().FirstOrDefault()) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                                if (spellPeriod == 0)
                                {
                                    spellPeriod = 10;
                                }
                                spellData.spellPeriod = spellPeriod;
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
            spellDataList = spellDataList.Distinct().ToList();
            string c = JsonConvert.SerializeObject(spellDataList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            ViewData["TablData"] = JsonConvert.SerializeObject(tablList);
            return spellDataList;
        }

        public List<SpellData> getRainSpellList()
        {
            DataTable dtRes = new DataTable();
            DataTable Dashdt = new DataTable();
            var spellDataList = new List<SpellData>();
            int resourceID = 0;
            var tablList = new List<RainTableClass>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    string getResFromTemp = "select DISTINCT r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblSheet s on r.resourceID = s.resourceID where rt.resourceTypeName = 'Rain Guages' and s.sheetInsertionDateTime > DATEADD(hour,-24,CURRENT_TIMESTAMP) ";
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
                        var tableclass = new RainTableClass();
                        bool spellOn = false;
                        SpellData spellData = new SpellData();
                        spellData.SpellNumber = 0;
                        string rainEndTime = (Dashdt.Rows[0]["tim"]).ToString();
                        string rainStartTime = (Dashdt.Rows[Dashdt.Rows.Count - 1]["tim"]).ToString();
                        foreach (DataRow dr in Dashdt.Rows)
                        {
                            spellData.resourceName = dr["Location"].ToString();
                            if (location == "") //first entry of a resource
                            {                                
                                spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                location = dr["Location"].ToString(); //save the location
                                currLevl = Math.Round((Convert.ToDouble(dr["Current_Level"]) ), 2).ToString(); //save current level
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
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) );
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true; //determines the continuety of the spell
                                    pondingsummer += Convert.ToDouble(dr["Current_Level"]) ; // adds to the sum of incoming data
                                    pondingentrycounter += 1; //adds 1 to the number of occurances
                                }
                            }
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"])  <= 0) //for second to last entry below the level
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
                            else if (location != "" && Convert.ToDouble(dr["Current_Level"])  > 0) //for second to last entry over the level
                            {
                                if (spellOn == true) //check for ongoing spell
                                {
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) );
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    spellOn = true;
                                }
                                else if (spellOn == false)
                                {
                                    spellCounter += 1; //if location is not cleared, there will be spell
                                    spellData.SpellNumber = spellCounter;
                                    spellData.ResourceId = Convert.ToInt32(drRes["resourceID"]);
                                    spellData.SpellEndTime = Convert.ToDateTime(dr["tim"]).ToString(); //spell end time will be last data
                                    spellData.SpellDataArray.Add(Convert.ToDouble(dr["Current_Level"]) );
                                    spellData.SpellTimeArray.Add(Convert.ToDateTime(dr["tim"]).ToString());
                                    pondingEndTime = Convert.ToDateTime(dr["tim"]); //spell end time will be last data
                                    spellOn = true;
                                }
                                pondingsummer = Convert.ToDouble(currLevl) + Convert.ToDouble(dr["Current_Level"]) ;
                                pondingentrycounter += 1;
                                if (maxLevel < Convert.ToDouble(dr["Current_Level"]) )
                                {
                                    maxLevel = Math.Round(Convert.ToDouble(dr["Current_Level"]) , 2);
                                    maxLevelTime = Convert.ToDateTime(dr["tim"]);
                                }
                            }
                            if (spellData.SpellDataArray.Count > 0)
                            {
                                var spellStartTime = spellData.SpellTimeArray.LastOrDefault();
                                spellData.SpellStartTime = spellStartTime;
                                var spellMax = Math.Round(spellData.SpellDataArray.DefaultIfEmpty().Max(), 2);
                                var spellMin = Math.Round(spellData.SpellDataArray.DefaultIfEmpty().Min(), 2);
                                var spellAvg = Math.Round(spellData.SpellDataArray.DefaultIfEmpty().Average(), 2);
                                var spellLastData = spellData.SpellDataArray.DefaultIfEmpty().FirstOrDefault();
                                double spellPeriod = (Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().FirstOrDefault()) - Convert.ToDateTime(spellData.SpellTimeArray.DefaultIfEmpty().LastOrDefault())).TotalMinutes;
                                if (spellPeriod == 0)
                                {
                                    spellPeriod = 10;
                                }
                                spellData.spellPeriod = spellPeriod;
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
            spellDataList = spellDataList.Distinct().ToList();
            string c = JsonConvert.SerializeObject(spellDataList);
            //c = c.Replace("&quote;","\"");
            ViewData["SpellData"] = c;
            ViewData["TablData"] = JsonConvert.SerializeObject(tablList);
            return spellDataList;
        }
        //////////////////31-01-19 End//////////////////////////


        // GET: Rain
        public List<RainTableClass> getRainTableList()
        {
            var tablList = new List<RainTableClass>();
            DataTable Dashdt = new DataTable();
            string Dashdtquery = ";WITH cte AS ( SELECT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, s.sheetInsertionDateTime AS tim, p.parameterUnit as pUnit, CONVERT(float,s.parameterValue - LEAD(s.parameterValue) OVER (ORDER BY s.sheetInsertionDateTime DESC))/10 AS FLOW_RATE, r.resourceNumber as rnum, s.parameterValue/(CONVERT(float,s.parameterValue - LEAD(s.parameterValue) OVER (ORDER BY s.sheetInsertionDateTime DESC))/10+0.0001) AS EstimatedTime, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Rain Guages' and s.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + DateTime.Now.AddHours(9).Date + "', 103), 121)) SELECT * FROM cte WHERE rn = 1  order by cast(rnum as INT) ASC ";
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
                        var tableclass = new RainTableClass();
                        tableclass.srNo = Convert.ToInt32(Dashdt.Rows.IndexOf(dr) + 1).ToString();
                        tableclass.pondingLocation = dr["Location"].ToString();
                        tableclass.currLevel = Math.Round(Convert.ToDouble(dr["Current_Level"])).ToString();
                        tableclass.pUnit = dr["pUnit"].ToString();
                        tableclass.RecTime = dr["tim"].ToString();
                        if (Dashdt.Rows.Count <= 1)
                        {
                            tableclass.flowRateUp = "-";
                            tableclass.flowRateDown = "-";
                        }
                        else if (Convert.ToDouble(dr["FLOW_RATE"]) > 0)
                        {
                            tableclass.flowRateUp = Math.Abs(Math.Round((Convert.ToDouble(dr["FLOW_RATE"])), 1)).ToString();
                            tableclass.flowRateDown = "-";
                        }
                        else
                        {
                            tableclass.flowRateDown = Math.Abs(Math.Round((Convert.ToDouble(dr["FLOW_RATE"])), 1)).ToString();
                            tableclass.flowRateUp = "-";
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

        public List<RainTableClass> getPostRainTableList(DateTime FinalTimeFrom, DateTime FinalTimeTo)
        {
            var tablList = new List<RainTableClass>();
            DataTable Dashdt = new DataTable();
            string Dashdtquery = ";WITH cte AS ( SELECT r.resourceName AS Location, '3' AS Min_Level, s.parameterValue AS Current_Level, s.sheetInsertionDateTime AS tim, p.parameterUnit as pUnit, CONVERT(float,s.parameterValue - LEAD(s.parameterValue) OVER (ORDER BY s.sheetInsertionDateTime DESC))/10 AS FLOW_RATE, r.resourceNumber as rnum, s.parameterValue/(CONVERT(float,s.parameterValue - LEAD(s.parameterValue) OVER (ORDER BY s.sheetInsertionDateTime DESC))/10+0.0001) AS EstimatedTime, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where rt.resourceTypeName = 'Rain Guages' and s.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeFrom + "', 103), 121) and s.sheetInsertionDateTime < CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeTo + "', 103))) SELECT * FROM cte WHERE rn = 1  order by cast(rnum as INT) ASC ";
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
                        var tableclass = new RainTableClass();
                        tableclass.srNo = Convert.ToInt32(Dashdt.Rows.IndexOf(dr) + 1).ToString();
                        tableclass.pondingLocation = dr["Location"].ToString();
                        tableclass.currLevel = Math.Round(Convert.ToDouble(dr["Current_Level"])).ToString();
                        tableclass.pUnit = dr["pUnit"].ToString();
                        tableclass.RecTime = dr["tim"].ToString();
                        if (Dashdt.Rows.Count<=1)
                        {
                            tableclass.flowRateUp = "-";
                            tableclass.flowRateDown = "-";
                        }
                        else if (Convert.ToDouble(dr["FLOW_RATE"]) > 0)
                        {
                            tableclass.flowRateUp = Math.Abs(Math.Round((Convert.ToDouble(dr["FLOW_RATE"])), 1)).ToString();
                            tableclass.flowRateDown = "-";
                        }
                        else
                        {
                            tableclass.flowRateDown = Math.Abs(Math.Round((Convert.ToDouble(dr["FLOW_RATE"])), 1)).ToString();
                            tableclass.flowRateUp = "-";
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
            string query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages') SELECT * FROM cte WHERE rn = 1";
            string S1query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages') SELECT COUNT(*) FROM cte WHERE rn = 1";
            string S2query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages') SELECT COUNT(*) FROM cte WHERE DeltaMinutes < 30 AND rn = 1";
            string S3query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, s.parameterValue AS ponding, p.parameterName as pName, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime, DATEDIFF(minute, s.sheetInsertionDateTime, GETDATE ()) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages') SELECT COUNT(*) FROM cte WHERE DeltaMinutes > 30 AND rn = 1";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlCommand cmd1 = new SqlCommand(S1query, conn);
                    SqlCommand cmd2 = new SqlCommand(S2query, conn);
                    SqlCommand cmd3 = new SqlCommand(S3query, conn);
                    ViewBag.TotalRain = Convert.ToInt32(cmd1.ExecuteScalar()).ToString();
                    ViewBag.RunningRain = Convert.ToInt32(cmd2.ExecuteScalar()).ToString();
                    ViewBag.InactiveRain = Convert.ToInt32(cmd3.ExecuteScalar()).ToString();
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
            ViewBag.RainMarkers = markers;
            ViewBag.RainjsonRes = JSONresult;
            var tablList = getRainTableList();
            return View(tablList);
        }

        public JsonResult LoadRainChartData()
        {
            DataTable dt = new DataTable();
            string JSONresult = "";
            string query = ";WITH cte AS ( SELECT rt.resourceTypeName as Type_, r.resourceName as category, 'Lahore' as townName, r.resourceName as townName2, p.parameterName as pName, s.parameterValue AS ponding, p.parameterUnit as unit, SUBSTRING(r.resourceGeoLocatin, 1, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) ELSE CHARINDEX(',', r.resourceGeoLocatin) - 1 END) AS lat ,SUBSTRING(r.resourceGeoLocatin, CASE CHARINDEX(',', r.resourceGeoLocatin) WHEN 0 THEN LEN(r.resourceGeoLocatin) + 1 ELSE CHARINDEX(',', r.resourceGeoLocatin) + 1 END, 1000) AS lng, ROW_NUMBER() OVER (PARTITION BY s.resourceID ORDER BY s.sheetInsertionDateTime DESC) AS rn, s.sheetInsertionDateTime FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID inner join tblResourceTypeParameter rtp on rt.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where rt.resourceTypeName = 'Rain Guages') SELECT * FROM cte WHERE rn = 1";
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

        public ActionResult IntesityReport()
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Rain Guages"))
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
                    string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'rain fall'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'rain fall' ";
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Rain Intensity\" },";
                        scriptString += "subtitles: [{text: \" Rain intesity Data Fetched for Today \" }],";
                        scriptString += "axisY: {suffix: \" mm\" },";
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
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + DateTime.Now.AddHours(9).Date+ "', 103), 121) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} mm</strong> at {x}\", ";
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
            var tablList = getRainTableList();
            return View(tablList);
        }
        [HttpPost]
        public ActionResult IntesityReport(FormCollection review)
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
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Rain Guages"))
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
                    string getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where resourceLocationName  = '" + resource + "' and rt.resourceTypeName = 'Rain Guages'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["ResourceID"]) + " and tp.parameterName = 'rain fall'";
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        //scriptString += "title: {text: \"Rain Fall\" },";
                        //scriptString += "subtitles: [{text: \"" + resName + "\" }],";
                        //scriptString += "title: {text: \"Rain Intensity\" },";
                        //scriptString += "subtitles: [{text: \" Rain intesity Data Fetched for Today \" }],";
                        scriptString += "title: {text: \"Rain Intensity\" },";
                        string TheSelectedResource = "";
                        if (resource == "All")
                        {
                            TheSelectedResource = "All Rain Guages";
                        }
                        else
                        {
                            TheSelectedResource = "" + resource + " Rain Guage";
                        }
                        //Session["ReportTitle"] = "Data Fetched for " + TheSelectedResource + " between " + FinalTimeFrom + " to " + FinalTimeTo + "";
                        scriptString += "subtitles: [{text: \" Data Fetched for " + TheSelectedResource + " between " + FinalTimeFrom + " to " + FinalTimeTo + "  \" }],";
                        scriptString += "axisY: {suffix: \"mm\" },";
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
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drRes["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drPar["parameterID"]) + " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeFrom + "', 103), 121) and e.sheetInsertionDateTime < CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeTo + "', 103), 121) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} mm</strong> at {x} \", ";
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
            var tablList = getPostRainTableList(FinalTimeFrom,FinalTimeTo);
            return View(tablList);
        }

        public ActionResult SpellReport()
        {
            rmsWasa01Entities db = new rmsWasa01Entities();
            IList<string> ResourceList = new List<string>();
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Rain Guages"))
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
                    string getResFromTemp = "select  parameterID from tblParameter where parameterName = 'rain fall'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        //string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceTypeParameter rtp on r.resourceTypeID = rtp.resourceTypeID inner join tblParameter p on rtp.parameterID = p.parameterID where p.parameterName = 'rain fall' ";
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        scriptString += "title: {text: \"Rain Intensity\" },";
                        scriptString += "subtitles: [{text: \" Rain intesity Data Fetched for Today \" }],";
                        scriptString += "axisY: {suffix: \" mm\" },";
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
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drPar["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drRes["parameterID"]) + " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + DateTime.Now.AddHours(9).Date + "', 103), 121) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + drPar["resourceLocationName"].ToString() + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} mm</strong> at {x}\", ";
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
            var tablList = getRainSpellList();
            return View(tablList);
        }
        [HttpPost]
        public ActionResult SpellReport(FormCollection review)
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
            foreach (var item in db.tblResources.AsQueryable().Where(item => item.tblResourceType.resourceTypeName == "Rain Guages"))
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
                    string getResFromTemp = "select r.resourceID, r.resourceLocationName from tblResource r inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where resourceLocationName  = '" + resource + "' and rt.resourceTypeName = 'Rain Guages'";
                    SqlDataAdapter sdaRes = new SqlDataAdapter(getResFromTemp, conn);
                    DataTable dtRes = new DataTable();
                    sdaRes.Fill(dtRes);
                    int ite = 0;
                    foreach (DataRow drRes in dtRes.Rows)
                    {
                        string resName = drRes["resourceLocationName"].ToString();
                        ite += 1;
                        string getParamsFromRes = "select tp.parameterID, tp.parameterName from tblParameter tp inner join tblResourceTypeParameter ttp on tp.parameterID = ttp.parameterID inner join tblResource r on ttp.resourceTypeID = r.resourceTypeID where r.resourceID = " + Convert.ToInt32(drRes["ResourceID"]) + " and tp.parameterName = 'rain fall'";
                        SqlDataAdapter sdaPar = new SqlDataAdapter(getParamsFromRes, conn);
                        DataTable dtPar = new DataTable();
                        sdaPar.Fill(dtPar);
                        scriptString += "var chart" + ite + " = new CanvasJS.Chart(\"chartContainer" + ite + "\", {";
                        scriptString += "theme: \"light2\",";
                        scriptString += "animationEnabled: true,";
                        scriptString += "zoomEnabled: true, ";
                        //scriptString += "title: {text: \"Rain Fall\" },";
                        //scriptString += "subtitles: [{text: \"" + resName + "\" }],";
                        scriptString += "title: {text: \"Rain Intensity\" },";
                        string TheSelectedResource = "";
                        if (resource == "All")
                        {
                            TheSelectedResource = "All Rain Guages";
                        }
                        else
                        {
                            TheSelectedResource = "" + resource + " Rain Guage";
                        }
                        //Session["ReportTitle"] = "Data Fetched for " + TheSelectedResource + " between " + FinalTimeFrom + " to " + FinalTimeTo + "";
                        scriptString += "subtitles: [{text: \" Data Fetched for " + TheSelectedResource + " between " + FinalTimeFrom + " to " + FinalTimeTo + "  \" }],";
                        scriptString += "axisY: {suffix: \"mm\" },";
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
                            aquery += "WHERE e.resourceID = " + Convert.ToInt32(drRes["resourceID"]) + " and e.parameterID = " + Convert.ToInt32(drPar["parameterID"]) + " and e.sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeFrom + "', 103), 121) and e.sheetInsertionDateTime < CONVERT(CHAR(24), CONVERT(DATETIME, '" + FinalTimeTo + "', 103), 121) ";
                            aquery += ") ";
                            aquery += "SELECT top 144000 parameterID, parameterValue, sheetInsertionDateTime FROM CTE WHERE RN < 144001 Order by sheetInsertionDateTime ASC";
                            string theQuery = aquery;
                            SqlDataAdapter sdaVal = new SqlDataAdapter(theQuery, conn);
                            DataTable dtVal = new DataTable();
                            sdaVal.Fill(dtVal);
                            scriptString += "{ type: \"line\", name: \"" + parName + "\", showInLegend: true,  markerSize: 0, xValueType: \"dateTime\", xValueFormatString: \"HH:mm:ss DD-MM-YYYY\", yValueFormatString: \"#,##0.##\", toolTipContent: \"{label}<br/>{name}, <strong>{y} mm</strong> at {x} \", ";
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
            var tablList = getPostRainSpellList(FinalTimeFrom, FinalTimeTo, resource);
            return View(tablList);
        }
    }
}