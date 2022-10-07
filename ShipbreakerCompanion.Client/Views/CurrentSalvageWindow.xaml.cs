using System;
using System.Windows;
using System.Windows.Input;

namespace ShipbreakerCompanion.Client.Views
{
    /// <summary>
    /// Interaction logic for CurrentSalvageWindow.xaml
    /// </summary>
    public partial class CurrentSalvageWindow : Window
    {
        public CurrentSalvageWindow()
        {
            InitializeComponent();
            Topmost = true;
            WindowToolPanel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Callback method for the Deactivated event of the window.
        /// Keeps the window on top of other windows.
        /// </summary>
        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        /// <summary>
        /// Callback method.
        /// Allows users to move the window by dragging on the drag control.
        /// </summary>
        private void OnDragControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        /// <summary>
        /// Callback method.
        /// Closes the window.
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Callback method.
        /// Shows the window controls when the mouse enters the window.
        /// </summary>
        private void OnWindowMouseEnter(object sender, MouseEventArgs e)
        {
            WindowToolPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Callback method.
        /// Hides the window controls when the mouse exits the window.
        /// </summary>
        private void OnWindowMouseLeave(object sender, MouseEventArgs e)
        {
            WindowToolPanel.Visibility = Visibility.Hidden;
        }
    }
}
