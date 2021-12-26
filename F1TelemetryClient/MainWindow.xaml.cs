using F1Telemetry;
using F1TelemetryApp.UserControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace F1TelemetryClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private F1UDP udp;
        private DispatcherTimer cleaner;

        public MainWindow()
        {
            InitializeComponent();

            this.cleaner = new DispatcherTimer();
            this.cleaner.Interval = TimeSpan.FromSeconds(30);
            this.cleaner.Tick += this.Cleaner_Tick;
            this.cleaner.Start();

            this.udp = new F1UDP("127.0.0.1", 20777);
            this.udp.SessionPacket += this.Upp_SessionPacket;
            this.udp.LapDataPacket += this.Udp_LapDataPacket;
            this.udp.ParticipantsPacket += this.Udp_ParticipantsPacket;

            this.listBox_drivers.Items.IsLiveSorting = true;
            this.listBox_drivers.Items.SortDescriptions.Add(new SortDescription("CarPosition", ListSortDirection.Ascending));
        }

        private void Udp_ParticipantsPacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var participants = this.udp.LastParticipantsPacket;
                this.PlyerList(participants.Participants.Length);
                var items = this.listBox_drivers.ItemsSource?.Cast<PlayerListItemData>();

                if (items != null)
                {
                    for (int i = 0; i < participants.Participants.Length; i++)
                    {
                        var current = participants.Participants[i];
                        var elem = items.Where(x => x.ArrayIndex == i).FirstOrDefault();
                        if (elem != null)
                        {
                            elem.DriverID = current.DriverID;
                            elem.Name = current.Name;
                            elem.Nationality = current.Nationality;
                            elem.TeamID = current.TeamID;
                            elem.IsAI = current.IsAIControlled;
                            elem.IsMyTeam = current.IsMyTeam;
                            elem.IsHuman = !current.IsAIControlled;
                            elem.RaceNumber = current.RaceNumber;
                        }
                    }
                }
            });
        }

        private void Udp_LapDataPacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var lapData = this.udp.LastLapDataPacket;
                var sessionData = this.udp.LastSessionDataPacket;
                var items = this.listBox_drivers.ItemsSource?.Cast<PlayerListItemData>();

                if (items != null)
                {
                    for (int i = 0; i < lapData.Lapdata.Length; i++)
                    {
                        var current = lapData.Lapdata[i];
                        var elem = items.Where(x => x.ArrayIndex == i).FirstOrDefault();
                        if (elem != null)
                        {
                            elem.CurrentLapTime = current.CurrentLapTime;
                            elem.TrackLengthPercent = current.LapDistance / sessionData.TrackLength * 100.0;
                            elem.CarPosition = current.CarPosition;

                            if (elem.CarPosition == 0) elem.Visibility = Visibility.Collapsed;
                            else elem.Visibility = Visibility.Visible;
                        }
                    }
                }
            });
        }

        private void Cleaner_Tick(object sender, EventArgs e)
        {
            this.cleaner.Stop();

            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCApproach();
            GC.WaitForFullGCComplete();
            GC.Collect();

            this.cleaner.Start();
        }

        private void Upp_SessionPacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var sessionData = this.udp.LastSessionDataPacket;
                var lapData = this.udp.LastLapDataPacket;
                var first = lapData?.Lapdata?.Where(x => x.CarPosition == 1).FirstOrDefault();

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Session: " + Regex.Replace(sessionData.SessionType.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim());
                sb.AppendLine("Laps: " + first.CurrentLapNum + " / " + sessionData.TotalLaps);
                sb.Append("TimeLeft: " + sessionData.SessionTimeLeft.ToString());

                this.textBlock_counterHead.Text = sb.ToString();
            });
        }

        private void listBox_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBox_drivers.IsLoaded)
            {
            }
        }

        private void listBox_drivers_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void PlyerList(int numberOfPlayers)
        {
            if (this.listBox_drivers.Items.Count == 0)
            {
                var tmp = new ObservableCollection<PlayerListItemData>();

                for (int i = 0; i < numberOfPlayers; i++)
                {
                    var data = new PlayerListItemData();
                    data.ArrayIndex = i;
                    tmp.Add(data);
                }

                this.listBox_drivers.ItemsSource = tmp;
            }
        }

    }
}
