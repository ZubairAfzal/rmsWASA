using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;

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
