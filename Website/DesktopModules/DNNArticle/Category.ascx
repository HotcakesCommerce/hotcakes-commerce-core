<%@ Control Language="C#" AutoEventWireup="true" Codebehind="Category.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.Category" %>

<asp:Label id="lbError" runat="server" resourcekey="lbError" CssClass="NormalRed" Visible="False"></asp:Label>
<asp:TreeView ID="tvCategory" runat="server"  CssClass="ZLDNN_Tree" HoverNodeStyle-CssClass="ZLDNN_TreeNodeSelected ZLDNN_TreeNodeSelectedUDF" NodeStyle-CssClass="ZLDNN_TreeNode ZLDNN_TreeNodeUDF" SelectedNodeStyle-CssClass="ZLDNN_TreeNodeSelected ZLDNN_TreeNodeSelectedUDF" OnSelectedNodeChanged="tvCategory_SelectedNodeChanged" OnTreeNodePopulate="tvCategory_TreeNodePopulate">
</asp:TreeView>
