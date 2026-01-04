using Avalonia;
using Avalonia.Controls;

namespace RevitAva.Views;

public partial class SettingView : Window
{
    public SettingView(ViewModels.SettingViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

#if DEBUG
        this.AttachDevTools();
#endif
    }
}
