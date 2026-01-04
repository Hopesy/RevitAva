using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RevitAva.Services.Interfaces;
using System.Collections.ObjectModel;

namespace RevitAva.ViewModels;

public partial class CurveArrayViewModel : ObservableObject
{
    private UIDocument _uiDocument = null!;
    private Document _document = null!;
    private readonly IRevitService _revitService;
    // 关闭窗口的回调
    public Action? CloseAction { get; set; }
    public ObservableCollection<FamilySymbolItem> FamilySymbols { get; } = new();
    [ObservableProperty]
    private FamilySymbolItem? _selectedFamilySymbol;
    [ObservableProperty]
    private int _count = 5;
    
    [ObservableProperty]
    private bool _includeEndPoints = true;

    public CurveArrayViewModel(IRevitService revitService)
    {
        _revitService = revitService;
    }

    /// <summary>
    /// 初始化 Revit 上下文
    /// </summary>
    public void Initialize(UIDocument uiDocument, Document document)
    {
        _uiDocument = uiDocument;
        _document = document;
        LoadFamilySymbols();
    }
    
    // 加载常规模型族类型
    private void LoadFamilySymbols()
    {
        var symbols = _revitService.GetAllFamilySymbols(_document);
        foreach (var symbol in symbols)
        {
            FamilySymbols.Add(new FamilySymbolItem(symbol));
        }

        if (FamilySymbols.Count > 0)
        {
            SelectedFamilySymbol = FamilySymbols[0];
        }
    }

    // 需要执行的标记
    public bool ShouldExecute { get; private set; }
    /// 确认命令
    [RelayCommand]
    private void Confirm()
    {
        if (SelectedFamilySymbol == null || Count <= 0)
            return;
        ShouldExecute = true;
        ExecuteArray();
        CloseAction?.Invoke();
    }

    /// <summary>
    /// 取消命令
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        ShouldExecute = false;
        CloseAction?.Invoke();
    }

    /// <summary>
    /// 执行阵列（窗口关闭后由外部调用）
    /// </summary>
    public void ExecuteArray()
    {
        if (!ShouldExecute)
            return;
        // 测试：直接创建一堵墙
        using var transaction = new Transaction(_document, "测试创建墙");
        transaction.Start();
        
        var level = new FilteredElementCollector(_document)
            .OfClass(typeof(Level))
            .FirstElement() as Level;
        
        if (level != null)
        {
            var start = new XYZ(0, 0, 0);
            var end = new XYZ(10, 0, 0);
            var line = Line.CreateBound(start, end);
            Wall.Create(_document, line, level.Id, false);
        }
        
        transaction.Commit();
    }
}

/// <summary>
/// 族类型包装类，用于 UI 显示
/// </summary>
public class FamilySymbolItem
{
    public FamilySymbol Symbol { get; }
    public string DisplayName => $"{Symbol.FamilyName} : {Symbol.Name}";

    public FamilySymbolItem(FamilySymbol symbol)
    {
        Symbol = symbol;
    }
}
