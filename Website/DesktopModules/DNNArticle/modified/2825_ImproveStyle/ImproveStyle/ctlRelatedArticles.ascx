<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ctlRelatedArticles.ascx.cs"
            Inherits="ZLDNN.Modules.DNNArticle.ctlRelatedArticles" %>
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
<asp:Label ID="lbSaveArticle" runat="server" CssClass="Normal" Visible="false"></asp:Label>
    <div id="divFiles" runat="server" class="dnnFormItem">
        <asp:Label ID="lbRealtedArticles" runat="server" CssClass="Normal"></asp:Label>
        <asp:DataList ID="lstRelatedArticles" runat="server" Width="400" EnableViewState="false"
            ItemStyle-CssClass="FileManager_FileList" OnItemCommand="lstRelatedArticles_ItemCommand"
            OnItemDataBound="lstRelatedArticles_ItemDataBound">
            <ItemTemplate>
                <table cellspacing="4" cellpadding="4" border="0" width="100%">
                    <tr>
                        <td style="width: 100%; text-align: left">
                            <asp:Label Text='<%#DataBinder.Eval(Container.DataItem, "Title").ToString()%>' ID="lbTitle"
                                CssClass="Normal" runat="server">
                            </asp:Label>
                        </td>
                        <td>
                            <asp:Label Text='<%#DataBinder.Eval(Container.DataItem, "ItemId").ToString()%>' ID="Label1"
                                CssClass="Normal" runat="server">   </asp:Label>
                        </td>
                        <td>
                            <asp:LinkButton CausesValidation="false" CommandName="Remove" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ItemId").ToString()%>'
                                ID="LinkButton1" runat="server" ClientIDMode="AutoID">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/icon_recyclebin_16px.gif"
                                    AlternateText="Remove" />
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:DataList>
        <div class="dnnClear">
            <asp:TextBox runat="server" ID="txtKey" CssClass="NormalTextBox"></asp:TextBox>
            <asp:Button ID="cmdSearch" runat="server" OnClick="cmdSearch_Click" />
            <asp:Button ID="cmdShowAll" runat="server" OnClick="cmdShowAll_Click" />
        </div>
        <asp:DataList ID="lstAdd" runat="server" Width="400" EnableViewState="false" ItemStyle-CssClass="FileManager_FileList"
            OnItemCommand="lstAdd_ItemCommand" OnItemDataBound="lstRelatedArticles_ItemDataBound">
            <ItemTemplate>
                <table cellspacing="1" cellpadding="1" border="0" width="100%" class="dnnFormItem">
                    <tr>
                        <td style="width: 100%; text-align: left">
                            <asp:Label Text='<%#DataBinder.Eval(Container.DataItem, "Title").ToString()%>' ID="lbTitle"
                                CssClass="Normal" runat="server">
                            </asp:Label>
                        </td>
                        <td>
                            <asp:Label Text='<%#DataBinder.Eval(Container.DataItem, "ItemId").ToString()%>' ID="Label1"
                                CssClass="Normal" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:LinkButton CausesValidation="false" CommandName="Add" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "ItemId").ToString()%>'
                                ID="LinkButton1" runat="server" ClientIDMode="AutoID">
                                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/add.gif" AlternateText="Add" />
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:DataList>
    </div>
</asp:Panel>
