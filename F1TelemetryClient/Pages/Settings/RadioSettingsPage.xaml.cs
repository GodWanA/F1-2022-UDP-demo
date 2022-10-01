using F1TelemetryApp.Classes;
using F1TelemetryApp.Windows;
using System;
using System.Collections.Generic;
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

namespace F1TelemetryApp.Pages.Settings
{
    /// <summary>
    /// Interaction logic for RadioSettingsPage.xaml
    /// </summary>
    public partial class RadioSettingsPage : UserControl, ISettingPage
    {
        public RadioSettingsPage()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            //throw new NotImplementedException();
        }

        public void SaveData()
        {
            //throw new NotImplementedException();
        }

        private void button_messageTest_Click(object sender, RoutedEventArgs e)
        {
            RadioMessageWindow.CurrentRadioMessageWindow.AppendMessage("Test message", Colors.Magenta);
        }
    }
}
