using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAva.Services.Interfaces;

/// <summary>
/// Revit 上下文服务接口
/// </summary>
public interface IRevitContext
{
    UIApplication UIApplication { get; }
    UIDocument UIDocument { get; }
    Document Document { get; }
}
