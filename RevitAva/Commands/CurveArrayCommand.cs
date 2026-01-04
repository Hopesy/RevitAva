using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAva.Extensions;
using RevitAva.Views;

namespace RevitAva.Commands;

[Transaction(TransactionMode.Manual)]
public class CurveArrayCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var window = Host.GetService<CurveArrayView>();
            window.Initialize(commandData.Application.ActiveUIDocument);
            window.ShowModal();
            
            // ShowDialog 阻塞返回后，在 Revit 线程执行事务
            
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return Result.Failed;
        }
    }
}
