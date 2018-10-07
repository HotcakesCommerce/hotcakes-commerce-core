<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctlVersion.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ctlVersion" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
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
<asp:Panel ID="EditPanel" runat="server" Width="100%" >
<asp:Label ID="lbMessage" runat="server" ></asp:Label>
<table id="Table1" width="100%" runat="server">
    <tr>
        <td style="vertical-align: top;width:40%">
            <asp:DataGrid ID="grdVersions" Width="98%" AutoGenerateColumns="false" EnableViewState="false"
                runat="server" GridLines="None" CssClass="dnnGrid" OnItemCommand="grdVersions_ItemCommand" OnItemDataBound="grdVersions_ItemDataBound">
                <HeaderStyle CssClass="dnnGridHeader" VerticalAlign="Top" />
                <ItemStyle CssClass="dnnGridItem" HorizontalAlign="Left" />
                <AlternatingItemStyle CssClass="dnnGridAltItem" />
                <EditItemStyle CssClass="dnnFormInput" />
                <SelectedItemStyle CssClass="dnnFormError" />
                <FooterStyle CssClass="dnnGridFooter" />
                <PagerStyle CssClass="dnnGridPager" />
                <Columns>
                    <asp:TemplateColumn HeaderText="Version">
                        <ItemTemplate>
                            <asp:Label ID="Label11" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "VersionNumber").ToString()%> '></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Date">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CreatedDate").ToString()%> '></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="RollBack">
                        <ItemTemplate>
                            <asp:LinkButton CausesValidation="false" CommandName="rollback" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ItemID").ToString()%>'
                                ID="cRollBack" runat="server">
                                <asp:Image ID="Image3" runat="server" ImageUrl="~/images/restore.gif" AlternateText="rollback" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="View">
                        <ItemTemplate>
                            <asp:LinkButton CausesValidation="false" CommandName="view" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ItemID").ToString()%>'
                                ID="cPreview" runat="server">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/view.gif" AlternateText="Preview" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Delete">
                        <ItemTemplate>
                            <asp:LinkButton CausesValidation="false" CommandName="delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ItemID").ToString()%>'
                                ID="cDelete" runat="server">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/delete.gif" AlternateText="Delete" />
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        
        </td>
        <td style="vertical-align: top">
            <table runat="server" id="tbPreview" style="text-align: left" visible="false" class="dnnFormItem">
                <tr>
                    <td style="vertical-align: top">
                        <asp:Label ID="lblVersion" runat="server" resourcekey="lblVersion" ></asp:Label>
                    </td>
                    <td style="vertical-align: top">
                        <asp:Label ID="lbVersion" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top">
                        <asp:Label ID="lblTitle" runat="server" resourcekey="lblTitle" ></asp:Label>
                    </td>
                    <td style="vertical-align: top">
                        <asp:Label ID="lbTitle" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top">
                        <asp:Label ID="lblSummary" runat="server" resourcekey="lblSummary" ></asp:Label>
                    </td>
                    <td style="vertical-align: top">
                        <asp:Label ID="lbDescription" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top">
                        <asp:Label ID="lblContent" runat="server" resourcekey="lblContent" ></asp:Label>
                    </td>
                    <td style="vertical-align: top">
                        <asp:Label ID="lbContent" runat="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top">
                        <asp:Label ID="lblPages" runat="server" resourcekey="lblPages" ></asp:Label>
                    </td>
                    <td style="vertical-align: top">
                        <asp:DataList ID="lstPages" runat="server" OnItemDataBound="lstPages_ItemDataBound">
                            <ItemTemplate>
                                <table cellspacing="4" cellpadding="4" border="0" width="100%">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblPageTitle" runat="server" resourcekey="lblPageTitle" ></asp:Label>
                                            <asp:Label ID="Label2" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "Title").ToString()%>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblPageContent" runat="server" resourcekey="lblPageContent" ></asp:Label>
                                            <br />
                                            <asp:Label ID="Label4" runat="server"  Text='<%#Server.HtmlDecode(DataBinder.Eval(Container.DataItem, "Content").ToString())%>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:DataList>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</asp:Panel>