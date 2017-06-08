<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Editor.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Parts.Editors.RadEditor.Editor" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadEditor ID="radEditor" Skin="Metro" runat="server">
    <Tools>
        <telerik:EditorToolGroup>
            <telerik:EditorTool Name="Bold" />
            <telerik:EditorTool Name="Italic" />
            <telerik:EditorTool Name="Underline" />
            <telerik:EditorTool Name="StrikeThrough" />
            <telerik:EditorTool Name="ForeColor" />
            <telerik:EditorSeparator />
            <telerik:EditorTool Name="Paste" />
            <telerik:EditorTool Name="PasteStrip" />
            <telerik:EditorSeparator />
            <telerik:EditorTool Name="InsertUnorderedList" />
            <telerik:EditorTool Name="InsertOrderedList" />
            <telerik:EditorTool Name="InsertLink" />
            <telerik:EditorTool Name="Unlink" />
            <telerik:EditorSeparator />
            <telerik:EditorTool Name="InsertSymbol" />
            <telerik:EditorTool Name="ToggleScreenMode" />
        </telerik:EditorToolGroup>
    </Tools>
</telerik:RadEditor>
