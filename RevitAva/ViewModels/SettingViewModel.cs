using CommunityToolkit.Mvvm.ComponentModel;

namespace RevitAva.ViewModels;

public partial class SettingViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "RevitAva 设置";
}
