using System.Windows;

namespace RoomFinishes.Controls
{
    public partial class PluginControl : Window
    {
        public PluginControl() => InitializeComponent();

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}