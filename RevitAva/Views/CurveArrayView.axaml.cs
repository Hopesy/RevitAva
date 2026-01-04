using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using RevitAva.Messages;
using RevitAva.ViewModels;

namespace RevitAva.Views;

public partial class CurveArrayView : Window, IRecipient<CloseWindowMessage>
{
    private readonly CurveArrayViewModel _viewModel;

    public CurveArrayView(CurveArrayViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;

        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(CloseWindowMessage message)
    {
        if (message.Value == typeof(CurveArrayView))
        {
            Close();
        }
    }

    /// <summary>
    /// 窗口关闭后执行
    /// </summary>
    public void Execute() => _viewModel.Execute();

    protected override void OnClosed(EventArgs e)
    {
        WeakReferenceMessenger.Default.Unregister<CloseWindowMessage>(this);
        base.OnClosed(e);
    }
}
