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

            for (int i = 0; i < GamesHistoryGrid.Columns.Count; i++)
            {
                GamesHistoryGrid.Columns[i].Width = 100;
            }

        }
        #endregion

        #region Methods

        #region LeagueMethods
        public void LoadTeamGames()
        {
            var matchList = api.GetMatchList(region, summonerId, null, queue, season, beginDate, DateTime.Now);
            for (int i = 0; i < matchList.Matches.Count; i++)
            {
                matchesDetails.Add(api.GetMatch(region, matchList.Matches[i].MatchID));
                Thread.Sleep(1000);
                while (matchesDetails.Last().Teams == null)
                {
                    matchesDetails[matchesDetails.IndexOf(matchesDetails.Last())] = api.GetMatch(region, matchList.Matches[i].MatchID);
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
                            
                            //Match Date
                            singleMatch.GameDate = match.MatchCreation.ToShortDateString();

                            //Result
                            if (player.TeamId == match.Teams[0].TeamId)
                                if (match.Teams[0].Winner)
                                    singleMatch.Result = "Victory";
                                else singleMatch.Result = "Defeat";
                            else
                            {
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

                            //Wards
                            singleMatch.Wards = player.Stats.WardsPlaced;

                            //Pinks
                            singleMatch.Pinks = player.Stats.VisionWardsBoughtInGame;

                            //Damage Output
                            singleMatch.DamageOutput = player.Stats.TotalDamageDealtToChampions;

                            //First Blood
                            singleMatch.FirstBlood = player.Stats.FirstBloodKill;

                            Matches.Add(singleMatch);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        /*public void PrintMatesDatas()
        {
            int k = 1;
            int j = 0;
            foreach (MatchDetail match in matchesDetails)
            {
                foreach (Participant player in match.Participants)
                {
                    for (int z = 0; z < match.ParticipantIdentities.Count; z++)
                    {
                        if ((match.ParticipantIdentities[z].Player.SummonerId == summonerId ||
                        match.ParticipantIdentities[z].Player.SummonerId == long.Parse(ConfigurationManager.AppSettings["Summoner2Id"]) ||
                        match.ParticipantIdentities[z].Player.SummonerId == long.Parse(ConfigurationManager.AppSettings["Summoner3Id"]) ||
                        match.ParticipantIdentities[z].Player.SummonerId == long.Parse(ConfigurationManager.AppSettings["Summoner4Id"]))
                            &&
                        match.ParticipantIdentities[z].ParticipantId == player.ParticipantId)
                        {
                            int i = 0;
                            GamesHistoryGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                            //Match Date
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            label[j].Content = match.MatchCreation;
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Result
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            if (player.TeamId == match.Teams[0].TeamId)
                                if (match.Teams[0].Winner)
                                    label[j].Content = "Victory";
                                else label[j].Content = "Defeat";
                            else
                            {
                                if (match.Teams[1].Winner)
                                    label[j].Content = "Victory";
                                else label[j].Content = "Defeat";
                            }
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Player
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            foreach (ParticipantIdentity singleParticipantIdentity in match.ParticipantIdentities)
                            {
                                if (singleParticipantIdentity.ParticipantId == player.ParticipantId)
                                {
                                    label[j].Content = singleParticipantIdentity.Player.SummonerName;
                                    break;
                                }
                            }
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Champion
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            label[j].Content = staticApi.GetChampion(region, player.ChampionId).Name;
                            Thread.Sleep(1000);
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Kills
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            label[j].Content = player.Stats.Kills;
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Deaths
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            label[j].Content = player.Stats.Deaths;
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Assists
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            label[j].Content = player.Stats.Assists;
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Wards
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            label[j].Content = player.Stats.WardsPlaced;
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Pinks
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            label[j].Content = player.Stats.VisionWardsBoughtInGame;
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            //Damage Output
                            label.Add(new Label() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) });
                            label[j].Content = player.Stats.TotalDamageDealtToChampions;
                            Grid.SetRow(label[j], k);
                            Grid.SetColumn(label[j], i++);
                            GamesHistoryGrid.Children.Add(label[j++]);

                            k++;
                            break;
                        }
                    }
                }
            }
        }*/

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
