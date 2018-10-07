<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DNNArticle_List.ascx.cs" Inherits=" ZLDNN.Modules.DNNArticle_List.DNNArticle_List" %>
<%@ Register TagPrefix="zldnn" TagName="ArticleList" Src="~/desktopmodules/DNNArticle/ctlArticleListBase.ascx" %>




<asp:HyperLink ID="cmdRSS" Target="_blank" runat="server" CssClass="cmdRSS">
</asp:HyperLink>  
    
<asp:DropDownList ID="cboOrderField" runat="server" AutoPostBack="true" CssClass="Normal" OnSelectedIndexChanged="cboOrderField_SelectedIndexChanged">
    <asp:ListItem Value="" Selected="True" resourcekey="liDefault"></asp:ListItem>
    <asp:ListItem Value=" TITLE DESC " resourcekey="liTitleDesc"></asp:ListItem>
    <asp:ListItem Value=" TITLE " resourcekey="liTitleAsc"></asp:ListItem>
    <asp:ListItem Value=" PUBLISHDATE DESC " resourcekey="liPublishDateDesc"> </asp:ListItem>
    <asp:ListItem Value=" PUBLISHDATE ASC " resourcekey="liPublishDateAsc"> </asp:ListItem>
                 
    <asp:ListItem Value=" VOTENUMBER DESC" resourcekey="liVoteNumberDesc"></asp:ListItem>
    <asp:ListItem Value=" VOTENUMBER ASC" resourcekey="liVoteNumberAsc"></asp:ListItem>
                  
    <asp:ListItem Value=" COMMENTNUMBER DESC" resourcekey="liCommentNumberDesc"></asp:ListItem>
    <asp:ListItem Value=" COMMENTNUMBER ASC" resourcekey="liCommentNumberAsc"></asp:ListItem>
    
    <asp:ListItem Value=" CLICKS DESC" resourcekey="liClicksDesc"></asp:ListItem>
    <asp:ListItem Value=" CLICKS ASC" resourcekey="liClicksAsc"></asp:ListItem>
              
</asp:DropDownList>

 

<zldnn:ArticleList runat="server" ID="MyArticleList"></zldnn:ArticleList>

<asp:Literal ID="ltNoRecord" runat="server"></asp:Literal>
<asp:HyperLink ID="lnkMore" runat="server"></asp:HyperLink>