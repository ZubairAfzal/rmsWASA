using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wasaRms
{
    public class StorageTankClass
    {
        public string V1NV { get; set; }
        public string V2NV { get; set; }
        public string V3NV { get; set; }
        public string I1A { get; set; }
        public string I2A { get; set; }
        public string I3A { get; set; }
        public string Wkwatt { get; set; }
        public string VARkvar { get; set; }
        public string VAkva { get; set; }
        public string VASUMkva { get; set; }
        public string PF { get; set; }
        public string FreqHz { get; set; }
        public string V12v { get; set; }
        public string V23v { get; set; }
        public string V13v { get; set; }
        public string V1THD { get; set; }
        public string V2THD { get; set; }
        public string V3THD { get; set; }
        public string P1Status { get; set; }
        public string P2Staus { get; set; }
        public string CurrentTrip1 { get; set; }
        public string CurrentTrip2 { get; set; }
        public string VoltageTrip1 { get; set; }
        public string VoltageTrip2 { get; set; }
        public string P1AutoMannual { get; set; }
        public string P2AutoMannual { get; set; }
        public string TankLevel1ft { get; set; }
        public string TankLevel2ft { get; set; }
    }
}