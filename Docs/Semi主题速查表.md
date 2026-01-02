# Semi.Avalonia 主题使用速查表

## 概述

本项目使用 **Semi.Avalonia 11.3.7.2** 作为 UI 主题库，这是一个受 Semi Design 启发的 Avalonia UI 主题。

## 安装配置

```xml
<!-- RevitAva.csproj - 已安装 -->
<PackageReference Include="Semi.Avalonia" Version="11.3.7.2" />
```

```csharp
// Application.cs - 已配置
Avalonia.Application.Current!.Styles.Add(new SemiTheme());
```

---

## Button 按钮

### 主题 (Theme)

Semi.Avalonia 提供 4 种按钮主题：

| 主题名称 | 资源键 | 外观 | 适用场景 |
|---------|-------|------|---------|
| **Light** | `{DynamicResource LightButton}` | 浅色填充 | 默认主题，次要操作 |
| **Solid** | `{DynamicResource SolidButton}` | 实心填充 | 主要操作按钮 |
| **Outline** | `{DynamicResource OutlineButton}` | 仅边框 | 次要操作，强调轮廓 |
| **Borderless** | `{DynamicResource BorderlessButton}` | 无边框 | 辅助操作，文本链接风格 |

### 颜色类 (Classes)

配合主题使用的颜色样式类：

| 类名 | 用途 | 颜色主题 |
|------|------|---------|
| `Primary` | 主色（默认） | 蓝色系 |
| `Secondary` | 次要色 | 灰色系 |
| `Tertiary` | 第三级 | 浅灰色系 |
| `Success` | 成功状态 | 绿色系 |
| `Warning` | 警告状态 | 橙色/黄色系 |
| `Danger` | 危险/删除操作 | 红色系 |

### 尺寸类 (Size Classes)

| 类名 | 效果 |
|------|------|
| `Large` | 大尺寸按钮 |
| `Small` | 小尺寸按钮 |
| （无类） | 默认中等尺寸 |

### 特殊样式

| 类名 | 效果 |
|------|------|
| `Colorful` | AI 风格彩色效果 |

---

## 使用示例

### 基础用法

```xml
<!-- 默认 Light 主题 + Primary 颜色（蓝色） -->
<Button Content="默认按钮" />

<!-- Solid 主题 + Primary 颜色 -->
<Button Content="主按钮" Theme="{DynamicResource SolidButton}" Classes="Primary" />

<!-- Outline 主题 + Success 颜色（绿色） -->
<Button Content="成功" Theme="{DynamicResource OutlineButton}" Classes="Success" />

<!-- Borderless 主题 + Danger 颜色（红色） -->
<Button Content="删除" Theme="{DynamicResource BorderlessButton}" Classes="Danger" />
```

---

### 主题 × 颜色组合示例

#### Solid 主题（实心）

```xml
<StackPanel Spacing="10">
    <!-- 主色 - 蓝色 -->
    <Button Content="Primary" Classes="Primary" Theme="{DynamicResource SolidButton}" />

    <!-- 次要色 - 灰色 -->
    <Button Content="Secondary" Classes="Secondary" Theme="{DynamicResource SolidButton}" />

    <!-- 第三级 - 浅灰色 -->
    <Button Content="Tertiary" Classes="Tertiary" Theme="{DynamicResource SolidButton}" />

    <!-- 成功 - 绿色 -->
    <Button Content="Success" Classes="Success" Theme="{DynamicResource SolidButton}" />

    <!-- 警告 - 橙色 -->
    <Button Content="Warning" Classes="Warning" Theme="{DynamicResource SolidButton}" />

    <!-- 危险 - 红色 -->
    <Button Content="Danger" Classes="Danger" Theme="{DynamicResource SolidButton}" />
</StackPanel>
```

#### Outline 主题（轮廓）

```xml
<StackPanel Orientation="Horizontal" Spacing="10">
    <Button Content="Primary" Classes="Primary" Theme="{DynamicResource OutlineButton}" />
    <Button Content="Success" Classes="Success" Theme="{DynamicResource OutlineButton}" />
    <Button Content="Warning" Classes="Warning" Theme="{DynamicResource OutlineButton}" />
    <Button Content="Danger" Classes="Danger" Theme="{DynamicResource OutlineButton}" />
</StackPanel>
```

