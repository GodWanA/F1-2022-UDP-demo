using F1TelemetryApp.Classes;
using F1TelemetryApp.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            RadioMessageWindow.CurrentRadioMessageWindow.AppendMessage("Test message", new SolidColorBrush(Colors.Magenta));
        }
    }
}
