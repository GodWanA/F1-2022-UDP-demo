using F1Telemetry.Models.LapDataPacket;
using F1TelemetryApp.Classes;
using F1TelemetryApp.UserControls.RadioMessages;
using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace F1TelemetryApp.Windows
{
    /// <summary>
    /// Interaction logic for RadioMessage.xaml
    /// </summary>
    public partial class RadioMessageWindow : Window, IConnectUDP, IDisposable
    {
        private static bool _isWindowEnabled;
        internal static bool IsWindowEnabled
        {
            get
            {
                return RadioMessageWindow._isWindowEnabled;
            }
            set
            {
                if (value != RadioMessageWindow._isWindowEnabled) RadioMessageWindow._isWindowEnabled = value;
                RadioMessageWindow.WindowEnabledChanged?.Invoke(value, new EventArgs());
            }
        }
        internal static RadioOptions DemageOption { get; set; }

        internal delegate void WindowEnabledChangedEventHandler(bool value, EventArgs e);
        internal delegate void MessageRemoveEventHandler(object sender, EventArgs e);
        internal static WindowEnabledChangedEventHandler WindowEnabledChanged;
        internal static MessageRemoveEventHandler MessageRemove;
        internal static RadioMessageWindow CurrentRadioMessageWindow { get; private set; }
        public bool CanShowMessages { get; private set; } = true;

        private bool disposedValue;
        private bool isSession;
        private bool isCarDemage;
        private readonly string regPath;

        public RadioMessageWindow()
        {
            this.regPath = this.CreateRegPath();
            this.CreateCommon();
        }

        public RadioMessageWindow(Window owner)
        {
            this.regPath = this.CreateRegPath();
            this.CreateCommon();
            this.Owner = owner;
        }

        private void CreateCommon()
        {
            InitializeComponent();

            this.DataContext = this;

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width - 10;
            this.Top = SystemParameters.PrimaryScreenHeight / 2 - this.Height / 2;
            //this.Opacity = 0;

            //this.LoadWindowPosition(this.regPath);

            RadioMessageWindow.WindowEnabledChanged += this.RadioMessage_WindowEnabledChanged;
            RadioMessageWindow.MessageRemove += this.RadioMessage_MessageRemove;
            //RadioMessageWindow.CurrentRadioMessageWindow = this;
        }


        private void RadioMessage_MessageRemove(object sender, EventArgs e)
        {
            if (this.stackPanel_messages.Children.Count == 0) this.Opacity = 0;
            else this.Opacity = 1;
        }

        private void RadioMessage_WindowEnabledChanged(bool value, EventArgs e)
        {
            if (!value) this.Close();
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.CarDemagePacket -= Connention_CarDemagePacket;
                u.Connention.SessionPacket -= Connention_SessionPacket;
                u.Connention.EventPacketDriverOnPits2 -= Connention_EventPacketDriverOnPits2;
            }
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.CarDemagePacket += Connention_CarDemagePacket;
                u.Connention.SessionPacket += Connention_SessionPacket;
                u.Connention.EventPacketDriverOnPits2 += Connention_EventPacketDriverOnPits2;
            }
        }

        private void Connention_EventPacketDriverOnPits2(F1Telemetry.CustomModels.EventPackets.DriverOnPits2 packet, EventArgs e)
        {
            if (packet != null)
            {
                var driver = u.Connention?.CurrentParticipantsPacket?.Participants[packet.VehicleIndex];

                if (driver != null)
                {
                    var msg = driver.ParticipantName + " is in the PIT.";
                    this.Dispatcher.Invoke(() => this.AppendMessage(msg, u.PickTeamColor(driver.TeamID)));
                }
            }
        }

        private void Connention_SessionPacket(F1Telemetry.Models.SessionPacket.PacketSessionData packet, EventArgs e)
        {
            if (packet != null && !this.isSession)
            {
                this.isSession = true;
                var lapdata = u.Connention.CurrentLapDataPacket?.Lapdata[packet.Header.Player1CarIndex];
                this.CanShowMessages = !packet.IsGamePaused && lapdata?.DriverStatus != F1Telemetry.Helpers.Appendences.DriverSatuses.InGarage;

                this.Dispatcher.Invoke(() =>
                {
                    if (!this.CanShowMessages) this.Opacity = 0;

                    this.isSession = false;
                });
            }
        }

        private void Connention_CarDemagePacket(F1Telemetry.Models.CarDamagePacket.PacketCarDamageData packet, EventArgs e)
        {
            if (packet != null && !this.isCarDemage)
            {
                this.isCarDemage = true;

                var lastDemagePacket = u.Connention.LastCarDemagePacket;
                var participants = u.Connention.CurrentParticipantsPacket;

                if (RadioMessageWindow.DemageOption != RadioOptions.Disabled)
                {
                    bool ok = RadioMessageWindow.DemageOption == RadioOptions.AllDriver;

                    for (int i = 0; i < packet.CarDamageData.Length; i++)
                    {
                        if (!ok) ok = RadioMessageWindow.DemageOption == RadioOptions.PlayerOnly && (packet.Header.Player1CarIndex == i || packet.Header.Player2CarIndex == i);
                        if (ok) ok = lastDemagePacket != null;

                        if (ok)
                        {
                            var currDemage = packet?.CarDamageData[i];
                            var lastDemage = lastDemagePacket?.CarDamageData[i];
                            var driver = participants?.Participants[i];
                            var msg = new StringBuilder();

                            if (currDemage != null && lastDemage != null)
                            {
                                if (currDemage.DiffuserDemage > lastDemage.DiffuserDemage) CalculateDemage(ref msg, "Diffuser", currDemage.DiffuserDemage);
                                if (currDemage.ExhasutDemage > lastDemage.ExhasutDemage) CalculateDemage(ref msg, "Exhasut", currDemage.ExhasutDemage);
                                if (currDemage.FloorDemage > lastDemage.FloorDemage) CalculateDemage(ref msg, "Floor", currDemage.FloorDemage);
                                if (currDemage.FrontLeftWingDemage > lastDemage.FrontLeftWingDemage) CalculateDemage(ref msg, "Front left wing", currDemage.FrontLeftWingDemage);
                                if (currDemage.FrontRightWingDemage > lastDemage.FrontRightWingDemage) CalculateDemage(ref msg, "Front right wing", currDemage.FrontRightWingDemage);
                                if (currDemage.RearWingDemage > lastDemage.RearWingDemage) CalculateDemage(ref msg, "Rear wing", currDemage.RearWingDemage);
                                if (currDemage.SidepodDemage > lastDemage.SidepodDemage) CalculateDemage(ref msg, "Sidepod", currDemage.SidepodDemage);

                                if (msg.Length > 0 && driver != null) this.AppendMessage(driver.ParticipantName, msg.ToString(), u.PickTeamColor(driver.TeamID));
                            }
                        }
                    }
                }
                this.isCarDemage = false;
            }
        }

        private void CalculateDemage(ref StringBuilder sb, string componentName, double CurrHP)
        {
            string range = string.Empty;

            if (CurrHP < 10) range = "minor ";
            else if (CurrHP > 10 && CurrHP != 100) range = "major ";
            else range = "lost ";

            sb.AppendWithSeparator(range + componentName, ", ");
        }

        private void AppendMessage(string driverName, string message, Brush teamColor)
        {
            string msg = driverName + " has " + message + (message.Contains(", ") ? " demages." : " demage.");

            this.Dispatcher.Invoke((Action)(() =>
            {
                this.AppendMessage(msg, teamColor);
            }), DispatcherPriority.Background);
        }

        internal void AppendMessage(string message, Brush teamColor)
        {
            if (this.CanShowMessages)
            {
                var msg = new RadioMessageCard(
                            message,
                            teamColor,
                            RadioMessageCard.MessageType.Engineer,
                            10
                        );
                //msg.OnDeathHandler += this.OnDeath_msg;
                this.stackPanel_messages.Children.Add(msg);
                this.Opacity = 1;
            }
        }

        private void OnDeath_msg(object sender, EventArgs e)
        {
            this.stackPanel_messages.Children.Remove(sender as FrameworkElement);
            RadioMessageWindow.MessageRemove?.Invoke(this, new EventArgs());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (RadioMessageWindow.CurrentRadioMessageWindow == null) RadioMessageWindow.CurrentRadioMessageWindow = this;
            else this.Close();

            this.SubscribeUDPEvents();

            this.FreezeColors();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeUDPEvents();
        }

        public void Show()
        {
            if (RadioMessageWindow.IsWindowEnabled) base.Show();
        }

        public enum RadioOptions
        {
            Disabled,
            AllDriver,
            PlayerOnly,
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SaveWindowPosition(this.regPath);
            RadioMessageWindow.CurrentRadioMessageWindow = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    RadioMessageWindow.WindowEnabledChanged -= this.RadioMessage_WindowEnabledChanged;
                    RadioMessageWindow.MessageRemove -= this.RadioMessage_MessageRemove;

                    this.UnsubscribeUDPEvents();

                    foreach (var item in this.stackPanel_messages.Children)
                    {
                        if (item is IDisposable) (item as IDisposable).Dispose();
                    }

                    if (RadioMessageWindow.CurrentRadioMessageWindow == this) RadioMessageWindow.CurrentRadioMessageWindow = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RadioMessage()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