#### Borderless 主题（无边框）

```xml
<StackPanel Spacing="10">
    <Button Content="链接样式" Theme="{DynamicResource BorderlessButton}" Classes="Primary" />
    <Button Content="删除" Theme="{DynamicResource BorderlessButton}" Classes="Danger" />
</StackPanel>
```

---

### 尺寸变化

```xml
<!-- 大按钮 -->
<Button Content="大按钮" Classes="Large Primary" Theme="{DynamicResource SolidButton}" />

<!-- 默认尺寸 -->
<Button Content="默认" Classes="Primary" Theme="{DynamicResource SolidButton}" />

<!-- 小按钮 -->
<Button Content="小按钮" Classes="Small Primary" Theme="{DynamicResource SolidButton}" />
```

---

### 禁用状态

```xml
<Button Content="禁用按钮"
        Classes="Danger"
        Theme="{DynamicResource SolidButton}"
        IsEnabled="False" />
```

---

### Colorful AI 样式

```xml
<StackPanel Spacing="10">
    <!-- Colorful + Solid -->
    <Button Content="Colorful Primary"
            Classes="Colorful Primary"
            Theme="{DynamicResource SolidButton}" />

    <!-- Colorful + Outline -->
    <Button Content="Colorful Success"
            Classes="Colorful Success"
            Theme="{DynamicResource OutlineButton}" />
</StackPanel>
```

---

### 组合多个 Classes

```xml
<!-- 大号 + 成功色 + Solid 主题 -->
<Button Content="大成功按钮"
        Classes="Large Success"
        Theme="{DynamicResource SolidButton}" />

<!-- 小号 + 危险色 + Outline 主题 -->
<Button Content="小删除"
        Classes="Small Danger"
        Theme="{DynamicResource OutlineButton}" />

<!-- Colorful + 大号 + Primary -->
<Button Content="彩色大按钮"
        Classes="Colorful Large Primary"
        Theme="{DynamicResource SolidButton}" />
```

---

