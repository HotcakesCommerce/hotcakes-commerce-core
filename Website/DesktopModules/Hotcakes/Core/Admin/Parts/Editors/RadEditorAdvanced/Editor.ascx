<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Editor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Parts.Editors.RadEditor.Editor" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadEditor ID="radEditor" Skin="Metro" runat="server">
    <Tools>
        <telerik:EditorToolGroup>
            <telerik:EditorTool Name="Undo" />
            <telerik:EditorSeparator />
            <telerik:EditorTool Name="Bold" />
            <telerik:EditorTool Name="Italic" />
            <telerik:EditorTool Name="Underline" />
            <telerik:EditorTool Name="StrikeThrough" />
            <telerik:EditorTool Name="InsertUnorderedList" />
            <telerik:EditorTool Name="InsertOrderedList" />
            <telerik:EditorTool Name="Indent" />
            <telerik:EditorTool Name="Outdent" />
            <telerik:EditorTool Name="ForeColor" />
            <telerik:EditorTool Name="FormatBlock" />
            <telerik:EditorSeparator />
            <telerik:EditorTool Name="Paste" />
            <telerik:EditorTool Name="PasteStrip" />
            <telerik:EditorSeparator />
            <telerik:EditorTool Name="InsertLink" />
            <telerik:EditorTool Name="Unlink" />
            <telerik:EditorTool Name="InsertTable"/>
            <telerik:EditorTool Name="InsertImage"/>
            <telerik:EditorSeparator />
            <telerik:EditorTool Name="InsertSymbol" />
            <telerik:EditorTool Name="ToggleScreenMode" />
        </telerik:EditorToolGroup>
    </Tools>
</telerik:RadEditor>
