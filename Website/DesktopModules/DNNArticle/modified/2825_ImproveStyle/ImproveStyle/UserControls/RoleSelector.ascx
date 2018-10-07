<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoleSelector.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.RoleSelector" %>

 <%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
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
<telerik:RadAjaxManagerProxy ID="AjaxManagerProxy1" runat="server">
    <AjaxSettings>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>

<asp:Panel runat="server" ID="panelRoles" style="float:left;background:#eee;padding:10px;margin:0 0 10px" >
    <asp:Label ID="lbAllUsers" runat="server" ></asp:Label>
    <asp:DataList ID="lstRoles" runat="server"  EnableViewState="false" OnItemDataBound="lstRelatedArticles_ItemDataBound"
        ItemStyle-CssClass="FileManager_FileList" OnItemCommand="lstRoles_ItemCommand" >
        <ItemTemplate>
            <asp:LinkButton CausesValidation="false" CommandName="Remove" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "RoleName").ToString()%>'
                ID="LinkButton1" runat="server"  ClientIDMode="AutoID">
                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/icon_recyclebin_16px.gif"
                    AlternateText="Remove" />
            </asp:LinkButton>
            <asp:Label Text='<%#DataBinder.Eval(Container.DataItem, "RoleName").ToString()%>' ID="lbTitle"
                CssClass="Normal" runat="server">
            </asp:Label>
        </ItemTemplate>
    </asp:DataList>
    <telerik:RadComboBox ID="cboRoles" runat="server" Width="300px" Height="250px" Visible="True"
        InputCssClass="cboUserEnter" EmptyMessage="Select a role" EnableLoadOnDemand="True"
        ShowMoreResultsBox="true" EnableVirtualScrolling="true" OnItemsRequested="RadComboBox2_ItemsRequested">
    </telerik:RadComboBox>
    <asp:LinkButton ID="cmdAdd" runat="server" CssClass="dnnSecondaryAction" 
        onclick="cmdAdd_Click"></asp:LinkButton>
</asp:Panel>
