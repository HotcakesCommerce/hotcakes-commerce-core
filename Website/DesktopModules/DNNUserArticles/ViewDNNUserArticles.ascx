<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNUserArticles.ViewDNNUserArticles"
            AutoEventWireup="true" Explicit="True" Codebehind="ViewDNNUserArticles.ascx.cs" %>
<%@ Register TagPrefix="zldnn" TagName="ArticleList" Src="~/desktopmodules/DNNArticle/ctlArticleListBase.ascx" %>
<table>
    <tr>
        <td valign="middle">
            <asp:HyperLink ID="cmdRSS" Target="_blank" runat="server">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/rss.gif" AlternateText="xml" /></asp:HyperLink>
        </td>
        <td valign="middle">
            <asp:LinkButton ID="cmdMyArticles" runat="server" CssClass="CommandButton" resourcekey="cmdMyArticles"
                            CausesValidation="False" OnClick="cmdMyArticles_Click"></asp:LinkButton>
        </td>
        <td valign="middle">
            <asp:LinkButton ID="cmdNewContent" runat="server" CssClass="CommandButton" resourcekey="cmdNewContent"
                            CausesValidation="False" OnClick="cmdNewContent_Click"></asp:LinkButton>
        </td>
    </tr>
</table>
<zldnn:ArticleList runat="server" ID="MyArticles"></zldnn:ArticleList>