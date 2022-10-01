using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace F1TelemetryApp.UserControls.RadioMessages
{
    /// <summary>
    /// Interaction logic for RadioMessageCard.xaml
    /// </summary>
    public partial class RadioMessageCard : UserControl, IDisposable, INotifyPropertyChanged
    {
        DispatcherTimer deathTimer = new DispatcherTimer();
        private bool disposedValue;
        private string _messageText;
        private SolidColorBrush _teamColor;
        public bool IsAlive { get; set; } = true;
        public string MessageText
        {
            get { return this._messageText; }
            protected set
            {
                if (value != this._messageText)
                {
                    this._messageText = value;
                    this.OnPropertyChanged("MessageText");
                }
            }
        }

        public SolidColorBrush TeamColor
        {
            get { return this._teamColor; }
            protected set
            {
                if (value != this._teamColor)
                {
                    this._teamColor = value;
                    this.OnPropertyChanged("TeamColor");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RadioMessageCard()
        {
            this.InitializeComponent();
            this.DataContext = this;

            this.StartDeathTimer(10);
            this.SetMessageType(MessageType.Engineer);
        }

        public RadioMessageCard(string message, Color teamColor, MessageType type, int lifeTimeInSec)
        {
            this.InitializeComponent();
            this.DataContext = this;

            this.SetMessageType(type);

            var brush = new SolidColorBrush(teamColor);
            if (brush.CanFreeze) brush.Freeze();

            this.TeamColor = brush;
            this.MessageText = "\"" + message + "\"";
            this.StartDeathTimer(lifeTimeInSec);
        }

        private void StartDeathTimer(int lifeTimeInSec)
        {
            this.deathTimer.IsEnabled = false;
            this.deathTimer.Interval = TimeSpan.FromSeconds(lifeTimeInSec);
            this.deathTimer.Tick += DeathTimer_Tick;
            this.deathTimer.IsEnabled = true;
            this.deathTimer.Start();
        }

        private void DeathTimer_Tick(object sender, EventArgs e)
        {
            this.deathTimer.Stop();
            this.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.deathTimer.Tick -= this.DeathTimer_Tick;
                    if (this.Parent != null && this.Parent is Panel)
                    {
                        var p = (this.Parent as Panel);
                        p.Children.Remove(this);
                        this.IsAlive = false;
                    }
                    this.MessageText = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this.deathTimer = null;
                this._messageText = null;
                this.PropertyChanged = null;
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RadioMessageCard()
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

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), DispatcherPriority.Background);
        }

        public enum MessageType
        {
            Engineer,
            Driver,
        }

        private void SetMessageType(MessageType type)
        {
            string imagePath = "Images/RadioMessages/";

            switch (type)
            {
                case MessageType.Engineer:
                    imagePath += "icon_engineer.png";

                    this.Dispatcher.Invoke(() =>
                    {
                        this.border_driver.Visibility = Visibility.Hidden;
                        this.border_engineer.Visibility = Visibility.Visible;
                        this.textBlock_message.HorizontalAlignment = HorizontalAlignment.Right;
                        this.image_icon.HorizontalAlignment = HorizontalAlignment.Right;
                    });

                    break;
                case MessageType.Driver:
                    imagePath += "icon_driver.png";

                    this.Dispatcher.Invoke(() =>
                    {
                        this.border_driver.Visibility = Visibility.Visible;
                        this.border_engineer.Visibility = Visibility.Visible;
                        this.textBlock_message.HorizontalAlignment = HorizontalAlignment.Left;
                        this.image_icon.HorizontalAlignment = HorizontalAlignment.Left;
                    });

                    break;
            }

            if (imagePath?.Length > 0) this.Dispatcher.Invoke(() => this.image_icon.Source = u.GetBitmapImage(imagePath));
        }
    }
}
