<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArticleList.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.DNNArticleSearch.ArticleList" %>
<%@ Register TagPrefix="zldnn" TagName="list" Src="~/desktopmodules/DNNArticle/ctlArticleListBase.ascx" %>

<zldnn:list ID="myArticleList" runat="server" />
<asp:LinkButton ID="cmdArchiveList" runat="server" resourcekey="cmdArchiveList" CssClass="CommandButton" OnClick="cmdArchiveList_Click"></asp:LinkButton>
<asp:Label ID="lbNoResult" runat="server" Visible="False" resourcekey="lbNoResult"></asp:Label>