## 完整示例布局

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="RevitAva.Views.SettingView"
        Title="Semi.Avalonia 按钮示例"
        Width="800" Height="600">

    <ScrollViewer Padding="20">
        <StackPanel Spacing="20">

            <!-- Solid 主题 -->
            <TextBlock Text="Solid 主题（实心）" FontSize="18" FontWeight="Bold" />
            <StackPanel Spacing="10" Orientation="Horizontal">
                <Button Content="Primary" Classes="Primary" Theme="{DynamicResource SolidButton}" Width="120" />
                <Button Content="Secondary" Classes="Secondary" Theme="{DynamicResource SolidButton}" Width="120" />
                <Button Content="Success" Classes="Success" Theme="{DynamicResource SolidButton}" Width="120" />
                <Button Content="Warning" Classes="Warning" Theme="{DynamicResource SolidButton}" Width="120" />
                <Button Content="Danger" Classes="Danger" Theme="{DynamicResource SolidButton}" Width="120" />
            </StackPanel>

            <!-- Outline 主题 -->
            <TextBlock Text="Outline 主题（轮廓）" FontSize="18" FontWeight="Bold" Margin="0,20,0,0" />
            <StackPanel Spacing="10" Orientation="Horizontal">
                <Button Content="Primary" Classes="Primary" Theme="{DynamicResource OutlineButton}" Width="120" />
                <Button Content="Secondary" Classes="Secondary" Theme="{DynamicResource OutlineButton}" Width="120" />
                <Button Content="Success" Classes="Success" Theme="{DynamicResource OutlineButton}" Width="120" />
                <Button Content="Warning" Classes="Warning" Theme="{DynamicResource OutlineButton}" Width="120" />
                <Button Content="Danger" Classes="Danger" Theme="{DynamicResource OutlineButton}" Width="120" />
            </StackPanel>

            <!-- Light 主题 -->
            <TextBlock Text="Light 主题（浅色）" FontSize="18" FontWeight="Bold" Margin="0,20,0,0" />
            <StackPanel Spacing="10" Orientation="Horizontal">
                <Button Content="Primary" Classes="Primary" Theme="{DynamicResource LightButton}" Width="120" />
                <Button Content="Success" Classes="Success" Theme="{DynamicResource LightButton}" Width="120" />
                <Button Content="Warning" Classes="Warning" Theme="{DynamicResource LightButton}" Width="120" />
                <Button Content="Danger" Classes="Danger" Theme="{DynamicResource LightButton}" Width="120" />
            </StackPanel>

            <!-- Borderless 主题 -->
            <TextBlock Text="Borderless 主题（无边框）" FontSize="18" FontWeight="Bold" Margin="0,20,0,0" />
            <StackPanel Spacing="10" Orientation="Horizontal">
                <Button Content="链接样式" Classes="Primary" Theme="{DynamicResource BorderlessButton}" />
                <Button Content="成功" Classes="Success" Theme="{DynamicResource BorderlessButton}" />
                <Button Content="警告" Classes="Warning" Theme="{DynamicResource BorderlessButton}" />
                <Button Content="删除" Classes="Danger" Theme="{DynamicResource BorderlessButton}" />
            </StackPanel>

            <!-- 尺寸变化 -->
            <TextBlock Text="尺寸变化" FontSize="18" FontWeight="Bold" Margin="0,20,0,0" />
            <StackPanel Spacing="10" Orientation="Horizontal" VerticalAlignment="Center">
                <Button Content="大" Classes="Large Primary" Theme="{DynamicResource SolidButton}" />
                <Button Content="中" Classes="Primary" Theme="{DynamicResource SolidButton}" />
                <Button Content="小" Classes="Small Primary" Theme="{DynamicResource SolidButton}" />
            </StackPanel>

            <!-- 禁用状态 -->
            <TextBlock Text="禁用状态" FontSize="18" FontWeight="Bold" Margin="0,20,0,0" />
            <StackPanel Spacing="10" Orientation="Horizontal">
                <Button Content="禁用 Solid" Classes="Primary" Theme="{DynamicResource SolidButton}" IsEnabled="False" Width="120" />
                <Button Content="禁用 Outline" Classes="Danger" Theme="{DynamicResource OutlineButton}" IsEnabled="False" Width="120" />
            </StackPanel>

            <!-- Colorful AI 样式 -->
            <TextBlock Text="Colorful AI 样式" FontSize="18" FontWeight="Bold" Margin="0,20,0,0" />
            <StackPanel Spacing="10">
                <Button Content="Colorful Primary" Classes="Colorful Primary" Theme="{DynamicResource SolidButton}" Width="200" />
                <Button Content="Colorful Success" Classes="Colorful Success" Theme="{DynamicResource OutlineButton}" Width="200" />
            </StackPanel>

        </StackPanel>
    </ScrollViewer>
</Window>
```

---

## 常见错误

### ❌ 错误 1：直接赋值字符串

```xml
<!-- ❌ 错误：编译失败 -->
<Button Theme="Outline" Classes="Danger">按钮</Button>

<!-- ✅ 正确：使用 DynamicResource -->
<Button Theme="{DynamicResource OutlineButton}" Classes="Danger">按钮</Button>
```

**错误信息**：
```
error AVLN3000: Unable to find suitable setter for property Theme
```

---

### ❌ 错误 2：拼写错误

```xml
<!-- ❌ 错误：主题名称拼写错误 -->
<Button Theme="{DynamicResource OutLine}">按钮</Button>

<!-- ✅ 正确 -->
<Button Theme="{DynamicResource OutlineButton}">按钮</Button>
```

**注意**：
- 所有主题资源键都以 `Button` 结尾
- 使用驼峰命名：`OutlineButton`，不是 `OutLine`

---

### ❌ 错误 3：颜色类拼写错误

```xml
<!-- ❌ 错误：Classes 值区分大小写 -->
<Button Classes="primary" Theme="{DynamicResource SolidButton}">按钮</Button>

