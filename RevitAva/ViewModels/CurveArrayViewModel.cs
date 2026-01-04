using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RevitAva.Messages;
using RevitAva.Services.Interfaces;
using RevitAva.Views;
using System.Collections.ObjectModel;

namespace RevitAva.ViewModels;

public partial class CurveArrayViewModel : ObservableObject
{
    private readonly IRevitContext _revitContext;
    private readonly ICurveArrayService _curveArrayService;

    public bool ShouldExecute { get; private set; }
    public ObservableCollection<FamilySymbolItem> FamilySymbols { get; } = [];

    [ObservableProperty]
    private FamilySymbolItem? _selectedFamilySymbol;

    [ObservableProperty]
    private int _count = 5;

    [ObservableProperty]
    private bool _includeEndPoints = true;

    public CurveArrayViewModel(IRevitContext revitContext, ICurveArrayService curveArrayService, IMessenger messenger)
    {
        _revitContext = revitContext;
        _curveArrayService = curveArrayService;
        LoadFamilySymbols();
    }

    private void LoadFamilySymbols()
    {
        FamilySymbols.Clear();
        var symbols = _curveArrayService.GetAllFamilySymbols(_revitContext.Document);

        foreach (var symbol in symbols)
        {
            FamilySymbols.Add(new FamilySymbolItem(symbol));
        }

        if (FamilySymbols.Count > 0)
        {
            SelectedFamilySymbol = FamilySymbols[0];
        }
    }

    [RelayCommand]
    private void Confirm()
    {
        if (SelectedFamilySymbol == null || Count <= 0)
            return;

        ShouldExecute = true;
        WeakReferenceMessenger.Default.Send(new CloseWindowMessage(typeof(CurveArrayView)));
    }

    [RelayCommand]
    private void Cancel()
    {
        ShouldExecute = false;
        WeakReferenceMessenger.Default.Send(new CloseWindowMessage(typeof(CurveArrayView)));
    }

    /// <summary>
    /// 窗口关闭后执行
    /// </summary>
    public void Execute()
    {
        if (!ShouldExecute || SelectedFamilySymbol == null)
            return;

        var curve = _curveArrayService.PickModelCurve(_revitContext.UIDocument);
        if (curve == null)
            return;

        using var transaction = new Transaction(_revitContext.Document, "沿曲线阵列");
        transaction.Start();

        _curveArrayService.ArrayFamilyAlongCurve(
            _revitContext.Document,
            SelectedFamilySymbol.Symbol,
            curve,
            Count,
            IncludeEndPoints);

        transaction.Commit();
    }
}

public class FamilySymbolItem(FamilySymbol symbol)
{
    public FamilySymbol Symbol { get; } = symbol;
    public string DisplayName => $"{Symbol.FamilyName} : {Symbol.Name}";
}
