<%@ Control Language="C#" Inherits="ZLDNN.Modules.CategoryArticleList.ViewCategoryArticleList"
            AutoEventWireup="false" Explicit="True" Codebehind="ViewCategoryArticleList.ascx.cs" %>
            <%@ Register TagPrefix="zldnn" TagName="PageNav" Src="~/desktopmodules/DNNArticle/PageNav.ascx" %>
<asp:Literal ID="ltHeader" runat="server"></asp:Literal>
<asp:Repeater ID="rptContent" runat="server" OnItemDataBound="rptContent_ItemDataBound">
    <ItemTemplate>
    </ItemTemplate>
</asp:Repeater>
<asp:DataList ID="lstCategory" DataKeyField="ItemID" runat="server" CssClass="ZLDNN_CategoryList" OnItemDataBound="lstCategory_ItemDataBound">
    <ItemTemplate>
    </ItemTemplate>
</asp:DataList>
<asp:Literal ID="ltFooter" runat="server"></asp:Literal>

<zldnn:PageNav runat="server" ID="MyPageNav"></zldnn:PageNav>