using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wasaRms
{
    public class RainTableClass
    {
        public string srNo { get; set; }
        public string pondingLocation { get; set; }
        public string currLevel { get; set; }
        public string pUnit { get; set; }
        public string clearanceTime { get; set; }
        public string flowRateUp { get; set; }
        public string flowRateDown { get; set; }
        public string estClTime { get; set; }
        public string maxLevel { get; set; }
        public string maxLevelTime { get; set; }
        public string avgPonding { get; set; }
        public string pondingPeriod { get; set; }
        public string RecTime { get; set; }
    }
}