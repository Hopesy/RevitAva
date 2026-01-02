# RevitAva

> 在 Revit 插件中使用 Avalonia UI 框架构建现代化用户界面。

- **.NET 8** + **Avalonia UI 11.2.7**
- **Revit 2026** + **Tuna.Revit.Extensions**
- **CommunityToolkit.Mvvm** + **Microsoft.Extensions.Hosting**

---

## 安装依赖

### 必需的 NuGet 包

```bash
# Avalonia 核心包
dotnet add package Avalonia --version 11.2.7
dotnet add package Avalonia.Desktop --version 11.2.7
dotnet add package Avalonia.Themes.Fluent --version 11.2.7

# Debug 模式下的开发工具（F12 调试）
dotnet add package Avalonia.Diagnostics --version 11.2.7

# MVVM 框架（可选）
dotnet add package CommunityToolkit.Mvvm --version 8.4.0
```

### 项目配置

在 `.csproj` 文件中添加：

```xml
<PropertyGroup>
  <!-- 启用 Avalonia 编译时绑定 -->
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
</PropertyGroup>
```

---

## 快速开始：在 Revit 插件中使用 Avalonia

### 步骤 1：初始化 Avalonia 框架

在 Revit 插件的 `OnStartup` 方法中初始化 Avalonia：

```csharp
public class Application : IExternalApplication
{
    private static bool _avaloniaInitialized;

    public Result OnStartup(UIControlledApplication application)
    {
        // 初始化 Avalonia
         AppBuilder.Configure<Avalonia.Application>()
            .UsePlatformDetect()      // 1. 检测平台并加载对应后端
            .LogToTrace()             // 2. 启用日志输出
            .SetupWithoutStarting();  // 3. 初始化框架但不启动生命周期
        // 4. 添加主题
        Avalonia.Application.Current!.Styles.Add(new FluentTheme());

        return Result.Succeeded;
    }
}
```

### 步骤 2：创建 Avalonia 窗口

```csharp
// Views/SettingView.axaml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="RevitAva.Views.SettingView"
        Title="设置" Width="800" Height="600">
    <TextBlock Text="Hello Avalonia!" />
</Window>
```

```csharp
// Views/SettingView.axaml.cs
public partial class SettingView : Window
{
    public SettingView()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();  // 启动插件后打开功能窗口按F12打开调试工具
#endif
    }
}
```

### 步骤 3：在 Revit 命令中显示窗口

```csharp
public class SettingCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var window = new SettingView();
        window.Show();  // 显示非模态窗口

        return Result.Succeeded;
    }
}
```

**完成！** 就这么简单，三个步骤即可在 Revit 中使用 Avalonia。

---
## 原理解释

### 初始化

> `UsePlatformDetect()` - 平台检测与后端加载

```
检测操作系统
    ↓
Windows → 加载 Avalonia.Win32.dll
    ├─ Win32WindowImpl (窗口管理)
    ├─ SkiaRenderTarget (Skia 渲染引擎)
    ├─ Win32InputManager (输入处理)
    └─ Win32ClipboardImpl (剪贴板)
```

**作用**：注册平台相关的服务，让 Avalonia 知道如何在 Windows 上创建窗口、渲染 UI、处理输入。

 > `SetupWithoutStarting()` - 初始化框架

- ✅ 初始化了 Avalonia 框架:平台服务、样式系统、资源系统等
- ✅ 可以创建窗口和控件
- ❌ **不启动消息循环**
- ❌ **不创建应用程序生命周期管理器**


> 添加主题。Avalonia 没有内置主题，必须手动添加。FluentTheme 提供了所有控件的默认样式。

```csharp
Avalonia.Application.Current!.Styles.Add(new FluentTheme());
```

### 生命周期

> 为什么用 `SetupWithoutStarting()` 而不是 `StartWithClassicDesktopLifetime()`？
    ↓
* `StartWithClassicDesktopLifetime()` -> 阻塞在这里,Revit 主窗口无法显示,Revit 看起来"卡死"了
*  `SetupWithoutStarting()` -> 初始化Avalonia框架配置但不启动应用程序生命周期，由Revit统一管理消息循环

```csharp
// Revit 插件
AppBuilder.Configure<Avalonia.Application>()
    .UsePlatformDetect()
    .SetupWithoutStarting();  // ← 立即返回
```

**效果**：
```
Revit 启动
    ↓
加载插件
    ↓
调用 OnStartup()
    ↓
SetupWithoutStarting() ← 立即返回
    ↓
✅ Revit 继续启动
✅ Revit 主窗口正常显示
```

