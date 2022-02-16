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

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for TyreDataControl.xaml
    /// </summary>
    public partial class TyreDataControl : UserControl, INotifyPropertyChanged
    {
        private double _wear;
        private double _demage;
        private double _breakTemperature;
        private double _condition;
        private double _tyreInnerTemperature;
        private double _tyreSurfaceTemperature;
        private double _pressure;

        public TyreDataControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public double Wear
        {
            get
            {
                return this._wear;
            }
            set
            {
                if (value != this._wear)
                {
                    this._wear = value;
                    this.Condition = 100.0 - this.Wear;

                    var red = (byte)Math.Round(255 * this.Wear / 100.0 * 3.0303030303);
                    var green = (byte)Math.Round(255 * this.Condition / 100.0 * 3.0303030303);

                    if (this.Wear <= 66)
                    {
                        this.progreassBar_Condition.Foreground = new SolidColorBrush(Color.FromRgb(red, 255, 0));
                    }
                    else
                    {
                        this.progreassBar_Condition.Foreground = new SolidColorBrush(Color.FromRgb(255, green, 0));
                    }


                    this.OnPropertyChanged("Wear");
                }
            }
        }

        public double Demage
        {
            get
            {
                return this._demage;
            }
            set
            {
                if (value != this._demage)
                {
                    this._demage = value;
                    this.OnPropertyChanged("Demage");
                }
            }
        }

        public double Condition
        {
            get
            {
                return this._condition;
            }
            set
            {
                if (value != this._condition)
                {
                    this._condition = value;
                    this.OnPropertyChanged("Condition");
                }
            }
        }

        public double BrakesTemperature
        {
            get
            {
                return this._breakTemperature;
            }
            set
            {
                if (value != this._breakTemperature)
                {
                    this._breakTemperature = value;
                    this.OnPropertyChanged("BrakesTemperature");
                }
            }
        }
        public double TyreInnerTemperature
        {
            get
            {
                return this._tyreInnerTemperature;
            }
            set
            {
                if (value != this._tyreInnerTemperature)
                {
                    this._tyreInnerTemperature = value;
                    this.OnPropertyChanged("TyreInnerTemperature");
                }
            }
        }
        public double TyreSurfaceTemperature
        {
            get
            {
                return this._tyreSurfaceTemperature;
            }
            set
            {
                if (value != this._tyreSurfaceTemperature)
                {
                    this._tyreSurfaceTemperature = value;
                    this.OnPropertyChanged("TyreSurfaceTemperature");
                }
            }
        }
        public double Pressure
        {
            get
            {
                return this._pressure;
            }
            set
            {
                if (value != this._pressure)
                {
                    this._pressure = value;
                    this.OnPropertyChanged("Pressure");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
