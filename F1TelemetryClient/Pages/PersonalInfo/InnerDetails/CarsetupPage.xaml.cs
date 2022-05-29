using F1Telemetry.Models.CarSetupsPacket;
using F1TelemetryApp.Classes;
using F1TelemetryApp.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static F1TelemetryApp.Classes.IGridResize;

namespace F1TelemetryApp.Pages.PersonalInfo.InnerDetails
{
    /// <summary>
    /// Interaction logic for CarsetupPage.xaml
    /// </summary>
    public partial class CarsetupPage : UserControl, IConnectUDP, INotifyPropertyChanged, IDisposable, IGridResize
    {
        private bool isWorkingCarSetup;

        public event PropertyChangedEventHandler PropertyChanged;

        private int _frontWing;

        public int FrontWing
        {
            get { return _frontWing; }
            set
            {
                if (value != this._frontWing)
                {
                    this._frontWing = value;
                    this.OnPropertyChanged("FrontWing");
                }
            }
        }

        private int _rearWing;

        public int RearWing
        {
            get { return _rearWing; }
            set
            {
                if (value != this._rearWing)
                {
                    this._rearWing = value;
                    this.OnPropertyChanged("RearWing");
                }
            }
        }

        private int _differentialOnThrottle;

        public int DifferentialOnThrottle
        {
            get { return _differentialOnThrottle; }
            set
            {
                if (value != this._differentialOnThrottle)
                {
                    _differentialOnThrottle = value;
                    this.OnPropertyChanged("DifferentialOnThrottle");
                }
            }
        }

        private int _differentialOffThrottle;

        public int DifferentialOffThrottle
        {
            get { return _differentialOffThrottle; }
            set
            {
                if (value != this._differentialOffThrottle)
                {
                    this._differentialOffThrottle = value;
                    this.OnPropertyChanged("DifferentialOffThrottle");
                }
            }
        }

        private float _frontChamber;

        public float FrontChamber
        {
            get { return _frontChamber; }
            set
            {
                if (value != this._frontChamber)
                {
                    _frontChamber = value;
                    this.OnPropertyChanged("FrontChamber");
                }
            }
        }

        private float _rearChamber;

        public float RearChamber
        {
            get { return _rearChamber; }
            set
            {
                if (value != this._rearChamber)
                {
                    _rearChamber = value;
                    this.OnPropertyChanged("RearChamber");
                }
            }
        }

        private float _frontToe;

        public float FrontToe
        {
            get { return _frontToe; }
            set
            {
                if (value != this._frontToe)
                {
                    _frontToe = value;
                    this.OnPropertyChanged("FrontToe");
                }
            }
        }

        private float _rearToe;
        private bool disposedValue;

        public float RearToe
        {
            get { return _rearToe; }
            set
            {
                if (value != this._rearToe)
                {
                    _rearToe = value;
                    this.OnPropertyChanged("RearToe");
                }
            }
        }

        private int _frontSuspension;

        public int FrontSuspension
        {
            get { return _frontSuspension; }
            set
            {
                if (value != this._frontSuspension)
                {
                    _frontSuspension = value;
                    this.OnPropertyChanged("FrontSuspension");
                }
            }
        }

        private int _rearSuspension;

        public int RearSuspension
        {
            get { return _rearSuspension; }
            set
            {
                if (value != this._rearSuspension)
                {
                    _rearSuspension = value;
                    this.OnPropertyChanged("RearSuspension");
                }
            }
        }

        private int _frontAntiRollBar;

        public int FrontAntiRollBar
        {
            get { return _frontAntiRollBar; }
            set
            {
                if (value != this._frontAntiRollBar)
                {
                    _frontAntiRollBar = value;
                    this.OnPropertyChanged("FrontAntiRollBar");
                }
            }
        }

        private int _rearAntiRollBar;

        public int RearAntiRollBar
        {
            get { return _rearAntiRollBar; }
            set
            {
                if (value != this._rearAntiRollBar)
                {
                    _rearAntiRollBar = value;
                    this.OnPropertyChanged("RearAntiRollBar");
                }
            }
        }

        private int _frontSuspensionHeight;

        public int FrontSuspensionHeight
        {
            get { return _frontSuspensionHeight; }
            set
            {
                if (value != this._frontSuspensionHeight)
                {
                    _frontSuspensionHeight = value;
                    this.OnPropertyChanged("FrontSuspensionHeight");
                }
            }
        }

        private int _rearSuspensionHeight;

        public int RearSuspensionHeight
        {
            get { return _rearSuspensionHeight; }
            set
            {
                if (value != this._rearSuspensionHeight)
                {
                    _rearSuspensionHeight = value;
                    this.OnPropertyChanged("RearSuspensionHeight");
                }
            }
        }

        private int _breakPressure;

