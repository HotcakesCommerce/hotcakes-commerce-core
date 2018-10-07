<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryManagement.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.CategoryManagement" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="CategoryManagementEdit" Src="~/desktopmodules/DNNArticle/CategoryManagementEdit.ascx" %>
<%@ Register TagPrefix="zldnn" TagName="CategoryManagementAdd" Src="~/desktopmodules/DNNArticle/CategoryManagementAdd.ascx" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

 <script type="text/javascript" language="javascript">
   function nodePopulating(sender, eventArgs) {

        var node = eventArgs.get_node();
       /* var context = eventArgs.get_context();
       console.log(node.get_value());
        context["CategoryID"] = node.get_value();
        context["portalid"] = <%=PortalSettings.PortalId %>;
        context["homedirectory"] = '<%=PortalSettings.HomeDirectory %>';
        context["includeid"] = true;
        */
    }
</script>
<telerik:RadFormDecorator ID="FormDecorator1" runat="server" DecoratedControls="all"
    ControlsToSkip="Default"></telerik:RadFormDecorator>

<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" BackColor="#eeeeee"
    BorderStyle="Solid" BorderWidth="1px" Transparency="50" Width="256px" Height="64px">
    <table width="100%" height="100%">
        <tr>
            <td style="vertical-align: middle; height: 64px">
                <asp:Image ID="Image1" ImageUrl="~/desktopmodules/DNNArticle/images/loading.gif"
                    runat="server" AlternateText="loading" />
            </td>
        </tr>
    </table>
</telerik:RadAjaxLoadingPanel>

    <table cellspacing="2" cellpadding="2">
        <tr>
            <td style="vertical-align: top;padding:5px;">
                <telerik:RadTreeView ID="RadTreeView2" runat="server" CausesValidation="false" OnNodeClick="RadTreeView2_NodeClick"
                    OnNodeDrop="RadTreeView2_NodeDrop" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" 
                    OnContextMenuItemClick="RadTreeView2_ContextMenuItemClick" OnNodeExpand="RadTreeView2_NodeExpand">
                   
                    <ContextMenus>
                        <telerik:RadTreeViewContextMenu runat="server" ID="menu1" >
                            <Items>
                                <telerik:RadMenuItem Value="Add" Text="Add">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="AddMore" Text="Buch Add">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Edit" Text="Edit">
                                </telerik:RadMenuItem>
                              
                                <telerik:RadMenuItem Value="MoveUp" Text="Move Up">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="MoveDown" Text="Move Down">
                                </telerik:RadMenuItem>
                            </Items>
                        </telerik:RadTreeViewContextMenu>
                        <telerik:RadTreeViewContextMenu runat="server" ID="menuroot" >
                            <Items>
                                <telerik:RadMenuItem Value="Add" Text="Add">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="AddMore" Text="Buch Add">
                                </telerik:RadMenuItem>
                                
                            </Items>
                        </telerik:RadTreeViewContextMenu>
                        <telerik:RadTreeViewContextMenu runat="server" ID="menuleaf" >
                            <Items>
                                <telerik:RadMenuItem Value="Add" Text="Add">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="AddMore" Text="Buch Add">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="Edit" Text="Edit">
                                </telerik:RadMenuItem>
                              <telerik:RadMenuItem Value="Delete" Text="Delete">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="MoveUp" Text="Move Up">
                                </telerik:RadMenuItem>
                                <telerik:RadMenuItem Value="MoveDown" Text="Move Down">
                                </telerik:RadMenuItem>
                            </Items>
                        </telerik:RadTreeViewContextMenu>
                    </ContextMenus>
                </telerik:RadTreeView>
            </td>
            <td style="vertical-align: top;">
               <zldnn:CategoryManagementEdit runat="server" id="CategoryManagementEdit" visible="false"></zldnn:CategoryManagementEdit>
               <zldnn:CategoryManagementAdd runat="server" id="CategoryManagementAdd" visible="false"></zldnn:CategoryManagementAdd>
            </td>
        </tr>
    </table>

<asp:LinkButton ID="cmdAdd" runat="server" CssClass="AddLabel" resourcekey="Add"
    CausesValidation="false" onclick="cmdAdd_Click" ></asp:LinkButton> 
<asp:LinkButton ID="cmdAddMore" runat="server" CssClass="AddLabel" resourcekey="AddMore"
    CausesValidation="false" onclick="cmdAddMore_Click" ></asp:LinkButton> <asp:LinkButton ID="cmdCancel" runat="server" CssClass="CancelLabel" resourcekey="cmdCancel"
    CausesValidation="false" OnClick="cmdCancel_Click">Cancel</asp:LinkButton>