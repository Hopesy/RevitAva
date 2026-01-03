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

> Semi.Avalonia 提供 4 种按钮主题：

| 主题名称       | 资源键                               | 外观     | 适用场景               |
| -------------- | ------------------------------------ | -------- | ---------------------- |
| **Light**      | `{DynamicResource LightButton}`      | 浅色填充 | 默认主题，次要操作     |
| **Solid**      | `{DynamicResource SolidButton}`      | 实心填充 | 主要操作按钮           |
| **Outline**    | `{DynamicResource OutlineButton}`    | 仅边框   | 次要操作，强调轮廓     |
| **Borderless** | `{DynamicResource BorderlessButton}` | 无边框   | 辅助操作，文本链接风格 |

* 颜色类 (Classes):配合主题使用的颜色样式类：

| 类名        | 用途          | 颜色主题    |
| ----------- | ------------- | ----------- |
| `Primary`   | 主色（默认）  | 蓝色系      |
| `Secondary` | 次要色        | 灰色系      |
| `Tertiary`  | 第三级        | 浅灰色系    |
| `Success`   | 成功状态      | 绿色系      |
| `Warning`   | 警告状态      | 橙色/黄色系 |
| `Danger`    | 危险/删除操作 | 红色系      |

* 尺寸类 (Size Classes)

| 类名     | 效果         |
| -------- | ------------ |
| `Large`  | 大尺寸按钮   |
| `Small`  | 小尺寸按钮   |
| （无类） | 默认中等尺寸 |

* 特殊样式

| 类名       | 效果            |
| ---------- | --------------- |
| `Colorful` | AI 风格彩色效果 |

---

### 基础用法

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
        <!-- Colorful + Solid -->
    <Button Content="Colorful Primary"
            Classes="Colorful Primary"
            Theme="{DynamicResource SolidButton}" />
```
---

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
<StackPanel Orientation="Horizontal" Spacing="10" HorizontalAlignment="Right">
    <!-- 取消按钮 - 次要 -->
    <Button Content="取消" Classes="Secondary" Theme="{DynamicResource OutlineButton}" Width="100" />
    <!-- 确定按钮 - 主要 -->
    <Button Content="确定" Classes="Primary" Theme="{DynamicResource SolidButton}" Width="100" />
</StackPanel>
<StackPanel Spacing="10">
    <TextBlock Text="确定要删除此项吗？此操作不可撤销。" />
    <StackPanel Orientation="Horizontal" Spacing="10">
        <Button Content="取消" Classes="Secondary" Theme="{DynamicResource OutlineButton}" Width="100" />
        <Button Content="确认删除" Classes="Danger" Theme="{DynamicResource SolidButton}" Width="100" />
    </StackPanel>
</StackPanel>
```
---
##  其他
> Semi.Avalonia 还支持其他控件，可以在需要时添加：

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