### 消息循环本质

> Avalonia窗口如何工作:所有 Windows 应用程序都依赖**消息循环**来处理 UI 事件

*  Windows的消息分发基于HWND，不是基于框架
* 每个窗口有自己的HWND和WndProc
* 只要有一个消息循环在运行，所有窗口都能工作
* Revit 的消息循环已经在运行了，只需要"注册"(创建窗口)，系统自动处理其余的

```csharp
// Windows 消息循环（简化）
while (GetMessage(out MSG msg, IntPtr.Zero, 0, 0))
{
    // 1. 获取消息（鼠标点击、键盘输入等）
    // 2. 分发消息到对应的窗口
    DispatchMessage(ref msg);
}
```

**消息流程**：
```
用户点击按钮
    ↓
Windows 生成 WM_LBUTTONDOWN 消息
    ↓
放入消息队列
    ↓
GetMessage() 取出消息
    ↓
DispatchMessage() 根据 HWND 分发
    ↓
调用窗口的 WndProc 处理
```

**关键点**：
- ✅ Avalonia 窗口**自动注册**到 Windows 消息系统
- ✅ 使用 Revit 的消息循环（共享操作系统的消息循环）
- ✅ 不需要启动新的消息循环
- ✅ 不会阻塞 Revit

---

## 开发

### AXAML 预览

> Avalonia预览器在Revit插件中不工作,推荐使用使用 **Avalonia DevTools**（运行时调试）

- Avalonia 预览器需要应用程序入口点（`Main` 方法）
- Revit 插件是类库项目（Library），没有 `Main` 入口点
- 预览器需要启动完整的应用程序实例才能渲染 UI

#### 1. 启用 DevTools

在窗口的构造函数中添加：

```csharp
public SettingView()
{
    InitializeComponent();

#if DEBUG
    // Debug 模式下启用 DevTools（按 F12 打开）
    this.AttachDevTools();
#endif
}
```

**注意**：确保已安装 `Avalonia.Diagnostics` 包：
```bash
dotnet add package Avalonia.Diagnostics --version 11.2.7
```

#### 2. 使用 DevTools

1. 在 Revit 中加载插件
2. 打开 Avalonia 窗口（点击"设置"按钮）
3. **确保窗口处于焦点状态**
4. **按 `F12` 键**打开 DevTools

#### 3. DevTools 功能

| 功能            | 说明                                           |
| --------------- | ---------------------------------------------- |
| **Visual Tree** | 查看控件层次结构，类似 WPF 的 Live Visual Tree |
| **Properties**  | 实时查看和修改控件属性                         |
| **Styles**      | 查看应用的样式和样式优先级                     |
| **Layout**      | 查看布局信息、边距、对齐方式                   |
| **Events**      | 监控事件触发（PointerPressed、Click 等）       |
| **Console**     | 查看日志输出和错误信息                         |

#### 4. 调试技巧

**实时修改属性**：
```
1. 在 Visual Tree 中选择控件
2. 在 Properties 面板修改属性值
3. 立即看到效果（无需重新编译）
```

**查找控件**：
```
1. 点击 DevTools 的"选择"按钮
2. 在窗口中点击任意控件
3. 自动在 Visual Tree 中定位该控件
```

**样式调试**：
```
1. 选择控件
2. 查看 Styles 面板
3. 查看哪些样式被应用，哪些被覆盖
```

### 项目结构

```
RevitAva/
├── Application.cs          # Revit 插件入口，初始化 Avalonia
├── Commands/               # Revit 命令
├── Views/                  # Avalonia 视图（.axaml）
├── ViewModels/             # 视图模型（MVVM）
├── Services/               # 服务层
└── Docs/                   # 详细文档
    ├── 原理.md             # 技术原理详解
    ├── 生命周期对比.md     # 生命周期详细对比
    └── 消息循环详解.md     # 消息循环机制详解
```

---
### 核心要点总结

1. **初始化**：使用 `SetupWithoutStarting()` 初始化框架，不启动生命周期
2. **生命周期**：Revit 控制应用程序生命周期，Avalonia 只负责窗口
3. **消息循环**：Avalonia 窗口自动注册到 Windows 消息系统，使用 Revit 的消息循环
4. **共存原理**：通过独立的 HWND 和窗口过程，Avalonia 和 WPF 和平共处

**更多详细信息**，请查看 `Docs/` 目录下的文档。
---

**License**: MIT
