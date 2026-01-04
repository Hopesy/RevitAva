using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAva.Extensions;
using RevitAva.Views;

namespace RevitAva.Commands;
// show非模态:事务都要放到外部事件里
// showdialog模式窗口: 要在return result之前执行事务
//1.在viewmodel里面先执行Transaction，再关掉窗口(不适合交互选取PickObject的场景)
//2.在viewmodel里面关掉窗口,在command里面调用viewmodel实例里面的方法执行Transaction
//3.Transaction直接定义在command里面，窗口关闭后通过viewmodel仅仅传递数据
[Transaction(TransactionMode.Manual)]
public class CurveArrayCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        try
        {
            var window = Host.GetService<CurveArrayView>();
            window.ShowWindow();
            //由于不想在pick的时候临时关闭窗口，索性在窗口关闭后在执行pick，trans
            window.Execute();
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return Result.Failed;
        }
    }
}
