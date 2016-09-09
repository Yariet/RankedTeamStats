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
        
        #region Private

        private string gameDate;
        private string result;
        private string player;
        private string champion;
        private long kills;
        private long deaths;
        private long assists;
        private long wards;
        private long pinks;
        private long cs;
        private long gold;
        private long damageOutput;
        private bool firstBlood;

        #endregion

        #region Public

        public string GameDate 
        {
            get { return gameDate; }
            set { gameDate = value; }
        }
        public string Result
        {
            get { return result; }
            set { result = value; }
        }
        public string Player
        {
            get { return player; }
            set { player = value; }
        }
        public string Champion
        {
            get { return champion; }
            set { champion = value; }
        }
        public long Kills
        {
            get { return kills; }
            set { kills = value; }
        }
        public long Deaths
        {
            get { return deaths; }
            set { deaths = value; }
        }
        public long Assists
        {
            get { return assists; }
            set { assists = value; }
        }
        public long Wards
        {
            get { return wards; }
            set { wards = value; }
        }
        public long Pinks
        {
            get { return pinks; }
            set { pinks = value; }
        }

        public long CS
        {
            get { return cs; }
            set { cs = value; }
        }

        public long Gold
        {
            get { return gold; }
            set { gold = value; }
        }

        public long DamageOutput
        {
            get { return damageOutput; }
            set { damageOutput = value; }
        }

        public bool FirstBlood
        {
            get { return firstBlood; }
            set { firstBlood = value; }
        }

        #endregion

        #endregion
    }
}
