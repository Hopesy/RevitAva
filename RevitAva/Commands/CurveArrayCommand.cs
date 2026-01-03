using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RevitAva.Services.Interfaces;
using RevitAva.ViewModels;
using RevitAva.Views;

namespace RevitAva.Commands;

/// <summary>
/// 曲线阵列命令
/// </summary>
[Transaction(TransactionMode.Manual)]
public class CurveArrayCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;

            if (uiDoc == null)
            {
                message = "请先打开一个 Revit 文档";
                return Result.Failed;
            }

            // 从 DI 容器获取服务
            var logger = Host.GetService<ILogger<CurveArrayCommand>>();
            var revitService = Host.GetService<IRevitService>();

            // 创建 ViewModel（手动注入依赖）
            var viewModelLogger = Host.GetService<ILogger<CurveArrayViewModel>>();
            var viewModel = new CurveArrayViewModel(viewModelLogger, revitService, uiDoc);

            // 创建并显示窗口
            var window = new CurveArrayView(viewModel);
            window.Show();

            logger.LogInformation("曲线阵列窗口已打开");
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = $"打开曲线阵列窗口失败: {ex.Message}";
            return Result.Failed;
        }
    }
}
