using F1TelemetryApp.Classes;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace F1TelemetryApp.Pages.Settings
{
    /// <summary>
    /// Interaction logic for UDPSettingsPage.xaml
    /// </summary>
    public partial class UDPSettingsPage : UserControl, ISettingPage
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
                this.combobox_addresses.SelectedIndex != 0
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

        public void LoadData()
        {
            Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.IsEnabled = false;
                    this.Cursor = Cursors.Wait;
                    this.textblock_error.Text = "Exploring local network...";
                });

                if (u.Connention != null)
                {
                    var res = NetWorkDetector.Ping_all();
                    int savedIndex = -1;
                    if (res != null)
                    {
                        int i = 0;
                        var sb = new StringBuilder();

                        foreach (var item in res)
                        {
                            sb.Clear();
                            sb.Append(item["IP"]);

                            if (item["HostName"] != null) sb.Append(" (" + item["HostName"] + ")");
                            else sb.Append(" (" + item["MACAddress"] + ")");

                            this.Dispatcher.Invoke(() => this.combobox_addresses.Items.Add(sb.ToString()));

                            if (item["IP"] == u.Connention?.IPString) savedIndex = i;

                            i++;
                        }
                    }

                    var tmp = u.Connention.IPString.Split('.', StringSplitOptions.TrimEntries);

                    if (tmp != null)
                    {
                        if (savedIndex > -1) this.Dispatcher.Invoke(() => this.combobox_addresses.SelectedIndex = savedIndex);
                    }
                }

                this.Dispatcher.Invoke(() =>
                {
                    this.IsEnabled = true;
                    this.Cursor = null;
                    this.textblock_error.Text = "";
                });
            });

            this.textbox_port.Text = u.Connention?.Port.ToString();
            this.slider_maxpacket.Value = u.Connention?.NumberOfPacketPerSecond ?? -1;
            this.checkbox_multicore.IsChecked = u.Connention?.IsAsyncPacketProcessEnabled;
        }

        public void SaveData()
        {
            if (u.Connention != null)
            {
                if (this.IsValidIP())
                {
                    string oIP = u.Connention.IPString;
                    int oPort = u.Connention.Port;

                    try
                    {
                        u.Connention.Close();

                        var ip = this.combobox_addresses.SelectedItem as string;
                        ip = ip.Remove(ip.IndexOf(" "));

                        //StringBuilder ip = new StringBuilder()
                        //    .Append(this.textbox_ip0.Text).Append(".")
                        //    .Append(this.textbox_ip1.Text).Append(".")
                        //    .Append(this.textbox_ip2.Text).Append(".")
                        //    .Append(this.textbox_ip3.Text);

                        int port = int.Parse(this.textbox_port.Text);

                        u.Connention?.Connect(ip, port);
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
