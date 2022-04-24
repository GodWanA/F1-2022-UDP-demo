using F1TelemetryApp.Classes;
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

        private bool IsValidIP()
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
                return true;
            }
            else
            {
                this.textblock_error.Text = "Not a valid IP adress";
                return false;
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

        internal void LoadData()
        {
            if (u.Connention != null)
            {
                var tmp = u.Connention.IPAdress.Split('.', StringSplitOptions.TrimEntries);

                if (tmp != null)
                {
                    this.textbox_ip0.Text = tmp[0];
                    this.textbox_ip1.Text = tmp[1];
                    this.textbox_ip2.Text = tmp[2];
                    this.textbox_ip3.Text = tmp[3];

                    this.textbox_port.Text = u.Connention.Port.ToString();
                }

                this.slider_maxpacket.Value = u.Connention.NumberOfPacketPerSecond;
                this.checkbox_multicore.IsChecked = u.Connention.IsAsyncPacketProcessEnabled;
            }
        }

        internal void SaveData()
        {
            if (u.Connention != null)
            {
                if (this.IsValidIP())
                {
                    string oIP = u.Connention.IPAdress;
                    int oPort = u.Connention.Port;

                    try
                    {
                        u.Connention.Close();

                        StringBuilder ip = new StringBuilder()
                            .Append(this.textbox_ip0.Text).Append(".")
                            .Append(this.textbox_ip1.Text).Append(".")
                            .Append(this.textbox_ip2.Text).Append(".")
                            .Append(this.textbox_ip3.Text);

                        int port = int.Parse(this.textbox_port.Text);

                        u.Connention.Connect(ip.ToString(), port);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        u.Connention.Connect(oIP, oPort);
                    }
                }

                u.Connention.NumberOfPacketPerSecond = this.slider_maxpacket.Value;
                u.Connention.IsAsyncPacketProcessEnabled = this.checkbox_multicore.IsChecked.Value;
            }
        }
    }
}
