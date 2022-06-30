using F1TelemetryApp.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace F1TelemetryApp.UserControls.Events
{
    /// <summary>
    /// Interaction logic for EventControll.xaml
    /// </summary>
    public partial class EventControll : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public EventControll()
        {
            InitializeComponent();

            this.DataContext = this;
            this.textblock_time.Text = "On: " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), DispatcherPriority.Background);
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), System.Windows.Threading.DispatcherPriority.DataBind);
        }

        private void SetHeaderColor(string v)
        {
            switch (v)
            {
                default:
                case "warning":
                    this.HeaderColor = new SolidColorBrush(Colors.Orange);
                    break;
                case "driveronpit":
                    this.HeaderColor = new SolidColorBrush(Colors.White);
                    break;
            }

            if (this.HeaderColor.CanFreeze) this.HeaderColor.Freeze();

            if (this.HeaderColor.Color.IsLightColor()) this.TextColor = new SolidColorBrush(Colors.Black);
            else this.TextColor = new SolidColorBrush(Colors.White);

            if (this.TextColor.CanFreeze) this.TextColor.Freeze();
        }

        private string _headerText;

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                if (value != this._headerText)
                {
                    _headerText = value;
                    this.SetHeaderColor(value.ToLower());
                    this.OnPropertyChanged("HeaderText");
                }
            }
        }

        private string _messageText;

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                if (value != this._messageText)
                {
                    _messageText = value;
                    this.OnPropertyChanged("MessageText");
                }
            }
        }

        private SolidColorBrush _headerColor;

        public SolidColorBrush HeaderColor
        {
            get { return _headerColor; }
            private set
            {
                if (value != this._headerColor)
                {
                    _headerColor = value;
                    this.OnPropertyChanged("HeaderColor");
                }
            }
        }

        private SolidColorBrush _textColor;

        public SolidColorBrush TextColor
        {
            get { return _textColor; }
            private set
            {
                if (value != this._textColor)
                {
                    _textColor = value;
                    this.OnPropertyChanged("TextColor");
                }
            }
        }

    }
}
