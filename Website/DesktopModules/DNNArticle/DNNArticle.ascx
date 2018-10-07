<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNArticle.DNNArticle" AutoEventWireup="true" Explicit="True" Codebehind="DNNArticle.ascx.cs" %>

<%@ Register TagPrefix="zldnn" TagName="ArticleList" Src="~/desktopmodules/DNNArticle/ctlArticleListBase.ascx" %>
<asp:Panel ID="panel1" runat="server" >

    <asp:HyperLink ID="cmdRSS" Target="_blank" runat="server" CssClass="cmdRSS cmdRSSUDF">
       </asp:HyperLink>
    
    <asp:Label ID="lbCategory" CssClass="Normal" runat="server"></asp:Label>

    <asp:DropDownList ID="cboCategory" runat="server"  AutoPostBack="True" CssClass="Normal CategoryDropdown" OnSelectedIndexChanged="cboCategory_SelectedIndexChanged">
    </asp:DropDownList>
    
 <%-- <asp:LinkButton ID="cmdNewContent" runat="server" CssClass="AddLabel" 
                    CausesValidation="False" OnClick="cmdNewContent_Click"></asp:LinkButton>--%>
                    
    <asp:HyperLink ID="lkNewContent" runat="server" CssClass="AddLabel"  ></asp:HyperLink>
    <br />
    <zldnn:ArticleList runat="server" ID="MyArticleList"></zldnn:ArticleList>
    <asp:Label ID="lbNoCategory" runat="server" resourcekey="lbNoCategory"></asp:Label>

    <asp:LinkButton ID="cmdUnApproved" runat="server" CssClass="ArticleLabel" OnClick="cmdUnApproved_Click"></asp:LinkButton>
    <asp:LinkButton ID="cmdMyArticles" runat="server" CssClass="ArticleLabel" OnClick="cmdMyArticles_Click"></asp:LinkButton>
    <asp:LinkButton ID="cmdCommentManagement" CssClass="CommentsLabel" resourcekey="CommentManagement.Text" runat="server" OnClick="cmdCommentManagement_Click"></asp:LinkButton> 
    <asp:LinkButton ID="cmdTemplateEditor" runat="server" CssClass="TemplateLabel" Visible="false"  resourcekey="TemplateEditor.Text" OnClick="cmdTemplateEditor_Click"></asp:LinkButton>
    <asp:LinkButton ID="cmdMyComments" runat="server" CssClass="CommentsLabel"  resourcekey="cmdMyComments.Text" OnClick="cmdMyComments_Click"></asp:LinkButton>
    <asp:LinkButton ID="cmdCommentsofMyArticle" runat="server" CssClass="CommentsLabel" resourcekey="cmdCommentsofMyArticle.Text" OnClick="cmdCommentsofMyArticle_Click"></asp:LinkButton>

<asp:Panel ID="panelContent" runat="server" Visible="false" >
        <asp:LinkButton ID="cmdCategoryList" runat="server" CssClass="CategoryLabel"  OnClick="cmdCategoryList_Click"></asp:LinkButton>
        <asp:LinkButton ID="cmdArticleList" runat="server" CssClass="ArticleLabel" OnClick="cmdArticleList_Click"></asp:LinkButton>
        <asp:LinkButton ID="cmdTagList" runat="server" CssClass="TagLabel" resourcekey="TagList.Text" OnClick="cmdTagList_Click"></asp:LinkButton> 
        <asp:LinkButton ID="cmdExtraFields" runat="server" CssClass="TemplateLabel" resourcekey="ExtraFieldList.Text" OnClick="cmdExtraFields_Click"></asp:LinkButton> 
    </asp:Panel>
</asp:Panel>