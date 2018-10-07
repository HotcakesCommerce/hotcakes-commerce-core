<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tools.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.Tools" %>
<div id="Div1" class="dnnFormItem  dnnClear">
    <asp:LinkButton ID="cmdUpgradeViewPermission" CssClass="dnnPrimaryAction" resourcekey="cmdUpgradeViewPermission"
        runat="server" OnClick="cmdUpgradeViewPermission_Click"></asp:LinkButton>
</div>

<div id="Div3" class="dnnFormItem  dnnClear">
    <asp:LinkButton ID="cmdAssignTagImage" CssClass="dnnPrimaryAction" resourcekey="cmdAssignTagImage"
        runat="server" onclick="cmdAssignTagImage_Click" ></asp:LinkButton>
</div>

<div id="Div2" class="dnnFormItem  dnnClear">
    <asp:LinkButton ID="cmdSetDNNArticleCategory" resourcekey="cmdSetDNNArticleCategory"
        runat="server" OnClick="cmdSetDNNArticleCategory_Click" CssClass="dnnPrimaryAction"></asp:LinkButton>
    <asp:LinkButton ID="cmdSetURLTitle" resourcekey="cmdSetURLTitle" runat="server" CssClass="dnnPrimaryAction"
        OnClick="cmdSetURLTitle_Click"></asp:LinkButton>
    <asp:LinkButton ID="cmdCancel" resourcekey="cmdCancel" runat="server" CssClass="dnnSecondaryAction"
        OnClick="cmdCancel_Click"></asp:LinkButton>
</div>
<asp:Label ID="lbUpgradeMessage" runat="server"></asp:Label>


<div id="Div4" class="dnnFormItem  dnnClear">
    <asp:CheckBox ID="chkEditDebug" runat="server" />  
    <asp:LinkButton ID="cmdSaveEditDebug" resourcekey="cmdSaveEditDebug" 
        runat="server" CssClass="dnnSecondaryAction" onclick="cmdSaveEditDebug_Click"></asp:LinkButton>
</div>
