using Avalonia;
using Avalonia.Controls;

namespace RevitAva.Views;

public partial class SettingView : Window
{
    public SettingView()
    {
        InitializeComponent();

#if DEBUG_R26
        // Debug 模式下启用 DevTools（按 F12 打开）
        this.AttachDevTools();
#endif
    }
}
