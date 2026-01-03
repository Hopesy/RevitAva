using Avalonia.Controls;
using RevitAva.ViewModels;

namespace RevitAva.Views;

/// <summary>
/// 曲线阵列视图
/// </summary>
public partial class CurveArrayView : Window
{
    public CurveArrayView(CurveArrayViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
