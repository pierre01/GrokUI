using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GrokUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string GrokUrl = "https://grok.com"; // Update if the exact web URL differs
        private const string GrokAPI = "https://console.x.ai/"; // Update if the exact web URL differs
        private const string UserDataFolder = "WebViewData"; // Relative to app exe; persists cookies/login

        public MainWindow()
        {
            InitializeComponent();
            TitleBarGrid.MouseLeftButtonDown += TitleBarGrid_MouseLeftButtonDown;
        }

        private void TitleBarGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Restore window position and size
            RestoreWindowState();

            // Initialize WebView2 with persistent user data for cookie/login remembrance
            await InitializeWebViewAsync();
        }

        private async Task InitializeWebViewAsync()
        {
            try
            {
                // Create a persistent environment (handles cookies, storage across sessions)
                string userDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UserDataFolder);
                var env = await CoreWebView2Environment.CreateAsync(null, userDataPath);

                // Ensure WebView2 is initialized with the environment
                await webView.EnsureCoreWebView2Async(env);

                // Navigate to Grok web version
                webView.Source = new Uri(GrokUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RestoreWindowState()
        {
            var settings = Settings.Default;

            // Set manual location to allow custom positioning
            WindowStartupLocation = WindowStartupLocation.Manual;

            // Load saved values
            Left = settings.WindowLeft;
            Top = settings.WindowTop;
            Width = settings.WindowWidth;
            Height = settings.WindowHeight;

            // Check if window is within current screen bounds (handles resolution changes)
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            if (Left < 0 || Left >= screenWidth || Top < 0 || Top >= screenHeight ||
                Left + Width > screenWidth || Top + Height > screenHeight)
            {
                // Reset to default if out of bounds (center on screen)
                Left = (screenWidth - Width) / 2;
                Top = (screenHeight - Height) / 2;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save window position and size only if not minimized/maximized
            if (WindowState == WindowState.Normal)
            {
                var settings = Settings.Default;
                settings.WindowLeft = Left;
                settings.WindowTop = Top;
                settings.WindowWidth = Width;
                settings.WindowHeight = Height;
                settings.Save();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            // Optional: Change button symbol
            // MaxRestoreBtn.Content = (WindowState == WindowState.Maximized) ? "🗗" : "🗖";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Hover effect for close button (red on hover)
        private void CloseBtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Button)sender).Background = System.Windows.Media.Brushes.Red;
        }

        private void CloseBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Button)sender).Background = System.Windows.Media.Brushes.Transparent;
        }

        // Optional: Make title bar draggable
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // Drag only on the title bar grid (name it if needed)
        }

        private void ApiClicked(object sender, RoutedEventArgs e)
        {
            // Navigate to Grok api version
            webView.Source = new Uri(GrokAPI);

        }

        private void GrokClicked(object sender, RoutedEventArgs e)
        {
            // Navigate to Grok web version
            webView.Source = new Uri(GrokUrl);

        }
    }
}