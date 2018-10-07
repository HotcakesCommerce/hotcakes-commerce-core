<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNArticleArchive.ViewDNNArticleArchive"
            AutoEventWireup="true" Explicit="True" Codebehind="ViewDNNArticleArchive.ascx.cs" %>
<%@ Register TagPrefix="zldnn" TagName="list" Src="~/desktopmodules/DNNArticle/ctlArticleListBase.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="PageNav" Src="~/desktopmodules/DNNArticle/PageNav.ascx" %>

<asp:LinkButton ID="cmdArchiveList" runat="server" Visible="false" resourcekey="cmdArchiveList"
                CssClass="CommandButton" OnClick="cmdArchiveList_Click"></asp:LinkButton>
<asp:Panel ID="Panel1" runat="server">
    <asp:Panel runat="server" ID="PanelCategory">
    <asp:Label ID="lbCategory" resourcekey="lbCategory" CssClass="ArchiveCategoryLabel" runat="server">Category: </asp:Label>
    <asp:DropDownList ID="cboCategory" runat="server" Width="271px" AutoPostBack="True" CssClass="ArchiveCategorySelection" OnSelectedIndexChanged="cboCategory_SelectedIndexChanged">
    </asp:DropDownList>
    </asp:Panel>
     <asp:Panel runat="server" ID="PanelYear">
         <asp:Label ID="Label1" resourcekey="lbYear" CssClass="ArchiveYearLabel" runat="server">Year: </asp:Label>
    <asp:DropDownList ID="cboYear" runat="server" AutoPostBack="True" CssClass="ArchiveYearSelection" 
             onselectedindexchanged="cboYear_SelectedIndexChanged">
    </asp:DropDownList>
      </asp:Panel>    
    <asp:DataList ID="lstContent" runat="server" CellPadding="4" OnItemCommand="lstContent_ItemCommand" OnItemDataBound="lstContent_ItemDataBound">
        <ItemTemplate>
            <asp:LinkButton ID="lblContent" runat="server" CausesValidation="false" CssClass="Normal" />
        </ItemTemplate>
    </asp:DataList>
    <zldnn:PageNav runat="server" ID="MyPageNav" ></zldnn:PageNav>
</asp:Panel>
<zldnn:list ID="ArticleList" runat="server" Visible="false" />