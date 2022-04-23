using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace F1TelemetryApp.Pages.Settings
{
    /// <summary>
    /// Interaction logic for UDPSettingsPage.xaml
    /// </summary>
    public partial class UDPSettingsPage : UserControl
    {
        private static readonly Regex numberRegex = new Regex("\\d");

        public UDPSettingsPage()
        {
            InitializeComponent();
        }

        private void textbox_ip0_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!UDPSettingsPage.numberRegex.IsMatch(e.Text)) e.Handled = true;
        }

        private static bool IsByte(string text)
        {
            byte n;
            if (byte.TryParse(text, out n)) return true;
            else return false;
        }

        private void IsValidIP()
        {
            ushort n;

            if (
                UDPSettingsPage.IsByte(this.textbox_ip0.Text)
                && UDPSettingsPage.IsByte(this.textbox_ip1.Text)
                && UDPSettingsPage.IsByte(this.textbox_ip2.Text)
                && UDPSettingsPage.IsByte(this.textbox_ip3.Text)
                && ushort.TryParse(this.textbox_port.Text, out n)
            )
            {
                this.textblock_error.Text = "";
            }
            else
            {
                this.textblock_error.Text = "Not a valid IP adress";
            }
        }

        private void textbox_ip0_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsValidIP();
        }

        private void textbox_port_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsValidIP();
        }
    }
}