<!-- ✅ 正确：首字母大写 -->
<Button Classes="Primary" Theme="{DynamicResource SolidButton}">按钮</Button>
```

---

## 主题资源完整列表

### 按钮主题

```xml
<!-- 可用的按钮主题资源 -->
{DynamicResource LightButton}        <!-- 浅色主题 -->
{DynamicResource SolidButton}        <!-- 实心主题 -->
{DynamicResource OutlineButton}      <!-- 轮廓主题 -->
{DynamicResource BorderlessButton}   <!-- 无边框主题 -->
```

### 颜色类

```xml
<!-- 可用的颜色样式类（区分大小写） -->
Classes="Primary"     <!-- 主色 -->
Classes="Secondary"   <!-- 次要色 -->
Classes="Tertiary"    <!-- 第三级 -->
Classes="Success"     <!-- 成功 -->
Classes="Warning"     <!-- 警告 -->
Classes="Danger"      <!-- 危险 -->
```

### 尺寸类

```xml
Classes="Large"   <!-- 大尺寸 -->
Classes="Small"   <!-- 小尺寸 -->
<!-- 默认中等尺寸不需要类 -->
```

### 特殊样式类

```xml
Classes="Colorful"  <!-- AI 彩色风格 -->
```

---

## 其他控件（未来扩展）

Semi.Avalonia 还支持其他控件，可以在需要时添加：

- ToggleButton（切换按钮）
- SplitButton（分割按钮）
- DropDownButton（下拉按钮）
- TextBox（文本框）
- CheckBox（复选框）
- RadioButton（单选按钮）
- ComboBox（下拉框）
- ... 等

参考文档：https://docs.irihi.tech/semi/

---

## 快速参考

### 最常用组合

```xml
<!-- 主操作按钮 -->
<Button Content="确定" Classes="Primary" Theme="{DynamicResource SolidButton}" />

<!-- 次要操作 -->
<Button Content="取消" Classes="Secondary" Theme="{DynamicResource OutlineButton}" />

<!-- 危险操作（删除） -->
<Button Content="删除" Classes="Danger" Theme="{DynamicResource OutlineButton}" />

<!-- 成功/确认 -->
<Button Content="保存" Classes="Success" Theme="{DynamicResource SolidButton}" />

<!-- 文本链接风格 -->
<Button Content="了解更多" Theme="{DynamicResource BorderlessButton}" />
```

---

### 对话框按钮组

```xml
<StackPanel Orientation="Horizontal" Spacing="10" HorizontalAlignment="Right">
    <!-- 取消按钮 - 次要 -->
    <Button Content="取消" Classes="Secondary" Theme="{DynamicResource OutlineButton}" Width="100" />

    <!-- 确定按钮 - 主要 -->
    <Button Content="确定" Classes="Primary" Theme="{DynamicResource SolidButton}" Width="100" />
</StackPanel>
```

---

### 危险操作确认

```xml
<StackPanel Spacing="10">
    <TextBlock Text="确定要删除此项吗？此操作不可撤销。" />

    <StackPanel Orientation="Horizontal" Spacing="10">
        <Button Content="取消" Classes="Secondary" Theme="{DynamicResource OutlineButton}" Width="100" />
        <Button Content="确认删除" Classes="Danger" Theme="{DynamicResource SolidButton}" Width="100" />
    </StackPanel>
</StackPanel>
```

---

## 总结

✅ **4 种主题**：Light, Solid, Outline, Borderless
✅ **6 种颜色**：Primary, Secondary, Tertiary, Success, Warning, Danger
✅ **2 种尺寸**：Large, Small（默认中等）
✅ **特殊样式**：Colorful AI 风格

**记住要点**：
1. Theme 必须使用 `{DynamicResource XXXButton}`
2. Classes 区分大小写（首字母大写）
3. 可以组合多个 Classes（用空格分隔）

**参考文档**：
- [Semi.Avalonia 官方文档](https://docs.irihi.tech/semi/en/docs/basic/theme-and-class/)
- [Semi.Avalonia GitHub](https://github.com/irihitech/Semi.Avalonia)
