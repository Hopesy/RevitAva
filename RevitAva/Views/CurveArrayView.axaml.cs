using Avalonia.Controls;
using Autodesk.Revit.UI;
using RevitAva.ViewModels;

namespace RevitAva.Views;

public partial class CurveArrayView : Window
{
    private readonly CurveArrayViewModel _viewModel;

    public CurveArrayView(CurveArrayViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        viewModel.CloseAction = () => Close();
    }

    public void Initialize(UIDocument uiDocument)
    {
        _viewModel.Initialize(uiDocument, uiDocument.Document);
    }

}
