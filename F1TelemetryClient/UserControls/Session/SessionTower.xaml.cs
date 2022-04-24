using F1Telemetry.Models.ParticipantsPacket;
using F1Telemetry.Models.SessionHistoryPacket;
using F1TelemetryApp.Classes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace F1TelemetryApp.UserControls.Session
{
    /// <summary>
    /// Interaction logic for SessionTower.xaml
    /// </summary>
    public partial class SessionTower : UserControl, IConnectUDP, INotifyPropertyChanged
    {
        private ObservableCollection<PlayerListItemData> participantsList;

        private bool isWorking_ParticipantsData;
        private bool isWorking_sessionHistory;

        private DispatcherTimer timer = new DispatcherTimer();

        public event PropertyChangedEventHandler PropertyChanged;
        public event SelectionChangedEventHandler SelectionChanged;

        public ObservableCollection<PlayerListItemData> ParticipantsList
        {
            get { return participantsList; }
            set
            {
                if (value != participantsList)
                {
                    participantsList = value;
                    this.OnPropertyChanged("ParticipantsList");
                }
            }
        }

        public SessionTower()
        {
            InitializeComponent();

            this.DataContext = this;
            this.participantsList = new ObservableCollection<PlayerListItemData>();
            this.listBox_drivers.Items.SortDescriptions.Add(new SortDescription("CarPosition", ListSortDirection.Ascending));

            //this.timer.Tick += Timer_Tick;
            //this.timer.Interval = TimeSpan.FromSeconds(2);
            //this.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer.Stop();

            if (this.IsLoaded)
            {
                this.listBox_drivers.Items.Refresh();
                GC.Collect();
            }

            this.timer.Start();
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.SessionHistoryPacket += Connention_SessionHistoryPacket;
                u.Connention.ParticipantsPacket += Connention_ParticipantsPacket;
                u.Connention.SessionPacket += Connention_SessionPacket;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.SessionHistoryPacket -= Connention_SessionHistoryPacket;
                u.Connention.ParticipantsPacket -= Connention_ParticipantsPacket;
                u.Connention.SessionPacket -= Connention_SessionPacket;
            }
        }

        private void Connention_SessionPacket(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Connention_ParticipantsPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_ParticipantsData && u.CanDoUdp)
            {
                this.isWorking_ParticipantsData = true;
                this.Dispatcher.Invoke(() =>
                {
                    var data = sender as PacketParticipantsData;
                    this.UpdateParticipants(data);
                    this.isWorking_ParticipantsData = false;
                }, DispatcherPriority.Background);
            }
        }

        private void UpdateParticipants(PacketParticipantsData participants)
        {
            int index = this.listBox_drivers.SelectedIndex;
            int n = participants.Participants.Length;

            if (this.participantsList.Count != n)
            {
                this.participantsList.Clear();

                for (int i = 0; i < n; i++)
                {
                    var data = new PlayerListItemData();
                    data.ArrayIndex = i;
                    this.participantsList.Add(data);
                }
            }

            if (index != -1) this.listBox_drivers.SelectedIndex = index;
            else if (participants.Header.Player1CarIndex != 255) this.listBox_drivers.SelectedItem = this.participantsList[participants.Header.Player1CarIndex];

            if (this.listBox_drivers.SelectedItem != null)
            {
                this.listBox_drivers.ScrollIntoView(this.listBox_drivers.SelectedItem);
            }

            //if (PlayerListItemData.PosChange)
            //{
            this.listBox_drivers.Items.Refresh();
            //    GC.Collect();
            //    PlayerListItemData.PosChange = false;
            //}
        }


        private void Connention_SessionHistoryPacket(object sender, EventArgs e)
        {
            if (u.CanDoUdp && !this.isWorking_sessionHistory)
            {
                this.isWorking_sessionHistory = true;
                this.Dispatcher.Invoke(() =>
                {
                    var curHistory = sender as PacketSessionHistoryData;
                    this.UpdateSessionHistory(curHistory);
                    //GC.Collect();
                    this.isWorking_sessionHistory = false;
                }, DispatcherPriority.Background);
            }
        }

        private void UpdateSessionHistory(PacketSessionHistoryData curHistory)
        {
            var arrayHistory = u.Connention.LastSessionHistoryPacket;
            var lapData = u.Connention.LastLapDataPacket;

            //var itemSource = this.listBox_drivers.ItemsSource?.Cast<PlayerListItemData>();
            var itemSource = this.ParticipantsList;
            var curItem = itemSource?.Where(x => x.ArrayIndex == curHistory.CarIndex).FirstOrDefault();
            Brush fontColor = Brushes.White;

            if (curItem != null)
            {
                if (curItem.CarPosition == 1)
                {
                    curItem.IntervalTime = "interval";
                    curItem.LeaderIntervalTime = "leader";
                    curItem.TextColor = fontColor;
                }
                else if (!curItem.IsOut)
                {
                    curItem.IntervalTime = "";
                    curItem.LeaderIntervalTime = "";
                    var curLapdata = lapData?.Lapdata[curItem.ArrayIndex];

                    var prevItem = itemSource.Where(x => x.CarPosition == curItem.CarPosition - 1).FirstOrDefault();
                    if (prevItem != null)
                    {
                        var prevHistory = arrayHistory.Where(x => x?.CarIndex == prevItem.ArrayIndex).FirstOrDefault();
                        string s = u.CalculateDelta(lapData, curHistory, prevHistory, out fontColor);
                        if (s != null && s != "") curItem.IntervalTime = s;
                        curItem.TextColor = fontColor;
                        prevItem = null;
                        prevItem = null;
                    }

                    var firstItem = itemSource.Where(x => x.CarPosition == 1).FirstOrDefault();
                    if (firstItem != null)
                    {
                        var firstHistory = arrayHistory.Where(x => x?.CarIndex == firstItem.ArrayIndex).FirstOrDefault();
                        string s = u.CalculateDelta(lapData, curHistory, firstHistory, out fontColor);
                        if (s != null && s != "") curItem.LeaderIntervalTime = s;
                        firstItem = null;
                        firstHistory = null;
                    }
                }
                else
                {
                    curItem.IntervalTime = "";
                    curItem.setStateText();
                    curItem.TextColor = Brushes.White;
                }
            }

            //if (PlayerListItemData.PosChange) this.listBox_drivers.Items.Refresh();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeUDPEvents();
            this.listBox_drivers.Items.IsLiveSorting = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeUDPEvents();
            this.listBox_drivers.Items.IsLiveSorting = false;
        }

        private void listBox_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            u.SelectedIndex = this.listBox_drivers.SelectedIndex;
            u.SelectedItem = this.listBox_drivers.SelectedItem;

            if (this.IsLoaded)
            {
                if (this.listBox_drivers.SelectedIndex > -1)
                {
                    var selected = this.listBox_drivers.SelectedItem as PlayerListItemData;

                    if (selected.CarPosition > 0)
                    {
                        foreach (var item in this.participantsList) item.IsSelected = false;

                        selected.IsSelected = true;
                        this.SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(e.RoutedEvent, e.RemovedItems, e.AddedItems));
                        //this.personalInfo.LoadWear();
                    }
                    else
                    {
                        this.listBox_drivers.SelectedItem = null;
                    }
                }
            }
        }
    }
}
