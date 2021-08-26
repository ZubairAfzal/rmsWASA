using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wasaRms
{
    public class StorageParameterChartClass
    {
        public string locationName { get; set; }
        public string parameterName { get; set; }
        public double totalHours { get; set; }
        public double workingInHours { get; set; }
        public double nonWorkingInHours { get; set; }
        public double workingInHoursRemote { get; set; }
        public double workingInHoursManual { get; set; }
        public double workingInHoursScheduling { get; set; }
        public double availableHours { get; set; }
        public double nonAvailableHours { get; set; }

        public string totalHoursString { get; set; }
        public string workingInHoursString { get; set; }
        public string nonWorkingInHoursString { get; set; }
        public string workingInHoursRemoteString { get; set; }
        public string workingInHoursManualString { get; set; }
        public string workingInHoursSchedulingString { get; set; }
        public string availableHoursString { get; set; }
        public string nonAvailableHoursString { get; set; }


        public double minValue { get; set; }
        public double maxValue { get; set; }
        public double avgVale { get; set; }

        public double avgOfAvailableHours { get; set; }
        public double avgOfNonAvailableHours { get; set; }
    }
}