        public int BrakePressure
        {
            get { return _breakPressure; }
            set
            {
                if (value != this._breakPressure)
                {
                    _breakPressure = value;
                    this.OnPropertyChanged("BreakPressure");
                }
            }
        }

        private int _brakeBias;

        public int BrakeBias
        {
            get { return _brakeBias; }
            set
            {
                if (value != this._brakeBias)
                {
                    _brakeBias = value;
                    this.OnPropertyChanged("BrakeBias");
                }
            }
        }


        private float _tyreFrontLeft;

        public float TyreFrontLeft
        {
            get { return _tyreFrontLeft; }
            set
            {
                if (value != this._tyreFrontLeft)
                {
                    _tyreFrontLeft = value;
                    this.OnPropertyChanged("TyreFrontLeft");
                }
            }
        }

        private float _tyreFrontRight;

        public float TyreFrontRight
        {
            get { return _tyreFrontRight; }
            set
            {
                if (value != this._tyreFrontRight)
                {
                    _tyreFrontRight = value;
                    this.OnPropertyChanged("TyreFrontRight");
                }
            }
        }

        private float _tyreRearLeft;

        public float TyreRearLeft
        {
            get { return _tyreRearLeft; }
            set
            {
                if (value != this._tyreRearLeft)
                {
                    _tyreRearLeft = value;
                    this.OnPropertyChanged("TyreRearLeft");
                }
            }
        }

        private float _tyreRearRight;

        public float TyreRearRight
        {
            get { return _tyreRearRight; }
            set
            {
                if (value != this._tyreRearRight)
                {
                    _tyreRearRight = value;
                    this.OnPropertyChanged("TyreRearRight");
                }
            }
        }

        private int _ballast;

        public int Ballast
        {
            get { return _ballast; }
            set
            {
                if (value != this._ballast)
                {
                    _ballast = value;
                    this.OnPropertyChanged("Ballast");
                }
            }
        }

        private float _fuel;

        public float Fuel
        {
            get { return _fuel; }
            set
            {
                if (value != this._fuel)
                {
                    _fuel = value;
                    this.OnPropertyChanged("Fuel");
                }
            }
        }


