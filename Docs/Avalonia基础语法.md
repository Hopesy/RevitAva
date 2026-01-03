
    # Avalonia 样式定义与绑定语法详解

## 目录
- [1. 样式系统基础](#1-样式系统基础)
- [2. 选择器 (Selectors)](#2-选择器-selectors)
- [3. 模板系统](#3-模板系统)
- [4. 数据绑定语法](#4-数据绑定语法)
- [5. 资源系统](#5-资源系统)
- [6. 主题与样式继承](#6-主题与样式继承)

---

## 1. 样式系统基础

### 1.1 样式定义语法

在 Avalonia 中，样式使用 `<Style>` 标签定义，通常放置在 `<Window.Styles>` 或 `<Application.Styles>` 中：

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Window.Styles>
        <Style Selector="Button">
            <Setter Property="Background" Value="Blue"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="Padding" Value="10,5"/>
        </Style>
    </Window.Styles>
</Window>
```

### 1.2 Setter 语法

每个样式包含一个或多个 `Setter`，用于设置属性值：

```xml
<!-- 基本 Setter -->
<Setter Property="Width" Value="100"/>

<!-- 使用绑定的 Setter -->
<Setter Property="IsVisible" Value="{Binding IsActive}"/>

<!-- 使用资源的 Setter -->
<Setter Property="Background" Value="{DynamicResource PrimaryBrush}"/>
```

---

## 2. 选择器 (Selectors)

Avalonia 的选择器语法比 WPF 更强大，类似 CSS。

### 2.1 基本选择器

#### 类型选择器
```xml
<!-- 匹配所有 Button 控件 -->
<Style Selector="Button">
    <Setter Property="Background" Value="Blue"/>
</Style>
```

#### 名称选择器
```xml
<!-- 匹配 x:Name="MyButton" 的控件 -->
<Style Selector="#MyButton">
    <Setter Property="Background" Value="Red"/>
</Style>
```

#### 类选择器 (StyleClass)
```xml
<!-- 定义样式类 -->
<Button Classes="primary large">Click Me</Button>

<!-- 匹配包含 primary 类的按钮 -->
<Style Selector="Button.primary">
    <Setter Property="Background" Value="Blue"/>
</Style>

<!-- 匹配同时包含多个类的按钮 -->
<Style Selector="Button.primary.large">
    <Setter Property="FontSize" Value="20"/>
</Style>
```

### 2.2 组合选择器

#### 后代选择器 (空格)
```xml
<!-- 匹配 StackPanel 内的所有 Button -->
<Style Selector="StackPanel Button">
    <Setter Property="Margin" Value="5"/>
</Style>
```

#### 子选择器 (>)
```xml
<!-- 仅匹配 StackPanel 的直接子 Button -->
<Style Selector="StackPanel > Button">
    <Setter Property="Margin" Value="10"/>
</Style>
```

#### 兄弟选择器
```xml
<!-- 匹配同一级的相邻兄弟元素 -->
<Style Selector="Button + TextBlock">
    <Setter Property="Margin" Value="5,0,0,0"/>
</Style>
```

### 2.3 伪类选择器

#### 交互状态伪类
```xml
<!-- 鼠标悬停状态 -->
<Style Selector="Button:pointerover">
    <Setter Property="Background" Value="LightBlue"/>
</Style>

<!-- 按下状态 -->
<Style Selector="Button:pressed">
    <Setter Property="Background" Value="DarkBlue"/>
</Style>

<!-- 获得焦点状态 -->
<Style Selector="TextBox:focus">
    <Setter Property="BorderBrush" Value="Blue"/>
    <Setter Property="BorderThickness" Value="2"/>
</Style>

<!-- 禁用状态 -->
<Style Selector="Button:disabled">
    <Setter Property="Opacity" Value="0.5"/>
</Style>
```

#### 选中状态伪类
```xml
<!-- CheckBox 选中状态 -->
<Style Selector="CheckBox:checked">
    <Setter Property="Foreground" Value="Green"/>
</Style>

<!-- ListBoxItem 选中状态 -->
<Style Selector="ListBoxItem:selected">
    <Setter Property="Background" Value="Blue"/>
</Style>
```

#### 其他常用伪类
```xml
<!-- 第 n 个子元素 -->
<Style Selector="ListBoxItem:nth-child(2n)">
    <Setter Property="Background" Value="LightGray"/>
</Style>

<!-- 第一个子元素 -->
<Style Selector="StackPanel > :first-child">
    <Setter Property="Margin" Value="0"/>
</Style>

<!-- 最后一个子元素 -->
<Style Selector="StackPanel > :last-child">
    <Setter Property="Margin" Value="0"/>
</Style>
```

### 2.4 模板部分选择器 (Template Parts)

```xml
<!-- 选择控件模板中的特定部分 -->
<Style Selector="Button /template/ ContentPresenter">
    <Setter Property="Margin" Value="5"/>
</Style>

<!-- 选择 ScrollViewer 中的滚动条 -->
<Style Selector="ScrollViewer /template/ ScrollBar">
    <Setter Property="Background" Value="Transparent"/>
</Style>
```

### 2.5 属性选择器

```xml
<!-- 匹配具有特定属性值的控件 -->
<Style Selector="Button[IsDefault=True]">
    <Setter Property="BorderBrush" Value="Blue"/>
    <Setter Property="BorderThickness" Value="2"/>
</Style>
```

### 2.6 NOT 选择器

```xml
<!-- 匹配不包含 primary 类的按钮 -->
<Style Selector="Button:not(.primary)">
    <Setter Property="Background" Value="Gray"/>
</Style>

<!-- 匹配未禁用的按钮 -->
<Style Selector="Button:not(:disabled)">
    <Setter Property="Cursor" Value="Hand"/>
</Style>
```

---

## 3. 模板系统

### 3.1 ControlTemplate（控件模板）

控件模板定义控件的视觉结构：

```xml
<Style Selector="Button.custom">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    CornerRadius="5">
                <ContentPresenter Content="{TemplateBinding Content}"
                                  HorizontalAlignment="Center"
                                  VerticalAlignment="Center"/>
            </Border>
        </ControlTemplate>
    </Setter>
</Style>
```

#### TemplateBinding
用于在模板中绑定到模板化父控件的属性：

```xml
<!-- 简写语法 -->
<Border Background="{TemplateBinding Background}"/>

<!-- 完整绑定语法（支持转换器） -->
<Border Background="{Binding Background, RelativeSource={RelativeSource TemplatedParent}}"/>
```

### 3.2 DataTemplate（数据模板）

数据模板定义数据对象的显示方式：

```xml
<!-- 在 ListBox 中使用 DataTemplate -->
<ListBox Items="{Binding Users}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <Image Source="{Binding Avatar}" Width="32" Height="32"/>
                <TextBlock Text="{Binding Name}" Margin="10,0,0,0"/>
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### 类型化 DataTemplate
```xml
<!-- 为特定类型定义模板 -->
<Window.DataTemplates>
    <DataTemplate DataType="{x:Type local:UserViewModel}">
        <StackPanel>
            <TextBlock Text="{Binding Name}" FontWeight="Bold"/>
            <TextBlock Text="{Binding Email}" FontSize="12"/>
        </StackPanel>
    </DataTemplate>
</Window.DataTemplates>

<!-- 使用时会自动应用模板 -->
<ContentControl Content="{Binding CurrentUser}"/>
```

### 3.3 ItemsPanelTemplate（容器面板模板）

定义 ItemsControl 中项的排列方式：

```xml
<ListBox Items="{Binding Items}">
    <ListBox.ItemsPanel>
        <ItemsPanelTemplate>
            <WrapPanel Orientation="Horizontal"/>
        </ItemsPanelTemplate>
    </ListBox.ItemsPanel>
</ListBox>
```

### 3.4 FlyoutPresenter 模板

用于弹出菜单和工具提示：

```xml
<Button Content="Open Menu">
    <Button.Flyout>
        <MenuFlyout>
            <MenuItem Header="选项 1"/>
            <MenuItem Header="选项 2"/>
        </MenuFlyout>
    </Button.Flyout>
</Button>
```

---

## 4. 数据绑定语法

### 4.1 基本绑定

```xml
<!-- 绑定到 DataContext 的属性 -->
<TextBlock Text="{Binding Name}"/>

<!-- 绑定到指定路径 -->
<TextBlock Text="{Binding User.Name}"/>

<!-- 双向绑定 -->
<TextBox Text="{Binding Name, Mode=TwoWay}"/>
```

### 4.2 绑定模式 (Mode)

```xml
<!-- OneWay: 单向绑定（源 → 目标） -->
<TextBlock Text="{Binding Name, Mode=OneWay}"/>

<!-- TwoWay: 双向绑定（源 ↔ 目标） -->
<TextBox Text="{Binding Name, Mode=TwoWay}"/>

<!-- OneTime: 一次性绑定 -->
<TextBlock Text="{Binding Name, Mode=OneTime}"/>

<!-- OneWayToSource: 反向单向绑定（目标 → 源） -->
<Slider Value="{Binding Volume, Mode=OneWayToSource}"/>
```

### 4.3 RelativeSource 绑定

```xml
<!-- 绑定到自身 -->
<Border Width="{Binding $self.Height}"/>

<!-- 绑定到父控件 -->
<TextBlock Text="{Binding $parent.DataContext.Title}"/>

<!-- 绑定到指定类型的父控件 -->
<TextBlock Text="{Binding $parent[Window].Title}"/>

<!-- 绑定到模板化父控件 -->
<Border Background="{Binding $parent[TemplatedControl].Background}"/>
```

### 4.4 ElementName 绑定

```xml
<!-- 绑定到其他命名元素 -->
<Slider x:Name="VolumeSlider" Minimum="0" Maximum="100"/>
<TextBlock Text="{Binding #VolumeSlider.Value}"/>

<!-- 注意：Avalonia 使用 # 前缀，而非 ElementName= -->
```

### 4.5 绑定到静态资源

```xml
<!-- 绑定到静态属性 -->
<TextBlock Text="{Binding Source={x:Static local:Constants.AppName}}"/>
```

### 4.6 转换器 (Converter)

```xml
<!-- 使用值转换器 -->
<TextBlock Text="{Binding IsActive, Converter={StaticResource BoolToStringConverter}}"/>

<!-- 带参数的转换器 -->
<TextBlock Foreground="{Binding Status,
                               Converter={StaticResource StatusToColorConverter},
                               ConverterParameter=Warning}"/>
```

转换器定义示例：

```csharp
public class BoolToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "激活" : "未激活";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == "激活";
    }
}
```

### 4.7 多重绑定 (MultiBinding)

```xml
<TextBlock>
    <TextBlock.Text>
        <MultiBinding Converter="{StaticResource StringFormatConverter}">
            <Binding Path="FirstName"/>
            <Binding Path="LastName"/>
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```

### 4.8 绑定回退值与目标空值

```xml
<!-- FallbackValue: 绑定失败时的回退值 -->
<TextBlock Text="{Binding Name, FallbackValue='未命名'}"/>

<!-- TargetNullValue: 源值为 null 时的显示值 -->
<TextBlock Text="{Binding Description, TargetNullValue='暂无描述'}"/>
```

### 4.9 字符串格式化

```xml
<!-- StringFormat -->
<TextBlock Text="{Binding Price, StringFormat='￥{0:F2}'}"/>
<TextBlock Text="{Binding Count, StringFormat='共 {0} 项'}"/>
<TextBlock Text="{Binding Date, StringFormat='日期：{0:yyyy-MM-dd}'}"/>
```

### 4.10 Compiled Bindings（编译时绑定）

Avalonia 支持编译时类型检查的绑定，性能更好：

```xml
<!-- 启用编译绑定 -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:vm="using:MyApp.ViewModels"
        x:DataType="vm:MainViewModel">

    <!-- 编译时检查类型 -->
    <TextBlock Text="{Binding Name}"/>
    <TextBox Text="{Binding Email}"/>
</Window>
```

---

## 5. 资源系统

### 5.1 定义资源

```xml
<Window.Resources>
    <!-- 纯色画刷 -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#007ACC"/>

    <!-- 渐变画刷 -->
    <LinearGradientBrush x:Key="GradientBrush" StartPoint="0%,0%" EndPoint="100%,100%">
        <GradientStop Color="Blue" Offset="0"/>
        <GradientStop Color="Purple" Offset="1"/>
    </LinearGradientBrush>

    <!-- 数值资源 -->
    <system:Double x:Key="DefaultFontSize">14</system:Double>

    <!-- 几何图形 -->
    <StreamGeometry x:Key="CheckIcon">M 0,5 L 5,10 L 15,0</StreamGeometry>
</Window.Resources>
```

### 5.2 使用资源

```xml
<!-- StaticResource: 静态资源（编译时解析） -->
<Button Background="{StaticResource PrimaryBrush}"/>

<!-- DynamicResource: 动态资源（运行时解析，支持主题切换） -->
<Button Background="{DynamicResource PrimaryBrush}"/>
```

### 5.3 资源字典 (ResourceDictionary)

```xml
<!-- App.axaml -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceInclude Source="/Styles/Colors.axaml"/>
            <ResourceInclude Source="/Styles/Buttons.axaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

独立资源文件示例（Colors.axaml）：

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <SolidColorBrush x:Key="PrimaryColor" Color="#007ACC"/>
    <SolidColorBrush x:Key="SecondaryColor" Color="#68217A"/>
    <SolidColorBrush x:Key="AccentColor" Color="#FFB900"/>
</ResourceDictionary>
```

---

## 6. 主题与样式继承

### 6.1 样式继承 (BasedOn)

```xml
<!-- 基础样式 -->
<Style Selector="Button.base" x:Key="BaseButtonStyle">
    <Setter Property="Padding" Value="10,5"/>
    <Setter Property="CornerRadius" Value="4"/>
</Style>

<!-- 继承样式 -->
<Style Selector="Button.primary" BasedOn="{StaticResource BaseButtonStyle}">
    <Setter Property="Background" Value="Blue"/>
    <Setter Property="Foreground" Value="White"/>
</Style>
```

### 6.2 主题切换

Avalonia 支持运行时主题切换：

```csharp
// 切换到深色主题
Application.Current.RequestedThemeVariant = ThemeVariant.Dark;

// 切换到浅色主题
Application.Current.RequestedThemeVariant = ThemeVariant.Light;
```

在 XAML 中定义主题变体：

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.ThemeDictionaries>
            <!-- 浅色主题 -->
            <ResourceDictionary x:Key="Light">
                <SolidColorBrush x:Key="BackgroundBrush" Color="White"/>
                <SolidColorBrush x:Key="ForegroundBrush" Color="Black"/>
            </ResourceDictionary>

            <!-- 深色主题 -->
            <ResourceDictionary x:Key="Dark">
                <SolidColorBrush x:Key="BackgroundBrush" Color="#1E1E1E"/>
                <SolidColorBrush x:Key="ForegroundBrush" Color="White"/>
            </ResourceDictionary>
        </ResourceDictionary.ThemeDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### 6.3 Fluent 主题

使用官方 Fluent 主题：

```xml
<!-- App.axaml -->
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MyApp.App">
    <Application.Styles>
        <FluentTheme />
    </Application.Styles>
</Application>
```

自定义 Fluent 主题颜色：

```xml
<Application.Styles>
    <FluentTheme>
        <FluentTheme.Palettes>
            <ColorPaletteResources x:Key="Light"
                                   Accent="Blue"
                                   AltHigh="White"
                                   AltLow="White"/>
            <ColorPaletteResources x:Key="Dark"
                                   Accent="Blue"
                                   AltHigh="#1E1E1E"
                                   AltLow="#1E1E1E"/>
        </FluentTheme.Palettes>
    </FluentTheme>
</Application.Styles>
```

---

## 7. 常用控件样式示例

### 7.1 圆角按钮

```xml
<Style Selector="Button.rounded">
    <Setter Property="Background" Value="#007ACC"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="Padding" Value="15,8"/>
    <Setter Property="CornerRadius" Value="20"/>
    <Setter Property="BorderThickness" Value="0"/>
</Style>

<Style Selector="Button.rounded:pointerover">
    <Setter Property="Background" Value="#005A9E"/>
</Style>

<Style Selector="Button.rounded:pressed">
    <Setter Property="Background" Value="#004377"/>
</Style>
```

### 7.2 自定义 TextBox

```xml
<Style Selector="TextBox.modern">
    <Setter Property="BorderBrush" Value="#CCCCCC"/>
    <Setter Property="BorderThickness" Value="0,0,0,2"/>
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="Padding" Value="5"/>
</Style>

<Style Selector="TextBox.modern:focus">
    <Setter Property="BorderBrush" Value="#007ACC"/>
</Style>
```

### 7.3 卡片容器

```xml
<Style Selector="Border.card">
    <Setter Property="Background" Value="White"/>
    <Setter Property="BorderBrush" Value="#E0E0E0"/>
    <Setter Property="BorderThickness" Value="1"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="Padding" Value="16"/>
    <Setter Property="BoxShadow" Value="0 2 8 0 #10000000"/>
</Style>

<Style Selector="Border.card:pointerover">
    <Setter Property="BoxShadow" Value="0 4 12 0 #20000000"/>
</Style>
```

---

## 8. 动画与过渡

### 8.1 Transitions（属性过渡）

```xml
<Button Content="Hover Me">
    <Button.Styles>
        <Style Selector="Button">
            <Setter Property="Background" Value="Blue"/>
            <Setter Property="Transitions">
                <Transitions>
                    <BrushTransition Property="Background" Duration="0:0:0.3"/>
                    <DoubleTransition Property="Opacity" Duration="0:0:0.2"/>
                </Transitions>
            </Setter>
        </Style>
        <Style Selector="Button:pointerover">
            <Setter Property="Background" Value="LightBlue"/>
        </Style>
    </Button.Styles>
</Button>
```

### 8.2 Animations（关键帧动画）

```xml
<Border x:Name="AnimatedBorder">
    <Border.Styles>
        <Style Selector="Border">
            <Style.Animations>
                <Animation Duration="0:0:2" IterationCount="Infinite">
                    <KeyFrame Cue="0%">
                        <Setter Property="Opacity" Value="1"/>
                    </KeyFrame>
                    <KeyFrame Cue="50%">
                        <Setter Property="Opacity" Value="0.5"/>
                    </KeyFrame>
                    <KeyFrame Cue="100%">
                        <Setter Property="Opacity" Value="1"/>
                    </KeyFrame>
                </Animation>
            </Style.Animations>
        </Style>
    </Border.Styles>
</Border>
```

---

## 9. 命令绑定

### 9.1 基本命令绑定

```xml
<!-- 绑定到 ViewModel 的命令 -->
<Button Content="保存" Command="{Binding SaveCommand}"/>

<!-- 带参数的命令 -->
<Button Content="删除"
        Command="{Binding DeleteCommand}"
        CommandParameter="{Binding SelectedItem}"/>
```

### 9.2 事件转命令

使用 `Interaction.Behaviors`：

```xml
<Window xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity"
        xmlns:ia="clr-namespace:Avalonia.Xaml.Interactions.Core;assembly=Avalonia.Xaml.Interactions">

    <ListBox Items="{Binding Items}">
        <i:Interaction.Behaviors>
            <ia:EventTriggerBehavior EventName="DoubleTapped">
                <ia:InvokeCommandAction Command="{Binding OpenItemCommand}"/>
            </ia:EventTriggerBehavior>
        </i:Interaction.Behaviors>
    </ListBox>
</Window>
```

---

## 10. 附加属性 (Attached Properties)

### 10.1 内置附加属性

```xml
<!-- Grid 附加属性 -->
<Grid>
    <Button Grid.Row="0" Grid.Column="1"/>
</Grid>

<!-- DockPanel 附加属性 -->
<DockPanel>
    <Button DockPanel.Dock="Top"/>
</DockPanel>

<!-- Canvas 附加属性 -->
<Canvas>
    <Rectangle Canvas.Left="50" Canvas.Top="100"/>
</Canvas>
```

### 10.2 自定义附加属性

```csharp
public class WatermarkHelper
{
    public static readonly AttachedProperty<string> WatermarkProperty =
        AvaloniaProperty.RegisterAttached<WatermarkHelper, TextBox, string>("Watermark");

    public static string GetWatermark(TextBox element)
        => element.GetValue(WatermarkProperty);

    public static void SetWatermark(TextBox element, string value)
        => element.SetValue(WatermarkProperty, value);
}
```

使用自定义附加属性：

```xml
<TextBox local:WatermarkHelper.Watermark="请输入用户名"/>
```

---

## 总结

Avalonia 的样式系统继承了 WPF 的优点，并增强了选择器功能（类似 CSS）。主要特点：

1. **强大的选择器**：支持伪类、属性选择器、NOT 选择器等
2. **灵活的模板**：ControlTemplate、DataTemplate 分离视觉和逻辑
3. **丰富的绑定**：支持 Compiled Bindings，性能更优
4. **主题系统**：内置深色/浅色主题切换支持
5. **跨平台**：一套代码可在 Windows、macOS、Linux 运行

建议实践中遵循以下原则：
- 样式集中管理在资源字典中
- 使用 DynamicResource 支持主题切换
- 优先使用 Compiled Bindings 提升性能
- 合理使用模板部分选择器精细控制样式
