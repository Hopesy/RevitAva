using Avalonia.Markup.Xaml;

namespace RevitAva;

public partial class AvaloniaApp: Avalonia.Application
{
    public override void Initialize()
    {
        // 1. 获取 this.GetType() => RevitAva.AvaloniaApp
        // 2. 在嵌入资源中查找与该类型关联的 XAML
        // 3. 通过 x:Class 建立的映射关系定位到 AvaloniaApp.axaml
        AvaloniaXamlLoader.Load(this);
    }
}