using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Extensions.Logging;
using RevitAva.Services.Interfaces;

namespace RevitAva.Services;

/// <summary>
/// Revit 操作服务实现类
/// </summary>
public class RevitService : IRevitService
{
    private readonly ILogger<RevitService> _logger;

    public RevitService(ILogger<RevitService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取文档中所有常规模型族类型
    /// </summary>
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
            return new List<FamilySymbol>();
        }
    }

    /// <summary>
    /// 提示用户选择模型线
    /// </summary>
    public Curve? PickModelCurve(UIDocument uiDocument)
    {
        try
        {
            var selection = uiDocument.Selection;
            var reference = selection.PickObject(ObjectType.Element, new ModelCurveSelectionFilter(), "请选择一条模型线");

            if (reference == null)
            {
                _logger.LogWarning("用户取消了选择");
                return null;
            }

            var element = uiDocument.Document.GetElement(reference);
            if (element is ModelCurve modelCurve)
            {
                _logger.LogInformation("用户选择了模型线: {ElementId}", element.Id);
                return modelCurve.GeometryCurve;
            }

            _logger.LogWarning("选中的元素不是模型线");
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

    /// <summary>
    /// 沿曲线阵列族实例
    /// </summary>
    public int ArrayFamilyAlongCurve(Document document, FamilySymbol familySymbol, Curve curve, int count, bool includeEndPoints)
    {
        try
        {
            // 激活族类型（如果尚未激活）
            if (!familySymbol.IsActive)
            {
                using var activateTransaction = new Transaction(document, "激活族类型");
                activateTransaction.Start();
                familySymbol.Activate();
                activateTransaction.Commit();
            }

            int createdCount = 0;

            using var transaction = new Transaction(document, "沿曲线阵列族实例");
            transaction.Start();

            // 计算阵列点
            var points = CalculateArrayPoints(curve, count, includeEndPoints);

            // 在每个点创建族实例
            foreach (var point in points)
            {
                // 创建族实例（在项目中，Level 1）
                var level = new FilteredElementCollector(document)
                    .OfClass(typeof(Level))
                    .FirstElement() as Level;

                if (level == null)
                {
                    _logger.LogError("未找到标高");
                    transaction.RollBack();
                    return 0;
                }

                var instance = document.Create.NewFamilyInstance(point, familySymbol, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                if (instance != null)
                {
                    createdCount++;
                }
            }

            transaction.Commit();
            _logger.LogInformation("成功创建 {Count} 个族实例", createdCount);
            return createdCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "阵列族实例时发生错误");
            return 0;
        }
    }

    /// <summary>
    /// 计算阵列点位置
    /// </summary>
    private List<XYZ> CalculateArrayPoints(Curve curve, int count, bool includeEndPoints)
    {
        var points = new List<XYZ>();

        if (count <= 0)
        {
            return points;
        }

        if (count == 1)
        {
            // 只有一个点，放在中点
            points.Add(curve.Evaluate(0.5, true));
            return points;
        }

        if (includeEndPoints)
        {
            // 包含端点：均匀分布在曲线上（包括起点和终点）
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

    /// <summary>
    /// 模型线选择过滤器
    /// </summary>
    private class ModelCurveSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is ModelCurve;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
