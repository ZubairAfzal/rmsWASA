using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wasaRms
{
    public class summaryPondingReportTable
    {
        public string srNo { get; set; }
        public string reportDate { get; set; }
        public string reportTime { get; set; }
        public string rainStopTime { get; set; }
        public string pondingPoint { get; set; }
        public string statusCurrent { get; set; }
        public string clearanceTime { get; set; }
        public string rainDuration { get; set; }
        public string maxPondingLevel { get; set; }
    }
}