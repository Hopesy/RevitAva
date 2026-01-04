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
        // showdialog模式下: 总之要在return result之前执行事务
        //1.在viewmodel里面先执行Transaction再关掉窗口
        //2.在viewmodel里面关掉窗口,在command里面调用viewmodel实例里面的方法执行事务
        //3.把事务放到command里面，窗口关闭后通过viewmodel只是传递数据
        try
        {
            var window = Host.GetService<CurveArrayView>();
            window.Initialize(commandData.Application.ActiveUIDocument);
            window.ShowModal();
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return Result.Failed;
        }
    }
}
