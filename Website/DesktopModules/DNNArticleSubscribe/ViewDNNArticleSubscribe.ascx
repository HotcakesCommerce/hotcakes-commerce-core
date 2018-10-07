<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNArticleSubscribe.ViewDNNArticleSubscribe"
            AutoEventWireup="true" Explicit="True" Codebehind="ViewDNNArticleSubscribe.ascx.cs" %>
<asp:Label ID="lbNote" CssClass="Normal" runat="server"></asp:Label>
<asp:TreeView ID="tvCategory" runat="server" HoverNodeStyle-CssClass="ZLDNN_TreeNodeSelected"
              NodeStyle-CssClass="ZLDNN_TreeNode" SelectedNodeStyle-CssClass="ZLDNN_TreeNodeSelected">
</asp:TreeView>
<div class="dnnFormItem">
<asp:Button ID="cmdSaveSettings" runat="server" CssClass="StandardButton" resourcekey="cmdSaveSettings"  OnClick="cmdSaveSettings_Click"/>
<asp:Button ID="cmdRegister" runat="server" CssClass="StandardButton" resourcekey="cmdRegister"  OnClick="cmdRegister_Click"/>
<asp:Button ID="cmdLogin" runat="server" CssClass="StandardButton" resourcekey="cmdLogin"  OnClick="cmdLogin_Click"/>
</div>