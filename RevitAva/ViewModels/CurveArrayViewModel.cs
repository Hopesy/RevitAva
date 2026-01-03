using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RevitAva.Services.Interfaces;

namespace RevitAva.ViewModels;

/// <summary>
/// 曲线阵列视图模型
/// </summary>
public partial class CurveArrayViewModel : ObservableObject
{
    private readonly ILogger<CurveArrayViewModel> _logger;
    private readonly IRevitService _revitService;
    private readonly UIDocument _uiDocument;
    private Curve? _selectedCurve;

    [ObservableProperty]
    private string _title = "曲线阵列";

    [ObservableProperty]
    private ObservableCollection<FamilySymbolItem> _familySymbols = new();

    [ObservableProperty]
    private FamilySymbolItem? _selectedFamilySymbol;

    [ObservableProperty]
    private int _arrayCount = 10;

    [ObservableProperty]
    private bool _includeEndPoints = true;

    [ObservableProperty]
    private string _statusMessage = "请先加载族类型";

    [ObservableProperty]
    private bool _isProcessing = false;

    public CurveArrayViewModel(ILogger<CurveArrayViewModel> logger, IRevitService revitService, UIDocument uiDocument)
    {
        _logger = logger;
        _revitService = revitService;
        _uiDocument = uiDocument;
    }

    /// <summary>
    /// 加载族类型
    /// </summary>
    [RelayCommand]
    private void LoadFamilySymbols()
    {
        try
        {
            IsProcessing = true;
            StatusMessage = "正在加载族类型...";

            var symbols = _revitService.GetAllFamilySymbols(_uiDocument.Document);
            FamilySymbols.Clear();

            foreach (var symbol in symbols)
            {
                FamilySymbols.Add(new FamilySymbolItem
                {
                    FamilySymbol = symbol,
                    DisplayName = $"{symbol.FamilyName} : {symbol.Name}"
                });
            }

            if (FamilySymbols.Count > 0)
            {
                SelectedFamilySymbol = FamilySymbols[0];
                StatusMessage = $"已加载 {FamilySymbols.Count} 个族类型";
            }
            else
            {
                StatusMessage = "未找到常规模型族类型";
            }

            _logger.LogInformation("加载了 {Count} 个族类型", FamilySymbols.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载族类型时发生错误");
            StatusMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    /// <summary>
    /// 选择曲线
    /// </summary>
    [RelayCommand]
    private void PickCurve()
    {
        try
        {
            StatusMessage = "请在 Revit 中选择一条模型线...";

            // 隐藏窗口以便用户选择
            // (窗口会通过 Command 传递进来或者通过 WeakReference 持有)

            _selectedCurve = _revitService.PickModelCurve(_uiDocument);

            if (_selectedCurve != null)
            {
                StatusMessage = "已选择曲线，点击'执行阵列'开始";
                _logger.LogInformation("用户选择了曲线");
            }
            else
            {
                StatusMessage = "未选择曲线或选择已取消";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "选择曲线时发生错误");
            StatusMessage = $"选择失败: {ex.Message}";
        }
    }

    /// <summary>
    /// 执行阵列
    /// </summary>
    [RelayCommand]
    private void ExecuteArray()
    {
        try
        {
            // 验证输入
            if (SelectedFamilySymbol == null)
            {
                StatusMessage = "请先选择族类型";
                return;
            }

            if (_selectedCurve == null)
            {
                StatusMessage = "请先选择曲线";
                return;
            }

            if (ArrayCount <= 0)
            {
                StatusMessage = "阵列数量必须大于 0";
                return;
            }

            IsProcessing = true;
            StatusMessage = "正在执行阵列...";

            int createdCount = _revitService.ArrayFamilyAlongCurve(
                _uiDocument.Document,
                SelectedFamilySymbol.FamilySymbol,
                _selectedCurve,
                ArrayCount,
                IncludeEndPoints
            );

            if (createdCount > 0)
            {
                StatusMessage = $"成功创建 {createdCount} 个族实例";
                _logger.LogInformation("阵列执行成功，创建了 {Count} 个实例", createdCount);
            }
            else
            {
                StatusMessage = "阵列执行失败，未创建任何实例";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行阵列时发生错误");
            StatusMessage = $"阵列失败: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }
}

/// <summary>
/// 族类型包装类（用于 ComboBox 绑定）
/// </summary>
public class FamilySymbolItem
{
    public required FamilySymbol FamilySymbol { get; set; }
    public required string DisplayName { get; set; }
}
