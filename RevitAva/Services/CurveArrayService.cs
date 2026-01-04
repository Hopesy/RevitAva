using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Extensions.Logging;
using RevitAva.Services.Interfaces;

namespace RevitAva.Services;

/// <summary>
/// 曲线阵列服务实现
/// </summary>
public class CurveArrayService : ICurveArrayService
{
    private readonly ILogger<CurveArrayService> _logger;

    public CurveArrayService(ILogger<CurveArrayService> logger)
    {
        _logger = logger;
    }

    public List<FamilySymbol> GetAllFamilySymbols(Document document)
    {
        try
        {
            var collector = new FilteredElementCollector(document)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .Cast<FamilySymbol>()
                .Where(fs => fs.Family != null)
                .OrderBy(fs => fs.FamilyName)
                .ThenBy(fs => fs.Name)
                .ToList();

            _logger.LogInformation("找到 {Count} 个常规模型族类型", collector.Count);
            return collector;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取族类型列表时发生错误");
            return [];
        }
    }

    public Curve? PickModelCurve(UIDocument uiDocument)
    {
        try
        {
            var reference = uiDocument.Selection.PickObject(
                ObjectType.Element, 
                new ModelCurveSelectionFilter(), 
                "请选择一条模型线");

            if (uiDocument.Document.GetElement(reference) is ModelCurve modelCurve)
            {
                _logger.LogInformation("用户选择了模型线: {ElementId}", modelCurve.Id);
                return modelCurve.GeometryCurve;
            }

            return null;
        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            _logger.LogInformation("用户取消了选择操作");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "选择模型线时发生错误");
            return null;
        }
    }

    public int ArrayFamilyAlongCurve(Document document, FamilySymbol familySymbol, Curve curve, int count, bool includeEndPoints)
    {
        try
        {
            // 激活族类型
            if (!familySymbol.IsActive)
            {
                familySymbol.Activate();
                document.Regenerate();
            }

            // 获取标高
            var level = new FilteredElementCollector(document)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .OrderBy(l => l.Elevation)
                .FirstOrDefault();

            if (level == null)
            {
                _logger.LogError("未找到标高");
                return 0;
            }

            // 计算阵列点并创建实例
            var points = CalculateArrayPoints(curve, count, includeEndPoints);
            int createdCount = 0;

            foreach (var point in points)
            {
                var instance = document.Create.NewFamilyInstance(
                    point, 
                    familySymbol, 
                    level, 
                    Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                if (instance != null) createdCount++;
            }

            _logger.LogInformation("成功创建 {Count} 个族实例", createdCount);
            return createdCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "阵列族实例时发生错误");
            return 0;
        }
    }

    private static List<XYZ> CalculateArrayPoints(Curve curve, int count, bool includeEndPoints)
    {
        if (count <= 0) return [];
        if (count == 1) return [curve.Evaluate(0.5, true)];

        var points = new List<XYZ>();

        if (includeEndPoints)
        {
            // 包含端点：均匀分布（包括起点和终点）
            for (int i = 0; i < count; i++)
            {
                double parameter = (double)i / (count - 1);
                points.Add(curve.Evaluate(parameter, true));
            }
        }
        else
        {
            // 不包含端点：在曲线内部均匀分布
            for (int i = 0; i < count; i++)
            {
                double parameter = (i + 1.0) / (count + 1.0);
                points.Add(curve.Evaluate(parameter, true));
            }
        }

        return points;
    }

    private class ModelCurveSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem) => elem is ModelCurve;
        public bool AllowReference(Reference reference, XYZ position) => false;
    }
}
