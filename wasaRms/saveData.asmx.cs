using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;

namespace wasaRms
{
    /// <summary>
    /// Summary description for saveData
    /// </summary>
    /// FOR WASA TTLOGIX
    [WebService(Namespace = "http://wasarms.ttlogix.com/")]
    /// FOR WASA AMAZON WEB SERVER
    //[WebService(Namespace = "http://http://35.166.74.72/")]
    /// FOR LOCALHOST
    //[WebService(Namespace = "http://localhost/")]


    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class saveData : System.Web.Services.WebService
    {
        public int c_id;

        [WebMethod]

        public void getDashboardTable()
        {
            List<DashboardTableClass> list = new List<DashboardTableClass>();
            DataTable dt = new DataTable();
            DashboardTableClass dtc = new DashboardTableClass();
            using (SqlConnection con1 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string query = "";
                query += ";WITH cte AS ( SELECT* FROM ( SELECT DISTINCT r.resourceName AS Location, r.resourceID, p.parameterName AS pID, s.parameterValue AS pVal, s.sheetInsertionDateTime as tim , DATEDIFF(minute, s.sheetInsertionDateTime, DATEADD(hour, 0,GETDATE ())) as DeltaMinutes FROM tblSheet s inner join tblResource r on s.resourceID = r.resourceID inner join tblParameter p on s.parameterID = p.parameterID inner join tblResourceType rt on r.resourceTypeID = rt.resourceTypeID where r.resourceID = 1085 and sheetInsertionDateTime >= CONVERT(CHAR(24), CONVERT(DATETIME, '" + DateTime.Now.AddMinutes(-30) + "', 103), 121)  ) AS SourceTable PIVOT ( SUM(pVal) FOR pID IN ( [V1N (V)],[V2N (V)],[V3N (V)],[I1 (A)],[I2 (A)],[I3 (A)],[W (kwatt)],[VAR (kvar)],[VA (kva)],[VA-SUM (kva)],[PF],[Freq (Hz)],[V12 (v)],[V23 (v)],[V13 (v)],[V1 THD (%)],[V2 THD (%)],[V3 THD (%)],[P1 Status],[P2 Status],[P3 Status],[P4 Status],[P1 Auto/Mannual],[P2 Auto/Mannual],[P3 Auto/Mannual],[P4 Auto/Mannual],[Tank Level1 (ft)] ) ) AS PivotTable ) SELECT* FROM cte order by cast(resourceID as INT) ASC, tim DESC ";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con1;
                    con1.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(dt);
                        if (dt.Rows.Count > 1) 
                        {
                            dt.Rows.RemoveAt(0);
                            var abc = Math.Round(((Convert.ToDouble(dt.Rows[0][30])) - (Convert.ToDouble(dt.Rows[1][30])) / 3), 2);
                            dtc.srNo = "1";
                            dtc.location = dt.Rows[0][0].ToString();
                            dtc.currentLevel = Math.Round((Convert.ToDouble(dt.Rows[0][30])), 1).ToString();
                            dtc.waterInGallon = (String.Format("{0:n0}", Math.Round((Convert.ToDouble(dt.Rows[0][30]) * 89695.23), 2))).ToString() + " Gallons/1,345,428 Gallons";
                            dtc.waterPercentage = Math.Round((Convert.ToDouble(dt.Rows[0][30]) * 89695.23) / 13394.2846, 0).ToString() + " %";
                            dtc.currentTime = dt.Rows[0][2].ToString();
                            double a = Convert.ToDouble(dt.Rows[0][30]);
                            double b = Convert.ToDouble(dt.Rows[1][30]);
                            double in_out = (-1) * Math.Round(((b - a) / 3), 3);
                            dtc.inOutIntensity = in_out.ToString() + " (ft/min)";
                            list.Add(dtc);
                        }
                        else
                        {
                            dtc.srNo = "1";
                            dtc.location = "Lawrence Road Storage Tank";
                            dtc.currentLevel = "Inactive";
                            dtc.waterInGallon = "Inactive";
                            dtc.waterPercentage = "Inactive";
                            dtc.currentTime = "Inactive";
                            dtc.inOutIntensity = "Inactive";
                        }
                    }
                    con1.Close();

                }
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Write(js.Serialize(list));
        }

