using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace wasaRms
{
    public class StorageTankTableData
    {
        [Display(Name = "Location Name")]
        public string locationName { get; set; }
        public double WorkingInHours { get; set; }

        public double P1WorkingInHours { get; set; }
        public double P2WorkingInHours { get; set; }
        public double P3WorkingInHours { get; set; }
        public double P4WorkingInHours { get; set; }
        public double P1WorkingInHoursManual { get; set; }
        public double P2WorkingInHoursManaul { get; set; }
        public double P3WorkingInHoursManaul { get; set; }
        public double P4WorkingInHoursManaul { get; set; }
        public double P1WorkingInHoursAuto { get; set; }
        public double P2WorkingInHoursAuto { get; set; }
        public double P3WorkingInHoursAuto { get; set; }
        public double P4WorkingInHoursAuto { get; set; }
        public double P1WorkingInHoursRemote { get; set; }
        public double P2WorkingInHoursRemote { get; set; }
        public double P3WorkingInHoursRemote { get; set; }
        public double P4WorkingInHoursRemote { get; set; }
        public List<double> pumpStatus1 { get; set; }
        public List<double> pumpStatus2 { get; set; }
        public string workingHoursTodayP1 { get; set; }
        public string workingHoursTodayP2 { get; set; }
        public string workingHoursTodayP3 { get; set; }
        public string workingHoursTodayP4 { get; set; }
        public string workingHoursTodayRemoteP1 { get; set; }
        public string workingHoursTodayManualP1 { get; set; }
        public string workingHoursTodaySchedulingP1 { get; set; }
        public string workingHoursTodayRemoteP2 { get; set; }
        public string workingHoursTodayManualP2 { get; set; }
        public string workingHoursTodaySchedulingP2 { get; set; }

        public string workingHoursTodayRemoteP3 { get; set; }
        public string workingHoursTodayManualP3 { get; set; }
        public string workingHoursTodaySchedulingP3 { get; set; }
        public string workingHoursTodayRemoteP4 { get; set; }
        public string workingHoursTodayManualP4 { get; set; }
        public string workingHoursTodaySchedulingP4 { get; set; }
        public List<double> waterFlow { get; set; }
        public string waterDischarge { get; set; }
        public string accWaterDischargePerDay { get; set; }
        public List<double> CurrentTrip1 { get; set; }
        public List<double> CurrentTrip2 { get; set; }
        public List<double> FreqHz { get; set; }
        public List<double> I1A { get; set; }
        public List<double> I2A { get; set; }
        public List<double> I3A { get; set; }
        public List<double> P1AutoMannual { get; set; }
        public List<double> P1Status { get; set; }
        public List<double> P2AutoMannual { get; set; }
        public List<double> P2Status { get; set; }

        public List<double> P3AutoMannual { get; set; }
        public List<double> P3Status { get; set; }
        public List<double> P4AutoMannual { get; set; }
        public List<double> P4Status { get; set; }

        public List<double> PF { get; set; }
        public List<double> TankLevel1ft { get; set; }
        public List<double> TankLevel2ft { get; set; }
        public List<double> V1THD { get; set; }
        public List<double> V12_V { get; set; }
        public List<double> V13_V { get; set; }
        public List<double> V1N_V { get; set; }
        public List<double> V2THD { get; set; }
        public List<double> V23_V { get; set; }
        public List<double> V2N_V { get; set; }
        public List<double> V3THD { get; set; }
        public List<double> V3N_V { get; set; }
        public List<double> VA_kva { get; set; }
        public List<double> VA_SUM_kva { get; set; }
        public List<double> VAR_kvar { get; set; }
        public List<double> VoltageTrip1 { get; set; }
        public List<double> VoltageTrip2 { get; set; }
        public List<double> W_kwatt { get; set; }
        public string chartString { get; set; }
        public List<string> SpellTimeArray = new List<string>();
        public string tankLevelAverage { get; set; }


    }
}