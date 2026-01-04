using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAva.Services.Interfaces;

namespace RevitAva.Services;

/// <summary>
/// Revit 上下文服务
/// </summary>
public class RevitContext : IRevitContext
{
    private readonly UIApplication _uiApplication;

    public RevitContext(UIApplication uiApplication)
    {
        _uiApplication = uiApplication;
    }

    public UIApplication UIApplication => _uiApplication;
    public UIDocument UIDocument => _uiApplication.ActiveUIDocument;
    public Document Document => UIDocument.Document;
}