        [WebMethod]
        public string getMarkers()
        {

            string tempName = "";
            string datetimed = "";
            string markers = "[";
            string parameterValuesString = "";
            string q1 = "";
            q1 += "select top(28)  r.resourceLocationName,  t.resourceTypeName, p.ParameterName, e.parameterValue, e.sheetInsertionDateTime  from tblSheet e ";
            q1 += "left join tblParameter p on e.parameterID = p.parameterID ";
            q1 += "left join tblResource r on e.resourceID = r.resourceID ";
            q1 += "left join tblResourceType t on r.resourceTypeID = t.resourceTypeID ";
            q1 += "where e.sheetInsertionDateTime = (select max(sheetInsertionDateTime) from tblSheet where ResourceID = 1085) ";
            q1 += "and r.resourceID = 1085 order by p.parameterMaxThr";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(q1, conn);
                    conn.Open();
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
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    conn.Close();
                }

            }
            markers = markers.Remove(markers.Length - 1, 1);
            markers += "]";
            var data = new { status = "Success" };
            return markers;
        }

        [WebMethod]
        public string insertData(string code, string val, string time)
        {
            string Message;
            BAL bal = new BAL();
            int result = bal.saveData(code, val, time);
            if (result == 1)
            {
                Message = "Details are inserted successfully";
            }
            else
            {
                Message = "Details are not inserted successfully";
            }
            return Message;
        }

        [WebMethod]
        public string tankThreshold()
        {
            double thresholdValue = 0;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string query = "select maxThr from tblResource where resourceID = 1085 ";

                    SqlCommand cmdIn = new SqlCommand(query, conn);
                    conn.Open();
                    thresholdValue = Convert.ToDouble(cmdIn.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    thresholdValue = 12;
                }
                conn.Close();
            }
            return ""+thresholdValue+"";
        }

        [WebMethod]
        public string getModeStatus()
        {
            int mode, b1, b2, b3, b4;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                try
                {
                    string query = "select modeStatus from tblResource where resourceID = 1085 ";
                    string btn1Query = "select paramButtonStatus from tblParameter where parameterID = 1026";
                    string btn2Query = "select paramButtonStatus from tblParameter where parameterID = 1027";
                    string btn3Query = "select paramButtonStatus from tblParameter where parameterID = 1028";
                    string btn4Query = "select paramButtonStatus from tblParameter where parameterID = 1029";
                    SqlCommand cmdIn = new SqlCommand(query, conn);
                    SqlCommand cmdb1 = new SqlCommand(btn1Query, conn);
                    SqlCommand cmdb2 = new SqlCommand(btn2Query, conn);
                    SqlCommand cmdb3 = new SqlCommand(btn3Query, conn);
                    SqlCommand cmdb4 = new SqlCommand(btn4Query, conn);
                    conn.Open();
                    mode = Convert.ToInt32(cmdIn.ExecuteScalar());
                    b1 = Convert.ToInt32(cmdb1.ExecuteScalar());
                    b2 = Convert.ToInt32(cmdb2.ExecuteScalar());
                    b3 = Convert.ToInt32(cmdb3.ExecuteScalar());
                    b4 = Convert.ToInt32(cmdb4.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    mode = 0;
                    b1 = 0;
                    b2 = 0;
                    b3 = 0;
                    b4 = 0;
                }
                conn.Close();
            }
            return "{\"mode\":\""+mode+ "\",\"B1\":\"" + b1 + "\",\"B2\":\"" + b2 + "\",\"B3\":\"" + b3 + "\",\"B4\":\"" + b4 + "\"}";
        }

        [WebMethod]
        public string insertUpdateData(string code, string val, string time, string lat, string lng)
        {
            string Message;
            BAL bal = new BAL();
            int result = bal.saveData(code, val, time, lat, lng);
            if (result == 1)
            {
                Message = "Details are inserted successfully";
            }
            else
            {
                Message = "Details are not inserted successfully";
            }
            return Message;
        }

        [WebMethod]
        public string execQuery(string query)
        {
            BAL bal = new BAL();
            string result = bal.execQuery(query);
            return result;
        }

        [WebMethod]
        public string stormWaterApi(string code, string data, string time)
        {
            string Message;
            BAL bal = new BAL();
            int result = bal.saveStormWaterData(code, data, time);
            if (result == 1)
            {
                Message = "Details are inserted successfully";
            }
            else
            {
                Message = "Details are not inserted successfully";
            }
            return Message;
        }
        [WebMethod]
        public string tankLevelApi(string code, string data, string time)
        {
            string Message;
            BAL bal = new BAL();
            int result = bal.saveTankLevelData(code, data, time);
            if (result == 1)
            {
                Message = "Details are inserted successfully";
            }
            else
            {
                Message = "Details are not inserted successfully";
            }
            return Message;
        }

        [WebMethod]
        public string GetTankLevel(string ResourceNumber)
        {
            BAL bal = new BAL();
            double returnStatus = bal.GetTankLevel(ResourceNumber);
            return returnStatus.ToString();
        }
    }
}
