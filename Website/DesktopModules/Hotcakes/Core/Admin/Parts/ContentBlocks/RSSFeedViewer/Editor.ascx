<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.RSSFeedViewer.Editor" CodeBehind="Editor.ascx.cs" %>

<div class="hcForm" style="width: 60%">
    <div class="hcFormItem">
        <label class="hcLabel">Feed URL</label>
        <asp:TextBox ID="FeedField" runat="server" Columns="80" />
    </div>
    <div class="hcFormItem">
        <asp:CheckBox runat="server" ID="chkShowTitle" Text="Show Title" />
    </div>
    <div class="hcFormItem">
        <asp:CheckBox runat="server" ID="chkShowDescription" Text="Show Description" />
    </div>
    <div class="hcFormItem">
        <label class="hcLabel">Maximum Items</label>
        <asp:TextBox runat="server" ID="MaxItemsField" Columns="6" Text="5" Width="100px" />
    </div>
</div>

<ul class="hcActions">
    <li>
        <asp:LinkButton ID="btnSave" runat="server" CssClass="hcPrimaryAction"
            Text="Save Changes" OnClick="btnSave_Click" />
    </li>
    <li>
        <asp:LinkButton ID="btnCancel" CausesValidation="false" CssClass="hcSecondaryAction"
            Text="Cancel" runat="server" OnClick="btnCancel_Click" />
    </li>
</ul>
