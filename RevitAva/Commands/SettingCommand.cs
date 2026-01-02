using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAva.Views;

namespace RevitAva.Commands;

[Transaction(TransactionMode.Manual)]
public class SettingCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        // 显示 Avalonia 设置窗口
        var window = new SettingView();
        window.Show();
        return Result.Succeeded;
    }
}
