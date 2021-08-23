using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wasaRms
{
    public class PondingSpellClass
    {
        public string srNo { get; set; }
        public string pondingLocation { get; set; }
        public string currLevel { get; set; }
        public string currTime { get; set; }
        public string pUnit { get; set; }
        public string clearanceTime { get; set; }
        public string flowRateUp { get; set; }
        public string flowRateDown { get; set; }
        public string estClTime { get; set; }
        public string maxLevel { get; set; }
        public string maxLevelTime { get; set; }
        public string avgPonding { get; set; }
        public string pondingPeriod { get; set; }
        public string deltaTime { get; set; }
        public string comment { get; set; }
        public string NoPondingComment { get; set; }
        public string minThr { get; set; }
        public string maxThr { get; set; }
        public string date { get; set; }
        public int SpellNumber { get; set; }
        public string SpellStartTime { get; set; }
        public string SpellEndTime { get; set; }
        public double SpellMin { get; set; }
        public double SpellMax { get; set; }
        public double SpellAvg { get; set; }
        public string spellFlowUp { get; set; }
        public string spellFlowDown { get; set; }
        public List<double> SpellDataArray = new List<double>();
        public List<string> SpellTimeArray = new List<string>();
        public int ResourceId { get; set; }
        public string resourceName { get; set; }
        public string spellMaxTime { get; set; }
        public string spellMinTime { get; set; }
        public string spellPeriod { get; set; }
        public string estSpellClearanceTime { get; set; }
    }
}