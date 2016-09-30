using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RiotSharp.MatchEndpoint;
using RiotSharp.StaticDataEndpoint;

namespace WpfApplication1
{
    public class AllMatches
    {
        #region Properties
        
        #region Player

        public long GameId { get; set; }
        public string GameDate { get; set; }
        public string Result{ get; set; }
        public string Player{ get; set; }
        public string Champion{ get; set; }
        public long Kills{ get; set; }
        public long Deaths{ get; set; }
        public long Assists{ get; set; }
        public long Wards{ get; set; }
        public double Wards10{ get; set; }
        public double Wards20{ get; set; }
        public double Wards30{ get; set; }
        public long Pinks{ get; set; }
        public long CS{ get; set; }
        public double CS10{ get; set; }
        public double CS20{ get; set; }
        public double CS30{ get; set; }
        public double CSDiff10{ get; set; }
        public double CSDiff20{ get; set; }
        public double CSDiff30{ get; set; }
        public double CSDiffEnd{ get; set; }
        public long Gold{ get; set; }
        public long DamageOutput{ get; set; }
        public int FirstBlood{ get; set; }

        #endregion

        #region Team

        public int FirstDrake{ get; set; }
        public int FirstBaron{ get; set; }
        public long Drakes{ get; set; }
        public long DrakesGiven{ get; set; }
        public long Barons{ get; set; }
        public long BaronsGiven{ get; set; }
        public int FirstTurret{ get; set; }
        public double TeamGoldDiff10{ get; set; }
        public double TeamGoldDiff20{ get; set; }
        public double TeamGoldDiff30{ get; set; }
        public double TeamGoldDiffEnd{ get; set; }

        #endregion

        #endregion

    }
}
