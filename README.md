# RevitAva

基于 Avalonia UI 框架的 Revit 插件项目。

## 项目简介

RevitAva 是一个 Revit 2026 插件，使用 Avalonia UI 框架构建用户界面。Avalonia 是一个跨平台的 XAML UI 框架，提供了现代化的 UI 开发体验。

## 技术栈

- **.NET 8** (C# 12)
- **Avalonia UI 11.2.7** - 跨平台 UI 框架
- **CommunityToolkit.Mvvm** - MVVM 框架
- **Microsoft.Extensions.Hosting** - 依赖注入和应用程序生命周期管理
- **Serilog** - 日志框架
- **Tuna.Revit.Extensions** - Revit API 扩展

## 项目结构

```
RevitAva/
├── Application.cs          # Revit 插件入口点
├── Host.cs                 # 应用程序主机配置
├── Commands/               # Revit 命令
│   └── SettingCommand.cs
├── Views/                  # Avalonia 视图
│   ├── SettingView.axaml
│   └── SettingView.axaml.cs
├── ViewModels/             # 视图模型
│   └── SettingViewModel.cs
├── Services/               # 服务层
│   └── AssemblyResolver.cs
├── Extensions/             # 扩展方法
└── Resources/              # 资源文件
    ├── Icons/
    └── Styles/
```

## Avalonia 在 Revit 插件中的技术架构

### 为什么可以在 Revit 中使用 Avalonia？

Revit 是基于 WPF 的应用程序，而 Avalonia 可以与 WPF 共存，因为它们都基于 Windows 的底层消息循环机制。

#### 技术原理

```
┌─────────────────────────────���───────┐
│   Windows 操作系统消息循环           │
│   (GetMessage/DispatchMessage)      │
└─────────────────────────────────────┘
           ↓              ↓
    ┌──────────┐    ┌──────────┐
    │   WPF    │    │ Avalonia │
    │ (Revit)  │    │  窗口    │
    └──────────┘    └──────────┘
```

**关键点**：
- Avalonia 和 WPF 共享的是**操作系统级别的消息循环**，而不是"借用"彼此的消息循环
- 每个窗口都有自己的 HWND（窗口句柄）和窗口过程
- `Avalonia.Win32.dll` 负责与 Windows API 交互，创建和管理窗口

### 初始化流程

在 `Application.cs` 中，Avalonia 的初始化非常简单：

```csharp
private static void InitializeAvalonia()
{
    if (_avaloniaInitialized) return;

    AppBuilder.Configure<Avalonia.Application>()
        .UsePlatformDetect()      // 自动检测平台（Windows/Linux/macOS）
        .LogToTrace()             // 启用日志输出
        .SetupWithoutStarting();  // 初始化但不启动应用程序生命周期

    // 设置 Fluent 主题
    Avalonia.Application.Current!.Styles.Add(new FluentTheme());

    _avaloniaInitialized = true;
}
```

**重要**：
- 使用 `SetupWithoutStarting()` 而不是 `StartWithClassicDesktopLifetime()`
- 不能使用 `UseDesktopLifetime()`，因为 Revit 已经控制了应用程序生命周期
- 只需初始化 Avalonia 框架，窗口会自动使用 Windows 消息循环

### 显示窗口

显示 Avalonia 窗口非常简单：

```csharp
var window = new SettingView();
window.Show();
```

窗口会自动：
1. 创建 Windows 原生窗口（HWND）
2. 注册到当前线程的消息队列
3. 处理自己的 UI 事件

## Avalonia 预览器限制

### 为什么 Avalonia 预览器在类库项目中不工作？

**WPF vs Avalonia 预览器的区别**：

| 特性 | WPF | Avalonia |
|------|-----|----------|
| **平台** | Windows 原生 | 跨平台 |
| **预览器类型** | IDE 内置 | 独立进程 |
| **需要入口点** | ❌ 不需要 | ✅ 需要 |
| **设计时支持** | 完整的设计时模式 | 需要运行时环境 |
| **类库预览** | ✅ 直接支持 | ⚠️ 需要特殊配置 |

**原因**：
- WPF 预览器直接解析 XAML 并渲染，不需要运行应用程序
- Avalonia 预览器需要启动完整的应用程序实例，需要 Main 入口点
- Revit 插件必须是类库（Library），不能有 Main 入口点

### 解决方案：使用 Avalonia DevTools

对于 Revit 插件开发，推荐使用**运行时调试**方式：

#### 1. 启用 DevTools

在窗口的构造函数中添加：

```csharp
public SettingView()
{
    InitializeComponent();

#if DEBUG_R26
    // Debug 模式下启用 DevTools（按 F12 打开）
    this.AttachDevTools();
#endif
}
```

#### 2. 使用 DevTools

1. 在 Revit 中加载插件
2. 打开 Avalonia 窗口（点击"设置"按钮）
3. 确保窗口处于焦点状态
4. **按 `F12` 键**打开 DevTools

#### 3. DevTools 功能

- **Visual Tree（可视化树）**：查看控件层次结构
- **Properties（属性）**：查看和修改控件属性
- **Styles（样式）**：查看应用的样式
- **Layout（布局）**：查看布局信息和边距
- **Events（事件）**：监控事件触发

## 开发指南

### 构建项目

```bash
# 使用 Debug R26 配置编译
dotnet build -c "Debug R26"

# 使用 Release R26 配置编译
dotnet build -c "Release R26"
```

**注意**：项目使用自定义配置名称（Debug R26 / Release R26），必须指定配置名称。

### 调试流程

1. **编译项目**：
   ```bash
   dotnet build -c "Debug R26"
   ```

2. **启动 Revit**：
   - 项目配置为自动启动 Revit 2026
   - 插件会自动复制到 Revit 插件目录

3. **查看日志**：
   - 日志文件位置：`bin/Debug R26/net8.0-windows/Logs/RevitAva.log`
   - 控制台输出（如果从 Visual Studio 启动）

4. **使用 DevTools**：
   - 打开 Avalonia 窗口后按 `F12`
   - 实时调试和调整 UI

### 添加新窗口

1. **创建 AXAML 文件**：
   ```xml
   <Window xmlns="https://github.com/avaloniaui"
           xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
           x:Class="RevitAva.Views.MyView"
           Title="我的窗口">
       <!-- UI 内容 -->
   </Window>
   ```

2. **创建 Code-behind**：
   ```csharp
   public partial class MyView : Window
   {
       public MyView()
       {
           InitializeComponent();
   #if DEBUG_R26
           this.AttachDevTools();
   #endif
       }
   }
   ```

3. **显示窗口**：
   ```csharp
   var window = new MyView();
   window.Show();
   ```

### MVVM 模式

项目使用 CommunityToolkit.Mvvm，推荐的 ViewModel 写法：

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "默认标题";

    [RelayCommand]
    private void DoSomething()
    {
        // 命令逻辑
    }
}
```

## 常见问题

### Q: 为什么不能使用 Avalonia 预览器？

A: Avalonia 预览器需要应用程序入口点（Main 方法），而 Revit 插件是类库项目。使用 DevTools（F12）进行运行时调试是更好的选择。

### Q: 如何在 Avalonia 窗口中访问 Revit API？

A: 通过依赖注入或静态服务定位器：

```csharp
// 在 Command 中传递 UIApplication
public class MyCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiApp = commandData.Application;
        var window = new MyView(uiApp);
        window.Show();
        return Result.Succeeded;
    }
}
```

### Q: Avalonia 窗口是否会阻塞 Revit？

A: 不会。使用 `Show()` 方法显示的窗口是非模态的，不会阻塞 Revit 主线程。如果需要模态窗口，使用 `ShowDialog()` 方法。

### Q: 如何处理 Avalonia 和 WPF 的资源冲突？

A: Avalonia 和 WPF 使用不同的命名空间和资源系统，不会冲突。注意：
- WPF 使用 `pack://application:,,,/` URI
- Avalonia 使用 `avares://` URI

## 许可证

请参考项目根目录的 LICENSE.txt 文件。

## 贡献

欢迎提交 Issue 和 Pull Request。

---

**技术支持**：如有问题，请查看日志文件或使用 Avalonia DevTools 进行调试。
