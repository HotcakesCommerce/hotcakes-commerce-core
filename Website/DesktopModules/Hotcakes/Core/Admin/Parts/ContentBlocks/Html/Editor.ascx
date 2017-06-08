<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.Html.Editor" CodeBehind="Editor.ascx.cs" %>
<%@ Register Src="../../../Controls/HtmlEditor.ascx" TagName="HtmlEditor"
    TagPrefix="hcc" %>

<div class="hcForm">
    <div class="hcFormItem">
        <label class="hcLabel">Html Code<i class="hcLocalizable"></i></label>
        <hcc:HtmlEditor ID="HtmlEditor1" runat="server" EditorHeight="400" EditorWidth="700"
            EditorWrap="false" />
    </div>
</div>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" runat="server" CssClass="hcPrimaryAction"
            Text="Save Changes" OnClick="btnSave_Click" CausesValidation="false" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" runat="server" CssClass="hcSecondaryAction"
            Text="Cancel" OnClick="btnCancel_Click" CausesValidation="false" />
    </li>
</ul>
