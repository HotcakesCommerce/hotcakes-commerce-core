<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DNNArticleList.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.DNNArticleList" %>
<table id="tbCatalog" runat="server" cellspacing="1" cellpadding="1" border="0">
    <tr>
        <td style="vertical-align: middle" >
            <asp:Label ID="lbCategory" resourcekey="lbCategory" CssClass="Normal" runat="server">Category: </asp:Label></td>
        <td valign="middle">
            <asp:DropDownList ID="cboCategory" runat="server" CssClass="Normal" Width="271px" AutoPostBack="True" OnSelectedIndexChanged="cboCategory_SelectedIndexChanged">
            </asp:DropDownList></td>
    </tr>
</table>


<%@ Register TagPrefix="zldnn" TagName="ArticleManagement" Src="~/desktopmodules/DNNArticle/UserControls/ArticleManagement.ascx" %>

<zldnn:ArticleManagement runat="server" id="ArticleManagement"></zldnn:ArticleManagement>