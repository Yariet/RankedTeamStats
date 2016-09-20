using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using RiotSharp;
using RiotSharp.ChampionEndpoint;
using RiotSharp.GameEndpoint;
using RiotSharp.MatchEndpoint;
using RiotSharp.MatchEndpoint.Enums;
using RiotSharp.StaticDataEndpoint.Champion;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        private RiotApi api = RiotApi.GetInstance(ConfigurationManager.AppSettings["ApiKey"]);
        private StaticRiotApi staticApi = StaticRiotApi.GetInstance(ConfigurationManager.AppSettings["ApiKey"]);
        private StatusRiotApi statusApi = StatusRiotApi.GetInstance();
        private Region region = (Region)Enum.Parse(typeof(Region), ConfigurationManager.AppSettings["Region"]);
        private long summonerId = long.Parse(ConfigurationManager.AppSettings["Summoner1Id"]);
        private List<Queue> queue = new List<Queue>();
        private List<Season> season = new List<Season>();
        private DateTime beginDate = new DateTime(2016, 9, 1);
        private List<MatchDetail> matchesDetails = new List<MatchDetail>();
        private List<Label> label = new List<Label>();
        private List<AllMatches> Matches = new List<AllMatches>();
        private AllMatches singleMatch;
        private MyConverter myConverter = new MyConverter();

        DataGrid dghc = null;
        private IEnumerable<StackPanel> collection;

        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
            queue.Add(Queue.RankedTeam5x5); 
            season.Add(Season.Season2016);
            LoadTeamGames();
            ICollectionView matchesView = CollectionViewSource.GetDefaultView(Matches);

            matchesView.GroupDescriptions.Add(new PropertyGroupDescription("Player"));

            GamesHistoryGrid.DataContext = matchesView;

        }
        #endregion

        #region Methods

        #region LeagueMethods
        public void LoadTeamGames()
        {
            var matchList = api.GetMatchList(region, summonerId, null, queue, season, beginDate, DateTime.Now);
            for (int i = 0; i < matchList.Matches.Count; i++)
            {
                matchesDetails.Add(api.GetMatch(region, matchList.Matches[i].MatchID, true));
                Thread.Sleep(1000);
                while (matchesDetails.Last().Teams == null)
                {
                    matchesDetails[matchesDetails.IndexOf(matchesDetails.Last())] = api.GetMatch(region, matchList.Matches[i].MatchID, true);
                    Thread.Sleep(1000);
                }

            }

            PrintMatchesDatas();
        }

        public void PrintMatchesDatas()
        {
            foreach (MatchDetail match in matchesDetails)
            {
                foreach (Participant player in match.Participants)
                {
                    for (int z = 0; z < match.ParticipantIdentities.Count; z++)
                    {
                        if ((match.ParticipantIdentities[z].Player.SummonerId == summonerId ||
                        match.ParticipantIdentities[z].Player.SummonerId == long.Parse(ConfigurationManager.AppSettings["Summoner2Id"]) ||
                        match.ParticipantIdentities[z].Player.SummonerId == long.Parse(ConfigurationManager.AppSettings["Summoner3Id"]) ||
                        match.ParticipantIdentities[z].Player.SummonerId == long.Parse(ConfigurationManager.AppSettings["Summoner4Id"]) ||
                        match.ParticipantIdentities[z].Player.SummonerId == long.Parse(ConfigurationManager.AppSettings["Summoner5Id"]))
                            &&
                        match.ParticipantIdentities[z].ParticipantId == player.ParticipantId)
                        {
                            singleMatch = new AllMatches();
                            int team = 0;
                            int wardsMin;
                            
                            //Match Date
                            singleMatch.GameDate = match.MatchCreation.ToShortDateString();

                            //Result
                            if (player.TeamId == match.Teams[0].TeamId)
                                if (match.Teams[0].Winner)
                                    singleMatch.Result = "Victory";
                                else singleMatch.Result = "Defeat";
                            else
                            {
                                team = 1;
                                if (match.Teams[1].Winner)
                                    singleMatch.Result = "Victory";
                                else singleMatch.Result = "Defeat";
                            }

                            //Player
                            foreach (ParticipantIdentity singleParticipantIdentity in match.ParticipantIdentities)
                            {
                                if (singleParticipantIdentity.ParticipantId == player.ParticipantId)
                                {
                                    singleMatch.Player = singleParticipantIdentity.Player.SummonerName;
                                    break;
                                }
                            }

                            //Champion
                            singleMatch.Champion = staticApi.GetChampion(region, player.ChampionId).Name;
                            Thread.Sleep(1000);

                            //Kills
                            singleMatch.Kills = player.Stats.Kills;

                            //Deaths
                            singleMatch.Deaths = player.Stats.Deaths;

                            //Assists
                            singleMatch.Assists = player.Stats.Assists;

                            //Gold
                            singleMatch.Gold = player.Stats.GoldEarned;

                            //CS
                            singleMatch.CS = player.Stats.MinionsKilled;

                            //CS@10
                            singleMatch.CS10 = player.Timeline.CreepsPerMinDeltas.ZeroToTen;

                            //CS@20
                            singleMatch.CS20 = player.Timeline.CreepsPerMinDeltas.TenToTwenty;

                            //CS@30
                            singleMatch.CS30 = player.Timeline.CreepsPerMinDeltas.TwentyToThirty;

                            //CSDiff@10
                            try
                            {
                                singleMatch.CSDiff10 = player.Timeline.CsDiffPerMinDeltas.ZeroToTen;
                            }
                            catch
                            { }

                            //CSDiff@20
                            try
                            {
                                singleMatch.CSDiff20 = player.Timeline.CsDiffPerMinDeltas.TenToTwenty;
                            }
                            catch
                            { }

                            //CSDiff@30
                            try
                            {
                                singleMatch.CSDiff30 = player.Timeline.CsDiffPerMinDeltas.TwentyToThirty;
                            }
                            catch
                            { }

                            //CSDiff@END
                            try
                            {
                                singleMatch.CSDiffEnd = player.Timeline.CsDiffPerMinDeltas.ThirtyToEnd;
                            }
                            catch
                            { }

                            //Wards
                            singleMatch.Wards = player.Stats.WardsPlaced;

                            //Wards@10
                            wardsMin = 0;
                            for (int i = 0; i < match.Timeline.Frames.Count; i++)
                            {
                                if (match.Timeline.Frames[i].Timestamp.Minutes <= 10)
                                {
                                    if(match.Timeline.Frames[i].Events != null)
                                        for (int k = 0; k < match.Timeline.Frames[i].Events.Count; k++)
                                        {
                                            if (match.Timeline.Frames[i].Events[k].EventType == EventType.WardPlaced)
                                                if (match.Timeline.Frames[i].Events[k].CreatorId == player.ParticipantId && match.Timeline.Frames[i].Events[k].WardType != WardType.Undefined && match.Timeline.Frames[i].Events[k].WardType != WardType.TeemoMushroom)
                                                    wardsMin += 1;
                                        }
                                }
                            }
                            singleMatch.Wards10 = wardsMin;

                            //Wards@20
                            wardsMin = 0;
                            for (int i = 0; i < match.Timeline.Frames.Count; i++)
                            {
                                if (match.Timeline.Frames[i].Timestamp.Minutes <= 20 && match.Timeline.Frames[i].Timestamp.Minutes > 10)
                                {
                                    if (match.Timeline.Frames[i].Events != null)
                                        for (int k = 0; k < match.Timeline.Frames[i].Events.Count; k++)
                                        {
                                            if (match.Timeline.Frames[i].Events[k].EventType == EventType.WardPlaced)
                                                if (match.Timeline.Frames[i].Events[k].CreatorId == player.ParticipantId && match.Timeline.Frames[i].Events[k].WardType != WardType.Undefined && match.Timeline.Frames[i].Events[k].WardType != WardType.TeemoMushroom)
                                                    wardsMin += 1;
                                        }
                                }
                            }
                            singleMatch.Wards20 = wardsMin;

                            //Wards@30
                            wardsMin = 0;
                            for (int i = 0; i < match.Timeline.Frames.Count; i++)
                            {
                                if (match.Timeline.Frames[i].Timestamp.Minutes <= 30 && match.Timeline.Frames[i].Timestamp.Minutes > 20)
                                {
                                    if (match.Timeline.Frames[i].Events != null)
                                        for (int k = 0; k < match.Timeline.Frames[i].Events.Count; k++)
                                        {
                                            if (match.Timeline.Frames[i].Events[k].EventType == EventType.WardPlaced)
                                                if (match.Timeline.Frames[i].Events[k].CreatorId == player.ParticipantId && match.Timeline.Frames[i].Events[k].WardType != WardType.Undefined && match.Timeline.Frames[i].Events[k].WardType != WardType.TeemoMushroom)
                                                    wardsMin += 1;
                                        }
                                }
                            }
                            singleMatch.Wards30 = wardsMin;

                            //Pinks
                            singleMatch.Pinks = player.Stats.VisionWardsBoughtInGame;

                            //Damage Output
                            singleMatch.DamageOutput = player.Stats.TotalDamageDealtToChampions;

                            //First Blood
                            singleMatch.FirstBlood = player.Stats.FirstBloodKill;


                            //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@   TEAM STATS   @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                            //First Dragon
                            singleMatch.FirstDrake = match.Teams[team].FirstDragon;

                            //First Baron
                            singleMatch.FirstBaron = match.Teams[team].FirstBaron;

                            //First Turret
                            singleMatch.FirstTurret = match.Teams[team].FirstTower;

                            //Drakes killed
                            singleMatch.Drakes = match.Teams[team].DragonKills;

                            //Drakes Given
                            singleMatch.DrakesGiven = team == 1 ? match.Teams[team - 1].DragonKills : match.Teams[team + 1].DragonKills;

                            //Barons killed
                            singleMatch.Barons = match.Teams[team].BaronKills;

                            //Barons Given
                            singleMatch.BaronsGiven = team == 1 ? match.Teams[team - 1].BaronKills : match.Teams[team + 1].BaronKills;

                            //Team GoldDiff@10
                            double teamGold= 0;
                            foreach (var goldPlayer in match.Participants)
                            {
                                if(goldPlayer.TeamId == match.Teams[team].TeamId)
                                    teamGold += (goldPlayer.Timeline.GoldPerMinDeltas.ZeroToTen * 10);
                                else
                                    teamGold -= (goldPlayer.Timeline.GoldPerMinDeltas.ZeroToTen * 10);
                            }
                            singleMatch.TeamGoldDiff10 = teamGold;

                            //Team GoldDiff@20
                            teamGold = 0;
                            foreach (var goldPlayer in match.Participants)
                            {
                                if (goldPlayer.TeamId == match.Teams[team].TeamId)
                                    teamGold += (goldPlayer.Timeline.GoldPerMinDeltas.TenToTwenty * 10);
                                else
                                    teamGold -= (goldPlayer.Timeline.GoldPerMinDeltas.TenToTwenty * 10);
                            }
                            singleMatch.TeamGoldDiff20 = teamGold;

                            //Team GoldDiff@30
                            teamGold = 0;
                            foreach (var goldPlayer in match.Participants)
                            {
                                if (goldPlayer.TeamId == match.Teams[team].TeamId)
                                    teamGold += (goldPlayer.Timeline.GoldPerMinDeltas.TwentyToThirty * 10);
                                else
                                    teamGold -= (goldPlayer.Timeline.GoldPerMinDeltas.TwentyToThirty * 10);
                            }
                            singleMatch.TeamGoldDiff30 = teamGold;

                            //Team GoldDiff@End
                            teamGold = 0;
                            foreach (var goldPlayer in match.Participants)
                            {
                                if (goldPlayer.TeamId == match.Teams[team].TeamId)
                                    teamGold += (goldPlayer.Timeline.GoldPerMinDeltas.ThirtyToEnd * 10);
                                else
                                    teamGold -= (goldPlayer.Timeline.GoldPerMinDeltas.ThirtyToEnd * 10);
                            }
                            singleMatch.TeamGoldDiffEnd = teamGold;

                            Matches.Add(singleMatch);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

    }

    public static class Extensions
    {
        public static T FindParentOfType<T>(this FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)
            {
                if (parent is T)
                    return (T)(object)parent;

                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
            return default(T);
        }

        // Methods
        public static List<T> GetChildrenByType<T>(this UIElement element) where T : UIElement
        {
            return element.GetChildrenByType<T>(null);
        }

        public static List<T> GetChildrenByType<T>(this UIElement element, Func<T, bool> condition) where T : UIElement
        {
            List<T> results = new List<T>();
            GetChildrenByType<T>(element, condition, results);
            return results;
        }

        private static void GetChildrenByType<T>(UIElement element, Func<T, bool> condition, List<T> results) where T : UIElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(element, i) as UIElement;
                if (child != null)
                {
                    T t = child as T;
                    if (t != null)
                    {
                        if (condition == null)
                        {
                            results.Add(t);
                        }
                        else if (condition(t))
                        {
                            results.Add(t);
                        }
                    }
                    GetChildrenByType<T>(child, condition, results);
                }
            }
        }

        public static bool HasChildrenByType<T>(this UIElement element, Func<T, bool> condition) where T : UIElement
        {
            return (element.GetChildrenByType<T>(condition).Count != 0);
        }
    }

    public class MyConverter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            CollectionViewGroup cvg = value as CollectionViewGroup;
            string param = parameter as string;
            switch (param)
            {
                case "Kills":
                    return cvg.Items.Sum(x => (x as AllMatches).Kills);
                case "Deaths":
                    return cvg.Items.Sum(x => (x as AllMatches).Deaths);
                case "Assists":
                    return cvg.Items.Sum(x => (x as AllMatches).Assists);
                case "Wards":
                    return cvg.Items.Sum(x => (x as AllMatches).Wards);
                case "Pinks":
                    return cvg.Items.Sum(x => (x as AllMatches).Pinks);
                case "DamageOutput":
                    return cvg.Items.Sum(x => (x as AllMatches).DamageOutput);
                default:
                    return 0;
            }
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
