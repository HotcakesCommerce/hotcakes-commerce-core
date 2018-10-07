<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNArticleTagCloud.ArticleListByTag" AutoEventWireup="true" Explicit="True" Codebehind="ArticleListByTag.ascx.cs" %>

<%@ Register TagPrefix="zldnn" TagName="list" Src="~/desktopmodules/DNNArticle/ctlArticleListBase.ascx" %>
<zldnn:list id="myArticleList" runat="server"  />

<asp:Label ID="lbNoContent" resourcekey="lbNoContent" runat="server" ></asp:Label>
<asp:linkbutton cssclass="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" borderstyle="none" text="Cancel" OnClick="cmdCancel_Click" causesvalidation="False"></asp:linkbutton>&nbsp;