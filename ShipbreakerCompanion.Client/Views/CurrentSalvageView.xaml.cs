using System.Windows.Controls;

namespace ShipbreakerCompanion.Client.Views
{
    /// <summary>
    /// Interaction logic for CurrentSalvageView.xaml
    /// </summary>
    public partial class CurrentSalvageView : UserControl
    {
        public CurrentSalvageView()
        {
            InitializeComponent();
        }

        private void OnPopOutClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var salvageWindow = new CurrentSalvageWindow
            {
                DataContext = DataContext
            };
            salvageWindow.Show();
        }
    }
}