        public CarsetupPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.CarSetupsPacket += Connention_CarSetupsPacket;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.CarSetupsPacket -= Connention_CarSetupsPacket;
            }
        }

        private void Connention_CarSetupsPacket(PacketCarSetupData packet, EventArgs e)
        {
            if (this.CanUpdateEvent() && !this.isWorkingCarSetup)
            {
                this.isWorkingCarSetup = true;
                this.UpdateCarSetup(ref packet);
                this.isWorkingCarSetup = false;
            }
        }

        private void UpdateCarSetup(ref PacketCarSetupData packet)
        {
            if (packet != null)
            {
                if (u.SelectedIndex > -1 && u.SelectedIndex < packet.CarSetups.Length)
                {
                    if (u.SelectedPlayer != null)
                    {
                        var userSetup = packet.CarSetups[u.SelectedPlayer.ArrayIndex];
                        var fc = MathF.Round(userSetup.FrontChamber, 2);
                        var rc = MathF.Round(userSetup.RearChamber, 2);
                        var ft = MathF.Round(userSetup.FrontToe, 2);
                        var rt = MathF.Round(userSetup.RearToe, 2);
                        var f = MathF.Round(userSetup.FuelLoad, 2);

                        var tyre_fl = MathF.Round(userSetup.TyrePressure["FrontLeft"], 2);
                        var tyre_fr = MathF.Round(userSetup.TyrePressure["FrontRight"], 2);
                        var tyre_rl = MathF.Round(userSetup.TyrePressure["RearLeft"], 2);
                        var tyre_rr = MathF.Round(userSetup.TyrePressure["RearRight"], 2);

                        this.Dispatcher.Invoke(() =>
                        {
                            this.FrontWing = userSetup.FrontWing;
                            this.RearWing = userSetup.RearWing;
                            this.DifferentialOnThrottle = userSetup.DifferentialOnThrottle;
                            this.DifferentialOffThrottle = userSetup.DifferentialOffThrottle;
                            this.FrontChamber = fc;
                            this.RearChamber = rc;
                            this.FrontToe = ft;
                            this.RearToe = rt;
                            this.FrontSuspension = userSetup.FrontSuspension;
                            this.RearSuspension = userSetup.RearSuspension;
                            this.FrontAntiRollBar = userSetup.FrontAntiRollBar;
                            this.RearAntiRollBar = userSetup.RearAntiRollBar;
                            this.FrontSuspensionHeight = userSetup.FrontSuspensionHeight;
                            this.RearSuspensionHeight = userSetup.RearSuspensionHeight;
                            this.BrakePressure = userSetup.BrakePressure;
                            this.BrakeBias = userSetup.BrakeBias;
                            this.TyreFrontLeft = tyre_fl;
                            this.TyreFrontRight = tyre_fr;
                            this.TyreRearLeft = tyre_rl;
                            this.TyreRearRight = tyre_rr;
                            this.Ballast = userSetup.Ballast;
                            this.Fuel = f;
                        });
                    }
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CanUpdateEvent()
        {
            bool ret = false;
            this.Dispatcher.Invoke(() => ret = this.IsLoaded && u.CanDoUdp);
            return ret;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeUDPEvents();
            this.LoadSetup();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeUDPEvents();
        }

        internal void LoadSetup()
        {
            if (u.Connention != null)
            {
                var p = u.Connention.LastCarSetupPacket;
                this.UpdateCarSetup(ref p);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.UnsubscribeUDPEvents();
                    this.PropertyChanged = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CarsetupPage()
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

        private void button_copyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            var dt = new DataTable("Car Setup");
            dt.Columns.Add(new DataColumn("Key"));
            dt.Columns.Add(new DataColumn("Value"));

            // Wings:
            CarsetupPage.AppendRow(ref dt, "Front Wing", this.FrontWing.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Rear Wing", this.RearWing.ToString("0.##"));

            // Differential:
            CarsetupPage.AppendRow(ref dt, "Differential Adjustment ON Throttle", this.DifferentialOnThrottle.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Differential Adjustment OFF Throttle", this.DifferentialOffThrottle.ToString("0.##"));

            // Suspension geometry:
            CarsetupPage.AppendRow(ref dt, "Front Chamber", this.FrontChamber.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Rear Chamber", this.RearChamber.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Front Toe", this.FrontToe.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Rear Toe", this.RearToe.ToString("0.##"));

            // Suspension:
            CarsetupPage.AppendRow(ref dt, "Front Suspension", this.FrontSuspension.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Rear Suspension", this.RearSuspension.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Front Anti-Roll Bar", this.FrontAntiRollBar.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Rear Anti-Roll Bar", this.RearAntiRollBar.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Front Suspension Height", this.FrontSuspensionHeight.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Rear Suspension Height", this.RearSuspensionHeight.ToString("0.##"));

            // Brakes:
            CarsetupPage.AppendRow(ref dt, "Brake Pressure", this.BrakePressure.ToString("0.##"));
            CarsetupPage.AppendRow(ref dt, "Brake Bias", this.BrakeBias.ToString("0.##"));

            // Weight:
            CarsetupPage.AppendRow(ref dt, "Ballast", this.Ballast.ToString());

            // Fuel:
            CarsetupPage.AppendRow(ref dt, "Fuel Load", this.Fuel.ToString());


            var sb = new StringBuilder();
            var html = new StringBuilder().Append("<table><tbody>");

            int padKey = dt.AsEnumerable().Max(x => (x["Key"] as string).Length);
            int padValue = dt.AsEnumerable().Max(x => (x["Value"] as string).Length);

            sb.AppendLine("+-" + "".PadRight(padKey, '-') + "-+-" + "".PadLeft(padValue, '-') + "-+");

            foreach (DataRow row in dt.Rows)
            {
                var k = row["Key"] as string;
                var v = row["Value"] as string;

                sb.AppendLine("| " + k.PadRight(padKey, ' ') + " | " + v.PadLeft(padValue, ' ') + " |");
                sb.AppendLine("+-" + "".PadRight(padKey, '-') + "-+-" + "".PadLeft(padValue, '-') + "-+");
                html.Append("<tr>").Append("<td>" + k + "</td>").Append("<td>" + v + "</td>").Append("</tr>");
            }

            html.Append("</tbody></table>");

            Clipboard.Clear();

            var dataObject = new DataObject();

            dataObject.SetText(sb.ToString(), TextDataFormat.UnicodeText);
            dataObject.SetText(html.ToString(), TextDataFormat.Html);


            Clipboard.SetDataObject(dataObject);
            Clipboard.Flush();
        }

        private static void AppendRow(ref DataTable dt, string key, string value)
        {
            var row = dt.NewRow();
            row["Key"] = key;
            row["Value"] = value;
            dt.Rows.Add(row);
        }

        internal void CalculateView(GridSizes gridSizes)
        {
            switch (gridSizes)
            {
                case GridSizes.XS:
                    this.ResizeXS();
                    break;
                case GridSizes.XM:
                    this.ResizeXM();
                    break;
                case GridSizes.MD:
                    this.ResizeMD();
                    break;
                case GridSizes.LG:
                    this.ResizeLG();
                    break;
                case GridSizes.XL:
                    this.ResizeXL();
                    break;
            }

            // this.UpdateLayout();
        }

        public void ResizeXS()
        {

        }

        public void ResizeXM()
        {
            //throw new NotImplementedException();
        }

        public void ResizeMD()
        {
            //throw new NotImplementedException();
        }

        public void ResizeLG()
        {
            //throw new NotImplementedException();
        }

        public void ResizeXL()
        {
            //throw new NotImplementedException();
        }
    }
}
