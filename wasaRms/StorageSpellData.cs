using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wasaRms
{
    public class StoragePump1SpellData
    {

        public int spellMode { get; set; }
        public double spellPeriod { get; set; }
        public int SpellNumber { get; set; }
        public string SpellStartTime { get; set; }
        public string SpellEndTime { get; set; }
        public List<double> SpellDataArray = new List<double>();
        public List<string> SpellTimeArray = new List<string>();
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public double SpellPeriod { get; set; }
        public List<double> WellLevel1 = new List<double>();
        public int SpellMode { get; set; }
    }
    public class StoragePump2SpellData
    {
        public int SpellNumber { get; set; }
        public string SpellStartTime { get; set; }
        public string SpellEndTime { get; set; }
        public List<double> SpellDataArray = new List<double>();
        public List<string> SpellTimeArray = new List<string>();
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public double SpellPeriod { get; set; }
        public List<double> WellLevel1 = new List<double>();
        public int SpellMode { get; set; }
    }
}