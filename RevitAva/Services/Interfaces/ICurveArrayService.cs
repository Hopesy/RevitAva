using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAva.Services.Interfaces;

/// <summary>
/// 曲线阵列服务接口
/// </summary>
public interface ICurveArrayService
{
    /// <summary>
    /// 获取文档中所有常规模型族类型
    /// </summary>
    List<FamilySymbol> GetAllFamilySymbols(Document document);

    /// <summary>
    /// 提示用户选择模型线
    /// </summary>
    Curve? PickModelCurve(UIDocument uiDocument);

    /// <summary>
    /// 沿曲线阵列族实例
    /// </summary>
    /// <returns>创建的族实例数量</returns>
    int ArrayFamilyAlongCurve(Document document, FamilySymbol familySymbol, Curve curve, int count, bool includeEndPoints);
}
