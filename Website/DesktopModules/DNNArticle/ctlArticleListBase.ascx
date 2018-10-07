<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctlArticleListBase.ascx.cs"
    Inherits="ZLDNN.Modules.DNNArticle.ctlArticleListBase" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/jqx-all.js" Priority="200" />

<dnn:DnnCssInclude ID="DnnCssInclude3" runat="server"  FilePath="~/desktopmodules/DNNArticle/css/jqx.base.css" Priority="201"/>

<%@ Register TagPrefix="zldnn" TagName="PageNav" Src="~/desktopmodules/DNNArticle/PageNav.ascx" %>


<%--<%@ Register TagPrefix="zldnn" TagName="Theme" Src="DNNArticleThemeSelector.ascx" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

  <telerik:RadAjaxManagerProxy ID="AjaxManagerProxy1" runat="server" >
        
    <AjaxSettings>
        
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>
<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" BackColor="#eeeeee"
    BorderStyle="Solid" BorderWidth="1px" Transparency="50" Width="256px" Height="64px"
    RestoreOriginalRenderDelegate="false">
    <table width="100%" height="100%">
        <tr>
            <td style="vertical-align: middle; height: 64px">
                <asp:Image ID="Image1" ImageUrl="~/desktopmodules/dnnarticle/images/loading.gif"
                    runat="server" AlternateText="loading" />
            </td>
        </tr>
    </table>
</telerik:RadAjaxLoadingPanel>
 <zldnn:Theme runat="server" ID="DNNArticleThemeSelector"></zldnn:Theme>--%>

 <asp:Panel runat="server" ID="panelArticle">
     
  <asp:PlaceHolder ID="plBegin" runat="server"></asp:PlaceHolder>
   <asp:PlaceHolder ID="plOutSideControls" runat="server"></asp:PlaceHolder>
  <asp:PlaceHolder ID="plBegin1" runat="server"></asp:PlaceHolder>

    <asp:DataList ID="lstContent" DataKeyField="ItemID" runat="server" CssClass="ZLDNN_ArticleList"
        ItemStyle-CssClass="ZLDNN_ArticleList_Cell" OnItemDataBound="lstContent_ItemDataBound">
        <ItemTemplate>
           <div class="ZLDNN_ListItem">
            <asp:HyperLink ID="lkEdit" Visible="false" runat="server">
                <asp:Image ID="imgedit" runat="server" ImageUrl="~/desktopmodules/dnnarticle/images/edit_link.png"
                    AlternateText="Edit" resourcekey="Edit" /></asp:HyperLink>
            <asp:Label ID="lblNoApproved" resourcekey="lblNoApproved" runat="server" CssClass="NormalRed"
                Visible="false" />
            <asp:Literal ID="lblContent" runat="server" />
            <asp:PlaceHolder ID="pl" runat="server"></asp:PlaceHolder>
           </div>
        </ItemTemplate>
    </asp:DataList>

    <asp:Repeater ID="rptContent" runat="server" OnItemDataBound="rptContent_ItemDataBound">
        <ItemTemplate>
           <asp:HyperLink ID="lkEdit" Visible="false" runat="server">
                <asp:Image ID="imgedit" runat="server" ImageUrl="~/desktopmodules/dnnarticle/images/edit_link.png"
                    AlternateText="Edit" resourcekey="Edit" /></asp:HyperLink><asp:Label ID="lblNoApproved"
                        resourcekey="lblNoApproved" runat="server" CssClass="NormalRed" Visible="false" />
            <asp:Literal ID="lblContent" runat="server" /><asp:PlaceHolder ID="pl" runat="server">
            </asp:PlaceHolder>
        </ItemTemplate>
    </asp:Repeater>
   
    <asp:PlaceHolder ID="plEnd" runat="server"></asp:PlaceHolder>

    <zldnn:PageNav runat="server" ID="MyPageNav"></zldnn:PageNav>
    </asp:Panel>
    
   <asp:PlaceHolder ID="plEnd2" runat="server"></asp:PlaceHolder>