using MahApps.Metro.Controls;
using ShipbreakerCompanion.Client.Services;
using ShipbreakerCompanion.Client.ViewModels;
using System;
using System.Windows;

namespace ShipbreakerCompanion.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            // We will initialize everything here, even though it's not very clean, for the sake of simplicity.
            // In a bigger project, we'd use some IoC component, but that's unneeded complexity here for now.
            _vm = new MainViewModel(new ProfileService(), new ShipService());
            DataContext = _vm;
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            _vm.InitializeAsync().ConfigureAwait(false);
        }

        private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // F12 enables debug mode
            if (e.Key == System.Windows.Input.Key.F12)
            {
                _vm.EnableDebugMode();
            }
        }

        /// <summary>
        /// Callback.
        /// When main window is closed, shutdown the application.
        /// </summary>
        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
