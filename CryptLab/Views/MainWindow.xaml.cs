using System.Windows;
using System.Windows.Input;

namespace CryptLab.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool IsMaximized = false;
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
        {
            if (IsMaximized)
            {
                this.WindowState = WindowState.Normal;
                this.Width = 1080;
                this.Height = 720;

                IsMaximized = false;
            }
            else
            {
                this.WindowState = WindowState.Normal;

                IsMaximized = true;
            }
        }
    }
}