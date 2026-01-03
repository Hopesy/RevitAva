using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAva.Services.Interfaces;

/// <summary>
/// Revit 操作服务接口
/// 提供族类型选择、曲线选择、阵列等 Revit API 操作
/// </summary>
public interface IRevitService
{
    /// <summary>
    /// 获取文档中所有常规模型族类型
    /// </summary>
    /// <param name="document">当前文档</param>
    /// <returns>族类型列表</returns>
    List<FamilySymbol> GetAllFamilySymbols(Document document);

    /// <summary>
    /// 提示用户选择模型线
    /// </summary>
    /// <param name="uiDocument">UI文档</param>
    /// <returns>选中的曲线，如果取消则返回 null</returns>
    Curve? PickModelCurve(UIDocument uiDocument);

    /// <summary>
    /// 沿曲线阵列族实例
    /// </summary>
    /// <param name="document">当前文档</param>
    /// <param name="familySymbol">族类型</param>
    /// <param name="curve">曲线</param>
    /// <param name="count">阵列数量</param>
    /// <param name="includeEndPoints">是否包含端点</param>
    /// <returns>创建的族实例数量</returns>
    int ArrayFamilyAlongCurve(Document document, FamilySymbol familySymbol, Curve curve, int count, bool includeEndPoints);
}
