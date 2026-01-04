using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAva.Services.Interfaces;

namespace RevitAva.Commands;

[Transaction(TransactionMode.Manual)]
public class SwitchThemeCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var themeService = Host.GetService<IThemeService>();
            themeService.ToggleTheme();
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return Result.Failed;
        }
    }
}
