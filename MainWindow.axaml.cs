using Avalonia.Controls;
using PurrClicker.ViewModels;

namespace PurrClicker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}