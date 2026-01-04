using CommunityToolkit.Mvvm.Messaging.Messages;

namespace RevitAva.Messages;

/// <summary>
/// 关闭窗口消息
/// </summary>
public class CloseWindowMessage : ValueChangedMessage<Type>
{
    public CloseWindowMessage(Type windowType) : base(windowType) { }
}
